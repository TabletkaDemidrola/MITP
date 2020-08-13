using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Client
{
    class Client
    {
        static Object locker = new Object();
        static void PortScan(string ip, int port)
        {
            try
            {
                Console.WriteLine("Шлю на {0}:{1}",ip,port);
                SendMessage(ip, port);
            }
            catch(Exception)
            {
                Console.WriteLine("Ошибка: " + ip + ":" + port.ToString());
            }
        }

        static void Main(string[] args)
        {
            Task [] task = new Task[256];
            IpScanner ipScanner = new IpScanner();
            Console.WriteLine("Сканирую сеть...");
            for(int i = 0; i < 256; i++)
            {
                task[i] = new Task(()=> ipScanner.AsyncScan(i));
                Thread.Sleep(300);
                task[i].Start();
            }
            Task.WaitAll(task);

            Console.WriteLine("Закончил сканирование.");
            
            foreach(string ip in ipScanner.Ip)
            {
                task = new Task[48];
                task[0] = new Task(() => PortScan(ip, 23));
                task[0].Start();
                Thread.Sleep(1000);
                for (int port = 2000; port <= 2046; port++)
                {
                    task[port - 1999]= new Task(()=>PortScan(ip, port));
                    Thread.Sleep(1000);
                    task[port - 1999].Start();
                }
                Task.WaitAll(task);
            }

            Console.WriteLine("Ну вроде как все...");
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
