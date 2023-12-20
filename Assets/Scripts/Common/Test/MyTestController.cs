using OpenCover.Framework.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Assets.Scripts.Common.Test
{
    public class MyTestController : MonoBehaviour
    {
        public class MyTestInvokeInfo
        {
            public MonoBehaviour Instance;
            public MethodInfo Method; 
        }
        public List<MyTestInvokeInfo> MyTestInvokeInfos = new List<MyTestInvokeInfo>();
        private void Start()
        {
            /*
             * 为什么找不到捏
            var tInfo = new HashSet<Type>();
            foreach(var a in AppDomain.CurrentDomain.GetAssemblies())
            {
                var types = a.GetTypes()
                      .Where(m => { 
                          foreach(var x in m.GetMethods())
                          {
                            if(m.GetCustomAttributes(typeof(MyTestAttribute), false).Length > 0) return true;
                          }
                          return false;
                      })
                      .ToArray();
                tInfo.AddRange(types);
            }
            Debug.Log(tInfo.Count);
             */

            var allMono = GameObject.FindObjectsOfType<MonoBehaviour>();
            var blackList = new HashSet<Type>();
            foreach (var mono in allMono)
            {
                if (blackList.Contains(mono.GetType())) continue;
                var methods = mono.GetType().GetMethods()
                    .Where(m => m.GetCustomAttributes(typeof(MyTestAttribute), false).Length > 0)
                    .ToArray();
                foreach (var m in methods)
                {
                    MyTestInvokeInfos.Add(new MyTestInvokeInfo { Method = m, Instance = mono });
                }
                if (methods.Length == 0) blackList.Add(mono.GetType());
            }
        }
    }
}
