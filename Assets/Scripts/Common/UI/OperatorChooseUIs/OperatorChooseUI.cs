using Assets.Scripts.Entities;
using DG.Tweening;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Common.UI.OperatorChooseUIs
{
    internal class OperatorChooseUI : MonoBehaviour
    {
        public static async Task<Operator> ChooseOperator(IEnumerable<Operator> operators)
        {
            await Task.Delay(20);
            var comp = Instantiate(ResourceManager.Load<GameObject>("UIs/OperatorChooseCanvas"))
                .transform.GetChild(0)
                .GetComponent<OperatorChooseUI>();
            return await comp.ChooseOperatorInner(operators);
        }

        Operator choseOp = null;

        public async Task<Operator> ChooseOperatorInner(IEnumerable<Operator> operators)
        {
            // 1.创建子UI
            transform.Find("Scroll View").GetComponent<OperatorChooseScrollViewUI>().Inject(this, operators);

            // 2.等待finish
            while (choseOp is null) await Task.Delay(20);

            // 3.过渡消失
            GetComponent<CanvasGroup>().DOFade(0, 0.5f).OnComplete(() => Destroy(transform.parent.gameObject));
            return choseOp;
        }

        public void Choose(Operator op)
        {
            choseOp = op;
        }
    }
}
