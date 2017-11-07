using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class controller : MonoBehaviour {

    //这个类是本程序的总控单元
	server theServer;//socket控制单元
	informationShower theInformationShower;
	informationGeter theGeter;//信息采集单元

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
		theServer.clientMain ();//客户端的网络连接
		theGeter.makeStart();
		Invoke ("showTitle" , 0.7f);
		InvokeRepeating ("makeInformation", 0.5f, 0.05f);
		//瓶颈就在于这个发送时间不能太长也不能太短
		InvokeRepeating ("sendInformation" , 0.5f , 0.5f);
		InvokeRepeating ("showInformation" , 0.5f , 0.1f);//显示时间会比手机的时间间隔长一点，算是一个简单的优化
		InvokeRepeating ("showStepCount", 0.5f, 0.05f);
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
	void Update () 
	{
		
		if (Input.GetKeyDown (KeyCode.Escape))
			Application.Quit();
	}
}
