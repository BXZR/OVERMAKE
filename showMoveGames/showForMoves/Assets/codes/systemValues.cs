﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class systemValues : MonoBehaviour {

  //简单记录一下信息
	public static 	string linkServerLabel = "-------";
	public static int stepCountNow = 0;//记录走过多少步
	public static double stepLengthNow  = 0; //当前步长
	public static  double stepAngle = 0;//当前角度

	public static bool canFlashPosition = false;//是否可以更新人物坐标
}
