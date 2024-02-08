using Assets.Scripts.Services;
using Assets.Scripts.Services.Database;
using System;
using System.Collections.Generic;
using UnityEngine;
using XLua;

namespace Assets.Scripts
{
    public static class XLuaConfig
    {
        [CSharpCallLua]
        public static List<Type> mymodule_lua_call_cs_list = new List<Type>()
        {
            typeof(Func<IGameDatabase, bool>),
            typeof(System.Collections.IEnumerator),
            typeof(Action<GameObject, UnityEngine.Transform, UnityEngine.Vector3>)
        };

    }
}
