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
        
        public void GotDMG(int dmg)
        {
            HP -= dmg;
            if (HP <= 0) Destroy(gameObject);
        }
    }
}
