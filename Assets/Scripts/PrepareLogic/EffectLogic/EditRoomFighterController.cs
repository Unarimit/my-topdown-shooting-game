using DG.Tweening;
using UnityEngine;

namespace Assets.Scripts.PrepareLogic.EffectLogic
{
    internal class EditRoomFighterController : MonoBehaviour
    {
        private Transform m_CharactorTrans;
        float CHARACTER_END_SCALE = 0.7f; // 起飞动画参数
        
        public void Inject(Vector3 aimLocalPos)
        {
            m_CharactorTrans = transform.Find("modelroot");
            // 起飞过渡动画
            m_CharactorTrans.DOLocalMove(aimLocalPos, 0.5f);
            m_CharactorTrans.DOScale(CHARACTER_END_SCALE, 0.5f);
        }
        public void Update()
        {
            m_CharactorTrans.localPosition += flyVerticleSimulate();
        }
        private Vector3 flyVerticleSimulate()
        {
            return new Vector3(0, Mathf.Sin(Time.time * 2) * Time.deltaTime * 0.3f, 0);
        }
    }
}
