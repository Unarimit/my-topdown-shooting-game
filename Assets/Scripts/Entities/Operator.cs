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
        CV
    }
    /// <summary>
    /// 要存入数据库的信息
    /// </summary>
    [Serializable]
    public class Operator : ICloneable
    {
        /// <summary>
        /// 人物Id
        /// </summary>
        public string Id;
        public OperatorType Type = OperatorType.CA;
        public string Name = "empty";

        public string ModelResourceUrl;

        // 主要属性
        public int PropRed;
        public int PropGreen;
        public int PropBlue;

        // 技能id
        public int WeaponSkillId = 4;
        public int MainSkillId = 0;
        public int SlideSkillId = 7;

        // 体力系统
        public int MaxPower = 4;
        public int Power = 3;


        // 舰载机
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

        private MechaHead _mcHead = MechaHead.DefaultMecha();
        private MechaBody _mcBody = MechaBody.DefaultMecha();
        private MechaLeg _mcLeg = MechaLeg.DefaultMecha();

        public delegate void MechaChangeEvent(Operator @this, MechaBase oldMehca, MechaBase newMehca);
        public event MechaChangeEvent MechaChangeEventHandler;


        // 需要考虑删除或替换的属性
        public int RecoverHP => McBody.HPRecover;
        public float MaxSpeed => McLeg.Speed;

        public float ReviveTime = 5;


        //TODO: 使用次数较少，优化这个方案
        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
