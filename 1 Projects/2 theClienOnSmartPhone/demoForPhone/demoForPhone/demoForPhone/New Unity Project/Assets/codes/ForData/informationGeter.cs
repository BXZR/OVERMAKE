using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class informationGeter : MonoBehaviour {

    //这个类专门用来处理手机传感器信息
	private  string information = "";//用于显示的信息缓存
	private string GPSState = "";
	public string Information//对外只读
	{
		get{ return information;}
	}
	Gyroscope gyro;  //陀螺仪
	string informationForAY = "";
	string informationForAZ = "";
	string informationForAX = "";
	string informationForGyroDegree = ""; 
	string informationForGPSPosition= "";
	string informationForTimer = "";
	//加速计
	string informationForGY = "";
	string informationForGZ = "";
	string informationForGX = "";
	//磁力计
	string informationForMY = "";
	string informationForMZ = "";
	string informationForMX = "";

	//AHRS算法1的结果
	string AHRSZ = "";
	string AHRSZ2 = "";
	//IMU算法的结果
	string IMUZ = "";

	//一些用于计算的私有参数
	DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(2017, 10, 1)); //这个是用于计算时间戳用的基础时间
	//格林威治时间

	//两个在客户端使用的活动当前移动方向的方法
	AHRS  theAHRSController = new AHRS ();
	IMU theIMUController = new IMU ();

	public void makeStart()
	{
		//开启各种传感器
		Input.gyro.enabled = true; 
		Input.gyro.updateInterval = 0.05f;  
		Input.compass.enabled = true;
		Input.location .Start(0.75f,0.75f);
		//StartCoroutine(StartGPS());
        //开启数据收集

		//InvokeRepeating ("flash", 0.5f, 2f);
	}

	public string  getSendInformation()
	{
		//注意：发送的信息大项目以";"作为分隔符
		//大项目内部以“，"作为分隔符
		string sendString = informationForAY +";" + informationForGyroDegree +";" + informationForAX +";" + informationForAZ;
		sendString += ";" + informationForGX + ";" + informationForGY + ";" + informationForGZ  ;
		sendString += ";" + informationForMX + ";" + informationForMY + ";" + informationForMZ;


		informationForAY = "";
		informationForGyroDegree = "";
		informationForAX = "";
		informationForAZ = "";
		informationForGX = "";
		informationForGY = "";
		informationForGZ = "";
		informationForMX = "";
		informationForMY = "";
		informationForMZ = "";

		sendString += ";" +informationForGPSPosition +";" +informationForTimer +";"+AHRSZ  +";"+ IMUZ;

		informationForGPSPosition= "";
		informationForTimer = "";
		AHRSZ = "";
		IMUZ = "";
		return "A;"+sendString;
	}


	//同时传太多会丢包
	public string getSendInformation2()
	{
		//注意：发送的信息大项目以";"作为分隔符
		//大项目内部以“，"作为分隔符
		string sendString = informationForGPSPosition +";" +informationForTimer +";"+AHRSZ  +";"+ IMUZ;

		informationForGPSPosition= "";
		informationForTimer = "";
		AHRSZ = "";
		IMUZ = "";
		return "B;"+sendString;
	}


	void flash()
	{
		information = "";
	}
	public void makeShowInformation()
	{
		//为了减少Invoke并且增加可控制性，使用的是数值来控制
		//每隔一段时间清理一下，否则容量不足(UI无法承受这么多内容)
		systemValues .showValueCountNow ++;
		if (systemValues.showValueCountNow >= systemValues.showValuesCountMax) 
		{
			systemValues.showValueCountNow = 0;
			information = "";
		}

		systemValues.GPSUSELabel = "GPS状态：" + Input.location.status.ToString ();//+" "+GPSState  ;


		string theInformationNow = "";
		System.DateTime now = System.DateTime.Now;

		theInformationNow += string.Format("\n--------------------------------{0}-------------------------------" , now .ToLongTimeString());
		theInformationNow += string.Format ("\n加速计:({0} , {1} , {2})",Input .acceleration.x.ToString("f2") , Input .acceleration .y.ToString("f2") , Input .acceleration .z.ToString("f2"));
		theInformationNow += string.Format ("   陀螺仪:({0} , {1} , {2})", Input .gyro .rotationRateUnbiased.x.ToString("f2") , Input .gyro .rotationRateUnbiased .y.ToString("f2") , Input  .gyro .rotationRateUnbiased.z.ToString("f2"));
		theInformationNow += "\n陀螺仪角度 ： "+Input.gyro.attitude.eulerAngles.y + "  磁力计角度： "+Input .compass.trueHeading.ToString("f2") + "  磁力计角度2： "+ Input .compass.headingAccuracy.ToString("f2")+"\n";
		theInformationNow += string.Format ("磁力计:({0} , {1} , {2})", Input .compass.rawVector .x.ToString("f2") , Input .compass .rawVector.y.ToString("f2") , Input.compass .rawVector .z.ToString("f2"));
		theInformationNow += string.Format("   GPS ({0},{1})\n", Input.location .lastData.longitude, Input .location .lastData .latitude);
		information += theInformationNow;
	}
		
	//收集传感器数据
	public void  makeInformation()
	{
		try
		{
			makeShowInformation();


			systemValues . valueCount ++;
			if(systemValues .valueCount > systemValues .valueCountMax)
				CancelInvoke();
			
			informationForAY += (Input .acceleration.y  ).ToString("f4")+",";
			informationForGyroDegree += Input .compass.trueHeading.ToString("f4")+",";
			informationForAX  += (Input .acceleration .x).ToString("f4")+",";
			informationForAZ  += (Input .acceleration .z).ToString("f4")+",";

			informationForGX += Input  .gyro .rotationRateUnbiased.x.ToString("f4")+",";
			informationForGY += Input  .gyro .rotationRateUnbiased.y.ToString("f4")+",";
			informationForGZ += Input  .gyro .rotationRateUnbiased.z.ToString("f4")+",";

			informationForMX += Input .compass.rawVector .x.ToString("f4")+",";
			informationForMY += Input .compass.rawVector .y.ToString("f4")+",";
			informationForMZ += Input .compass.rawVector .z.ToString("f4")+",";

			informationForGPSPosition += Input.location .lastData.longitude +","+Input .location .lastData .latitude+",";
			long timeStamp = (long)(DateTime.Now - startTime).TotalMilliseconds; // 相差毫秒数
			informationForTimer +=  timeStamp +",";

			double SZUse =   theAHRSController .AHRSupdate2
				(Input.gyro.rotationRateUnbiased.x, Input.gyro.rotationRateUnbiased.y,Input.gyro.rotationRateUnbiased.z, 
					Input .acceleration .x, Input .acceleration .y, Input .acceleration .z, 
					Input .compass.rawVector .x, Input .compass.rawVector .y, Input .compass.rawVector .z
				) ;
			AHRSZ  +=  SZUse .ToString("f4")+",";

			double IMUZUse = theIMUController.IMUupdate
				(
					Input.gyro.rotationRateUnbiased.x, Input.gyro.rotationRateUnbiased.y,Input.gyro.rotationRateUnbiased.z, 
					Input .acceleration .x, Input .acceleration .y, Input .acceleration .z , Input .compass.trueHeading , Input .acceleration
				);
			IMUZ += IMUZUse.ToString("f4")+",";
		}
		catch(Exception d)
		{
			//message += "\nerror" + d.Message;
		}
	}

	 
	/*这是关于GPS调用的功能说明
	 LocationInfo
		属性如下：
		（1） altitude -- 海拔高度 
		（2） horizontalAccuracy -- 水平精度 
		（3） latitude -- 纬度 
		（4） longitude -- 经度 
		（5） timestamp -- 最近一次定位的时间戳，从1970开始 
		（6） verticalAccuracy -- 垂直精度  
	*/
	//获取GPS信息作为基准量
	/*
	IEnumerator StartGPS()  
	{  
		// Input.location 用于访问设备的位置属性（手持设备）, 静态的LocationService位置    
		// LocationService.isEnabledByUser 用户设置里的定位服务是否启用    
		if (!Input.location.isEnabledByUser)  
		{  
			GetGps = "isEnabledByUser value is:" + Input.location.isEnabledByUser.ToString() + " Please turn on the GPS";  
			yield return false;  
		}  

		// LocationService.Start() 启动位置服务的更新,最后一个位置坐标会被使用    
		Input.location.Start(10.0f, 10.0f);  

		int maxWait = 20;  
		while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)  
		{  
			// 暂停协同程序的执行(1秒)    
			yield return new WaitForSeconds(1);  
			maxWait--;  
		}  

		if (maxWait < 1)  
		{  
			GetGps = "Init GPS service time out";  
			yield return false;  
		}  

		if (Input.location.status == LocationServiceStatus.Failed)  
		{  
			GetGps = "Unable to determine device location";  
			yield return false;  
		}  
		else  
		{  
			GetGps = "N:" + Input.location.lastData.latitude + " E:" + Input.location.lastData.longitude;  
			GetGps = GetGps + " Time:" + Input.location.lastData.timestamp;  
			yield return new WaitForSeconds(100);  
		}  
	}
   */
		
}
