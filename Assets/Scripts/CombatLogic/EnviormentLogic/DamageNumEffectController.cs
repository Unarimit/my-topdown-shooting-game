using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.CombatLogic
{
    /// <summary>
    /// 初始化的时候立刻设置DmageNum
    /// </summary>
    public class DamageNumEffectController : MonoBehaviour
    {
        public int DamageNum { get; set; }
        private TextMeshPro _text;
        private float speed = 3;
        private float speed_weak = 5f;
        private void Start()
        {
            _text = GetComponent<TextMeshPro>();
            _text.text = DamageNum.ToString();
        }
        void Update()
        {
            transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);
            transform.Translate(transform.up * speed * Time.deltaTime); //使字体向上移动
            if (speed > 0)
            {
                speed -= speed_weak * Time.deltaTime;//使得向上移动的速度逐渐减少
            }
            else
            {
                Destroy(gameObject);
            }
        }

    }
}
