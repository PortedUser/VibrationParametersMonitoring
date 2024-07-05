using Microsoft.EntityFrameworkCore;
using Npgsql.Internal.TypeHandlers;
using System.Text.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace VibroControlServer.Models
{
    public partial class Connection
    {
        [NotMapped]
        public List<VibrationData> Vibrodata { get; set; }

        public Guid Uuid { get; set; }
        public Guid SensorUuid { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int CountPoints { get; set; }

        public string VibrationData { get; set; }


        public Connection() 
        {
            Vibrodata = new List<VibrationData>();
            Uuid = Guid.NewGuid();
        }

        public Connection(Guid sensorGuid)
        {
            SensorUuid = sensorGuid;
            StartTime = DateTime.Now;
            CountPoints = 0;
            Vibrodata = new List<VibrationData>();
            Uuid = Guid.NewGuid();
        }

        public void Add(VibrationData data)
        {
            CountPoints++;
            Vibrodata.Add(data);
            UpdJson();
        }

        public void UpdJson()
        {
            if (Vibrodata == null)
            {
                Vibrodata = new List<VibrationData>();
            }
            VibrationData = JsonSerializer.Serialize(Vibrodata);
        }

        public void UpdList()
        {
            Vibrodata = JsonSerializer.Deserialize<List<VibrationData>>(VibrationData);
        }

        public void Close() 
        {
            if (EndTime == null)
            {
                EndTime = DateTime.Now;
            }
        }
    }
}
