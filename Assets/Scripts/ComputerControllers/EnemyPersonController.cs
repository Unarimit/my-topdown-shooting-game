using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Scripts.ComputerControllers
{
    
    public class EnemyPersonController : PersonController
    {
        enum Statu
        {
            Idle,
            IdleMoving,
            Alert,
            AlertMoving,
            Shooting
        }
        private Statu _statu;
        private MyGun gun = new MyGun() { MuzzleVelocity = 1200, RateOfFile = 100 };
        public override void Start()
        {
            //MoveOnce(new Vector3(0, transform.position.y, 0), 3);
            //Aim(new Vector3(0, 0, 0));
            base.Start();
            _statu = Statu.Idle;
        }

        private void Update()
        {
            var msg = TryFind();
            if (_statu == Statu.Idle)
            {
                if(msg.Found) _statu = Statu.Alert;
            }
            else if(_statu == Statu.Alert)
            {
                if (msg.Found)
                {
                    Aim(msg.FoundPos);
                    _statu = Statu.Shooting;
                } 
            }
            else if (_statu == Statu.Shooting)
            {
                if (msg.Found) {
                    Aim(msg.FoundPos);
                    Shoot(gun, msg.FoundPos + new Vector3(0,0.5f,0));
                }
                else
                {
                    _statu = Statu.Alert;
                }
            }
        }

        struct FoundMsg
        {
            public bool Found;
            public Vector3 FoundPos;
        }
        /// <summary>
        /// 尝试发现敌人
        /// </summary>
        /// <returns></returns>
        private FoundMsg TryFind()
        {
            var forward = transform.forward;
            foreach(var x in _gameInformationManager.PlayerTeamTrans)
            {
                var vec = x.position - transform.position;
                if(Vector3.Angle(forward, vec) < 30) // in my eyes
                {
                    // it is in my eyes
                    Ray ray = new Ray(transform.position, x.position);
                    var hits = Physics.RaycastAll(ray, vec.magnitude, LayerMask.GetMask(new string[] { "Obstacle" }));
                    if(hits.Length == 0)
                    {
                        return new FoundMsg { Found = true,  FoundPos = x.position};
                    }
                }
            }
            return new FoundMsg { Found = false };
        }

    }
}
