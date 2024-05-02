using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;

IPAddress adr = IPAddress.Parse("127.0.0.1");
var tcpListener = new TcpListener(adr, 4001);
Console.WriteLine(adr);

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


    while (true)
    {
        count++;

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
            await stream.WriteAsync(Encoding.UTF8.GetBytes(GetCorrectString(count, 4)));
            Task.Delay(100);
        }
        catch (Exception)
        {
            break;
        }

        if (count >=99)
        {
            count = 0;
        }
    }
    tcpClient.Close();
}



string GetCorrectString(int number, int length)
{
    var res = number.ToString();

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

