using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.CombatLogic.UILogic
{
    public class GunStatuUIManager : MonoBehaviour
    {
        public static GunStatuUIManager Instance;
        public TextMeshProUGUI MaxAmmoText;
        public TextMeshProUGUI CurrentAmmoText;

        private int maxAmmo = 30;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Debug.LogWarning(transform.ToString() + " try to load another Manager");
        }

        public readonly Color AmmoWarning = new Color(1, 0, 0);
        public readonly Color AmmoNormal = new Color(1, 1, 1);

        /// <summary>
        /// 更新当前子弹数量，当数量小于最大子弹的1/5时，颜色变为红色
        /// </summary>
        /// <param name="k"></param>
        public void UpdateCurrentAmmo(int k)
        {
            CurrentAmmoText.text = k.ToString();
            if (maxAmmo != 0 && maxAmmo > k * 5)
            {
                CurrentAmmoText.color = AmmoWarning;
            }
            else
            {
                CurrentAmmoText.color = AmmoNormal;
            }
        }
        public void UpdateMaxAmmo(int k)
        {
            MaxAmmoText.text = k.ToString();
            maxAmmo = k;
        }
    }
}
