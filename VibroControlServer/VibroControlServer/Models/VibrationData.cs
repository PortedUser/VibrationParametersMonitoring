using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VibroControlServer.Models
{
    public class VibrationData
    {
        public VibrationData(int move, int speed, int acceleration) 
        {
            Acceleration = acceleration;
            Move = move;
            Speed = speed;
        }

        public int Acceleration { get; set; }
        public int Speed { get; private set; }
        public int Move { get; private set; }
    }
}
