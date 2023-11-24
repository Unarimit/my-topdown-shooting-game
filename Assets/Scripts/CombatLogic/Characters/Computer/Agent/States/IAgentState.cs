namespace Assets.Scripts.CombatLogic.Characters.Computer.Agent.States
{
    public enum StateType
    {
        Idle, React, Attack
    }

    public interface IAgentState
    {
        void OnEnter();

        void OnUpdate();

        void OnExit();
    }
}
