using DG.Tweening;
using UnityEngine;

namespace Assets.Scripts.CombatLogic.EnviormentLogic
{
    internal class MvpDisplayerController : MonoBehaviour
    {
        Transform modelRootTrans;
        Transform vcTrans;
        Animator animator;

        private int _animeInitID;

        private void Awake()
        {
            modelRootTrans = transform.Find("modelroot");
            vcTrans = transform.Find("VirtualCamera");
            animator = modelRootTrans.GetComponent<Animator>();
            _animeInitID = Animator.StringToHash("FirstInit");
            this.enabled = false;
            DOVirtual.DelayedCall(0.7f, () => this.enabled = true); // 在相机blend快要结束的时候开始动作
        }
        private void Start()
        {
            animator.SetTrigger(_animeInitID);
            var angle = modelRootTrans.eulerAngles;
            angle.y = vcTrans.eulerAngles.y + 180;
            modelRootTrans.DORotate(angle, 0.5f);
        }
    }
}
