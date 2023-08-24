using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.ComputerControllers
{
    public class EnemyPersonController : PersonController
    {

        private void Start()
        {
            //MoveOnce(new Vector3(0, transform.position.y, 0), 3);
            Aim(new Vector3(0, 0, 0));
        }

        MyGun gun = new MyGun() { MuzzleVelocity = 1200, RateOfFile = 100 };
        private void Update()
        {
            
            Shoot(gun, new Vector3(0, 0, 0));
        }
    }
}
