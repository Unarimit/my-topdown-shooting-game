using Assets.Scripts.CombatLogic.Characters;
using Assets.Scripts.CombatLogic.Characters.Computer.Agent;
using Assets.Scripts.CombatLogic.Characters.Player;
using Assets.Scripts.CombatLogic.CombatEntities;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts.CombatLogic.ContextExtends
{
    static internal class TransformHelper
    {
        public static List<Transform> FindOpTransform(this CombatContextManager context, Func<Transform, bool> condition)
        {
            var ret = new List<Transform>();
            foreach(var x in context.Operators.Keys)
            {
                if (condition(x)) ret.Add(x);
            }
            return ret;
        }
        /// <summary>
        /// 通过停止战斗角色更新的方式停止游戏
        /// </summary>
        /// <param name="context"></param>
        public static void ActiveAllCharacter(this CombatContextManager context, bool isActive)
        {
            foreach(var x in context.Operators.Keys)
            {
                // set update
                x.GetComponent<OperatorController>().enabled = isActive;
                if (context.Operators[x].IsPlayer)
                {
                    x.GetComponent<PlayerController>().enabled = isActive;
                }
                else
                {
                    x.GetComponent<AgentController>().enabled = isActive;
                    if (context.Operators[x].IsDead is false) // disable的时候设置stop会报错
                        x.GetComponent<NavMeshAgent>().isStopped = !isActive;
                }

                // clear anime
                x.GetComponent<OperatorController>().ClearAnimate();
            }
            foreach(var x in context.Fighters.Values)
            {
                x.enabled = isActive;
                x.GetComponent<NavMeshAgent>().enabled = isActive;
            }
            context.enabled = isActive;
        }
        public static void ActiveCharacter(this CombatContextManager context, Transform trans, bool isActive)
        {
            if (context.Operators.ContainsKey(trans))
            {
                // set update
                trans.GetComponent<OperatorController>().enabled = isActive;
                if (context.Operators[trans].IsPlayer)
                {
                    trans.GetComponent<PlayerController>().enabled = isActive;
                }
                else
                {
                    trans.GetComponent<AgentController>().enabled = isActive;
                    if (context.Operators[trans].IsDead is false) // disable的时候设置stop会报错
                        trans.GetComponent<NavMeshAgent>().isStopped = !isActive;
                }

                // clear anime
                trans.GetComponent<OperatorController>().ClearAnimate();
            }
            else if(context.Fighters.ContainsKey(trans))
            {
                context.Fighters[trans].enabled = isActive;
                context.Fighters[trans].GetComponent<NavMeshAgent>().enabled = isActive;
            }
        }



        /// <summary>
        /// 根据CombatOperator获取Transform，TODO:这只是个临时方法！
        /// </summary>
        public static Transform GetTransformByCop(this CombatContextManager context, CombatOperator cop)
        {
            foreach (var pair in context.Operators)
            {
                if (pair.Value == cop) return pair.Key;
            }
            return null;
        }
    }
}
