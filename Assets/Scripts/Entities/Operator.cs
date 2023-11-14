using Assets.Scripts.Entities.Mechas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Entities
{
    /// <summary>
    /// 要存入数据库的信息
    /// </summary>
    [Serializable]
    public class Operator
    {
        public string Name = "empty";

        public string ModelResourceUrl;

        public int PropRed;

        public int PropGreen;

        public int PropBlue;


        // 需要删除的属性
        public int HP;

        public int RecoverHP;

        public int WeaponType;

        public float ReviveTime = 5;

        // 对于装备是引用关系
        public MechaHead McHead;
        public MechaBody McBody;
        public MechaLeg McLeg;

    }
}
