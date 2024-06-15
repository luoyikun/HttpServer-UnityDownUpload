using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HTTPServerLib;
using HttpServer;
using System.Net;

namespace HTTPServerLib
{
    class Program
    {
        static int m_portDownload = 10089;
        static int m_portUpload = 3477;

        static bool m_isUseUpload = false;
        static void Main(string[] args)
        {
            if (m_isUseUpload == true)
            {
                MySqlMgr m_mysql = new MySqlMgr("192.168.8.163", "3306", "root", "123456", "luoyangpt919");

                RecvMgr recv = new RecvMgr();
                recv.StartServer(GetIp(), m_portUpload);
            }

            ExampleServer server = new ExampleServer(GetIp(), m_portDownload);
            server.Logger = new ConsoleLogger();
            server.SetRoot(@"D:\KServer");
            server.Start();
        }

        public static string GetIp()
        {
            string ip = "";
            string hostName = Dns.GetHostName();   //获取本机名                                 
            IPHostEntry IpEntry = Dns.GetHostEntry(hostName);
            for (int i = 0; i < IpEntry.AddressList.Length; i++)
            {
                //从IP地址列表中筛选出IPv4类型的IP地址
                //AddressFamily.InterNetwork表示此IP为IPv4,
                //AddressFamily.InterNetworkV6表示此地址为IPv6类型

                if (IpEntry.AddressList[i].AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    ip = IpEntry.AddressList[i].ToString();
                }
            }
            return ip;

        }
    }
}
