# AI 行为树自动迭代训练指南

## 系统概述

本系统实现了基于 LLM 的行为树自动迭代训练框架，通过 Unity MCP 与外部 AI 连接，实现对游戏 AI 的自动优化。

### 核心组件

1. **AITrainingConfig** - 训练配置 ScriptableObject
2. **AITrainingManager** - 训练数据收集和输出
3. **AITrainingGameStarter** - 训练流程控制
4. **BehaviorTreeVersionManager** - 行为树版本管理
5. **AITrainingSceneSetup** - 场景配置组件

## 快速开始

### 1. 设置场景

在 GameScene 的根物体上添加 `AITrainingSceneSetup` 组件：

```csharp
- EnableTrainingMode: true
- TeamBehaviorTree: 要训练的候选行为树
- EnemyBaseBehaviorTree: 原始基础行为树（敌人默认使用）
- BestBehaviorTree: 历史最优行为树（可选）
```

### 2. 创建训练配置

通过编辑器工具创建：
- Menu: `Tools/AI Training/Create Training Config`
- 或使用 `AITrainingSceneSetup` 的详细配置

### 3. 运行训练

进入 Play 模式后，系统会自动：
1. 初始化训练管理器
2. 对战多个对手（基础版、历史最优版、自对弈）
3. 收集战斗结果并输出到 Console

## MCP 集成

### 读取训练结果

训练结果以特定格式输出到 Unity Console：

```csharp
// 单场对战结果
[TRAINING_RESULT_BATTLE] {json}

// 会话最终结果
[TRAINING_RESULT_SESSION] {json}

// 最终报告
[TRAINING_FINAL_REPORT] {json}

// 人类可读摘要
[TRAINING_SUMMARY]
```

### LLM 迭代流程

```
┌─────────────────────────────────────────────────────────────┐
│                     AI 行为树迭代流程                        │
├─────────────────────────────────────────────────────────────┤
│                                                             │
│  1. 初始化阶段                                               │
│     └─> 加载基础行为树、历史最优版本                          │
│     └─> 设置训练配置（阵容、地图、对手）                      │
│                                                             │
│  2. 大调阶段（探索）                                         │
│     ┌─────────────────────────────────────────────────────┐ │
│     │  a. LLM 分析当前行为树结构                           │ │
│     │  b. 生成多个候选修改（新分支/替换节点/调整参数）       │ │
│     │  c. 对每个候选：                                      │ │
│     │     - 应用到候选行为树                                │ │
│     │     - 对战多个对手（基础版+历史最优版+自对弈）         │ │
│     │     - 收集胜率、伤亡、得分等数据                       │ │
│     │  d. 选择最优候选作为新版本                            │ │
│     └─────────────────────────────────────────────────────┘ │
│                                                             │
│  3. 评估与剪枝                                               │
│     └─> 对比新版本 vs 历史最优版                             │
│     └─> 如果更优：提升为最优版本                             │
│     └─> 如果更差：回滚并尝试其他候选                          │
│                                                             │
│  4. 微调节阶段（当大调不再进步时）                           │
│     └─> 在参数空间精细搜索                                   │
│     └─> 调整 Wait 时间、概率值、距离阈值等                    │
│                                                             │
│  5. 重复 2-4 直到满足停止条件                                 │
│     └─> 达到最大迭代次数 或                                   │
│     └─> 连续 N 次无显著进步                                  │
│                                                             │
└─────────────────────────────────────────────────────────────┘
```

## 训练配置说明

### AITrainingConfig 参数

| 参数 | 说明 |
|------|------|
| `UseAIForPlayerSlot` | AI 是否接管第0个单位（友军队长） |
| `TeamOperatorCount` | 友方干员数量 |
| `EnemyOperatorCount` | 敌方干员数量 |
| `TeamLevelOffset` | 友方等级偏移（测试劣势情况） |
| `EnemyLevelOffset` | 敌方等级偏移（正值=更强） |
| `MaxBattleDuration` | 单场最大时长（超时=失败） |
| `UseMultipleOpponents` | 是否测试多个对手类型 |

