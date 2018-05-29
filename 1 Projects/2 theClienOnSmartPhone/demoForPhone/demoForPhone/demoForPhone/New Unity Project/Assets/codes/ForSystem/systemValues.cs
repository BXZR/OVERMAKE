using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class systemValues : MonoBehaviour {

	//--------------------------------Data显示发送与获取部分-------------------------------------//
  //这个类专门用来记录一些传过来用于显示的内容
	public static bool isSystemStarted = false;//程序只可以开启一次
	public static bool isPaused = false;//是否已经暂停
	public static  int stepCountAll = 0;//总步数，这个是从服务器传过来的
	public static string titleLabel = "";//在titleLabel面板中显示的内容字符串
	public static string linkServerLabel = "服务器未连接";
	public static string GPSUSELabel = "GPS不可用";
	public static string  stepCountShow = "----";

	public static float showValuesCountMax = 50f;//显示50条数据之后刷新
	public static float showValueCountNow = 0f;//当前显示的条目数量
	public static int  valueCount = 0;//总数据条目
	public static int valueCountMax = 720000;//总数据量上限

	//这个数值是由用户控制的，用来收集当前的数据状态====================================
	public static int stairModeNow = 1;//0下1平2上
	public static int stepModeNow = 1;//0停1走 用作stepFilter的分类剔除
	public static void changeStairModeNow()
	{
		stairModeNow++;
		if (stairModeNow > 2)
			stairModeNow = 0;
	}
	public static string getStairModeStirng()
	{
		if (stairModeNow == 1)
			return "直行状态";
		if (stairModeNow == 0)
			return "下楼状态";
		if (stairModeNow == 2)
			return "上楼状态";

		return "未知状态";
	}

	public static void changeStepModeNow()
	{
		stepModeNow++;
		if (stepModeNow > 1)
			stepModeNow = 0;
	}
	public static string getStepModeStirng()
	{
		if (stepModeNow == 1)
			return "移动状态";
		if (stepModeNow == 0)
			return "停止状态";

		return "未知状态";
	}

	//====================================================================================
	public void makeFlush()//关闭的时候做一次清理
	{
		stepCountAll = 0;
	}

	//--------------------------------游戏部分-------------------------------------//
	//简单记录一下信息
	public static int stepCountNow = 0;//记录走过多少步
	public static double stepLengthNow  = 0; //当前步长
	public static  double stepAngle = 0;//当前角度
	public static double slopNow = 0;//当前姿态的一个小小的推断，如果比较大就认为是在奔跑
	public static double heightNow = 0;//当前姿态的一个小小的推断，如果比较大就认为是在奔跑
	public static  string positionNow = "(0.00,0.00,0.00)";//当前坐标
	public static bool canFlashPosition = false;//是否可以更新人物坐标
}
