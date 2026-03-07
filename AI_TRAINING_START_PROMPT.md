# AI训练迭代启动提示词

在新对话中发送以下内容，快速启动AI行为树迭代训练：

---

## 提示词（复制到Cline）

```
请继续AI行为树迭代训练。

## 当前状态

### 已有行为树版本
- **v2** - CharacterBeselBehavior_v2_WithRetreat (低血量撤退)
- **v3** - CharacterBeselBehavior_v3_OptimizedAttack (优化攻击逻辑) ✅ 当前最优
- **v4** - CharacterBeselBehavior_v4_FocusFire (集火逻辑) - 待测试
- **v5** - TweakedRetreatTask 代码已创建，待集成

### 当前最优版本
胜率: 66.7%, 均分: 155.7 (v3 vs v2)

### 上次迭代
完成了Agent行为统计系统，支持检测发呆/划水Agent，并输出行为树节点信息。

## 任务

1. 检查当前配置
2. 运行v4 vs v3训练（如果还没测）
3. 根据结果决定：
   - 如果v4优于v3 → 提升v4为最优，创建v5
   - 如果v3更优 → 尝试v5（参数微调）
4. 查看[AGENT_BEHAVIOR_TEAM/ENEMY]输出，识别发呆Agent
5. 针对发呆Agent改进行为树

## 关键文件

- 配置: `Assets/Resources/AITraining/New_Training_Config.asset`
- 行为树: `Assets/Resources/Characters/`
- 训练管理器: `Assets/Scripts/CombatLogic/LevelLogic/AITrainingManager.cs`
- 文档: `AI_TRAINING_GUIDE.md`

## 操作步骤

1. 用MCP检查Unity状态
2. 读取当前训练配置
3. 启动训练
4. 每60秒检查Console输出
5. 分析[AGENT_BEHAVIOR_TEAM]识别问题Agent
6. 创建新的行为树版本迭代

请开始迭代。
```

---

## 快速命令参考

在新对话中可以直接使用这些MCP命令：

```bash
# 1. 检查Unity状态
unity_mcp refresh_unity

# 2. 读取当前配置
unity_mcp read_file path=Assets/Resources/AITraining/New_Training_Config.asset

# 3. 启动训练
unity_mcp manage_editor action=play

# 4. 60秒后读取结果
unity_mcp read_console count=50 types=log

# 5. 停止训练
unity_mcp manage_editor action=stop
```

---

## 输出解读

训练时会输出：
- `[AGENT_BEHAVIOR_TEAM]` - 友方行为分析
- `[AGENT_BEHAVIOR_ENEMY]` - 敌方行为分析
- `[BEHAVIOR_SUMMARY]` - 发呆/划水计数
- `[TRAINING_RESULT_BATTLE]` - 战斗结果JSON

**重点关注**：
- `[发呆]` 标记的Agent（静止>50%，参与<20%）
- `[划水]` 标记的Agent（活跃度<30）
- `当前节点` - 卡住的行为树节点
- `最长停留` - 需要优化的节点

---

## 迭代流程

```
每次迭代循环：
1. 启动训练 → 2. 等待完成 → 3. 分析结果 → 4. 识别问题Agent
→ 5. 修改行为树 → 6. 创建新版本 → 7. 重复直到收敛
```

输入"停止迭代"可随时中断。