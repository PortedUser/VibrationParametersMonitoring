using System;
using System.Collections.Generic;

namespace VibroControlServer.Models
{
    public partial class Sensor
    {
        public Guid Uuid { get; set; }
        public string? UserName { get; set; }
        public int NumberOfConnections { get; set; }

        public Sensor() { }

        public Sensor(string userName = "")
        {
            UserName = userName;
            Uuid = Guid.NewGuid();
            NumberOfConnections = 0;
        }
    }
}
