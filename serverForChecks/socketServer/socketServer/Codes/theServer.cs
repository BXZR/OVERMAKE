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
   public  partial class theServer
    {

        private string IP = "219.216.78.119";//默认IP地址，可以换

        private static int myProt = 8886;   //端口  

        static Socket serverSocket = null;//保留服务器的socket引用
        private List<Socket> clientSockets = new List<Socket>();//客户端的socket连接

        private Thread theServerThread;//服务器主线程
        private List<Thread> theClientThreads = new List<Thread>();//用于保留客户端的线程引用，用于关闭之

        private bool opened = true;//是否开启
        private int mode = 1;//1 单人使用 2 多人使用

        public bool Opened//opened状态只读，可查询
        {
            get{ return opened;}
        }
        //这个适用于单个客户端的情况
        information theInformationController = null; //信息处理控制单元，必须要有，这个才是核心
        //这个适用于多个客户端的情况
        List<information> theInformationControllers;//信息处理单元组
        List<string> theIPS;//用字符串IP作为区分

        public theServer(string theIPUse = "", int port = 8886 )
        {
            if (SystemSave.theServrForAll != null)
                return;
            try
            {
                theInformationControllers = new List<socketServer.information>();
                theIPS = new List<string>();
                theInformationController = new socketServer.information();
                //设定IP地址的策略
                //首先看传入的IPUse是不是空，如果不是就用IPUse
                //如果IPUse为空，就是用SystemSave的IP，并且这个可以在设置面板设置
                //实际上SystemSave的IP在是真正需要使用的IP，theIPUse 只是一个扩展的功能
                if (string.IsNullOrEmpty(theIPUse) == false)
                {
                    IP = theIPUse;
                    myProt = port;
                }
                else
                {
                    IP = SystemSave.serverIP;
                    myProt = SystemSave.serverPort;
                }
                //IPAddress ip = IPAddress.Parse(IP);
                // serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                // serverSocket.Bind(new IPEndPoint(ip, myProt));  //绑定IP地址：端口  
                // serverSocket.Listen(10);    //设定最多10个排队连接请求  
                // MessageBox.Show("启动监听" + serverSocket.LocalEndPoint.ToString() + "成功" + "\ntype: server");
                //通过Clientsoket发送数据  
                SystemSave.theServrForAll = this;
            }
            catch
            {
                serverSocket = null;
                SystemSave.theServrForAll = null;
            }
        }

        //构造方法，用于建立服务器的对象，但是并不是立即开启
        public theServer( information theInformationControllerIn ,  string theIPUse = "", int port = 8886)
        {
            try
            {
                theInformationController = theInformationControllerIn;
                //设定IP地址的策略
                //首先看传入的IPUse是不是空，如果不是就用IPUse
                //如果IPUse为空，就是用SystemSave的IP，并且这个可以在设置面板设置
                //实际上SystemSave的IP在是真正需要使用的IP，theIPUse 只是一个扩展的功能
                if(string .IsNullOrEmpty (theIPUse) == false)
                { 
                    IP = theIPUse;
                    myProt = port;
                }
                else
                {
                    IP = SystemSave.serverIP;
                    myProt = SystemSave .serverPort;
                }
                //IPAddress ip = IPAddress.Parse(IP);
               // serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
               // serverSocket.Bind(new IPEndPoint(ip, myProt));  //绑定IP地址：端口  
               // serverSocket.Listen(10);    //设定最多10个排队连接请求  
                // MessageBox.Show("启动监听" + serverSocket.LocalEndPoint.ToString() + "成功" + "\ntype: server");
                //通过Clientsoket发送数据  
            }
            catch
            {
                serverSocket = null;
            }
           
        }
        //如果使用别的方法可以修改mode但是默认是不需要的
        //修改mode需要在start之前完成
        public void setMode(int modeIn = 1)
        {
            mode = modeIn;
        }

        //开启服务器的接收内容，实际上是开启上层的消息接收内容
        //专门开一个新的线程用于管理这些信息的接收
        //此外，由于消息内容推送的速度问题，有一些灵活的存储方法似乎不能使用
        public string startTheServer(bool useSystemSave = false)
       {
           //如果socket没有建立，或者没有总控单元，那么所有工作都不会开始
           if ( theInformationController == null)
                return "服务器无法开启，可能是初始化没有做好";

            if (useSystemSave)
            {
                IP = SystemSave.serverIP;
                myProt = SystemSave.serverPort;
            }
            IPAddress ip = IPAddress.Parse(IP);
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Console.WriteLine("IP is" + ip);
            serverSocket.Bind(new IPEndPoint(ip, myProt));  //绑定IP地址：端口  
            serverSocket.Listen(10);    //设定最多10个排队连接请求  
            opened = true;//标记，用与表示开启

            theServerThread = new Thread(ListenClientConnect);//新建服务器主要线程
            theServerThread .Start();//真正开启服务器
  
            return "服务器正式启动\n端口:"+myProt +"  IP:"+IP;
        }

        public string closeServer()
        {
            //如果socket没有建立，或者没有总控单元，那么所有工作都不会开始
            if (serverSocket == null || theInformationController == null)
                return "关闭失败，丢失引用或者根本就没开启";
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
            Console.WriteLine("Mode = " + mode);
            Console.WriteLine("opened = "+ opened);
            while (opened)
            {
                Console.WriteLine("Server started with mode " + mode);
                try
                {
                    //如果接受了一个新的连接
                    Socket clientSocket = serverSocket.Accept();
                    //尝试发送验证信息
                    //clientSocket.Send(Encoding.ASCII.GetBytes("Server Say Hello"));
                    //开启接收这个客户端的线程的方法
                    if (mode == 1)
                    {
                        Thread receiveThread = new Thread(ReceiveMessage);
                        theClientThreads.Add(receiveThread);//保留这个客户端连接的引用
                        receiveThread.Start(clientSocket);
                    }
                    else if (mode == 2)
                    {
                        Thread receiveThread = new Thread(ReceiveMessage2);
                        theClientThreads.Add(receiveThread);//保留这个客户端连接的引用
                        receiveThread.Start(clientSocket);
                    }
                  
                }
                catch
                {
                    Console.WriteLine("Server is closed with error");
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
             byte[] result = new byte[SystemSave.lengthForBuffer];
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
                    if (information != "bye" && information != "get")
                    {
                        //Console.WriteLine("\n------------------\n"+information+ "\n------------------\n");
                        //其实这个方法有非常冗余的封装，但是为了保证可扩展性可读性，暂时先不改了
                        //获取了消息information,处理过程需要新一层的封装了
                        //把信息存入到缓存里面去
                        //要保存多个数据，这里就需要做一下切分，也就是所谓的协议
                        //暂定的协议： 
                        //传输内容的大项目用';'切分
                        //传输内容的小项目用','切分
                        string[] theSplited = information.Split(';');
                        //因为信息的第一项是用来做报头了
                        if (theSplited[0] == "A")
                        {
                            for (int i = 1; i < theSplited.Length; i++)
                            {
                                //实际上下面所有的信息都会被存储，所以可以保证下标保持对应
                                switch (i)
                                {
                                    //第一大项： Y轴加速度
                                    case 1: { theInformationController.addInformation(UseDataType.accelerometerY, theSplited[1]); } break;
                                    //第二大项： 直接从unity里面获取到的角度(最先先用这个做，后期自己优化，本项也可以作为一个基础对照项)
                                    case 2: { theInformationController.addInformation(UseDataType.compassDegree, theSplited[2]); } break;//正北0度 
                                    //第三大项： X轴加速度
                                    case 3: { theInformationController.addInformation(UseDataType.accelerometerX, theSplited[3]); } break;
                                    //第四大项： Z轴加速度
                                    case 4: { theInformationController.addInformation(UseDataType.accelerometerZ, theSplited[4]); } break;
                                    //第五大项： X轴陀螺仪
                                    case 5: { theInformationController.addInformation(UseDataType.gyroX, theSplited[5]); } break;
                                    //第六大项： Y轴陀螺仪
                                    case 6: { theInformationController.addInformation(UseDataType.gyroY, theSplited[6]); } break;
                                    //第七大项： Z轴陀螺仪
                                    case 7: { theInformationController.addInformation(UseDataType.gyroZ, theSplited[7]); } break;
                                    //第八大项： X轴磁力计
                                    case 8: { theInformationController.addInformation(UseDataType.magnetometerX, theSplited[8]); } break;
                                    //第九大项： y轴磁力计
                                    case 9: { theInformationController.addInformation(UseDataType.magnetometerY, theSplited[9]); } break;
                                    //第十大项： z轴磁力计
                                    case 10: { theInformationController.addInformation(UseDataType.magnetometerZ, theSplited[10]); } break;
                                    //GPS
                                    case 11: { theInformationController.addInformation(UseDataType.GPS, theSplited[11]); } break;
                                    //时间戳
                                    case 12: { theInformationController.addInformation(UseDataType.timeStamp, theSplited[12]); } break;
                                    //AHRSZ信息
                                    case 13: { theInformationController.addInformation(UseDataType.AHRSZ, theSplited[13]); } break;
                                    //IMU信息
                                    case 14: { theInformationController.addInformation(UseDataType.IMUZ, theSplited[14]); } break;
                                }
                            }
                        }
                        else if (theSplited[0] == "B")
                        {
                            //如果网络带宽实在是不行，就考虑用这种分片的方法分着发送。
                            //这一点在客户端上也留有接口
                            /*
                            for (int i = 1; i < theSplited.Length; i++)
                            {
                                //实际上下面所有的信息都会被存储，所以可以保证下标保持对应
                                switch (i)
                                {
                                    //GPS
                                    case 1: { theInformationController.addInformation(UseDataType.GPS, theSplited[1]); } break;
                                    //时间戳
                                    case 2: { theInformationController.addInformation(UseDataType.timeStamp, theSplited[2]); } break;
                                    //AHRSZ信息
                                    case 3: { theInformationController.addInformation(UseDataType.AHRSZ, theSplited[3]); } break;
                                    //IMU信息
                                    case 4: { theInformationController.addInformation(UseDataType.IMUZ, theSplited[4]); } break;

                                }
                            }
                            */
                        }
                        //string sendString = SystemSave.allStepCount.ToString();
                        //sendString += ";" + SystemSave.stepLengthNow.ToString("f2") ;
                        //sendString += ";" + SystemSave.stepAngleNow.ToString("f2") ;
                        //myClientSocket.Send(Encoding.UTF8.GetBytes(sendString));//发送一个步数信息
                        string sendString = SystemSave.allStepCount.ToString();
                        sendString += ";" + SystemSave.stepLengthNow.ToString("f2");
                        sendString += ";" + SystemSave.stepAngleNow.ToString("f2");
                        sendString += ";" + SystemSave.slopNow.ToString("f2");
                        sendString += ";" + SystemSave.heightNow.ToString("f2");
                        myClientSocket.Send(Encoding.UTF8.GetBytes(sendString));//发送一个步数信息
                    }
                    else if (information == "get")
                    {
                        //3D显示客户端的需要
                        string sendString = SystemSave.allStepCount.ToString();
                        sendString += ";" + SystemSave.stepLengthNow.ToString("f2");
                        sendString += ";" + SystemSave.stepAngleNow.ToString("f2");
                        sendString += ";" + SystemSave.slopNow.ToString("f2");
                        sendString += ";" + SystemSave.heightNow.ToString("f2");
                        myClientSocket.Send(Encoding.UTF8.GetBytes(sendString));//发送一个步数信息
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
                    Console.WriteLine("传送信息失败");
                    myClientSocket.Shutdown(SocketShutdown.Both);
                    myClientSocket.Close();
                    return;
                }
            }
        }

        public delegate void showNewMainWindow(information theInformationController);
        public void showMainWindow(information theInformationController)
        {
            MainWindow aNewMainWindow = new MainWindow();
            aNewMainWindow.makeStart(theInformationController, false);
            //在这里对应不同客户端窗口的信息title(可以扩展一下)
            aNewMainWindow.Title = "PDR With SmartPhone No. " + theInformationControllers.Count;
            aNewMainWindow.Show();
        }
        //分割线
        private void ReceiveMessage2(object clientSocket)
        {
            information theInformationController = new socketServer.information() ;
            theInformationControllers.Add(theInformationController);
            //有关窗口UI等等的内容需要在一些特殊的线程里面处理
            System.Windows.Application.Current.Dispatcher.Invoke(System.Windows .Threading.DispatcherPriority.Normal , new showNewMainWindow(showMainWindow),theInformationController);
            Console.WriteLine("a new client");
            //获取到发送消息的客户端socket引用
            //专门用来接收这个客户端的信息的线程
            Socket myClientSocket = (Socket)clientSocket;
            //用于接收的内容的缓冲
            //每个线程分配一个缓冲区，主要是怕冲突问题
            byte[] result = new byte[SystemSave.lengthForBuffer];
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
                    if (information != "bye" && information != "get")
                    {
                        //Console.WriteLine("\n------------------\n"+information+ "\n------------------\n");
                        //其实这个方法有非常冗余的封装，但是为了保证可扩展性可读性，暂时先不改了
                        //获取了消息information,处理过程需要新一层的封装了
                        //把信息存入到缓存里面去
                        //要保存多个数据，这里就需要做一下切分，也就是所谓的协议
                        //暂定的协议： 
                        //传输内容的大项目用';'切分
                        //传输内容的小项目用','切分
                        string[] theSplited = information.Split(';');
                        //因为信息的第一项是用来做报头了
                        if (theSplited[0] == "A")
                        {
                            for (int i = 1; i < theSplited.Length; i++)
                            {
                                //实际上下面所有的信息都会被存储，所以可以保证下标保持对应
                                switch (i)
                                {
                                    //第一大项： Y轴加速度
                                    case 1: { theInformationController.addInformation(UseDataType.accelerometerY, theSplited[1]); } break;
                                    //第二大项： 直接从unity里面获取到的角度(最先先用这个做，后期自己优化，本项也可以作为一个基础对照项)
                                    case 2: { theInformationController.addInformation(UseDataType.compassDegree, theSplited[2]); } break;//正北0度 
                                    //第三大项： X轴加速度
                                    case 3: { theInformationController.addInformation(UseDataType.accelerometerX, theSplited[3]); } break;
                                    //第四大项： Z轴加速度
                                    case 4: { theInformationController.addInformation(UseDataType.accelerometerZ, theSplited[4]); } break;
                                    //第五大项： X轴陀螺仪
                                    case 5: { theInformationController.addInformation(UseDataType.gyroX, theSplited[5]); } break;
                                    //第六大项： Y轴陀螺仪
                                    case 6: { theInformationController.addInformation(UseDataType.gyroY, theSplited[6]); } break;
                                    //第七大项： Z轴陀螺仪
                                    case 7: { theInformationController.addInformation(UseDataType.gyroZ, theSplited[7]); } break;
                                    //第八大项： X轴磁力计
                                    case 8: { theInformationController.addInformation(UseDataType.magnetometerX, theSplited[8]); } break;
                                    //第九大项： y轴磁力计
                                    case 9: { theInformationController.addInformation(UseDataType.magnetometerY, theSplited[9]); } break;
                                    //第十大项： z轴磁力计
                                    case 10: { theInformationController.addInformation(UseDataType.magnetometerZ, theSplited[10]); } break;
                                    //GPS
                                    case 11: { theInformationController.addInformation(UseDataType.GPS, theSplited[11]); } break;
                                    //时间戳
                                    case 12: { theInformationController.addInformation(UseDataType.timeStamp, theSplited[12]); } break;
                                    //AHRSZ信息
                                    case 13: { theInformationController.addInformation(UseDataType.AHRSZ, theSplited[13]); } break;
                                    //IMU信息
                                    case 14: { theInformationController.addInformation(UseDataType.IMUZ, theSplited[14]); } break;
                                }
                            }
                        }
                        else if (theSplited[0] == "B")
                        {
                            //如果网络带宽实在是不行，就考虑用这种分片的方法分着发送。
                            //这一点在客户端上也留有接口
                            /*
                            for (int i = 1; i < theSplited.Length; i++)
                            {
                                //实际上下面所有的信息都会被存储，所以可以保证下标保持对应
                                switch (i)
                                {
                                    //GPS
                                    case 1: { theInformationController.addInformation(UseDataType.GPS, theSplited[1]); } break;
                                    //时间戳
                                    case 2: { theInformationController.addInformation(UseDataType.timeStamp, theSplited[2]); } break;
                                    //AHRSZ信息
                                    case 3: { theInformationController.addInformation(UseDataType.AHRSZ, theSplited[3]); } break;
                                    //IMU信息
                                    case 4: { theInformationController.addInformation(UseDataType.IMUZ, theSplited[4]); } break;

                                }
                            }
                            */
                        }
                        //string sendString = SystemSave.allStepCount.ToString();
                        //sendString += ";" + SystemSave.stepLengthNow.ToString("f2") ;
                        //sendString += ";" + SystemSave.stepAngleNow.ToString("f2") ;
                        //myClientSocket.Send(Encoding.UTF8.GetBytes(sendString));//发送一个步数信息
                        string sendString = SystemSave.allStepCount.ToString();
                        sendString += ";" + SystemSave.stepLengthNow.ToString("f2");
                        sendString += ";" + SystemSave.stepAngleNow.ToString("f2");
                        sendString += ";" + SystemSave.slopNow.ToString("f2");
                        sendString += ";" + SystemSave.heightNow.ToString("f2");
                        myClientSocket.Send(Encoding.UTF8.GetBytes(sendString));//发送一个步数信息
                    }
                    else if (information == "get")
                    {
                        string sendString = SystemSave.allStepCount.ToString();
                        sendString += ";" + SystemSave.stepLengthNow.ToString("f2");
                        sendString += ";" + SystemSave.stepAngleNow.ToString("f2");
                        sendString += ";" + SystemSave.slopNow.ToString("f2");
                        sendString += ";" + SystemSave.heightNow.ToString("f2");
                        myClientSocket.Send(Encoding.UTF8.GetBytes(sendString));//发送一个步数信息
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
                    Console.WriteLine("传送信息失败");
                    myClientSocket.Shutdown(SocketShutdown.Both);
                    myClientSocket.Close();
                    return;
                }
            }
        }


    }
}
