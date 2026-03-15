# AI行为树训练

## 流程

在claude code cli中，创建4个agent：Orchestrator、Critic、Planner、Coder

- 一定要创建agent，不能单agent扮演。目的是让每个agent的上下文专注于特定领域

Orchestrator是控制主要流程，是用户和agent team的交互接口。

主要流程为：
1. Orchestrator启动训练，在训练完成后，将数据发给Critic分析
2. Critic分析后，将决策投送给Planner
3. Planner迭代方案后，将方案发给Coder实现
4. Coder实现后，通知Orchestrator启动训练（即跳转到第一步）

这是一个循环流程

## 目标
基于v3(66.7%胜率)迭代优化，目标胜率>80%，均分>180。

## 4个Agent具体职责

### 1. Orchestrator (主控)

Orchestrator是控制主要流程，是用户和agent team的交互接口。按主要流程依次唤醒ai起来工作。输出：
- 变化和新增的文件名，只要文件名就行。

以下流程需要参考下文实现：
1. 检测训练完成时，使用mcp在console里面找包含`[AITraining] 对战结束`的日志，为了节省token

### 2. Critic (分析师)
分析训练结果(主要是每轮的`[TRAINING_RESULT_BATTLE]`)，输出：
- 胜率、均分、活跃度统计
- 核心问题(最多3个)
- 决策：[Continue]/[Promote]/[Rollback]

### 3. Planner (策略师)
基于Critic反馈设计1-2个具体改进：
- 修改哪个任务类
- 修改什么参数/逻辑
- 预期效果

### 4. Coder (编码师)
实现Planner的策略：
- 修改现有C#文件(优先)
- 或创建新任务类
- 直接编辑行为树YAML引用新类
- 创建新版本asset
- 通过mcp refresh后确定配置正确生效

## 关键规则

1. **基于v3修改**：每次复制v3，做小改动
2. **小步迭代**：一次只改一个参数或逻辑
3. **不回滚不询问**：自动决策，只报告结果
4. **简洁输出**：每轮报告不超过10行

## 可用任务类路径
`Assets/Scripts/CombatLogic/Characters/BehaviorTreeExtend/`

## 训练配置
- Config: `Assets/Resources/AITraining/New_Training_Config.asset`
- TeamBehaviorTree: 设置为新版本guid
- 运行Play Mode读取Console结果

## 输出格式

```
=== Iteration #N ===
Version: vX_Name
Result: Win/Loss X%
Key Issue: 核心问题
Action: 做了什么修改
Decision: Continue/Promote/Rollback
```