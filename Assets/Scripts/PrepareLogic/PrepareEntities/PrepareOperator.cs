using Assets.Scripts.Entities;

namespace Assets.Scripts.PrepareLogic.PrepareEntities
{
    public class PrepareOperator
    {
        public Operator OpInfo { get; }

        public bool IsChoose { get; set; }

        public PrepareOperator(Operator opInfo)
        {
            OpInfo = opInfo;
            IsChoose = false;
        }




    }
}
