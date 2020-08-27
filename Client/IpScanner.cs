using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace Client
{
    class IpScanner
    {
        protected object Locker = new object();
        public List<string> Ip; 

        public IpScanner()
        {
            Ip = new List<string>();
        }

        public bool GetPing(string host)
        {
            bool PingStatus = false;
            try
            {
                Ping P = new Ping();
                PingReply Status = P.Send(host);
                PingStatus = (Status.Status == IPStatus.Success);
            }
            catch {}
            return PingStatus;
        }

        public void Scan(object i)
        {
            string ip = "192.168.0." + i.ToString();
            Console.WriteLine("Пингую " + ip);
            if(GetPing(ip) && !Ip.Contains(ip))
            {
                lock (Locker)
                {
                    Ip.Add(ip);
                } 
            }
        }

    }
}
