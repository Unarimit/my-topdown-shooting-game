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
        enum State
        {
            Idle,
            Find,
            Shooting
        }
        private State _state;
        private Vector3 _instantiatePosition;
        protected override void Start()
        {
            //MoveOnce(new Vector3(0, transform.position.y, 0), 3);
            //Aim(new Vector3(0, 0, 0));
            base.Start();
            _state = State.Idle;
            _instantiatePosition = transform.position;

            if(_gunController != null)
            {
                _gunController.gunProperty.MuzzleVelocity = 1200;
                _gunController.gunProperty.RateOfFile = 300;
                _gunController.gunProperty.CurrentAmmo = 10;
                _gunController.gunProperty.MaxAmmo = 10;
            }
        }

        private void OnDestroy()
        {

        }

        private void Update()
        {
            TransferState();
            baseUpdate();
        }
        private Vector3 aimPos;
        private Transform aimTran;
        float _idleDelta = 0;
        private void TransferState()
        {
            if (_state == State.Idle)
            {
                aimPos = TryFindAim();
                if (aimPos != null)
                {
                    _state = State.Find;
                }
                else
                {
                    _idleDelta += Time.deltaTime;
                    if (_idleDelta > 1)
                    {
                        _idleDelta = 0;
                        RandomMove();
                    }
                }
            }
            else if(_state == State.Find)
            {
                MoveOnce(aimPos, 3);
                var msg = TrySeeCounters(_context.PlayerTeamTrans);
                if (msg.Found == true)
                {
                    transform.LookAt(msg.FoundPos);
                    aimTran = msg.FoundTrans;
                    _state = State.Shooting;
                }
                else if (!Moving)
                {
                    _state = State.Idle;
                }
            } 
            else if (_state == State.Shooting)
            {
                StopMoving();
                if (TrySeeAim(aimTran))
                {
                    Aim(aimTran.position);
                    Shoot(new Vector3(aimTran.position.x, 0.8f, aimTran.position.z));
                    _state = State.Shooting;
                }
                else
                {
                    _state = State.Find;
                }
            }
        }

        private float MoveRadius = 2.0f;
        private void RandomMove()
        {
            Vector3 moveVec = new Vector3(UnityEngine.Random.Range(-1, 1), 0, UnityEngine.Random.Range(-1, 1));
            moveVec = moveVec.normalized* MoveRadius;
            MoveOnce(new Vector3(_instantiatePosition.x, transform.position.y, _instantiatePosition.z) + moveVec, 2.0f);
        }

        private Vector3 TryFindAim()
        {
            return _context.PlayerTrans.position;
        }
    }
}
