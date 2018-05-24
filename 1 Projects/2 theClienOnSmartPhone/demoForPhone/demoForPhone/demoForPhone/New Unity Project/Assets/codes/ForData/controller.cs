using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class controller : MonoBehaviour {

    //这个类是本程序的总控单元
	server theServer;//socket控制单元
	informationShower theInformationShower;
	informationGeter theGeter;//信息采集单元
	OperateServer theOperateServer;//客户端操作服务端需要传送的信息的库
	void Start () 
	{

	}

	//button调用的方法 -----------------------------------------------------------------

	public void makeStart()
	{
		if (systemValues.isSystemStarted)
			return;
		systemValues.isSystemStarted = true;
		theServer = this.GetComponent <server> ();	
		theInformationShower = this.GetComponent <informationShower> ();
		theGeter = this.GetComponent <informationGeter> ();
		theOperateServer = this.GetComponent <OperateServer> ();
		theServer.clientMain ();//客户端的网络连接
		theGeter.makeStart();
		Invoke ("showTitle" , 0.7f);
		float timer = 1f / (float)server.HZ;

		InvokeRepeating ("makeInformation", 0.5f, timer);
		//瓶颈就在于这个发送时间不能太长也不能太短
		InvokeRepeating ("sendInformation" , 0.5f , timer*10);
		InvokeRepeating ("showInformation" , 0.5f , timer *2);//显示时间会比手机的时间间隔长一点，算是一个简单的优化
		InvokeRepeating ("showStepCount", 0.5f, timer);
	}

	void showTitle()
	{
		if(theInformationShower)
			theInformationShower.showTitle ();
	}

	public void makeEnd()
	{
		systemValues.isSystemStarted = false;
		server.isOpened = false;
		CancelInvoke ();
	}
	//Invoke调用的方法 ---------------------------------------------------------------------------------
	void makeInformation()
	{
		if (systemValues.isPaused)
			return;
		theGeter.makeInformation ();
	}

	void showStepCount()
	{
		if(systemValues .isPaused)
			return;
		theInformationShower.showSteps ();
	}
	void showInformation()
	{
		if(systemValues .isPaused)
			return;
		theInformationShower.showValues (theGeter.Information);
	}
	void sendInformation()
	{
		if (systemValues.isPaused)
			return;
	    string sendString = theGeter.getSendInformation ();
		theServer.send (sendString);

		//sendString = theGeter.getSendInformation2 ();
		//theServer.send (sendString);
	}

	public void sendClientOperateServerString(int index )
	{
		string operateSend = theOperateServer.getSentInformation (index);
		print ("OperateSend => "+ operateSend);
		theServer.send (operateSend);
	}


	void Update () 
	{
		
		if (Input.GetKeyDown (KeyCode.Escape)) 
		{
			//菜单界面直接退出
			if (Application.loadedLevelName == "SelectScene")
				Application.Quit ();
			else 
			{
				//直接退回到开始界面
				//资源等等的回收和处理问题由client的OnDestroy自行处理在这里不用管
				UnityEngine.SceneManagement.SceneManager.LoadScene ("SelectScene");
			}
		}
	}
}
