using Assets.Scripts.CombatLogic.CombatEntities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Unity.Burst.Intrinsics.X86.Avx;

namespace Assets.Scripts.CombatLogic.UILogic.StrategyMap
{
    internal class SmOperatorScrollViewItemUI : MonoBehaviour
    {
        // 基础信息
        TextMeshProUGUI nameTMP;
        TextMeshProUGUI typeNameTMP;
        RawImage HeadIconRawImg;

        // 死亡提示：OFFLINE,REVIVING等
        TextMeshProUGUI StatuTMP;

        // 血条
        Slider shieldSlider;
        TextMeshProUGUI shieldValTMP;

        private void Awake()
        {
            nameTMP = transform.Find("CNameTMP").GetComponent<TextMeshProUGUI>();
            HeadIconRawImg = transform.Find("HeadIconRawImage").GetComponent<RawImage>();
            typeNameTMP = transform.Find("HeadIconRawImage").Find("Panel").Find("CharacterTypeTMP").GetComponent<TextMeshProUGUI>();

            StatuTMP = transform.Find("StatuTextTMP").GetComponent<TextMeshProUGUI>();

            shieldSlider = transform.Find("HPSlider").GetComponent<Slider>();
            shieldValTMP = shieldSlider.transform.Find("Fill Area").Find("Fill").Find("ValTMP").GetComponent<TextMeshProUGUI>();
        }
        internal void Inject(CombatOperator cop)
        {
            gameObject.SetActive(true);

            // 基础信息
            nameTMP.text = cop.OpInfo.Name;
            typeNameTMP.text = cop.OpInfo.Type.ToString();
            var texture = ResourceManager.LoadModelHeadIcon(cop.OpInfo.ModelResourceUrl);
            if (texture != null) HeadIconRawImg.texture = texture;

            // 血条
            if (cop.CurrentHP != 0)
            {
                shieldSlider.gameObject.SetActive(true);
                StatuTMP.gameObject.SetActive(false);
                shieldSlider.value = (float) cop.CurrentHP / cop.MaxHP;
                shieldValTMP.text = cop.CurrentHP.ToString();
            }
            else // 或死亡提示
            {
                shieldSlider.gameObject.SetActive(false);
                StatuTMP.gameObject.SetActive(true);
            }


        }
    }
}
