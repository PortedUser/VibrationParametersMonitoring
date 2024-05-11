using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace TcpServer
{
    public partial class SensorsDatum
    {
        private int _w;
        private int _m;
        public Guid DataUuid { get; private set; }
        public Guid SensorUuid { get; private set; }
        public DateTime StartTime { get; private set; }
        public DateTime? EndTime { get; private set; }
        public int CountPoints { get;  set; }
        public int[] ListPoints { get; private set; } = null!;

        public SensorsDatum() {}
        public SensorsDatum(Guid sensorGuid) 
        {
            DataUuid = Guid.NewGuid();
            SensorUuid = sensorGuid;
            StartTime = DateTime.Now;
            CountPoints = 0;
            ListPoints = new int[100];
            _w = 5;
            _m = 10;
        }

        public void SetFilterParameter(int w, int m)
        {
            _w = w;
            _m = m;
        }
        public void Add(int numb)
        {
            CountPoints++;
            if (CountPoints >= ListPoints.Length - 1)
            {
                ListPoints = ListPoints.Concat(new int[100]).ToArray();
            }
            ListPoints[CountPoints] = numb;
        }

        public void Close()
        {
            EndTime = DateTime.Now;
            filterSignal();
            Add(-100);
            
        }

        private void filterSignal()
        {
            if (_m == 0 || _w ==0)
            {
                SetFilterParameter(10, 30);
            }
            var res = new List<int>();
            var M = _m;

            for (int i = 0; i < CountPoints; i++)
            {
                var point = 0;
                for (int j = i-(M-1)/2; j < (i+(M-1)/2); j++)
                {
                    if (j >= 0)
                    {
                        point += (int)(ListPoints[j] * GetG(i - j, M));
                    }

                }
                res.Add(point); 
            }
            ListPoints = res.ToArray();
            CountPoints = ListPoints.Length;

        }

        private double GetG(int ind, int M)
        {
            var res = Math.Pow(Math.E, (-1 * Math.Pow((_w * ind / M), 2)) / (2 * Math.Pow(_w, 2)));
            res = res / (_w * Math.Sqrt(2 * Math.PI));
            return res;
        }
    }
}
