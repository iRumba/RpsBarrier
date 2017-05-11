using System;
using System.Collections.Generic;

namespace RpsBarrier
{
    public class RpsBarrier
    {
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
                    _rpsRequestsTimeStamps.Clear();
                }
            }
        }
        static RpsBarrier _instance;
        LinkedList<DateTime> _rpsRequestsTimeStamps;

        RpsBarrier()
        {
            _rpsRequestsTimeStamps = new LinkedList<DateTime>();
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
            _rpsRequestsTimeStamps.Clear();
        }

        public bool CanExecute()
        {
            // Чистим список таймстампов операций
            ResetTimeStamps();

            // Если текущее количество операций меньше, чем максимально допустимое, 
            // то разрешаем выполнение и добавляем текущий отпечаток времени в хэшсет
            var res = _rpsRequestsTimeStamps.Count < MaxOperationsPerSecond;
            if (res)
                _rpsRequestsTimeStamps.AddLast(DateTime.UtcNow);
            return res;
        }

        void ResetTimeStamps()
        {
            var first = _rpsRequestsTimeStamps.First;
            if (first != null && first.Value.AddSeconds(1) < DateTime.UtcNow)
                _rpsRequestsTimeStamps.RemoveFirst();
        }
    }

}
