using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Text;
using System;

//这个类专门用来处理socket网络传输内容
public class server : MonoBehaviour {

	private static byte[] result = new byte[1024];  
	public static int myProt = 8886;   //端口  
	public static string  serverIP = "219.216.73.162";
	static Socket serverSocket;  
	static Thread myThread;
	static Socket clientSocketInServer;
	static Thread receiveThread;
	static Socket myClientSocket;
	static Socket clientSocket;

	private static bool play = true;
	public static bool isOpened = false;


//------------------------------------------------下面这些是真正用到的，毕竟只是客户端----------------------------------------------------------------------//
 
	public void clientMain(string message = "")
	{
		Thread th = new Thread (theClient);
		th.Start ();
	}  

	void theClient()
	{
		//设定服务器IP地址  
		IPAddress ip = IPAddress.Parse(serverIP);  
		if (clientSocket == null)
		{
			clientSocket = new Socket (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);  
			try
			{  
				clientSocket.Connect (new IPEndPoint (ip, 8886)); //配置服务器IP与端口  
				isOpened = true;
				systemValues .linkServerLabel = "已连接到服务器";
				//print ("连接服务器成功");  
			} catch {
				systemValues .linkServerLabel  = "服务器连接失败";
				//print ("连接服务器失败 ");  
				isOpened  = false;
				return;  
			}  

			//通过clientSocket接收数据  
			//int receiveLength = clientSocket.Receive(result);  
			//systemValues.stepCountShow = Encoding.UTF8.GetString (result, 0, receiveLength);
			//print("接收服务器消息\n"+Encoding.UTF8.GetString(result,0,receiveLength)+"\ntype: client");  
			//通过 clientSocket 发送数据  
		}
	}

	public void send(string message = "")
	{
		if (clientSocket != null && isOpened)
		{
			string sendMessage = message;
			clientSocket.Send (Encoding.UTF8.GetBytes (sendMessage));
			//print ("向服务器发送消息\n" + sendMessage);  
			//通过clientSocket接收数据  
			int receiveLength = clientSocket.Receive(result);  
			string reveiveString = Encoding.UTF8.GetString (result, 0, receiveLength);
			//从服务器获得的信息简单处理
			systemValues.stepCountShow = reveiveString;
			string[] split = reveiveString.Split (';');
			int stepCount = Convert.ToInt32 (split[0]);
			//角度随时会变
			systemValues.stepAngle = Convert.ToDouble (split[2]);
			if (stepCount > systemValues.stepCountNow) 
			{
				systemValues.stepCountNow = stepCount;
				systemValues.stepLengthNow = Convert.ToDouble (split[1]);
				systemValues.slopNow =  Convert.ToDouble (split[3]);
				systemValues.canFlashPosition = true;//走了一步需要更新坐标
			}
		}
	}
		
	void OnDestroy()
	{ 
		play = false;
		if(serverSocket !=null)
		serverSocket.Close ();
		if (clientSocket != null && isOpened) 
		{
			clientSocket.Send (Encoding.UTF8.GetBytes ("bye"));
			clientSocket.Close ();
			//receiveThread.Abort ();
		}
		//myThread.Abort();
	}


//-----------------------------------------预留的服务器的方法--------------------------------------------------------//
/*	private void serverMain()
	{
		Thread theOpening = new Thread (openServer);
	}
	void openServer()
	{
		try
		{
			//服务器IP地址  
			IPAddress ip = IPAddress.Parse(serverIP);  
			serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);  
			serverSocket.Bind(new IPEndPoint(ip, myProt));  //绑定IP地址：端口  
			serverSocket.Listen(10);    //设定最多10个排队连接请求  
			print("启动监听" + serverSocket.LocalEndPoint.ToString() + "成功" + "\ntype: server");  
			//通过Clientsoket发送数据  
			myThread = new Thread(ListenClientConnect);  
			myThread.Start();  
		}
		catch
		{
			print ("连接服务器失败");
		}

	}
	private   void ListenClientConnect()  
	{  

		while (play)  
		{  
			Socket clientSocket = serverSocket.Accept();  
			clientSocket.Send(Encoding.ASCII.GetBytes("Server Say Hello"));  
			//receiveThread.Abort ();
			receiveThread = new Thread(ReceiveMessage);  
			receiveThread.Start(clientSocket);  
		}  

	}  
	static	string theMessage;
	private  void ReceiveMessage(object clientSocket)
	{  
		Socket myClientSocket = (Socket)clientSocket;  
		while (play)  
		{  
			try  
			{  
				//通过clientSocket接收数据  
				int receiveNumber = myClientSocket.Receive(result); 
				//string theMessage = myClientSocket.RemoteEndPoint.ToString();
				theMessage = Encoding.ASCII.GetString(result, 0, receiveNumber);
				print("接收客户端"+"\n消息"+Encoding.ASCII.GetString(result, 0, receiveNumber)+"\ntype: server" );  
			}  
			catch(Exception ex)  
			{  
				print (ex.Message);  
				myClientSocket.Shutdown(SocketShutdown.Both);  
				myClientSocket.Close();  
				break;  
			}  
		}  
	}
*/
}
