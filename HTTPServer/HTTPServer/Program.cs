using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HTTPServerLib;
using HttpServer;

namespace HTTPServerLib
{
    class Program
    {
        static int m_portDownload = 10089;
        static int m_portUpload = 3477;
        static void Main(string[] args)
        {
            MySqlMgr m_mysql = new MySqlMgr("192.168.8.163", "3306", "root", "123456", "luoyangpt919");
            

            RecvMgr recv = new RecvMgr();
            recv.StartServer(GetIp(), m_portUpload);

            ExampleServer server = new ExampleServer(GetIp(), m_portDownload);
            server.SetRoot(@"D:\KServer");
            server.Logger = new ConsoleLogger();
            server.Start();
        }

        public static string GetIp()
        {
            string ip = "";
            var strHostName = System.Net.Dns.GetHostName();


            var ipEntry = System.Net.Dns.GetHostEntry(strHostName);


            var addr = ipEntry.AddressList;



            return addr[1].ToString();

        }
    }
}
