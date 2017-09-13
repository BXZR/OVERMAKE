using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace socketServer
{
    //这个类用于控制sock的发送和接受信息的过程
    class theServer
    {

        private string IP = "219.216.73.162";//默认IP地址，可以换


        private static int myProt = 8886;   //端口  

        static Socket serverSocket = null;//保留服务器的socket引用
        private List<Socket> clientSockets = new List<Socket>();//客户端的socket连接

        private Thread theServerThread;//服务器主线程
        private List<Thread> theClientThreads = new List<Thread>();//用于保留客户端的线程引用，用于关闭之

        private bool opened = true;//是否开启
        public bool Opened//opened状态只读，可查询
        {
            get{ return opened;}
        }

        information theInformationController = null; //信息处理控制单元，必须要有，这个才是核心

        //构造方法，用于建立服务器的对象，但是并不是立即开启
        public theServer( information theInformationControllerIn ,  string theIPUse = "219.216.73.162", int port = 8886)
        {
            try
            {
                theInformationController = theInformationControllerIn;
                IP = theIPUse;
                myProt = port;
                IPAddress ip = IPAddress.Parse(IP);
                serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                serverSocket.Bind(new IPEndPoint(ip, myProt));  //绑定IP地址：端口  
                serverSocket.Listen(10);    //设定最多10个排队连接请求  
                // MessageBox.Show("启动监听" + serverSocket.LocalEndPoint.ToString() + "成功" + "\ntype: server");
                //通过Clientsoket发送数据  
            }
            catch
            {
                serverSocket = null;
            }
           
        }

        //开启服务器的接收内容，实际上是开启上层的消息接收内容
        //专门开一个新的线程用于管理这些信息的接收
        //此外，由于消息内容推送的速度问题，有一些灵活的存储方法似乎不能使用
        public string startTheServer()
       {
           //如果socket没有建立，或者没有总控单元，那么所有工作都不会开始
           if (serverSocket == null || theInformationController == null)
                return "服务器无法开启，可能是初始化没有做好";
          theServerThread = new Thread(ListenClientConnect);//新建服务器主要线程
          theServerThread .Start();//真正开启服务器
          opened = true;//标记，用与表示开启
            return "服务器正式启动\n端口:"+myProt +"  IP:"+IP;
        }

        public string closeServer()
        {
            //如果socket没有建立，或者没有总控单元，那么所有工作都不会开始
            if (serverSocket == null || theInformationController == null)
                return "关闭失败，丢是引用或者根本就没开启";
            //关闭所有的客户端socket
            for (int i = 0; i < clientSockets.Count; i++ )
            {
                if (clientSockets[i] == null)
                    continue;
                clientSockets[i].Shutdown(SocketShutdown.Both);
                clientSockets[i].Close();
                clientSockets[i] = null;
            }
            //关掉客户端线程
            for (int i = 0; i < theClientThreads.Count; i++)
            {
                if (theClientThreads[i] == null)
                    continue;
                theClientThreads[i].Abort();
            }
             //关掉服务器线程和socket
            serverSocket.Close();
            theServerThread.Abort();

            opened = false;
            return "服务器socket关闭成功";
        }
         
        //用于接收信息的线程方法
        private void ListenClientConnect()
        {
            while (opened)
            {
                try
                {
                    //如果接受了一个新的连接
                    Socket clientSocket = serverSocket.Accept();
                    //尝试发送验证信息
                    //clientSocket.Send(Encoding.ASCII.GetBytes("Server Say Hello"));
                    //开启接收这个客户端的线程的方法
                    Thread receiveThread = new Thread(ReceiveMessage);
                    theClientThreads.Add(receiveThread);//保留这个客户端连接的引用
                    receiveThread.Start(clientSocket);
                }
                catch
                {
                    //如果服务器崩了，就直接关闭
                    closeServer();
                    break;
                }
            }
        }

        private void ReceiveMessage(object clientSocket)
        {
            //获取到发送消息的客户端socket引用
            //专门用来接收这个客户端的信息的线程
            Socket myClientSocket = (Socket)clientSocket;
            //用于接收的内容的缓冲
            //每个线程分配一个缓冲区，主要是怕冲突问题
             byte[] result = new byte[1024];
            while (true)
            {
                try
                {
                    //通过clientSocket接收数据  
                    int receiveNumber = myClientSocket.Receive(result);
                    // MessageBox.Show("接收客户端" + myClientSocket.RemoteEndPoint.ToString() + "\n消息" + Encoding.ASCII.GetString(result, 0, receiveNumber) + "\ntype: server");
                    //手机和PC的编码方法需要一样，否则诡异的乱码可能会出现
                    string information = Encoding.UTF8.GetString(result, 0, receiveNumber).ToString();
                    //以bye作为区分，如果是bye就认为客户端断开连接
                    if (information != "bye")
                    {
                        //获取了消息information,处理过程需要新一层的封装了
                        //把信息存入到缓存里面去
                        //要保存多个数据，这里就需要做一下切分，也就是所谓的协议
                        //暂定的协议： 
                        //传输内容的大项目用';'切分
                        //传输内容的小项目用','切分
                        string[] theSplited = information.Split(';');
                        //第一大项： Y轴加速度
                        theInformationController.addInformation(theSplited[0] , UseDataType.accelerometerY);
                        //第二大项： 直接从unity里面获取到的角度(最先先用这个做，后期自己优化，本项也可以作为一个基础对照项)
                        theInformationController.addInformation(theSplited[1], UseDataType.compassDegree);//正北0度
                        //第三大项： 陀螺仪的X轴
                        //第四大项： 陀螺仪的Y轴
                        //第五大项： 陀螺仪的Z轴
                    }
                    else//客户端请求关闭连接
                    {
                        myClientSocket.Shutdown(SocketShutdown.Both);
                        myClientSocket.Close();
                        return;//，这层死循环可以结束了
                    }
                }
                catch //如果发送信息居然失败了，就关掉这个客户端连接
                {
                    myClientSocket.Shutdown(SocketShutdown.Both);
                    myClientSocket.Close();
                    return;
                }
            }
        }

    }
}
