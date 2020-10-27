using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Takamachi660.FileTransmissionSolution;

namespace HttpServer
{
    public class RecvMgr
    {
        Socket listensocket;

        public void StartServer(string ip,int port)
        {
            //启动服务器
            listensocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //IPEndPoint ep = new IPEndPoint(IPAddress.Parse("192.168.8.163"), 3477);
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse(ip), port);
            listensocket.Bind(ep);
            listensocket.Listen(100);
            listensocket.BeginAccept(AcceptCB, null); //开始监听客户端的连接
            Console.WriteLine("启动文件接收服务器成功 at " + ip + ":" + port);
        }

        private void AcceptCB(IAsyncResult ar)
        {
            Socket socket = listensocket.EndAccept(ar);
            FileReceiver task = new FileReceiver();
            task.Socket = socket;
            task.EnabledIOBuffer = true;
            task.BlockFinished += new BlockFinishedEventHandler(task_BlockFinished);
            task.ConnectLost += new EventHandler(task_ConnectLost);
            task.AllFinished += new EventHandler(task_AllFinished);
            task.BlockHashed += new BlockFinishedEventHandler(task_BlockHashed);
            task.ErrorOccurred += new FileTransmissionErrorOccurEventHandler(task_ErrorOccurred);
            //task.FilePath = @"A:";
            task.Start();
            listensocket.BeginAccept(AcceptCB, null);
        }


        void task_ErrorOccurred(object sender, FileTransmissionErrorOccurEventArgs e)
        {
            Console.WriteLine(e.InnerException.Message);
        }

        void task_AllFinished(object sender, EventArgs e)
        {
            Console.WriteLine("接收完成");
        }

        void task_ConnectLost(object sender, EventArgs e)
        {
            Console.WriteLine("连接中断");
        }

        void task_BlockFinished(object sender, BlockFinishedEventArgs e)
        {
        }
        void task_BlockHashed(object sender, BlockFinishedEventArgs e)
        {
        }
        
    }
}
