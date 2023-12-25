using Assets.Scripts.CombatLogic.LevelLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class DestructibleObjectController : MonoBehaviour
    {
        public int HP = 10;
        public string DropoutId = null;
        public void GotDMG(int dmg)
        {
            HP -= dmg;
            if (HP <= 0) {
                if (DropoutId != null) { 
                    //GameLevelManager.Instance.
                }
                Destroy(gameObject);
            } 
        }
    }
}
