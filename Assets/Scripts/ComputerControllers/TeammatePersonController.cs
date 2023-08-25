using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts.ComputerControllers
{
    public class TeammatePersonController : PersonController
    {
        enum Status
        {
            Idle,
            Moving,
            Shooting
        }
        private Status _statu;
        private MyGun _gun = new MyGun() { MuzzleVelocity = 1200, RateOfFile = 100 };
        protected override void Start()
        {
            base.Start();
            base.FindAngle = 180;
            _statu = Status.Idle;
        }

        private void Update()
        {
            var findMsg = base.TryFindCounters(_gameInformationManager.EnemyTeamTrans);
            if(_statu == Status.Idle)
            {
                if (findMsg.Found)
                {
                    base.Aim(findMsg.FoundPos);
                    _statu = Status.Shooting;
                }
                else
                {
                    TeammateMove();
                    _statu = Status.Moving;
                }
            }
            else if(_statu == Status.Moving)
            {
                if (!Moving) _statu = Status.Idle;
                if (findMsg.Found)
                {
                    base.StopMoving();
                    base.Aim(findMsg.FoundPos);
                    _statu = Status.Shooting;
                }
                else
                {
                    TeammateMove();
                }
            }
            else if (_statu == Status.Shooting)
            {
                if (findMsg.Found)
                {
                    base.Aim(findMsg.FoundPos);
                    base.Shoot(_gun, findMsg.FoundPos);
                }
                else
                {
                    _statu = Status.Idle;
                    base.StopAimming();
                }
                
            }
        }

        private float KeepDistance = 2f;
        private float MoveDistance = 3f;
        private float FollowSpeed = 6f;
        private float ForwardSpeed = 2f;
        private void TeammateMove()
        {
            switch (_gameInformationManager.TeammateStatu)
            {
                case GameInformationManager.TeammateStatus.Follow:
                    var vec = _gameInformationManager.PlayerTeamTrans[0].position - transform.position;
                    float moveD = vec.magnitude - KeepDistance;
                    if(moveD > 0) MoveOnce(transform.position + vec.normalized * moveD, FollowSpeed);
                    break;
                case GameInformationManager.TeammateStatus.Forward:
                    MoveOnce(transform.position + transform.forward * MoveDistance, ForwardSpeed);
                    break;
                case GameInformationManager.TeammateStatus.StandBy:
                    base.StopMoving();
                    break;
                default:
                    break;
            }
        }

    }
}
