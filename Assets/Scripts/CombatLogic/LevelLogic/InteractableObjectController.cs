using Assets.Scripts.CombatLogic.Characters.Player;
using DG.Tweening;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.CombatLogic.LevelLogic
{
    internal class InteractableObjectController : MonoBehaviour
    {
        // component
        RectTransform _canvasTrans;
        CanvasGroup _canvasGroup;
        Slider _slider;
        GameObject _tipsGO;

        // field
        private float _duration = 1;
        private float _curDuration = 0;
        private string _interId;

        /// <summary>
        /// 创建一个互动物体
        /// </summary>
        /// <param name="interText">互动提示词</param>
        /// <param name="duration">互动需要持续时间</param>
        /// <returns></returns>
        public static Transform CreateInteractableObject(string interText, float duration, string interId)
        {
            var prefab = ResourceManager.Load<GameObject>("Characters/InteractableObject");
            var go = Instantiate(prefab, CombatContextManager.Instance.Enviorment);
            go.GetComponent<InteractableObjectController>().Init(interText, duration, interId);
            return go.transform;
        }

        public void Init(string interText, float duration, string interId)
        {
            _duration = duration;
            _interId = interId;
            if (interText != null) _tipsGO.transform.Find("TextTMP").GetComponent<TextMeshProUGUI>().text = interText;
        }

        private void Awake()
        {
            _canvasTrans = transform.Find("Canvas").GetComponent<RectTransform>();
            _canvasGroup = transform.Find("Canvas").GetComponent<CanvasGroup>();
            _slider = _canvasTrans.Find("Panel").Find("Slider").GetComponent<Slider>();
            _tipsGO = _canvasTrans.Find("Panel").Find("Tips").gameObject;
            _slider.gameObject.SetActive(false);
            _tipsGO.SetActive(true);
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == TestDB.PLAYER_TAG)
            {
                showInteractTip();
                other.gameObject.GetComponent<PlayerController>().InteractEventHandler += doInteract;
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.tag == TestDB.PLAYER_TAG)
            {
                closeInteractTip();
                other.gameObject.GetComponent<PlayerController>().InteractEventHandler -= doInteract;
            }
        }
        private void showInteractTip()
        {
            _canvasTrans.gameObject.SetActive(true);
            _canvasTrans.rotation = Quaternion.LookRotation(Camera.main.transform.forward);
            _canvasGroup.alpha = 0;
            _canvasGroup.DOFade(1, 0.2f);
        }
        private void closeInteractTip()
        {
            _canvasGroup.DOFade(0, 0.2f).OnComplete(() => {
                _canvasTrans.gameObject.SetActive(false);
            });

        }
        private void doInteract()
        {
            _tipsGO.SetActive(false);
            _slider.gameObject.SetActive(true);
            if(_curDuration > _duration)
            {
                finishInteract();
            }
            if (_duration != 0)
            {
                _curDuration += Time.deltaTime;
                _slider.value = _curDuration / _duration;
            }
            
        }
        private void finishInteract()
        {
            gameObject.SetActive(false);
            GameLevelManager.Instance.FinishInteract(_interId);
        }
    }
}
