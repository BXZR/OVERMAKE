using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class systemValues : MonoBehaviour {
  //这个类专门用来记录一些传过来用于显示的内容
	public static bool isSystemStarted = false;//程序只可以开启一次
	public static bool isPaused = false;//是否已经暂停
	public static  int stepCountAll = 0;//总步数，这个是从服务器传过来的
	public static string titleLabel = "";//在titleLabel面板中显示的内容字符串
	public static string linkServerLabel = "服务器未连接";
	public static string GPSUSELabel = "GPS--";
	public static string  stepCountShow = "----";

	public static float showValuesCountMax = 50f;//显示50条数据之后刷新
	public static float showValueCountNow = 0f;//当前显示的条目数量
	public static int  valueCount = 0;//总数据条目
	public static int valueCountMax = 720000;//总数据量上限

	public void makeFlush()//关闭的时候做一次清理
	{
		stepCountAll = 0;
	}

 
}
