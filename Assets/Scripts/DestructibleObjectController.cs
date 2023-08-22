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
        private int HP = 10;
        private void OnCollisionEnter(Collision collision)
        {
            if(collision.transform.tag == "Bullet")
            {
                HP -= 1;
            }
            if (HP <= 0) Destroy(gameObject);
        }
    }
}
