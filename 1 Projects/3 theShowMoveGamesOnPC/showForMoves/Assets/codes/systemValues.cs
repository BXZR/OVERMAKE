using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class systemValues : MonoBehaviour {

  //简单记录一下信息
	public static 	string linkServerLabel = "-------";
	public static int stepCountNow = 0;//记录走过多少步
	public static double stepLengthNow  = 0; //当前步长
	public static  double stepAngle = 0;//当前角度
	public static double slopNow = 0;//当前姿态的一个小小的推断，如果比较大就认为是在奔跑
	public static double height = 0;//当前姿态的一个小小的推断，如果比较大就认为是在奔跑
	public static string thePosition = "(0.00,0.00,0.00)";//这个客户端显示的坐标
	public static bool canFlashPosition = false;//是否可以更新人物坐标

	public static serverDataShowText theServerDataLabel;//用来显示来自server的Data数据的text
}
