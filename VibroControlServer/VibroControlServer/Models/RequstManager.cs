using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using VibroControlServer.Models.DataConnector;
using System.Threading.Tasks;

namespace VibroControlServer.Models
{
    public class RequestManager
    {
        private readonly DataConnectors _dataConnectors;

        public RequestManager(DataConnectors dataConnectors)
        {
            _dataConnectors = dataConnectors;
        }

        public async Task HandleClientRequestAsync(DataConnectors dataConnectors, string request, NetworkStream clientStream)
        {
            try
            {
                if (request.StartsWith("CreateSensor"))
                {
                    var sensor = new Sensor(); 
                    dataConnectors.AddSensor(sensor); 
                    dataConnectors.SaveChanges(); 

                   
                    var connection = new Connection(sensor.Uuid);
                    dataConnectors.AddConnection(connection); 
                    dataConnectors.SaveChanges(); 

                    await SendResponseAsync(clientStream, sensor.Uuid.ToString());
                }
                else if (request.StartsWith("connect_"))
                {
                    var sensorUuidString = request.Substring("connect_".Length);
                    if (Guid.TryParse(sensorUuidString, out Guid sensorUuid))
                    {
                        var connection = new Connection(sensorUuid);
                        dataConnectors.AddConnection(connection); 
                        dataConnectors.SaveChanges();

                        await SendResponseAsync(clientStream, "Connection created successfully");
                    }
                    else
                    {
                        await SendResponseAsync(clientStream, "Invalid sensor UUID format");
                    }
                }
                else if (request.StartsWith("in_"))
                {
                    var parts = request.Substring("in_".Length).Split('_');
                    if (parts.Length == 4 && Guid.TryParse(parts[0], out Guid connectionUuid) &&
                        int.TryParse(parts[1], out int move) &&
                        int.TryParse(parts[2], out int speed) &&
                        int.TryParse(parts[3], out int acceleration))
                    {
                        var connection = dataConnectors.GetConnection(c => c.Uuid == connectionUuid);
                        if (connection != null)
                        {
                            var vibrationData = new VibrationData(move, speed, acceleration);
                            connection.Add(vibrationData);
                            dataConnectors.SaveChanges(); 

                            await SendResponseAsync(clientStream, "Data added successfully");
                        }
                        else
                        {
                            await SendResponseAsync(clientStream, "Connection not found");
                        }
                    }
                    else
                    {
                        await SendResponseAsync(clientStream, "Invalid request format");
                    }
                }
                else if (request.StartsWith("get_"))
                {
                    Console.WriteLine("1111");
                    var connectionUuidString = request.Substring("get_".Length); 
                    if (Guid.TryParse(connectionUuidString, out Guid connectionUuid))
                    {
                        var connection = dataConnectors.GetConnection(c => c.Uuid == connectionUuid);
                        if (connection != null)
                        {
                            var moveBuilder = new StringBuilder();
                            var speedBuilder = new StringBuilder();
                            var accelerationBuilder = new StringBuilder();

                            foreach (var data in connection.Vibrodata)
                            {
                                moveBuilder.Append(data.Move).Append('%');
                                speedBuilder.Append(data.Speed).Append('%');
                                accelerationBuilder.Append(data.Acceleration).Append('%');
                            }

                            string moveData = moveBuilder.ToString().TrimEnd('%');
                            string speedData = speedBuilder.ToString().TrimEnd('%');
                            string accelerationData = accelerationBuilder.ToString().TrimEnd('%');

                            
                            string moveDataLength = moveData.Length.ToString();
                            while (moveDataLength.Length < 16)
                            {
                                moveDataLength = "0" + moveDataLength;
                            }

                            
                            await SendResponseAsync(clientStream, moveDataLength);

                           
                            var buffer = new byte[1024];
                            int bytesRead = await clientStream.ReadAsync(buffer, 0, buffer.Length);
                            string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                            if (response.Trim() == "1")
                            {
                              
                                await SendResponseAsync(clientStream, moveData);

                                
                                bytesRead = await clientStream.ReadAsync(buffer, 0, buffer.Length);
                                response = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                                if (response.Trim() == "1")
                                {
                                    
                                    string speedDataLength = speedData.Length.ToString();
                                    while (speedDataLength.Length < 16)
                                    {
                                        speedDataLength = "0" + speedDataLength;
                                    }

                                    
                                    await SendResponseAsync(clientStream, speedDataLength);

                                    
                                    bytesRead = await clientStream.ReadAsync(buffer, 0, buffer.Length);
                                    response = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                                    if (response.Trim() == "1")
                                    {
                                        
                                        await SendResponseAsync(clientStream, speedData);

                                        
                                        bytesRead = await clientStream.ReadAsync(buffer, 0, buffer.Length);
                                        response = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                                        if (response.Trim() == "1")
                                        {
                                            string accelerationDataLength = accelerationData.Length.ToString();
                                            while (accelerationDataLength.Length < 16)
                                            {
                                                accelerationDataLength = "0" + accelerationDataLength;
                                            }

                                            
                                            await SendResponseAsync(clientStream, accelerationDataLength);

                                            
                                            bytesRead = await clientStream.ReadAsync(buffer, 0, buffer.Length);
                                            response = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                                            if (response.Trim() == "1")
                                            {
                                                
                                                await SendResponseAsync(clientStream, accelerationData);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            await SendResponseAsync(clientStream, "Connection not found");
                        }
                    }
                    else
                    {
                        await SendResponseAsync(clientStream, "Invalid connection UUID format");
                    }
                }
                else
                {
                    await SendResponseAsync(clientStream, "Unknown request");
                }
            }
            catch (Exception ex)
            {
                await SendResponseAsync(clientStream, $"Error processing request: {ex.Message}");
            }
        }

        private async Task SendResponseAsync(NetworkStream clientStream, string message)
        {
            byte[] response = Encoding.UTF8.GetBytes(message);
            await clientStream.WriteAsync(response, 0, response.Length);
            await clientStream.FlushAsync();
        }


    }
}
