namespace Assets.Scripts.CombatLogic.Characters.Computer.Fighter.States
{
    internal interface IFighterState
    {
        public enum StateType
        {
            Idle, Attack, Return
        }

        void OnEnter();

        void OnUpdate();

        void OnExit();
    }
}
