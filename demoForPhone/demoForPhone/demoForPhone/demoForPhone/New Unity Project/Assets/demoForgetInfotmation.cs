using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class demoForgetInfotmation : MonoBehaviour {

	int allCount = 0;
	int maxCount = 3000;

	public server theServer; 
	public Text theshower;

	public  static  string information = "";
	string  message = "GPS可用";
	Gyroscope gyro;  //陀螺仪


	string informationForAY = "";
	string informationForGyroDegree = ""; 


	public void makeEnd()
	{
		server.isOpened = false;
		CancelInvoke ();
		theServer .send("bye");
	}

	public void makeStart()
	{
		Input.gyro.enabled = true; 
		Input.gyro.updateInterval = 0.05f;  
		Input.compass.enabled = true;
		InvokeRepeating ("makeInformation", 0.5f, 0.05f);
		InvokeRepeating ("sendInformation", 0.5f, 1f);
		//StartCoroutine (startGPS ());
		//locationServerStatus = Input.location.status; //返回设备服务状态  
		Input.location .Start(10,10);
		theServer.clientMain ();
	}






	public void sendInformation()
	{
		string sendString = informationForAY +";" + informationForGyroDegree +";";
		theServer.send (sendString);
		informationForAY = "";
		informationForGyroDegree = "";
	}

	public void  makeInformation()
	{
		try
		{
			allInformationMake();
			if(Input .location .isEnabledByUser == false)
				message = "GPS不可用";
			theshower.text ="<color=#8E1717>"  + message +"</color>   <color=#FFFF00>"+ server .limkS +"</color> "+"\n\n" +information;
			allCount ++;
			if(allCount >maxCount)
				CancelInvoke();

			informationForAY += (Input .acceleration .y).ToString("f4")+",";
			informationForGyroDegree += Input .compass.trueHeading.ToString("f4")+",";
		}
		catch(Exception d)
		{
			//message += "\nerror" + d.Message;
		}
	}



////////////////////////////////////////////////////////

	void allInformationMake()
	{
		string theInformationNow = "";
		theInformationNow += string.Format ("\n加速计:({0} , {1} , {2})",Input .acceleration.x.ToString("f2") , Input .acceleration .y.ToString("f2") , Input .acceleration .z.ToString("f2"));
		theInformationNow += string.Format ("   陀螺仪:({0} , {1} , {2})", Input .gyro .rotationRateUnbiased.x.ToString("f2") , Input .gyro .rotationRateUnbiased .y.ToString("f2") , Input  .gyro .rotationRateUnbiased.z.ToString("f2"));
		theInformationNow += string.Format ("   磁力计:({0} , {1} , {2})", Input .compass .rawVector .x.ToString("f2") , Input .compass .rawVector   .y.ToString("f2") , Input  .compass .rawVector .z.ToString("f2"));
		theInformationNow += string.Format("\nGPS ({0},{1})", Input.location .lastData.longitude, Input .location .lastData .latitude);
		information += theInformationNow;
	}
	/*
	 *LocationInfo
		属性如下：
		（1） altitude -- 海拔高度 
		（2） horizontalAccuracy -- 水平精度 
		（3） latitude -- 纬度 
		（4） longitude -- 经度 
		（5） timestamp -- 最近一次定位的时间戳，从1970开始 
		（6） verticalAccuracy -- 垂直精度  
	*/
	//获取GPS信息作为基准量
	private LocationServiceStatus locationServerStatus;  
	IEnumerator  startGPS()
	{
		locationServerStatus = Input.location.status; //返回设备服务状态  
		if (!Input.location.isEnabledByUser) {  
			//this.gps_info = "isEnabledByUser value is:"+Input.location.isEnabledByUser.ToString()+" Please turn on the GPS";   
			yield	return false;  
		}  

		//LocationService.Start();// 启动位置服务的更新,最后一个位置坐标会被使用  
		Input.location.Start(10.0f, 10.0f);  

		int maxWait = 20;  
		while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0) {  
			// 暂停协同程序的执行(1秒)  
			yield return new WaitForSeconds(1);  
			maxWait--;  
		}  

		if (maxWait < 1) {  
			//this.gps_info = "Init GPS service time out";  
			yield return false;  
		}  

		if (Input.location.status == LocationServiceStatus.Failed) {  
			//this.gps_info = "Unable to determine device location";  
			yield return false;  
		}   
		else {  
			print ("GPS is OK");
			//this.gps_info = "N:" + Input.location.lastData.latitude + " E:"+Input.location.lastData.longitude;  
			//this.gps_info = this.gps_info + " Time:" + Input.location.lastData.timestamp;  
			//yield return new WaitForSeconds(100);  
			//其实开启之后就可以自由取用了
		}  
	}

	void Start () 
	{
		//makeStart ();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Escape))
			Application.Quit();
	}
}
