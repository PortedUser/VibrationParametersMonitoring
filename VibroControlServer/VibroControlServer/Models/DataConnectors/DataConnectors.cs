using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VibroControlServer.Models.DataConnector
{
    public class DataConnectors
    {
        private sensor_dataContext _DBContext;

        public DataConnectors(sensor_dataContext dbContext) 
        { 
            _DBContext = dbContext;
        }

        public void SaveChanges()
        {
            updVibroData();
            _DBContext.SaveChanges();
        }

        public async void AsyncSaveChanges()
        {
            updVibroData();
            await _DBContext.SaveChangesAsync();
        }

        public Connection GetConnection(System.Linq.Expressions.Expression<Func<Connection, bool>> request)
        {
            return _DBContext.Connections.Where(request).First();
        }

        public Sensor GetSensor(System.Linq.Expressions.Expression<Func<Sensor, bool>> request)
        {
            return _DBContext.Sensors.Where(request).First();
        }

        public void Dispose()
        {
            _DBContext.Dispose();
        }

        public bool AddConnection(Connection connection)
        {
            var sensor = GetSensor(x => x.Uuid == connection.SensorUuid);
            if (sensor != null) 
            {
                _DBContext.Connections.Add(connection);
                sensor.NumberOfConnections++;
                return true;
            }
            return false;
        }

        public void AddSensor(Sensor sensor)
        {
            _DBContext.Sensors.Add(sensor);
        }

        private void updVibroData()
        {
            foreach (var item in _DBContext.Connections)
            {
                item.UpdJson();
            }
        }
    }
}
