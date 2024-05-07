using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using TcpServer;
//5a460c2d-6e38-4ae8-8a5c-baa910b592cd
IPAddress adr = IPAddress.Parse("127.0.0.1");
var tcpListener = new TcpListener(adr, 4001);
Console.WriteLine(adr);

var dbContext = new sensorsdataContext();
var data1 = dbContext.SensorsData.Where(o => o.DataUuid == new Guid("5a460c2d-6e38-4ae8-8a5c-baa910b592cd")).FirstOrDefault();
Console.WriteLine($"{data1.ListPoints.Length}  {data1.CountPoints}");



Console.WriteLine($"{data1.ListPoints.Length}  {data1.CountPoints}");
var rnd = new Random();
data1.CountPoints = 0;
for (int i = 0; i <= 200; i++)
{
    int a = (int)(Math.Sin(2 * Math.PI * i / 100) * 100) + 100 + rnd.Next(50);
    Console.WriteLine(a);
    data1.Add(a);
}
data1.Close();

dbContext.SaveChanges();
Console.WriteLine($"{data1.ListPoints.Length}  {data1.CountPoints}");

try
{
    tcpListener.Start();  
    Console.WriteLine("Сервер запущен. Ожидание подключений... ");

    while (true)
    {
        var tcpClient = await tcpListener.AcceptTcpClientAsync();

        await Task.Run(async () => await ProcessClientAsync(tcpClient));
    }
}
finally
{
    tcpListener.Stop();
}

async Task ProcessClientAsync(TcpClient tcpClient)
{
    
    
    var stream = tcpClient.GetStream();
    var count = 0;
    var gui = new Guid("5a460c2d-6e38-4ae8-8a5c-baa910b592cd");
    var data =  dbContext.SensorsData.Where(o=>o.DataUuid == new Guid("5a460c2d-6e38-4ae8-8a5c-baa910b592cd")).FirstOrDefault();


    while(count <= data.CountPoints)
    {
        
        Console.WriteLine(data.ListPoints[count]);
        try
        {
            var bt = await stream.ReadAsync(new byte[1]);

            if (bt != 1)
            {
                break;
            }
        }
        catch (Exception)
        {

            break;
        }

        try
        {
            await stream.WriteAsync(Encoding.UTF8.GetBytes(GetCorrectString(data.ListPoints[count], 4)));
            //await Task.Delay(100);
        }
        catch (Exception)
        {
            break;
        }

        //if (count >=99)
        //{
        //    count = 0;
        //}
        count++;
    }
    tcpClient.Close();
}



string GetCorrectString(int number, int length)
{
    var res = number.ToString();
    if (number == -1) return "-100";

    if (res.Length < length)
    {
        var stab = length - res.Length;
        var builder = new StringBuilder();

        for (int i = stab; i > 0; i--)
        {
            builder.Append("0");
        }
        builder.Append(res);
        res = builder.ToString();
    }

    return res;
}

