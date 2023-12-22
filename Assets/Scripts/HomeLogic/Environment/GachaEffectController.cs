using UnityEngine;

namespace Assets.Scripts.HomeLogic.Environment
{
    internal class GachaEffectController : MonoBehaviour
    {
        public GameObject m_Boom;
        public GameObject m_Tail;

        public float m_ForwordSpeed = 10f;
        public Vector3 m_AngleSpeed = Vector3.zero;
        public bool m_Freeze = false;
        public bool m_Over = false;

        private void Update()
        {
            if(m_Over is true)
            {
                gameObject.SetActive(false);
                Destroy(gameObject);
            }

            if (m_Freeze is true)
            {
                transform.eulerAngles = Vector3.zero;
                m_Tail.SetActive(false);
                m_Boom.SetActive(true);
                return;
            }
            transform.position += transform.forward * m_ForwordSpeed * Time.deltaTime;
            transform.eulerAngles += m_AngleSpeed * Time.deltaTime;
        }

    }
}
