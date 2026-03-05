# AI 行为树迭代训练工作流

## 系统现状

训练框架已就绪。核心流程：**修改行为树 → 运行训练 → 分析结果 → 迭代优化**

### 核心组件职责

| 组件 | 职责 |
|------|------|
| **AITrainingSceneManager** | 自动运行多轮对战，输出训练结果 |
| **AITrainingManager** | 收集战斗数据，计算胜率/得分 |
| **BehaviorTreeVersionManager** | 保存历史版本，管理最优版本 |
| **AITrainingConfig** | 配置行为树引用和测试参数 |

---

## LLM 迭代工作流

```
┌─────────────────────────────────────────────────────────────────┐
│                     行为树迭代优化流程                            │
├─────────────────────────────────────────────────────────────────┤
│                                                                 │
│  1. 【分析】读取当前最优行为树结构                                │
│     └─> 使用 BehaviorTreeVersionManager.BestBehaviorTree        │
│                                                                 │
│  2. 【修改】生成候选行为树（大调/微调）                          │
│     ├─> 大调：添加/删除/替换节点（结构调整）                    │
│     └─> 微调：修改参数（Wait时间、概率值等）                    │
│                                                                 │
│  3. 【测试】设置候选行为树并运行训练                              │
│     ├─> AITrainingSceneManager.TrainingConfig.TeamBehaviorTree  │
│     ├─> 自动对战3类对手：基础版、历史最优、自对弈               │
│     └─> 每对手3轮，共9场对战                                    │
│                                                                 │
│  4. 【评估】读取 [TRAINING_FINAL_REPORT] 结果                   │
│     ├─> 关注指标：WinRate、AverageVictoryScore                 │
│     └─> 对比当前最优版本                                        │
│                                                                 │
│  5. 【决策】                                                    │
│     ├─> 如果更优：提升为最优版本 → 继续步骤2迭代               │
│     ├─> 如果更差：回滚，尝试其他修改                            │
│     └─> 如果停滞：从大调切换到微调策略                          │
│                                                                 │
└─────────────────────────────────────────────────────────────────┘
```

---

## 实战：修改行为树

### 当前最优版本获取

```csharp
// 在 Unity 中查看
var best = BehaviorTreeVersionManager.Instance.BestBehaviorTree;
Debug.Log("当前最优: " + best.name);

// 查看历史版本
var versions = BehaviorTreeVersionManager.Instance.GetAllVersions();
foreach (var v in versions) {
    Debug.Log($"{v.VersionName}: 胜率{v.WinRate:P1}, 得分{v.AverageVictoryScore:F1}");
}
```

### 设置候选行为树

```csharp
// 方案1：直接赋值（运行时）
var sceneManager = FindObjectOfType<AITrainingSceneManager>();
sceneManager.TrainingConfig.TeamBehaviorTree = yourCandidateBehaviorTree;

// 方案2：在 Inspector 中设置
// 选中 GameScene 根物体 -> AITrainingSceneManager -> TrainingConfig -> TeamBehaviorTree
```

### 运行训练

```csharp
// 手动触发（Inspector 右键菜单或代码）
sceneManager.ManualStartTraining();

// 或进入 Play 模式自动开始（如果 AutoStartTraining = true）
```

---

## 解读训练结果

### Console 输出格式

```csharp
// 训练进度
[AITraining] 对战 BaseBehavior - 第 1/3 轮
[AITraining] 对战 BestBehavior - 第 2/3 轮  
[AITraining] 对战 Candidate - 第 3/3 轮

// 最终 JSON 结果（MCP解析）
[TRAINING_FINAL_REPORT] {
    "SessionId": "a1b2c3d4",
    "BehaviorTreeVersion": "BT_Candidate_v1",
    "BattleResults": [
        {"EnemyBehaviorTreeName":"Base","Result":"Win","VictoryScore":165},
        {"EnemyBehaviorTreeName":"Base","Result":"Win","VictoryScore":158},
        {"EnemyBehaviorTreeName":"Best","Result":"Loss","VictoryScore":0},
        ...
    ],
    "WinRate": 0.6667,              // 9场中赢6场
    "AverageVictoryScore": 120.5     // 平均胜利分
}

// 人类可读摘要
[TRAINING_SUMMARY]
=== 训练会话结果 [a1b2c3d4] ===
行为树版本: BT_Candidate_v1
测试场次: 9
胜率: 66.7%
平均胜利分: 120.5
详细结果:
  vs BaseBehavior: Win (伤亡 2/5, 得分 165.0)
  vs BestBehavior: Loss (伤亡 5/5, 得分 0.0)
  vs Candidate: Win (伤亡 1/5, 得分 180.0)
```

