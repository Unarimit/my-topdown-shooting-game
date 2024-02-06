using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Entities.HomeMessage
{
    /// <summary>
    /// 优先队列like的封装
    /// </summary>
    public class HomeMessageQueue : SortedSet<HomeMessage>
    {
        public HomeMessageQueue() : base(new HomeMessageComparer())
        {
            
        }
        public void Push(HomeMessage msg)
        {
            Add(msg);
        }

        public HomeMessage Peek()
        {
            return this.Min;
        }
        public HomeMessage Pop()
        {
            var temp = this.Min;
            Remove(temp);
            return temp;
        }
    }

    internal class HomeMessageComparer : IComparer<HomeMessage>
    {
        public int Compare(HomeMessage x, HomeMessage y)
        {
            if(x.Day == y.Day) return x.Id - y.Id;
            else return x.Day - y.Day;
        }
    }
}
