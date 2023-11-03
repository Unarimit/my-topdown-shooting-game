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
        public int HP;

        public int RecoverHP;

        public string Name = "empty";

        public int WeaponType;

        public float ReviveTime = 5;

    }
}
