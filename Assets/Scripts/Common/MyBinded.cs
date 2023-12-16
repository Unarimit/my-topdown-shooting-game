using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Common
{
    /// <summary>
    /// DataBind类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class MyBinded<T>
    {
        public T Data
        {
            get
            {
                return _data;
            }
            set
            {
                _data = value;
                if (OnDataChange != null) OnDataChange.Invoke();
            }
        }
        private T _data;
        public delegate void DataChangeEventHandler();
        public event DataChangeEventHandler OnDataChange;

    }
}
