using Assets.Scripts.CombatLogic.CombatEntities;
using Assets.Scripts.CombatLogic.ContextExtends;
using Assets.Scripts.Entities;
using DG.Tweening;
using Lovatto.MiniMap;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts.CombatLogic.UILogic.MiniMap
{
    /// <summary>
    /// 大部分代码复制自bl_MiniMapIcon类，主要修改互动显示
    /// </summary>
    internal class MyMinimapIcon : bl_MiniMapIconBase, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
    {

        #region Public members
        [Header("SETTINGS")]
        public float DestroyIn = 5f;
        [Header("REFERENCES")]
        public Image TargetGraphic;
        [SerializeField] private RectTransform CircleAreaRect = null;
        public Sprite DeathIcon = null;
        public GameObject textAreaBackground;
        [SerializeField] private Text InfoText = null;
        public CanvasGroup m_CanvasGroup;
        public CanvasGroup infoAlpha;
        public Canvas m_VerticleCanvas;
        #endregion

        #region Private members
        private Animator Anim;
        private float delay = 0.1f;
        private bl_MiniMapMaskHandler MaskHelper = null;
        private bl_MiniMapEntityBase miniMapItem;
        private bool isTextOpen = false;
        private float maxOpacity = 1;
        #endregion

        #region Public properties
        public RectTransform textRect { get; private set; }
        public override Image GetImage => TargetGraphic;

        public override float Opacity
        {
            get => m_CanvasGroup.alpha; set
            {
                m_CanvasGroup.alpha = value * maxOpacity;
            }
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        void Awake()
        {
            //Get the canvas group or add one if nt have.
            if (m_CanvasGroup == null)
            {
                m_CanvasGroup = GetComponent<CanvasGroup>();
            }
            if (GetComponent<Animator>() != null)
            {
                Anim = GetComponent<Animator>();
            }
            if (Anim != null) { Anim.enabled = false; }
            if (textAreaBackground != null) { textRect = textAreaBackground.GetComponent<RectTransform>(); textAreaBackground.SetActive(false); }
            m_CanvasGroup.alpha = 0;
            if (CircleAreaRect != null) { CircleAreaRect.gameObject.SetActive(false); }
        }
        private CombatContextManager _context => CombatContextManager.Instance;
        RectTransform panel;
        Vector2 panelInitSize;
        public void Inject(Transform trans, bool isPlayer)
        {
            if(isPlayer)  transform.Find("CanvasIcon").Find("Vision").gameObject.SetActive(true);
            panel = m_VerticleCanvas.transform.Find("Panel").GetComponent<RectTransform>();
            panelInitSize = panel.sizeDelta;

            if (_context.Operators.ContainsKey(trans))
            {
                opTrans = trans;
                inject(_context.Operators[trans]);
            }
            else if (_context.Fighters.ContainsKey(trans)) inject(_context.Fighters[trans].Model, _context.Fighters[trans].Team);

            m_VerticleCanvas.gameObject.SetActive(false);
        }

        Transform opTrans;
        Slider hpSlider;
        private void inject(CombatOperator cop)
        {
            var ptrans = m_VerticleCanvas.transform.Find("Panel");
            if (cop.Team == 0)
            {
                ptrans.Find("enemyHDRPanel").gameObject.SetActive(false);
            }
            else
            {
                ptrans.Find("teamHDRPanel").gameObject.SetActive(false);
            }
            hpSlider = ptrans.Find("CHPSlider").GetComponent<Slider>();
            var nameTMP = ptrans.Find("CNameTMP").GetComponent<TextMeshProUGUI>();
            var HeadIconRawImg = ptrans.Find("CHeadIconRawImg").GetComponent<RawImage>();
            var typeNameTMP = ptrans.Find("CHeadIconRawImg").Find("Panel").Find("CharacterTypeTMP").GetComponent<TextMeshProUGUI>();

            hpSlider.value = cop.CurrentHP / cop.MaxHP;
            nameTMP.text = cop.OpInfo.Name;
            var texture = ResourceManager.LoadIcon(cop.OpInfo.ModelResourceUrl);
            if (texture != null) HeadIconRawImg.texture = texture;
            typeNameTMP.text = cop.OpInfo.Type.ToString();
        }
        private void inject(Fighter fighter, int team)
        {
            var ptrans = m_VerticleCanvas.transform.Find("Panel");
            if (team == 0)
            {
                ptrans.Find("enemyHDRPanel").gameObject.SetActive(false);
            }
            else
            {
                ptrans.Find("teamHDRPanel").gameObject.SetActive(false);
            }
            var hpSlider = ptrans.Find("CHPSlider").GetComponent<Slider>();
            var nameTMP = ptrans.Find("CNameTMP").GetComponent<TextMeshProUGUI>();
            var HeadIconRawImg = ptrans.Find("CHeadIconRawImg").GetComponent<RawImage>();
            var typeNameTMP = ptrans.Find("CHeadIconRawImg").Find("Panel").Find("CharacterTypeTMP").GetComponent<TextMeshProUGUI>();

            hpSlider.value = 1;
            nameTMP.text = fighter.Operator.Name;
            var texture = ResourceManager.LoadIcon(fighter.Operator.ModelResourceUrl);
            if (texture != null) HeadIconRawImg.texture = texture;
            typeNameTMP.text = fighter.Type.To2WordsString();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (miniMapItem == null) return;
            if (!miniMapItem.IsInteractable || miniMapItem.InteractAction != bl_MiniMapEntityBase.InteracableAction.OnHover) return;

            m_VerticleCanvas.gameObject.SetActive(true);
            panel.sizeDelta = new Vector2(0, panelInitSize.y);
            panel.DOSizeDelta(panelInitSize, 0.3f);

            m_VerticleCanvas.transform.position = TargetGraphic.transform.position;
            if (opTrans != null) hpSlider.value = (float)_context.Operators[opTrans].CurrentHP / _context.Operators[opTrans].MaxHP;

        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (miniMapItem == null) return;
            if(opTrans != null)
            {
                _context.SwithAgentWithPlayer(opTrans);
            }
            if (!miniMapItem.IsInteractable || miniMapItem.InteractAction != bl_MiniMapEntityBase.InteracableAction.OnTouch) return;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            m_VerticleCanvas.gameObject.SetActive(false);
            
            if (miniMapItem == null) return;
            if (!miniMapItem.IsInteractable || miniMapItem.InteractAction != bl_MiniMapEntityBase.InteracableAction.OnHover) return;
        }


        private void Update()
        {
            if (m_VerticleCanvas.enabled)
            {
                m_VerticleCanvas.transform.position += new Vector3(0, 0.001f, 0) * Mathf.Sin(Time.time) * Time.deltaTime;
            }
        }

        #region default logic
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public override void SetUp(bl_MiniMapEntityBase item)
        {
            miniMapItem = item;
            m_CanvasGroup.interactable = miniMapItem.IsInteractable;
        }

        /// <summary>
        /// When player or the target die,desactive,remove,etc..
        /// call this for remove the item UI from Map
        /// for change to other icon and desactive in certain time
        /// or destroy immediate
        /// </summary>
        /// <param name="inmediate"></param>
        /// <param name="death"></param>
        public override void DestroyIcon(bool inmediate, Sprite death)
        {
            if (inmediate)
            {
                Destroy(gameObject);
            }
            else
            {
                //Change the sprite to icon death
                TargetGraphic.sprite = death == null ? DeathIcon : death;
                //destroy in 5 seconds
                Destroy(gameObject, DestroyIn);
            }
        }
        /// <summary>
        /// Get info to display
        /// </summary>
        /// <param name="info"></param>
        public override void SetText(string info)
        {
            if (InfoText == null)
                return;

            InfoText.text = info;
            if (textAreaBackground != null)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(textRect);
                LayoutRebuilder.ForceRebuildLayoutImmediate(InfoText.GetComponent<RectTransform>());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ico"></param>
        public override void SetIcon(Sprite ico)
        {
            TargetGraphic.sprite = ico;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newColor"></param>
        public override void SetColor(Color newColor)
        {
            TargetGraphic.color = newColor;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="active"></param>
        public override void SetActive(bool active)
        {
            gameObject.SetActive(active);
        }

        /// <summary>
        /// Show a visible circle area in the minimap with this
        /// item as center
        /// </summary>
        /// <param name="radius"></param>
        /// <param name="AreaColor"></param>
        public override RectTransform SetCircleArea(float radius, Color AreaColor)
        {
            if (CircleAreaRect == null) { return null; }

            var miniMap = bl_MiniMapUtils.GetMiniMap();
            MaskHelper = miniMap.MiniMapUI.minimapMaskManager;
            MaskHelper.SetMaskedIcon(CircleAreaRect);
            radius = radius * 10;
            radius = radius * miniMap.IconMultiplier;
            Vector2 r = new Vector2(radius, radius);
            CircleAreaRect.sizeDelta = r;
            CircleAreaRect.GetComponent<Image>().CrossFadeColor(AreaColor, 1, true, true);
            CircleAreaRect.gameObject.SetActive(true);

            return CircleAreaRect;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="active"></param>
        public override void SetActiveCircleArea(bool active)
        {
            if (active)
            {
                MaskHelper.SetMaskedIcon(CircleAreaRect);
                CircleAreaRect.gameObject.SetActive(true);
            }
            else
            {
                CircleAreaRect.SetParent(transform);
                CircleAreaRect.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="opacity"></param>
        public override void SetOpacity(float opacity)
        {
            if (m_CanvasGroup == null) return;

            maxOpacity = opacity;
            m_CanvasGroup.alpha = opacity;
        }

        /// <summary>
        /// 
        /// </summary>
        public override void ForceFaceUp()
        {
            if (textAreaBackground != null)
            {
                textRect.up = Vector3.up;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerator FadeIcon()
        {
            yield return new WaitForSeconds(delay);
            float d = 0;
            while (d < 1)
            {
                d += Time.deltaTime * 2;
                m_CanvasGroup.alpha = maxOpacity * d;
                yield return null;
            }
            if (Anim != null) { Anim.enabled = true; }
        }




        /// <summary>
        /// 
        /// </summary>
        IEnumerator FadeInfo(bool fadeOut)
        {
            if (!fadeOut) { textAreaBackground.SetActive(true); }
            float d = 0;
            while (d < 1)
            {
                d += Time.deltaTime * 4;
                if (fadeOut)
                {
                    infoAlpha.alpha = Mathf.Lerp(1, 0, d);
                }
                else
                {
                    infoAlpha.alpha = Mathf.Lerp(0, 1, d);
                }
                yield return null;
            }
            if (fadeOut) { textAreaBackground.SetActive(false); }
        }

        public override void SpawnedDelayed(float v) { delay = v; StartCoroutine(FadeIcon()); }
        #endregion
    }
}
