using VibroControlServer;
using VibroControlServer.Models;
using System.Text;
using System.Net.Sockets;
using System.Net;
using VibroControlServer.Models.DataConnector;

IPAddress adr = IPAddress.Parse("127.0.0.1");
var tcpListener = new TcpListener(adr, 4001);
Console.WriteLine(adr);

var dbContext = new DataConnectors(new sensor_dataContext());

try
{
    tcpListener.Start();
    Console.WriteLine("Сервер запущен. Ожидание подключений...");

    while (true)
    {
        var tcpClient = await tcpListener.AcceptTcpClientAsync();
        _ = Task.Run(async () => await ProcessClientAsync(tcpClient, dbContext));
    }
}
finally
{
    tcpListener.Stop();
}

async Task ProcessClientAsync(TcpClient tcpClient, DataConnectors db)
{
    try
    {
        using (tcpClient)
        {
            var stream = tcpClient.GetStream();
            var buffer = new byte[1024];
            int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
            string request = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();

            var requestManager = new RequestManager(dbContext);
            await requestManager.HandleClientRequestAsync(db, request, stream);
        }

    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error processing client request: {ex.Message}");
    }
}