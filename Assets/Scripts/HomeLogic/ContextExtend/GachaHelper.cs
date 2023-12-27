using Assets.Scripts.Common;
using Assets.Scripts.Entities;
using Assets.Scripts.Entities.Buildings;
using Assets.Scripts.Entities.Mechas;
using Assets.Scripts.HomeLogic.Environment;
using Assets.Scripts.HomeLogic.UILogic;
using Assets.Scripts.Services;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.HomeLogic.ContextExtend
{
    internal enum GachaType
    {
        SimpleCharacter,
        ExpensiveCharacter,
        SimpleMecha,
        ExpensiveMecha,
    }
    internal enum GachaRarity
    {
        Low, Middle, High
    }
    internal static class GachaHelper
    {
        public static void DoGacha(this HomeContextManager context, GachaType gacha) {
            if (IsCanGacha(context, gacha) is false) {
                Debug.LogWarning("Cannot afford, use IsCanGacha first");
                return;
            }
            context.Afford(getMatchCostList(gacha));
            if (gacha == GachaType.SimpleCharacter)
            {
                var op = gachaSimpleCharacter();
                MyServices.Database.Operators.Add(op);
                context.HomeVM.OperatorListDirtyMark = true;

                // 动画
                GachaViewManager.Instance.GachaCharacterAnime(context, op, UIManager.Instance);
            }
            else if (gacha == GachaType.ExpensiveCharacter)
            {
                var op = gachaExpensiveCharacter();
                MyServices.Database.Operators.Add(op);
                context.HomeVM.OperatorListDirtyMark = true;

                // TODO: 动画(根据op属性变动动画效果？
                GachaViewManager.Instance.GachaCharacterAnime(context, op, UIManager.Instance);

            }
            else if (gacha == GachaType.SimpleMecha)
            {
                var mecha = gachaSimpleMecha();
                MyServices.Database.Mechas.Add(mecha);
                // TODO: 动画, UI
            }
            else if (gacha == GachaType.ExpensiveMecha)
            {
                var mecha = gachaExpensiveMecha();
                MyServices.Database.Mechas.Add(mecha);
                // TODO: 动画, UI
            }
        }

        public static bool IsCanGacha(this HomeContextManager context, GachaType gacha)
        {
            return context.IsCanAfford(getMatchCostList(gacha));
        }

        public static string GetGachaFailedTips(this HomeContextManager context, GachaType gacha)
        {
            var cost = getMatchCostList(gacha);

            var sb = new StringBuilder();
            sb.Append("资源不足，需要：");
            foreach(var x in cost)
            {
                sb.Append($"{ItemHelper.GetItem(x.ItemId).ItemName}*{x.Amount}, ");
            }
            return sb.ToString();
        }

        public static string GetGachaNeed(this HomeContextManager context, GachaType gacha)
        {
            var cost = getMatchCostList(gacha);
            var sb = new StringBuilder();
            foreach (var x in cost)
            {
                sb.Append($"{x.Amount}\n");
            }
            return sb.ToString();
        }

        private static IList<Produce> getMatchCostList(GachaType gacha)
        {
            IList<Produce> temp;
            if (gacha == GachaType.SimpleCharacter) temp = MyConfig.SimpleCharacterCost;
            else if (gacha == GachaType.ExpensiveCharacter) temp = MyConfig.ExpensiveCharacterCost;
            else if (gacha == GachaType.SimpleMecha) temp = MyConfig.SimpleMechaCost;
            else if (gacha == GachaType.ExpensiveMecha) temp = MyConfig.ExpensiveMechaCost;
            else
            {
                Debug.LogWarning("GachaType can not load a expect value");
                return null;
            }
            return temp;
        }

        private static Operator gachaSimpleCharacter()
        {
            Operator op;
            if (Random.Range(0f, 1f) < 0.1f) // 10 %的概率抽到CV
            {
                op = new Operator
                {
                    Name = "CV_" + MyConfig.NameList[Random.Range(0, MyConfig.NameList.Count)].ToString(),
                    ModelResourceUrl = MyServices.Database.ModelList[Random.Range(0, MyServices.Database.ModelList.Count)],
                    WeaponSkillId = 6,
                    Type = OperatorType.CV,
                    Id = (MyServices.Database.Operators.Count + 1).ToString(),
                };
            }
            else
            {
                op = new Operator
                {
                    Name = "CA_" + MyConfig.NameList[Random.Range(0, MyConfig.NameList.Count)].ToString(),
                    ModelResourceUrl = MyServices.Database.ModelList[Random.Range(0, MyServices.Database.ModelList.Count)],
                    WeaponSkillId = 4,
                    Type = OperatorType.CA,
                    Id = (MyServices.Database.Operators.Count + 1).ToString(),
                };
            }
            op.PropGreen = Random.Range(1, 4);
            op.PropRed = Random.Range(1, 4);
            op.PropBlue = Random.Range(1, 4);

            return op;
        }
        private static Operator gachaExpensiveCharacter()
        {
            Operator op;
            if (Random.Range(0f, 1f) < 0.15f) // 15 %的概率抽到CV
            {
                op = new Operator
                {
                    Name = "CV_" + MyConfig.NameList[Random.Range(0, MyConfig.NameList.Count)].ToString(),
                    ModelResourceUrl = MyServices.Database.ModelList[Random.Range(0, MyServices.Database.ModelList.Count)],
                    WeaponSkillId = 6,
                    Type = OperatorType.CV,
                    Id = (MyServices.Database.Operators.Count + 1).ToString(),
                };
            }
            else
            {
                op = new Operator
                {
                    Name = "CA_" + MyConfig.NameList[Random.Range(0, MyConfig.NameList.Count)].ToString(),
                    ModelResourceUrl = MyServices.Database.ModelList[Random.Range(0, MyServices.Database.ModelList.Count)],
                    WeaponSkillId = 4,
                    Type = OperatorType.CA,
                    Id = (MyServices.Database.Operators.Count + 1).ToString(),
                };
            }
            op.PropGreen = Random.Range(3, 8);
            op.PropRed = Random.Range(3, 8);
            op.PropBlue = Random.Range(3, 8);

            return op;
        }
        private static MechaBase gachaSimpleMecha()
        {
            var r = Random.Range(0f, 1f);
            var mechaId = MyServices.Database.Mechas.Count + 1;
            if (r < 0.33f)
            {
                return new MechaLeg(name: "Leg II", "leg2", speed: Random.Range(3f, 4.5f), dodge: Random.Range(5, 11), mechaId);
            }
            else if(r < 0.66f)
            {
                return new MechaHead("Head II", "head2", critical: Random.Range(5, 11), accurate: Random.Range(5, 11), mechaId);
            }
            else
            {
                return new MechaBody("Body II", "body2", hp: Random.Range(10, 16), hpRecover: Random.Range(1, 4), mechaId);
            }
        }

        private static MechaBase gachaExpensiveMecha()
        {
            var r = Random.Range(0f, 1f);
            var mechaId = MyServices.Database.Mechas.Count + 1;
            if (r < 0.33f)
            {
                return new MechaLeg(name: "Leg II", "leg2", speed: Random.Range(5f, 6f), dodge: Random.Range(10, 26), mechaId);
            }
            else if (r < 0.66f)
            {
                return new MechaHead("Head II", "head2", critical: Random.Range(15, 36), accurate: Random.Range(15, 36), mechaId);
            }
            else
            {
                return new MechaBody("Body II", "body2", hp: Random.Range(15, 31), hpRecover: Random.Range(2, 6), mechaId);
            }
        }

    }
}
