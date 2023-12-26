
using UnityEngine;

namespace Assets.Scripts.HomeLogic.Environment.OperatorDecoration
{
    internal class TalkDecorationOperator : MonoBehaviour, IDecorationOperatorController
    {
        public void Inject(string param)
        {
            var ani = GetComponent<Animator>();
            ani.Update(Random.Range(0, float.Parse(param)));
        }
    }
}
