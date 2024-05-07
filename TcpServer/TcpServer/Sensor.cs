using System;
using System.Collections.Generic;

namespace TcpServer
{
    public partial class Sensor
    {
        public Sensor() { } 
        public Sensor(string userName = "") 
        {
            SensorUuid = Guid.NewGuid();
            NumberOfConnections = 0;
            Username = userName;
        }
        public Guid SensorUuid { get; set; }
        public int NumberOfConnections { get; set; }
        public string? Username { get; set; }
    }
}
