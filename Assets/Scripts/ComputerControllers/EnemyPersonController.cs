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
            IdleMoving,
            Alert,
            AlertMoving,
            Shooting
        }
        private State _state;
        private Vector3 _instantiatePosition;
        private MyGun _gun = new MyGun() { MuzzleVelocity = 1200, RateOfFile = 100 };
        protected override void Start()
        {
            //MoveOnce(new Vector3(0, transform.position.y, 0), 3);
            //Aim(new Vector3(0, 0, 0));
            base.Start();
            _state = State.Idle;
            _gameInformationManager.OnEnemyEngageEvent += OnEngage;
            _instantiatePosition = transform.position;
        }

        private void Update()
        {
            var msg = TryFindCounters(_gameInformationManager.PlayerTeamTrans);
            TransferState(msg);
        }

        private void OnEngage(Transform trans, Vector3 poi)
        {
            if (trans != null && trans == transform) return;
            TransferState(new FoundMsg { Found = true, FoundPos = poi, FromSelf = false });
        }

        float _idleDelta = 0;
        private void TransferState(FoundMsg msg)
        {
            if (_state == State.Idle)
            {
                if (msg.Found)
                {
                    _state = State.Alert;
                    _gameInformationManager.EnemyFindCounter(transform, msg.FoundPos);
                }
                else
                {
                    _idleDelta += Time.deltaTime;
                    if(_idleDelta > 1)
                    {
                        RandomMove();
                        _idleDelta = 0;
                        _state = State.IdleMoving;
                    }
                }
            }
            else if(_state == State.IdleMoving)
            {
                if (msg.Found)
                {
                    StopMoving();
                    _state = State.Alert;
                    _gameInformationManager.EnemyFindCounter(transform, msg.FoundPos);
                }
                else if (!Moving)
                {
                    _state = State.Idle;
                }
            } 
            else if (_state == State.Alert)
            {
                if (msg.Found)
                {
                    Aim(msg.FoundPos);
                    _state = State.Shooting;
                }
            }
            else if (_state == State.Shooting)
            {
                if (!msg.FromSelf) return;
                if (msg.Found)
                {
                    Aim(msg.FoundPos);
                    Shoot(_gun, new Vector3(msg.FoundPos.x, 0.8f, msg.FoundPos.z));
                }
                else
                {
                    _state = State.Alert;
                }
            }
        }

        private float MoveRadius = 2.0f;
        private void RandomMove()
        {
            Vector3 moveVec = new Vector3(UnityEngine.Random.Range(-1, 1), 0, UnityEngine.Random.Range(-1, 1));
            moveVec = moveVec.normalized* MoveRadius;
            MoveOnce(_instantiatePosition + moveVec, 2.0f);
        }
        

    }
}
