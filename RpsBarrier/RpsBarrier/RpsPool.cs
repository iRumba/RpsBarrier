using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RpsBarrier
{
    public class RpsPool
    {
        object _lockObject = new object();
        int _tasksPerSecond;
        static Stopwatch _sw = Stopwatch.StartNew();

        RpsPoolItem _currentItem;

        public RpsPool(int tasksPerSecond)
        {
            if (tasksPerSecond < 1)
                throw new InvalidOperationException();
            _tasksPerSecond = tasksPerSecond;

            var tmpItem = new RpsPoolItem();
            _currentItem = tmpItem;
            while (--tasksPerSecond > 0)
            {
                tmpItem.Next = new RpsPoolItem();
                tmpItem = tmpItem.Next;
            }
            tmpItem.Next = _currentItem;
        }

        public int TasksPerSecond
        {
            get
            {
                return _tasksPerSecond;
            }
        }

        public bool CanExecute()
        {
            lock (_lockObject)
            {
                var tmpStamp = _sw.ElapsedMilliseconds;
                var res = _currentItem.Next.StartingMilliseconds + 1000 <= tmpStamp;
                if (res)
                {
                    _currentItem = _currentItem.Next;
                    _currentItem.StartingMilliseconds = tmpStamp;
                }

                return res;
            }
        }
    }

    class RpsPoolItem
    {
        public RpsPoolItem Next { get; set; }
        public long StartingMilliseconds { get; set; } = -1000;
    }
}
