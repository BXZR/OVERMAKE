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
	//一些用于计算的私有参数
	DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(2017, 10, 1)); //这个是用于计算时间戳用的基础时间
	//格林威治时间

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

		return "A;"+sendString;
	}
	//同时传太多会丢包
	public string getSendInformation2()
	{
		//注意：发送的信息大项目以";"作为分隔符
		//大项目内部以“，"作为分隔符
		string sendString = informationForGPSPosition +";" +informationForTimer +";"+AHRSZ ;

		informationForGPSPosition= "";
		informationForTimer = "";
		AHRSZ = "";
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
			
			informationForAY += (Input .acceleration .y  ).ToString("f4")+",";
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

			double SZUse =  AHRSupdate
				(Input.gyro.rotationRateUnbiased.x, Input.gyro.rotationRateUnbiased.y,Input.gyro.rotationRateUnbiased.z, 
					Input .acceleration .x, Input .acceleration .y, Input .acceleration .z, 
					Input .compass.rawVector .x, Input .compass.rawVector .y, Input .compass.rawVector .z
				) + 180;
			AHRSZ  +=  SZUse .ToString("f4")+",";
		}
		catch(Exception d)
		{
			//message += "\nerror" + d.Message;
		}
	}



	//方法3  AHRS算法代码：磁力计+加计+陀螺版（来自网络有待进一步弄一波）

	double Kp = 2.0;                     // proportional gain governs rate of convergence to accelerometer/magnetometer
	double Ki = 0.005;                // integral gain governs rate of convergence of gyroscope biases
	double halfT = 0.025;                //必须设置为采样频率的一半
	double q0 = 1, q1 = 0, q2 = 0, q3 = 0;        // quaternion elements representing the estimated orientation
	double exInt = 0, eyInt = 0, ezInt = 0;        // scaled integral error

	////这个方法不可以每一次计算都会修改全局的数值，所以调用次数需要慎用
	public double AHRSupdate(double gx, double gy, double gz, double ax, double ay, double az, double mx, double my, double mz)
	{

		//Console.WriteLine(string .Format("{0},{1},{2},{3},{4},{5},{6},{7},{8}" , gx,gy,gz,ax,ay,az,mx,my,mz));
		double norm;
		double hx, hy, hz, bx, bz;
		double vx, vy, vz, wx, wy, wz;
		double ex, ey, ez;

		// auxiliary variables to reduce number of repeated operations
		double q0q0 = q0 * q0;
		double q0q1 = q0 * q1;
		double q0q2 = q0 * q2;
		double q0q3 = q0 * q3;
		double q1q1 = q1 * q1;
		double q1q2 = q1 * q2;
		double q1q3 = q1 * q3;
		double q2q2 = q2 * q2;
		double q2q3 = q2 * q3;
		double q3q3 = q3 * q3;

		// normalise the measurements
		norm = Math.Sqrt(ax * ax + ay * ay + az * az);
		ax = ax / norm;
		ay = ay / norm;
		az = az / norm;
		norm = Math.Sqrt(mx * mx + my * my + mz * mz);
		mx = mx / norm;
		my = my / norm;
		mz = mz / norm;

		// compute reference direction of flux
		hx = 2 * mx * (0.5 - q2q2 - q3q3) + 2 * my * (q1q2 - q0q3) + 2 * mz * (q1q3 + q0q2);
		hy = 2 * mx * (q1q2 + q0q3) + 2 * my * (0.5 - q1q1 - q3q3) + 2 * mz * (q2q3 - q0q1);
		hz = 2 * mx * (q1q3 - q0q2) + 2 * my * (q2q3 + q0q1) + 2 * mz * (0.5 - q1q1 - q2q2);
		bx = Math.Sqrt((hx * hx) + (hy * hy));
		bz = hz;

		// estimated direction of gravity and flux (v and w)
		vx = 2 * (q1q3 - q0q2);
		vy = 2 * (q0q1 + q2q3);
		vz = q0q0 - q1q1 - q2q2 + q3q3;
		wx = 2 * bx * (0.5 - q2q2 - q3q3) + 2 * bz * (q1q3 - q0q2);
		wy = 2 * bx * (q1q2 - q0q3) + 2 * bz * (q0q1 + q2q3);
		wz = 2 * bx * (q0q2 + q1q3) + 2 * bz * (0.5 - q1q1 - q2q2);

		// error is sum of cross product between reference direction of fields and direction measured by sensors
		ex = (ay * vz - az * vy) + (my * wz - mz * wy);
		ey = (az * vx - ax * vz) + (mz * wx - mx * wz);
		ez = (ax * vy - ay * vx) + (mx * wy - my * wx);

		// integral error scaled integral gain
		exInt = exInt + ex * Ki;
		eyInt = eyInt + ey * Ki;
		ezInt = ezInt + ez * Ki;

		// adjusted gyroscope measurements
		gx = gx + Kp * ex + exInt;
		gy = gy + Kp * ey + eyInt;
		gz = gz + Kp * ez + ezInt;

		// integrate quaternion rate and normalise
		q0 = q0 + (-q1 * gx - q2 * gy - q3 * gz) * halfT;
		q1 = q1 + (q0 * gx + q2 * gz - q3 * gy) * halfT;
		q2 = q2 + (q0 * gy - q1 * gz + q3 * gx) * halfT;
		q3 = q3 + (q0 * gz + q1 * gy - q2 * gx) * halfT;

		// normalise quaternion
		norm = Math.Sqrt(q0 * q0 + q1 * q1 + q2 * q2 + q3 * q3);
		q0 = q0 / norm;
		q1 = q1 / norm;
		q2 = q2 / norm;
		q3 = q3 / norm;
		// Console.WriteLine(string .Format ("q0 = {0} , q1 = {1} , q2 = {2} , q3 = {3}" , q0,q1,q2,q3));
		//四元数转换欧拉角
		double roll = Math.Atan2(2.0f * (q0 * q1 + q2 * q3), 1 - 2.0f * (q1 * q1 + q2 * q2))* 57.3;
		double pitch = Math.Asin(2.0f*(q0* q2 - q1* q3)) * 57.3;
		double yaw = Math.Atan2(2.0f * (q1 * q2 - q0 * q3), 2.0f * (q0 * q0 + q1 * q1) - 1) * 57.3;

		//Console.WriteLine("Y = " + Y);
		return yaw; //返回偏航角
	}


	/*
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
