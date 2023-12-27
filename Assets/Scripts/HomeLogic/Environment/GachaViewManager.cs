using Assets.Scripts.Common;
using Assets.Scripts.Common.Test;
using Assets.Scripts.Entities;
using Assets.Scripts.HomeLogic.ContextExtend;
using Assets.Scripts.HomeLogic.UILogic;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.HomeLogic.Environment
{
    internal class GachaViewManager : MonoBehaviour
    {
        public static GachaViewManager Instance;

        [SerializeField]
        GachaEffectController m_gachaEffectController;
        [SerializeField]
        Transform m_gachaBase;

        Transform gachaCharacter;
        Transform gachaBaseCharacter;

        private void Awake()
        {
            // singleton
            if (Instance == null) Instance = this;
            else Debug.LogWarning(transform.ToString() + " try to load another Manager");
        }

        public void Quit()
        {
            Destroy(gachaBaseCharacter.gameObject);
        }

        [MyTest]
        public void TestGachaCharacterAnime()
        {
            GachaCharacterAnime(HomeContextManager.Instance, MyServices.Database.Operators[0], UIManager.Instance);
        }

        public void GachaCharacterAnime(HomeContextManager context, Operator op, UIManager UI)
        {
            StartCoroutine(coreTask(context, op, UI));
            StartCoroutine(coreTask2(op));
        }

        // 动画方面（考虑跳过动画
        IEnumerator coreTask(HomeContextManager context, Operator op, UIManager UI)
        {
            // 1.相机转向
            UI.SwitchPage(HomePage.GachaCharacterView);

            yield return new WaitForSeconds(0.45f);

            // 2.播放动画
            var rarity = op.GetRarity();
            if(rarity < 10) m_gachaEffectController.Play(GachaRarity.Low);
            else if(rarity < 20 ) m_gachaEffectController.Play(GachaRarity.Middle);
            else m_gachaEffectController.Play(GachaRarity.High);
            while (m_gachaEffectController.Statu == GachaEffectStatu.Playing) yield return null;

            // 3. 爆炸效果时创建角色
            gachaCharacter = context.GenerateGachaDisplay(op, m_gachaEffectController.transform.position, Vector3.zero);

            // 4. 延时
            yield return new WaitForSeconds(1f);

            // 5. 相机转回
            var slider = SlideUI.CreateSlideUI();
            yield return null;
            while (slider.IsFinish is false) yield return null;
            UI.SwitchPage(HomePage.CoreCharacterView);

            // 6. 清空效果和角色
            Destroy(gachaCharacter.gameObject);
            m_gachaEffectController.MyReset();
        }

        // 延迟在CoreView创建角色，防止穿帮
        IEnumerator coreTask2(Operator op)
        {
            yield return new WaitForSeconds(0.8f); // UI延迟0.5秒...
            gachaBaseCharacter = HomeContextManager.Instance.GenerateGachaBaseDisplay(op, m_gachaBase.position, Vector3.zero);
        }

    }
}
