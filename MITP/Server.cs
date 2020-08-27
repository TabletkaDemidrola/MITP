using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace MITP
{
    class Server
    {
        static int PortScan(string ip, int portBegin, int portEnd, Logger logger)
        {
            TcpListener tcpServer;
            for (int i = 2001; i <= 2046; i++)
            {
                try
                {
                    tcpServer = new TcpListener(IPAddress.Parse(ip), i);
                    tcpServer.Start();
                    
                    tcpServer.Stop();
                    logger.WriteMessage($"Выбран порт - {i}");
                    return i;
                }
                catch
                {
                    logger.WriteMessage($"Порт {i} не доступен");
                    continue;
                }
            }
            logger.WriteMessage("Ни один порт из диапозона не доступен, выбран порт для незашифрованных текстовых сообщений");
            return 23;
        }

        static void Main(string[] args)
        {
            Logger logger = new Logger();
            
            IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddr = ipHost.AddressList[0];
            int port = PortScan(ipAddr.ToString(), 2000, 2046, logger);
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, port);
            logger.WriteMessage($"IP адрес - {ipAddr}");

            Socket sListener = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                sListener.Bind(ipEndPoint);
                sListener.Listen(10000);
                
                while (true)
                {
                    logger.WriteMessage($"Ожидаем соединение через порт {ipEndPoint}");
                    
                    Socket handler = sListener.Accept();
                    string data = null;
                    
                    byte[] bytes = new byte[1024];
                    int bytesRec = handler.Receive(bytes);
                    data += Encoding.UTF8.GetString(bytes, 0, bytesRec);
                    logger.WriteMessage($"Получено сообщение \"{data}\"");

                    if(data == "Do you understand me?")
                    {
                        string reply = "Yes, I do " + ipAddr.ToString() + ":" + port.ToString() + ":" + Environment.MachineName;
                        logger.WriteMessage($"Отправлен ответ \"{reply}\"");
                        byte[] msg = Encoding.UTF8.GetBytes(reply);
                        handler.Send(msg);
                    }
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                    logger.WriteMessage("Закрываем соединение");
                }
            }
            catch (Exception ex)
            {
                logger.WriteMessage("Что-то пошло не так...");
                logger.WriteMessage(ex.ToString());
            }
            finally
            {
                Console.ReadLine();
            }
        }
    }
}
