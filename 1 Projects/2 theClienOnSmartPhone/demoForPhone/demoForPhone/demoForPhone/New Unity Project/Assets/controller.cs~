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
		
	public void makeStart()
	{
		theServer = this.GetComponent <server> ();	
		theInformationShower = this.GetComponent <informationShower> ();
		theGeter = this.GetComponent <informationGeter> ();
		theServer.clientMain ();//客户端的网络连接
		theGeter.makeStart();
		Invoke ("showTitle" , 0.7f);
		InvokeRepeating ("sendInformation" , 0.5f , 1f);
		InvokeRepeating ("showInformation" , 0.5f , 0.05f);
		InvokeRepeating ("showStepCount", 0.5f, 0.05f);
	}


	void showStepCount()
	{
		theInformationShower.showSteps ();
	}

	void showTitle()
	{
		if(theInformationShower)
			theInformationShower.showTitle ();
	}

	public void makeEnd()
	{
		server.isOpened = false;
		CancelInvoke ();
	}
	void showInformation()
	{
		theInformationShower.showValues (theGeter.Information);
	}
	void sendInformation()
	{
		string sendString = theGeter.getSendInformation ();
		theServer.send (sendString);
	}
	void Update () 
	{
		
		if (Input.GetKeyDown (KeyCode.Escape))
			Application.Quit();
	}
}
