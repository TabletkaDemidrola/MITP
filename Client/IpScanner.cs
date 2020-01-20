using System.Collections.Generic;
using System.Net.NetworkInformation;

namespace Client
{
    class IpScanner
    {
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
                PingReply Status = P.Send(host, 1);
                PingStatus = (Status.Status == IPStatus.Success);
            }
            catch {}
            return PingStatus;
        }

        public void Scan()
        {
            for(int i = 0; i<=0; i++)
            {
                for(int j = 0; j<=255; j++)
                {
                    string ip = "192.168." + i.ToString() + "." + j.ToString();
                    if(GetPing(ip) && !Ip.Contains(ip))
                    {
                         Ip.Add(ip);
                    }
                }
            }
        }

    }
}
