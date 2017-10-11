using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class informationGeter : MonoBehaviour {

    //这个类专门用来处理手机传感器信息
	int allCount = 0;
	int maxCount = 720000;//这应该是一个很足够的量了，十小时的数据
	private  string information = "";//用于显示的信息缓存

	public string Information//对外只读
	{
		get{ return information;}
	}
	Gyroscope gyro;  //陀螺仪
	string informationForAY = "";
	string informationForAZ = "";
	string informationForAX = "";
	string informationForGyroDegree = ""; 



	public void makeStart()
	{
		//开启各种传感器
		Input.gyro.enabled = true; 
		Input.gyro.updateInterval = 0.05f;  
		Input.compass.enabled = true;
		Input.location .Start(10,10);
        //开启数据收集
		InvokeRepeating ("makeInformation", 0.5f, 0.05f);
		InvokeRepeating ("informationFlash", 0.5f, 2f);
	}



	//每隔一段时间清理一下，否则容量不足
	void informationFlash()
	{
		information = "";
	}


	public string  getSendInformation()
	{
		
		string sendString = informationForAY +";" + informationForGyroDegree +";" + informationForAX +";" + informationForAZ;
		informationForAY = "";
		informationForGyroDegree = "";
		informationForAX = "";
		informationForAZ = "";
		return sendString;
	}

	public void makeShowInformation()
	{
		string theInformationNow = "";
		theInformationNow += string.Format ("\n加速计:({0} , {1} , {2})",Input .acceleration.x.ToString("f2") , Input .acceleration .y.ToString("f2") , Input .acceleration .z.ToString("f2"));
		theInformationNow += string.Format ("   陀螺仪:({0} , {1} , {2})", Input .gyro .rotationRateUnbiased.x.ToString("f2") , Input .gyro .rotationRateUnbiased .y.ToString("f2") , Input  .gyro .rotationRateUnbiased.z.ToString("f2"));
		theInformationNow += "\n陀螺仪角度 ： "+Input.gyro.attitude.eulerAngles.y + "  磁力计角度： "+Input .compass.trueHeading.ToString("f2") + "  磁力计角度2： "+ Input .compass.headingAccuracy.ToString("f2")+"\n";
		theInformationNow += string.Format ("磁力计:({0} , {1} , {2})", Input .compass .rawVector .x.ToString("f2") , Input .compass .rawVector   .y.ToString("f2") , Input  .compass .rawVector .z.ToString("f2"));
		theInformationNow += string.Format("   GPS ({0},{1})", Input.location .lastData.longitude, Input .location .lastData .latitude);
		theInformationNow += "\n---------------------------\n";
		information += theInformationNow;
	}
		
	//收集传感器数据
	public void  makeInformation()
	{
		try
		{
			makeShowInformation();
			systemValues .GPSUSELabel = "GPS可用";
			if(Input .location .isEnabledByUser == false)
				systemValues .GPSUSELabel = "GPS不可用";
		
			allCount ++;
			if(allCount >maxCount)
				CancelInvoke();

			informationForAY += (Input .acceleration .y  ).ToString("f4")+",";

			informationForGyroDegree += Input .compass.trueHeading.ToString("f4")+",";
			informationForAX  += (Input .acceleration .x).ToString("f4")+",";
			informationForAZ  += (Input .acceleration .z).ToString("f4")+",";
		}
		catch(Exception d)
		{
			//message += "\nerror" + d.Message;
		}
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
		
}
