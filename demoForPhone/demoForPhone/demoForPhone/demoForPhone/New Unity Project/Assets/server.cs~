using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Text;
using System;

public class server : MonoBehaviour {

	private static byte[] result = new byte[1024];  
	private static int myProt = 8886;   //端口  

	static Socket serverSocket;  
	static Thread myThread;
	static Socket clientSocketInServer;
	static Thread receiveThread;
	static Socket myClientSocket;
	static Socket clientSocket;

	public GameObject theCube;
	public static GameObject theCubeStyatic;

	private static bool play = true;
	public static bool isOpened = false;

	public static string  limkS = "未连接到服务器";
	 
	private void serverMain()
	{
 

		Thread theOpening = new Thread (openServer);
//		 

	}
 

	void openServer()
	{
		try
		{
		//服务器IP地址  
		IPAddress ip = IPAddress.Parse("219.216.73.162");  
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
	/// <summary>  
	/// 接收消息  
	/// </summary>  
	/// <param name="clientSocket"></param>  
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

	//////////////////////////////////////////////////////
 
	public void clientMain(string message = "")
	{
		 
		Thread th = new Thread (theClient);
		th.Start ();
	}  

	void theClient()
	{
 
		//设定服务器IP地址  
		IPAddress ip = IPAddress.Parse("219.216.73.162");  
		if (clientSocket == null)
		{
			clientSocket = new Socket (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);  

			try
			{  
				clientSocket.Connect (new IPEndPoint (ip, 8886)); //配置服务器IP与端口  
				isOpened = true;
				limkS  = "已连接到服务器";
				print ("连接服务器成功");  
			} catch {
				print ("连接服务器失败 ");  
				return;  
			}  

			//通过clientSocket接收数据  
			//int receiveLength = clientSocket.Receive(result);  
			//print("接收服务器消息\n"+Encoding.UTF8.GetString(result,0,receiveLength)+"\ntype: client");  
			//通过 clientSocket 发送数据  
		}
	}

	public void send(string message = "")
	{
		if (clientSocket != null)
		{
			string sendMessage = message;
			clientSocket.Send (Encoding.UTF8.GetBytes (sendMessage));
			print ("向服务器发送消息\n" + sendMessage);  
		}
	}

	void Start () 
	{
		theCubeStyatic = theCube;
		//欧拉角与Vector3的赋值大法
	}

	void OnDestroy()
	{ 
		play = false;
		if(serverSocket !=null)
		serverSocket.Close ();
		if (clientSocket != null) 
		{
			clientSocket.Send (Encoding.UTF8.GetBytes ("bye"));
			clientSocket.Close ();
			//receiveThread.Abort ();
		}
		//myThread.Abort();


	}

   void Update ()
   {
//		if (Input.GetKeyDown (KeyCode.Space)) 
//			serverMain ();
//		if (Input.GetKeyDown (KeyCode.L))
//			clientMain ();
//		if (Input.GetKeyDown (KeyCode.A))
//			send ("A");
//		if (Input.GetKeyDown (KeyCode.D))
//			send ("D");
//
//		if (theMessage == "A") {
//			theCubeStyatic.transform.Translate (new Vector3 (1, 0, 0));
//			theMessage = "";
//		} else if (theMessage == "D") {
//			theCubeStyatic.transform.Translate (new Vector3 (-1, 0, 0));
//			theMessage = "";
//		}
//		if (Input.GetKeyDown (KeyCode.B ))
//			send ("SSSSSSS");
		 
   }


}
