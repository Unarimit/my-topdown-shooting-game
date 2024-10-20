﻿using System;
using System.Collections.Generic;

namespace Assets.Scripts.CombatLogic.GOAPs.Builders
{
    internal class GOAPActionBuilder
    {
        /// <summary> debug用ActionName </summary>
        public string Name { get; }
        GOAPGraph _graph;

        List<GOAPStatus> condtions = new List<GOAPStatus>();
        List<GOAPStatus> effects = new List<GOAPStatus>();
        List<GOAPActionFactor> factors = new List<GOAPActionFactor>();
        bool isGoal = false;
        GOAPPlan plan = GOAPPlan.Null;
        float cost = 100; // 默认相当大的值
        public GOAPActionBuilder(GOAPGraph graph, string name)
        {
            Name = name;
            _graph = graph;
        }

        /// <summary>
        /// 添加必须满足的条件
        /// </summary>
        public GOAPActionBuilder AddCondition(GOAPStatus status)
        {
            condtions.Add(status);
            return this;
        }

        /// <summary>
        /// 添加影响因子
        /// </summary>
        public GOAPActionBuilder AddFactor(GOAPStatus status, float factor)
        {
            factors.Add(new GOAPActionFactor { Status = status, Factor = factor });
            return this;

        }

        /// <summary>
        /// 添加对世界状态的影响（可能）
        /// </summary>
        public GOAPActionBuilder AddEffect(GOAPStatus status)
        {
            effects.Add(status);
            return this;
        }
        /// <summary>
        /// 添加计划，对应行为树
        /// </summary>
        public GOAPActionBuilder SetPlan(GOAPPlan plan)
        {
            this.plan = plan;
            return this;
        }
        /// <summary>
        /// 设为最终目标
        /// </summary>
        public GOAPActionBuilder SetGoal()
        {
            isGoal = true;
            return this;
        }

        public GOAPActionBuilder SetCost(float cost)
        {
            this.cost = cost;
            return this;
        }

        public void BuildAction()
        {
            uint conditionsBM = 0;
            foreach (GOAPStatus status in condtions)
            {
                if ((int)status >= 32) throw new ArgumentException($"status {status}:{((int)status)} is too big cannot express in uint bitmap");
                conditionsBM |= (uint)1 << (int)status;
            }

            uint effectBM = 0;
            foreach(GOAPStatus status in effects)
            {
                if ((int)status >= 32) throw new ArgumentException($"status {status}:{((int)status)} is too big cannot express in uint bitmap");
                effectBM |= (uint)1 << (int)status;
            }

            if(effects.Count == 0 && isGoal is false)
            {
                throw new ArgumentException("a action is not `Goal` cannot without any effect");
            }

            _graph.Actions.Add(
                new GOAPAction() { 
                    ActionName = Name,
                    Cost = cost,
                    Conditions = conditionsBM,
                    Effects = effectBM,
                    Factors = factors.ToArray(), // 索引方式再考虑考虑，或者实际计算的时候再考虑也行
                    GOAPPlan = plan,
                }
            );
        }
    }
}
