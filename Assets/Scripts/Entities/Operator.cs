using Assets.Scripts.Entities.Mechas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Entities
{
    public enum OperatorType
    {
        CA,
        CV
    }
    /// <summary>
    /// 要存入数据库的信息
    /// </summary>
    [Serializable]
    public class Operator
    {
        public OperatorType Type = OperatorType.CA;
        public string Name = "empty";

        public string ModelResourceUrl;

        // 属性
        public int PropRed;

        public int PropGreen;

        public int PropBlue;

        // 技能id
        public int WeaponSkillId = 4;

        public int MainSkillId = 0;

        public int SlideSkillId = 7;

        public List<Fighter> Fighters;

        // 需要删除的属性
        public int RecoverHP => McBody.HPRecover;

        public float MaxSpeed => McLeg.Speed;

        public int WeaponType;

        public float ReviveTime = 5;

        // 对于装备是引用关系
        public MechaHead McHead = MechaHead.DefaultMecha();
        public MechaBody McBody = MechaBody.DefaultMecha();
        public MechaLeg McLeg = MechaLeg.DefaultMecha();

    }
}
