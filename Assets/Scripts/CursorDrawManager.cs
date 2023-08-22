using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts
{
    public class CursorDrawManager : MonoBehaviour
    {
        private void Awake()
        {
            Cursor.visible = false;
        }
        private void Update()
        {
            transform.position = Mouse.current.position.ReadValue();
        }
    }
}
