using UnityEngine;

namespace Assets.Scripts.HomeLogic.Environment.OperatorDecoration
{
    internal class WalkDecorationOperator : MonoBehaviour, IDecorationOperatorController
    {
        float speed;
        public void Inject(string param)
        {
            speed = float.Parse(param);
            var colider = gameObject.AddComponent<BoxCollider>();
            colider.isTrigger = true;
        }

        void Update()
        {
            transform.position += speed * transform.forward * Time.deltaTime;
        }
    }
}
