# AI 行为树迭代训练工作流（增强版）

## 系统现状

训练框架已升级，支持：
- **自动训练检测** - 定时检测训练完成状态
- **Agent行为统计** - 检测发呆、划水等异常行为
- **自动多轮迭代** - 配置后自动运行多轮训练

---

## 核心组件职责

| 组件 | 职责 |
|------|------|
| **AITrainingSceneManager** | 自动运行多轮对战，输出训练结果 |
| **AITrainingManager** | 收集战斗数据，计算胜率/得分，追踪Agent行为 |
| **AIAgentBehaviorTracker** | 实时追踪每个Agent的移动、战斗参与等行为 |
| **BehaviorTreeVersionManager** | 保存历史版本，管理最优版本 |
| **AITrainingConfig** | 配置行为树引用和测试参数 |

---

## 新增功能

### 1. Agent行为统计系统

新增文件：
- `AIAgentBehaviorStats.cs` - 行为统计数据结构
- `AIAgentBehaviorTracker.cs` - 实时行为追踪器

**统计指标**:
```csharp
// 位置相关 - 检测发呆
TotalDistanceMoved      // 总移动距离
StationaryPercentage    // 静止时间占比 (>50%视为发呆)

// 战斗参与 - 检测划水
CombatParticipationRate // 战斗参与率 (<20%视为划水)
TimesInAttackRange      // 进入攻击范围次数

// 综合评分
ActivityScore           // 活跃度评分 (0-100)
CombatScore             // 战斗参与评分 (0-100)
```

**Console输出标记（合并格式）**:

每场战斗结束后，会输出**3条**日志（2条行为分析 + 1条摘要）：

```csharp
[AGENT_BEHAVIOR_TEAM]
友方Agent行为分析:
  CV_Aoi: [正常] 移动45.3m 静止12.5% 参与率78.3%
  CA_Emi: [发呆] 移动8.2m 静止65.1% 参与率15.2%
    └─ 当前节点: BT_v4 (12.5s), 最长停留: BT_v4 (45.3s)
  CV_Kaori: [正常] 移动52.1m 静止8.3% 参与率85.6%
  ...

[AGENT_BEHAVIOR_ENEMY]
敌方Agent行为分析:
  Enemy_01: [正常] 移动38.5m 静止15.2% 参与率72.1%
  Enemy_02: [划水] 移动12.1m 静止55.3% 参与率22.5%
    └─ 当前节点: BT_v3 (8.3s), 最长停留: Wait (28.5s)
  ...

[BEHAVIOR_SUMMARY] 发呆Agent: 1, 划水Agent: 2, 团队平均活跃度: 62.5%
```

**输出字段说明**:
- `[正常]`/`[发呆]`/`[划水]` - 行为状态判定（基于静止时间和战斗参与率）
- `移动XXm` - 总移动距离（反映Agent活动范围）
- `静止XX%` - 静止时间占比（>50%视为发呆）
- `参与率XX%` - 战斗参与率（<20%视为划水）
- `当前节点` - 行为树当前状态（外部监控，不修改行为树节点）
- `最长停留` - 停留最久的节点（帮助诊断卡住原因）

**其他标记**:
- `[TRAINING_RESULT_BATTLE]` - 战斗结果（JSON格式，含行为统计数据）
- `[TRAINING_SUMMARY]` - 完整会话摘要

### 2. 自动迭代检测机制

**MCP自动检测流程**:
```
1. 启动训练（Enter Play Mode）
2. 等待60秒（预计单轮时长）
3. 读取Console检查 [TRAINING_RESULT_BATTLE]
4. 如果检测到2轮以上结果，自动停止训练
5. 分析结果，决定是否继续下一轮迭代
6. 如果用户未手动结束，重复步骤1-5
```

**检测标记**:
- `[AITraining] 训练会话初始化` - 训练开始
- `[TRAINING_RESULT_BATTLE]` - 单轮战斗结束
- `[TRAINING_SUMMARY]` - 完整会话结束

---

## LLM 迭代工作流（自动版）

### 自动迭代脚本（伪代码）

```python
# MCP自动迭代流程
while user_not_interrupt:
    # 1. 启动Unity训练
    unity.play()
    
    # 2. 等待训练完成（检测Console）
    wait_for_console("[TRAINING_RESULT_BATTLE]", timeout=120)
    
    # 3. 读取最近2轮结果
    results = read_console_logs("[TRAINING_RESULT_BATTLE]", count=2)
    
    # 4. 分析结果
    win_rate = calculate_win_rate(results)
    avg_score = calculate_avg_score(results)
    
    # 5. 决策
    if win_rate > 0.7 and avg_score > 160:
        # 优于当前最优，提升版本
        promote_to_best_version()
        create_next_iteration()
    elif win_rate < 0.3:
        # 表现太差，回滚
        rollback_to_best()
        try_different_strategy()
    else:
        # 继续微调
        create_next_iteration()
    
    # 6. 等待用户中断或继续
    sleep(5)
```

### 手动触发命令

在Cline对话中输入：
- `继续迭代` - 开始下一轮自动迭代
- `停止迭代` - 结束当前训练循环
- `查看结果` - 读取当前训练统计

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

// Agent行为分析（新增）
[AGENT_BEHAVIOR] CV_Aoi: [正常] 移动45.3m 静止12.5% 参与率78.3%
[AGENT_BEHAVIOR] CA_Emi: [发呆] 移动8.2m 静止65.1% 参与率15.2%  <-- 异常！
[BEHAVIOR_SUMMARY] 发呆Agent: 1, 划水Agent: 2, 团队平均活跃度: 62.5%

