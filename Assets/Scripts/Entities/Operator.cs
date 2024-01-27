using Assets.Scripts.Entities.Mechas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Entities
{
    public enum OperatorType
    {
        CA,
        CV,
        DD
    }
    /// <summary> 人物性格 </summary>
    public enum OperatorTrait
    {
        /// <summary> 激进的 </summary>
        Offensive,
        /// <summary> 战略家 </summary>
        Tactical,
        /// <summary> 胆小的 </summary>
        Timid,
    }
    /// <summary> 占用状态 </summary>
    public enum JobStatus
    {
        Nothing,
        HomeBuilding,
        Fighter
    }
    /// <summary> 人物工作信息 </summary>
    public struct OperatorJob
    {
        public OperatorJob(JobStatus jobStatus, string data)
        {
            JobStatus = jobStatus;
            Data = data;
        }
        public JobStatus JobStatus;
        public string Data;
    }
    /// <summary>
    /// 要存入数据库的信息
    /// </summary>
    [Serializable]
    public class Operator : ICloneable
    {
        /// <summary>
        /// 人物Id，由于生成敌人算法的影响，战斗场景可能出现相同的Id
        /// </summary>
        public string Id;
        public OperatorType Type = OperatorType.CA;
        public string Name = "empty";
        public string ModelResourceUrl;

        // 主要属性
        public int PropRed; // 1-10
        public int PropGreen;
        public int PropBlue;
        /// <summary> 性格 </summary>
        public OperatorTrait Trait;

        // 技能id
        public int WeaponSkillId = 4;
        public int MainSkillId = 0;
        public int SlideSkillId = 7;

        // 体力系统
        /// <summary> 最大体力，体力回复的上限 </summary>
        public int MaxPower = 4;
        /// <summary> 当前体力 </summary>
        public int Power = 3;

        // 工作信息
        /// <summary> 工作状态，如在建筑中或操作舰载机等 </summary>
        public OperatorJob Job = new OperatorJob(JobStatus.Nothing, null);

        // 舰载机，引用关系，数据库应该存key
        public List<Fighter> Fighters;

        // 装备，对于装备是引用关系，绑定事件方便数据库响应
        public MechaHead McHead { 
            get {
                return _mcHead;
            } 
            set {
                MechaChangeEventHandler.Invoke(this, _mcHead, value);
                _mcHead = value;
            }
        }
        public MechaBody McBody
        {
            get{
                return _mcBody;
            }
            set{
                MechaChangeEventHandler.Invoke(this, _mcBody, value);
                _mcBody = value;
            }
        }
        public MechaLeg McLeg {
            get{
                return _mcLeg;
            }
            set{
                MechaChangeEventHandler.Invoke(this, _mcLeg, value);
                _mcLeg = value;
            }
        }
        [NonSerialized]
        private MechaHead _mcHead = MechaHead.DefaultMecha();
        [NonSerialized]
        private MechaBody _mcBody = MechaBody.DefaultMecha();
        [NonSerialized]
        private MechaLeg _mcLeg = MechaLeg.DefaultMecha();

        public delegate void MechaChangeEvent(Operator @this, MechaBase oldMehca, MechaBase newMehca);
        public event MechaChangeEvent MechaChangeEventHandler;


        // 需要考虑删除或替换的属性
        public int RecoverHP => McBody.HPRecover;
        public float MaxSpeed => McLeg.Speed;

        public float ReviveTime => 5 - ((float)PropGreen / 20); // 最高减少一半，仍需修改

        /// <summary> 返回是 0 - 35 </summary>
        public int GetRarity()
        {
            int res = 0;
            if (Type == OperatorType.CV) res += 5;
            res += PropRed;
            res += PropBlue;
            res += PropGreen;
            return res;
        }


        //TODO: 使用次数较少，优化这个方案
        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
