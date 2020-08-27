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
        static void Main(string[] args)
        {
            //Task [] task = new Task[256];
            IpScanner ipScanner = new IpScanner();
            Console.WriteLine("Сканирую сеть...");

            //Тестировал различные реализации вызовов
            //Оставил потоки
            #region Классика 6:23 мин
            /*for (int i = 0; i < 256; i++)
            {
                ipScanner.Scan(i);
            }*/
            #endregion

            #region Таски 1:22 мин
            /*for (int i = 0; i < 256; i++)
            {
                task[i] = new Task(()=> ipScanner.Scan(i));
                task[i].Start();
                Thread.Sleep(300);
            }
            Task.WaitAll(task);*/
            #endregion

            #region Потоки 1.66 sec
            for (int i = 0; i < 256; i++)
            {
                Thread thread = new Thread(new ParameterizedThreadStart(ipScanner.Scan));
                thread.Start(i);
            }
            #endregion
       
            Console.WriteLine("Закончил сканирование.");
            Thread.Sleep(1000);
            foreach(var ip in ipScanner.Ip)
            {
                Sender sender = new Sender(ip, 23);
                Thread thread = new Thread(sender.PortScan);
                thread.Start();
                for (int port = 2000; port <= 2046; port++)
                {
                    Sender newSender = new Sender(ip, port);
                    Thread newThread = new Thread(newSender.PortScan);
                    newThread.Start();
                }
            }

            Console.WriteLine("Ну вроде как все...");
            Console.ReadLine();
        }
    }

    public class Sender
    {
        string ip;
        int port;

        public Sender(string ip, int port)
        {
            this.ip = ip;
            this.port = port;
        }

        public void PortScan()
        {
            try
            {
                Console.WriteLine("Шлю на {0}:{1}", ip.ToString(), port.ToString());
                SendMessage();
            }
            catch (Exception)
            {
                Console.WriteLine("Ошибка: " + ip + ":" + port.ToString());
            }
        }

        protected void SendMessage()
        {
            byte[] bytes = new byte[1024];
            IPHostEntry ipHost = Dns.GetHostEntry(ip);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, port);

            Socket sender = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            sender.Connect(ipEndPoint);

            Console.WriteLine("Сокет соединяется с {0} ", sender.RemoteEndPoint.ToString());
            byte[] msg = Encoding.UTF8.GetBytes("Але гараж?");
            int bytesSent = sender.Send(msg);

            int bytesRec = sender.Receive(bytes);
            Console.WriteLine("\nОтвет от сервера: {0}\n", Encoding.UTF8.GetString(bytes, 0, bytesRec));

            sender.Shutdown(SocketShutdown.Both);
            sender.Close();
        }
    }
}
