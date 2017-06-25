using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RpsBarrier
{
    public class RpsPool
    {
        object _lockObject = new object();
        int _tasksPerSecond;

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
                var tmpDate = DateTime.UtcNow;
                var res = _currentItem.Next.StartingDate.AddSeconds(1) < tmpDate;
                if (res)
                {
                    _currentItem = _currentItem.Next;
                    _currentItem.StartingDate = tmpDate;
                }

                return res;
            }
        }
    }

    class RpsPoolItem
    {
        public RpsPoolItem Next { get; set; }
        public DateTime StartingDate { get; set; }
    }
}
