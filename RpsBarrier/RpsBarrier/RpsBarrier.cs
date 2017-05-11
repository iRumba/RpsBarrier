using System;
using System.Collections.Generic;

namespace RpsBarrier
{
    public class RpsBarrier
    {
        DateTime[] _operations;
        int _firstIndex;
        int _lastIndex;

        int _maxOperationsPerSecond;

        public int MaxOperationsPerSecond
        {
            get
            {
                return _maxOperationsPerSecond;
            }
            set
            {
                if (_maxOperationsPerSecond != value)
                {
                    _maxOperationsPerSecond = value;
                    Reset();
                }
            }
        }
        static RpsBarrier _instance;

        RpsBarrier()
        {
            MaxOperationsPerSecond = 500;
        }

        public static RpsBarrier Instance
        {
            get
            {
                return _instance ?? (_instance = new RpsBarrier());
            }
        }

        public void Reset()
        {
            _operations = new DateTime[MaxOperationsPerSecond];
            _firstIndex = -1;
            _lastIndex = -1;
        }

        public bool CanExecute()
        {
            ResetTimeStamps();
            return AddLast(DateTime.UtcNow);
        }

        void ResetTimeStamps()
        {
            if (_firstIndex != -1 && _operations[_firstIndex].AddSeconds(1) < DateTime.UtcNow)
                RemoveFirst();
        }

        int GetNextIndex(int index)
        {
            return index >= MaxOperationsPerSecond - 1 ? 0 : ++index;
        }

        int GetPrevIndex(int index)
        {
            return index <= 0 ? MaxOperationsPerSecond - 1 : --index;
        }

        bool AddLast(DateTime date)
        {
            var next = GetNextIndex(_lastIndex);
            if (next == _firstIndex)
                return false;
            if (_firstIndex == -1)
                _firstIndex = next;
            _operations[(_lastIndex = next)] = date;
            return true;
        }

        void RemoveFirst()
        {
            if (_firstIndex != -1 && _firstIndex != _lastIndex)
                _firstIndex = GetNextIndex(_firstIndex);
        }
    }

}
