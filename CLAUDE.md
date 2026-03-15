# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is a Unity 2022.3.53f1c1 project for a cartoon-style top-down shooting game (俯视视角射击游戏) with home base management and combat gameplay. Uses Unity Universal Render Pipeline (URP).

**Branch Structure:**
- `dev` (default): Active development branch
- `main`: Latest stable release
- `dev-with-xlua`: Branch experimenting with xLua for hot updates

## Development Environment

- **Unity Version**: 2022.3.53f1c1
- **Render Pipeline**: URP 14.0.11
- **Scripting**: C# 9.0 (.NET Standard 2.1)
- **IDE**: Visual Studio / Rider (project files are generated)

### Key Third-Party Packages

- **Behavior Designer**: AI behavior tree system (in Assets/Plugins/)
- **Behavior Designer Tactical**: Tactical extensions for group AI
- **MagicaCloth2**: Cloth simulation
- **FernRenderCore**: NPR (non-photorealistic) rendering
- **com.coplaydev.unity-mcp**: MCP integration for AI tooling
- **DOTween**: Animation tweeting (Demigiant)

## Architecture

### Core Systems

**Combat Logic** (`Assets/Scripts/CombatLogic/`)
- `CombatContextManager`: Singleton managing all combat entities, teams, and game state
- `GameStartupManager`: Initializes levels, spawns characters, sets up NavMesh
- `GameLevelManager`: Handles win/lose conditions, level completion
- `SkillManager`: Handles skill casting and cooldowns

**AI System**
- `AgentController` (`Characters/Computer/Agent/`): AI character controller using Behavior Designer trees
- `PlayerController` (`Characters/Player/`): Player character controller
- Behavior tree extensions in `Characters/BehaviorTreeExtend/`: Custom tasks (FindCoverTask, FocusFireTask, ReloadTask, etc.)

**Entity Management**
- `CombatOperator`: Runtime combat data for characters
- `Operator`: Static character data (ScriptableObject-based)
- Characters spawned via `CombatContextManager.GenerateAgent()` / `GeneratePlayer()`

**AI Training Framework** (`LevelLogic/AITraining/`)
- `AITrainingSceneManager`: Automated training scene runner, runs multiple battle rounds
- `AITrainingManager`: Records battle data, outputs JSON results to console
- `BehaviorTreeVersionManager`: Manages behavior tree version history
- `AITrainingConfig`: ScriptableObject for training configuration

Training outputs console markers:
- `[TRAINING_RESULT_BATTLE]`: Single battle result (JSON)
- `[TRAINING_RESULT_SESSION]`: Full session result (JSON)
- `[AGENT_BEHAVIOR_TEAM]` / `[AGENT_BEHAVIOR_ENEMY]`: Agent behavior analysis
- `[BEHAVIOR_SUMMARY]`: Team activity summary

### Data Layer

**Services** (`Assets/Scripts/Services/`)
- `MyServices`: Static service locator pattern
- `MyConfig`: Game constants and enums
- `ResourceManager`: Asset loading abstraction
- Database classes for persistent data

**Entities** (`Assets/Scripts/Entities/`)
- Data models: `Operator`, `CombatLevelInfo`, `CombatLevelResult`
- Enums: `SkillType`, `SkillEffectType`, etc.
- Buildings and mecha customization data

### Scene Structure

Build settings scenes (in order):
1. `StartScene/Start` - Game entry
2. `HomeScene/Home` - Home base management
3. `PrepareScene/Prepare` - Pre-combat preparation
4. `HomeScene/HomeBackground` - Home scene background
5. `Playground/GameScene` - Main combat scene

### Scene Object Hierarchy (GameScene)

```
GameRoot (GameStartupManager, CombatContextManager, SkillManager)
├── Agents/ (Spawned characters)
├── Terrain/ (Generated terrain)
├── NavMesh Surface (AI navigation)
├── Effects/
└── UI/
```

## Working with AI Behavior Trees

### Creating Custom Tasks

Place in `Assets/Scripts/CombatLogic/Characters/BehaviorTreeExtend/`:

```csharp
[TaskCategory("AI/Actions")]
[TaskDescription("Description of what this task does")]
public class MyCustomTask : Action
{
    private AgentController _agentController;

    public override void OnStart()
    {
        _agentController = GetComponent<AgentController>();
    }

    public override TaskStatus OnUpdate()
    {
        // Return Success, Failure, or Running
        return TaskStatus.Success;
    }
}
```

### Common Behavior Tree Variables

The `AgentController` initializes these shared variables:
- `target` (SharedGameObjectList): Current enemy targets
- `teammate` (SharedGameObjectList): Nearby teammates
- `moveTarget` (SharedVector3): Movement destination
- `canBreak` (SharedBool): Whether action can be interrupted

### Running AI Training

1. Configure training in `Assets/Resources/AITraining/New_Training_Config.asset`
2. Open `GameScene` scene
3. Enable `AITrainingSceneManager.EnableTrainingMode = true`
4. Enter Play Mode - training runs automatically
5. Check Unity Console for `[TRAINING_RESULT_BATTLE]` JSON outputs

Key training config fields:
- `TeamBehaviorTree`: The behavior tree being tested
- `EnemyBehaviorTree` / `BaseBehaviorTree` / `BestBehaviorTree`: Opponents to test against
- `MaxBattleDuration`: Timeout in seconds (default 120)
- `TeamOperatorCount` / `EnemyOperatorCount`: Squad sizes

## Common Tasks

### Adding a New Skill

1. Add skill data to `Assets/Resources/SkillList.asset`
2. Create prefab in `Assets/Resources/Skills/`
3. Implement skill logic in `SkillManager` or create custom component

### Adding a New Character Type

1. Add to `Operator` ScriptableObject or create via code
2. Ensure model exists in Resources or Fbx loading path
3. Configure in `CombatContextManager.GenerateAgent()` pipeline

### Debugging Combat

- Use `[MyTest]` attribute on methods in `MyTestController` for in-editor testing
- Check `CombatContextManager.Operators` dictionary for runtime entity info
- Enable `VerboseLogging` in `AITrainingConfig` for detailed AI logs

### Scene Loading

Always use `SceneLoadHelper.MyLoadSceneAsync(string sceneName)` instead of direct SceneManager calls to ensure proper cleanup.

## Code Conventions

- Namespaces follow folder structure: `Assets.Scripts.CombatLogic`
- Private fields prefixed with underscore: `_context`
- Singleton pattern with `Instance` static property
- Events use delegate pattern: `public event Action<Args> OnEvent`
- Chinese comments are common in this codebase

## Important Notes

- **NavMesh**: Must call `NavMeshSurface.UpdateNavMesh()` after terrain generation
- **External Resources**: Models and paid assets are in `.gitignore` (not in repo)
- **Layer Configuration**: Characters use `Character` layer (see `MyConfig.CHARACTER_LAYER`)
- **Fighter System**: CV-type operators spawn fighter aircraft as separate entities
