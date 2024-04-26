using System;
using System.Text;
using System.Net;
using System.Net.Sockets;

class Program
{
    static void Main()
    {
        IPAddress adr = IPAddress.Parse("127.0.0.1");
        TcpListener server = new TcpListener(adr, 4001);
        server.Start();
        Console.WriteLine(adr);

        while (true)
        {
            TcpClient client = server.AcceptTcpClient();
            NetworkStream ns = client.GetStream();
            
            var msg = Encoding.ASCII.GetBytes("1000");

            while (client.Connected)
            {
                //int count = ns.Read(msg, 0, msg.Length);
                ns.Write(msg,0, msg.Length );
                Console.WriteLine(msg.Length);
                Console.WriteLine("Connect");
            }
        }
    }
}