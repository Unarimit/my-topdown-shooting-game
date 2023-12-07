using Assets.Scripts.CombatLogic.CombatEntities;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.CombatLogic.UILogic.CombatSummaryUIs
{
    internal class CsRankScrollViewItemUI : MonoBehaviour
    {
        TextMeshProUGUI nameTMP;
        TextMeshProUGUI typeNameTMP;
        RawImage HeadIconRawImg;

        Slider shieldSlider;
        TextMeshProUGUI shieldValTMP;
        Slider dmgSlider;
        TextMeshProUGUI dmgValTMP;
        private void Awake()
        {
            nameTMP = transform.Find("CNameTMP").GetComponent<TextMeshProUGUI>();
            HeadIconRawImg = transform.Find("HeadIconRawImage").GetComponent<RawImage>();
            typeNameTMP = transform.Find("HeadIconRawImage").Find("Panel").Find("CharacterTypeTMP").GetComponent<TextMeshProUGUI>();

            shieldSlider = transform.Find("ShieldSlider").GetComponent<Slider>();
            dmgSlider = transform.Find("DamageSlider").GetComponent<Slider>();


            shieldValTMP = shieldSlider.transform.Find("Fill Area").Find("Fill").Find("ValTMP").GetComponent<TextMeshProUGUI>();
            dmgValTMP = dmgSlider.transform.Find("Fill Area").Find("Fill").Find("ValTMP").GetComponent<TextMeshProUGUI>();
        }
        // 记得active设为true
        public void Inject(int rank, CombatOperator cop, int totalDmg, int totalReceive)
        {
            DOVirtual.DelayedCall(0.1f * rank, () =>
            {
                gameObject.SetActive(true);
                nameTMP.text = cop.OpInfo.Name;
                typeNameTMP.text = cop.OpInfo.Type.ToString();
                shieldSlider.value = 0;
                dmgSlider.value = 0;
                shieldValTMP.text = cop.StatReceiveDamage.ToString();
                dmgValTMP.text = cop.StatCauseDamage.ToString();
                var texture = ResourceManager.LoadIcon(cop.OpInfo.ModelResourceUrl);
                if (texture != null) HeadIconRawImg.texture = texture;
                if (totalReceive != 0) shieldSlider.DOValue((float)cop.StatReceiveDamage / totalReceive, 1);
                if(totalDmg != 0) dmgSlider.DOValue((float)cop.StatCauseDamage / totalDmg, 1);
            });
        }
    }
}
