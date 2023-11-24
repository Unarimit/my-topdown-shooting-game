namespace Assets.Scripts.CombatLogic.Characters.Computer.Agent.States
{
    public enum StateType
    {
        CaIdle, CaReact, CaAttack,
        CvIdle, CvFollow, CvHelp
    }

    public interface IAgentState
    {
        void OnEnter();

        void OnUpdate();

        void OnExit();
    }
}