// 最终 JSON 结果（MCP解析）
[TRAINING_FINAL_REPORT] {
    "SessionId": "a1b2c3d4",
    "BehaviorTreeVersion": "BT_Candidate_v1",
    "BattleResults": [...],
    "WinRate": 0.6667,
    "AverageVictoryScore": 120.5,
    "IdleAgentCount": 1,        // 新增：发呆Agent数量
    "SlackingAgentCount": 2,    // 新增：划水Agent数量
    "TeamAverageActivity": 62.5 // 新增：团队平均活跃度
}

// 人类可读摘要
[TRAINING_SUMMARY]
=== 训练会话结果 [a1b2c3d4] ===
行为树版本: BT_Candidate_v1
测试场次: 9
胜率: 66.7%
平均胜利分: 120.5
平均友方阵亡: 2.3
发呆Agent: 1/8      <-- 新增
划水Agent: 2/8      <-- 新增
团队活跃度: 62.5%   <-- 新增
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
| `IdleAgentCount` | 发呆Agent数 | 0 为最佳，>2 需改进行为树 |
| `SlackingAgentCount` | 划水Agent数 | 0 为最佳，>2 需检查攻击逻辑 |
| `TeamAverageActivity` | 团队活跃度 | >60% 可接受，>80% 优秀 |
| `vs BaseBehavior` | 对比基础版 | 必须赢，否则不如原始版 |
| `vs BestBehavior` | 对比历史最优 | 赢了才能成为新的最优 |

---

## 修改策略参考

### 大调策略（结构调整）

**场景1：修复发呆Agent**
```
问题：部分Agent静止时间过长(>50%)，战斗参与率低(<20%)

诊断：
- 检查是否有Agent卡在NavMesh边界
- 检查行为树是否进入死循环（如持续执行Wait）
- 检查视野检测范围是否过小

解决：
- 添加定期强制巡逻（每30秒随机移动）
- 优化FindCoverTask，避免选择不可达位置
- 扩大HasEnemyInSight检测范围
```

**场景2：修复划水Agent**
```
问题：Agent在战场边缘，不参与战斗

诊断：
- 检查SetPatrolTarget是否选择过远位置
- 检查是否有Agent被分配了非战斗任务
- 检查MoveToEnemyTask是否正确执行

解决：
- 限制巡逻范围在战斗区域内
- 添加强制返回战场中心的逻辑
- 优化目标选择，优先选择最近敌人
```

**场景3：增加撤退逻辑**
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

**场景4：优化攻击选择**
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

**场景5：添加集火**
```
Decorator (Cooldown 2s)
└── Sequence
    ├── FindAllyTarget  [自定义Action：寻找友方正在攻击的目标]
    └── Attack
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

## 自动迭代配置

### 配置文件示例

```yaml
# Assets/Resources/AITraining/New_Training_Config.asset

ConfigName: "Auto Iteration Config"
TeamBehaviorTree: {fileID: ...}  # 候选版本
EnemyBehaviorTree: {fileID: ...} # 当前最优版本
UseMultipleOpponents: 0           # 单对手快速测试

# 自动迭代设置
MaxBattleDuration: 120
TeamOperatorCount: 10
EnemyOperatorCount: 10
```

### MCP自动检测命令

```bash
# 每分钟检测一次Console输出
while true; do
    unity_mcp read_console --filter="TRAINING_RESULT_BATTLE"
    sleep 60
done
```

---

## 版本管理

### 自动保存

每次训练完成后，如果手动调用版本提升：

```csharp
// 在确认候选版本更优后
BehaviorTreeVersionManager.Instance.CreateNewVersion(
    description: "添加低血量撤退逻辑，修复2个发呆Agent",
    winRate: result.WinRate,
    avgScore: result.AverageVictoryScore,
    idleCount: result.IdleAgentCount,      // 新增
    slackCount: result.SlackingAgentCount  // 新增
);
```

### 版本回滚

```csharp
// 如果新版本表现不佳，回滚到历史版本
BehaviorTreeVersionManager.Instance.RollbackToVersion("v3_OptimizedAttack");
```

---

## 迭代终止条件

停止迭代当满足以下任一条件：

1. **收敛**：连续 3 次迭代无显著进步（得分提升 < 5%，发呆Agent数不变）
2. **饱和**：平均胜利分 > 200（接近理论上限）
3. **足够好**：胜率 > 90% 且平均得分 > 180 且发呆Agent = 0
4. **用户中断**：手动输入"停止迭代"

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

### Q: 如何检测Agent是否发呆？
查看Console中的 `[AGENT_BEHAVIOR]` 输出，标记为 `[发呆]` 或 `[划水]` 的Agent需要关注

### Q: 自动迭代卡住？
检查Unity是否还在Play模式，或Console是否有 `[TRAINING_RESULT_BATTLE]` 输出

---

## 快速启动命令（MCP）

```bash
# 1. 启动训练
unity_mcp play

# 2. 等待60秒后检查结果
sleep 60
unity_mcp read_console --filter="TRAINING_RESULT_BATTLE"

# 3. 停止训练
unity_mcp stop

# 4. 刷新资源
unity_mcp refresh
```

---

## 备注

- 新战场形势：CV类型角色现在会生产战斗机，战斗更加动态
- AI射击精度已调整，战斗结果更具参考价值
- 建议每次迭代后检查 `IdleAgentCount` 和 `SlackingAgentCount` 指标
- 自动迭代过程中，如遇异常可随时输入"停止迭代"中断