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
    public class Operator
    {
        public int HP { get; set; }

        public int RecoverHP { get; set; }

        public string Name { get; set; } = "empty";

        public int WeaponType { get; set; }

    }
}
