using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts.CombatLogic.UILogic
{
    public class CursorDrawManager : SubUIBase
    {
        private void Awake()
        {
            Cursor.visible = false;
        }
        private void OnGUI()
        {
            transform.position = Mouse.current.position.ReadValue();
        }
    }
}