### 关键评估指标

| 指标 | 说明 | 决策参考 |
|------|------|---------|
| `WinRate` | 总体胜率 | >70% 可接受，>80% 优秀 |
| `AverageVictoryScore` | 平均胜利分 | >150 可接受，>180 优秀 |
| `vs BaseBehavior` | 对比基础版 | 必须赢，否则不如原始版 |
| `vs BestBehavior` | 对比历史最优 | 赢了才能成为新的最优 |

---

## 修改策略参考

### 大调策略（结构调整）

**场景1：增加撤退逻辑**
```
原结构:
Selector
├── CanSeeEnemy → Attack
└── Idle

修改为:
Selector
├── IsHurt (血量<30%) → FindCover (寻找掩体)  [新增]
├── CanSeeEnemy → Attack
└── Idle
```

**场景2：优化攻击选择**
```
原结构:
Sequence
├── Aim
└── Shoot

修改为:
Selector
├── WithinAttackRange → Sequence(Aim, Shoot)
└── MoveToAttackRange  [新增]
```

**场景3：添加集火**
```
Decorator (Cooldown 2s)
└── Sequence
    ├── FindAllyTarget  [自定义Action：寻找友方正在攻击的目标]
    └── Attack
```

**场景4：添加技能使用（需先扩展 AgentController）**
```
当前局限：AgentController 只有 Aim/Shoot，没有 UseSkill 方法

扩展步骤：
1. 在 AgentController 添加：
   - UseSkill() - 使用主动技能
   - IsSkillReady() - 检查技能冷却
   - GetSkillRange() - 获取技能范围

2. 创建行为树节点：
   - UseSkill Action
   - IsSkillReady Conditional

3. 行为树结构：
Selector
├── Sequence [技能连招]
│   ├── IsSkillReady
│   └── UseSkill
├── Sequence [普通攻击]
│   ├── WithinAttackRange
│   └── Shoot
└── MoveToAttackRange
```

### 微调策略（参数优化）

**场景1：调整 Wait 时间**
```csharp
// 原值
Wait (1.0s) 

// 测试不同值
Wait (0.5s)  // 更频繁决策
Wait (2.0s)  // 减少计算负担
```

**场景2：调整概率**
```csharp
// 原值
RandomProbability (0.5)

// 测试
RandomProbability (0.3)  // 更保守
RandomProbability (0.7)  // 更激进
```

**场景3：调整距离阈值**
```csharp
// AttackDistance 使用武器范围
float distance = GetComponent<OperatorController>().Model.WeaponSkill.SkillInfo.RangeTip;
```

---

## 版本管理

### 自动保存

每次训练完成后，如果手动调用版本提升：

```csharp
// 在确认候选版本更优后
BehaviorTreeVersionManager.Instance.CreateNewVersion(
    description: "添加低血量撤退逻辑",
    winRate: result.WinRate,
    avgScore: result.AverageVictoryScore
);
```

### 版本回滚

```csharp
// 如果新版本表现不佳，回滚到历史版本
BehaviorTreeVersionManager.Instance.RollbackToVersion("base");
```

---

## 迭代终止条件

停止迭代当满足以下任一条件：

1. **收敛**：连续 3 次迭代无显著进步（得分提升 < 5%）
2. **饱和**：平均胜利分 > 200（接近理论上限）
3. **足够好**：胜率 > 90% 且平均得分 > 180

---

## 常见问题

### Q: 修改后没有生成单位？
检查 `AITrainingSceneManager.EnableTrainingMode = true`

### Q: 训练结果都是0分？
检查战斗是否正常结束（超时 120 秒视为失败）

### Q: 如何加速训练？
减小 `MaxBattleDuration` 或直接使用 `AITrainingSceneManager` 运行单轮测试

### Q: 行为树修改不生效？
确保修改的是 `ExternalBehaviorTree` 资源文件，且重新赋值给 `TrainingConfig.TeamBehaviorTree`