### 评估指标

```csharp
// 胜利分数计算公式
Score = 100                              // 基础胜利分
      + 存活单位数 × 10                   // 存活奖励
      + 剩余血量比例 × 50                 // 血量奖励
      + max(0, 30 - 战斗时间 × 0.25)     // 快速胜利奖励
```

## 行为树修改策略（供LLM参考）

### 大调策略

1. **增加新的行为分支**
   - 低血量时添加撤退逻辑
   - 添加集火/分散策略
   - 添加技能连招序列

2. **替换节点类型**
   - Selector → Sequence
   - 添加 Decorator（Cooldown、UntilSuccess）
   - 更换条件判断

3. **重构子树**
   - 将重复逻辑提取为子行为树
   - 添加并行处理（Parallel）
   - 优化选择器优先级

### 微调策略

1. **调整参数**
   - Wait 任务的时间
   - RandomProbability 的概率值
   - 距离阈值、角度阈值
   - Cooldown 时间

2. **调整权重**
   - PrioritySelector 的优先级
   - UtilitySelector 的效用函数

## 版本管理

### 版本存储结构

```
Assets/Resources/BehaviorTreeVersions/
├── BT_v0_Base_xxxxxx.asset      # 原始基础版本
├── BT_v1_xxxxxx.asset           # 第一次迭代
├── BT_v2_xxxxxx.asset           # 第二次迭代
└── version_history.json         # 版本元数据
```

### 版本信息

```json
{
  "VersionId": "a1b2c3d4",
  "VersionName": "BT_v1",
  "WinRate": 0.75,
  "AverageVictoryScore": 165.5,
  "Description": "添加低血量撤退逻辑",
  "ParentVersionId": "base"
}
```

## 使用示例

### 手动触发训练

```csharp
// 在 Inspector 中点击 "Start Training Now"
// 或代码调用：
var setup = FindObjectOfType<AITrainingSceneSetup>();
setup.StartTrainingNow();
```

### 通过 MCP 自动化

```python
# 伪代码示例
while iteration < max_iterations:
    # 1. 获取当前最优行为树
    current_best = get_best_behavior_tree()
    
    # 2. LLM 生成候选修改
    candidates = llm.generate_candidates(current_best, strategy="major")
    
    # 3. 测试每个候选
    for candidate in candidates:
        apply_behavior_tree(candidate)
        result = run_training(
            opponents=[base_version, best_version, candidate]
        )
        candidate.score = result.average_victory_score
    
    # 4. 选择最优
    best_candidate = max(candidates, key=lambda x: x.score)
    
    # 5. 对比当前最优
    if best_candidate.score > current_best.score + threshold:
        promote_to_best(best_candidate)
        save_version(best_candidate)
    else:
        # 切换到微调策略
        if no_improvement_count >= 3:
            strategy = "fine_tune"
```

## 调试与监控

### Console 日志标记

- `[AITraining]` - 训练系统日志
- `[TRAINING_RESULT_BATTLE]` - 单场结果（JSON）
- `[TRAINING_RESULT_SESSION]` - 会话结果（JSON）
- `[TRAINING_SUMMARY]` - 人类可读摘要

### 可视化调试

在 `AITrainingManager` Inspector 中可查看：
- 当前训练进度
- 已完成的战斗场次
- 实时胜率统计

## 注意事项

1. **行为树资源**：确保所有行为树都是 ExternalBehaviorTree（.asset 文件）
2. **场景配置**：GameScene 需要有 NavMesh 和必要的管理器
3. **性能考虑**：大量训练时建议降低画质或使用 Headless 模式
4. **版本控制**：自动生成的版本文件应加入 .gitignore

## 扩展功能

- 支持多场景并行测试
- 遗传算法优化行为树参数
- 自动行为树结构生成（从零开始）
- 对抗性训练（敌人也同时进化）