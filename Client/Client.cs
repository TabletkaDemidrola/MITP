using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Client
{
    class Client
    {
        static void Main(string[] args)
        {
            IpScanner ipScanner = new IpScanner();
            Console.WriteLine("Сканирую сеть...");
            ipScanner.Scan();
            Console.WriteLine("Закончил сканирование.");
            foreach(string ip in ipScanner.Ip)
            {
                for(int port = 2000; port <= 2046; port++)
                {
                    try
                    {
                        SendMessage(ip, port);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Ошибка: " + ip + ":" + port.ToString());
                    }
                }
            }
            Console.ReadLine();
        }

        static void SendMessage(string ip, int port)
        {
            byte[] bytes = new byte[1024];
            IPHostEntry ipHost = Dns.GetHostEntry(ip);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, port);

            Socket sender = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            sender.Connect(ipEndPoint);

            Console.WriteLine("Сокет соединяется с {0} ", sender.RemoteEndPoint.ToString());
            byte[] msg = Encoding.UTF8.GetBytes("Do you understand me?");
            int bytesSent = sender.Send(msg);
            
            int bytesRec = sender.Receive(bytes);
            Console.WriteLine("\nОтвет от сервера: {0}\n", Encoding.UTF8.GetString(bytes, 0, bytesRec));
            
            sender.Shutdown(SocketShutdown.Both);
            sender.Close();
        }
    }
}
