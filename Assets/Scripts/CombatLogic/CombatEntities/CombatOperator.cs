using Assets.Scripts.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.CombatLogic.CombatEntities
{
    internal class CombatOperator : Operator
    {
        public int CurrentHP { get; set; }

        public int Team { get; set; }

        public Transform Transform { get; set; }
    }
}
