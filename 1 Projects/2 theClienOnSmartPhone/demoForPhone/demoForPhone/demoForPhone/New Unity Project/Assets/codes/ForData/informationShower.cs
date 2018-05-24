using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class informationShower : MonoBehaviour {
	//这个类专门用于显示界面信息
	public Text titleLabelText;//用于显示title的text
	public Text informationLabelText;//用于显示title的text
	public Text stepCountShowText ;//显示总步数的label
	public Scrollbar theShowBar ;//显示用的slider
	private string showInformation = "";//显示在面板的传感器信息
	private string informationNow = "";//每一个时间间隔的信息


	public  void showTitle()
	{
		titleLabelText.text = "<color=#FF0F00>"  + systemValues .GPSUSELabel  +"</color>   <color=#FFFF00>"+ systemValues.linkServerLabel+"</color> ";
		titleLabelText.text += "\n服务器IP: " + server.serverIP + "\n服务器端口: " + server.myProt + "\n采样频率: " + server.HZ;
	}

	public  void  showValues(string showInformation)
	{
		informationLabelText.text = showInformation;
		theShowBar .value  =  1- systemValues .showValueCountNow / systemValues .showValuesCountMax;
	}


	public void showSteps()
	{
		string[] Splits = systemValues.stepCountShow.Split(';');
		string showInformation = "";
		if (Splits.Length >= 5)
		{
			showInformation += "步数：" + systemValues.stepCountNow;
			showInformation += "\n步长：" + systemValues.stepLengthNow;
			showInformation += "\n方向：" + systemValues.stepAngle ;
			showInformation += "\nZ轴状态：" + systemValues.stairModeNow.ToString ("f0");
			//showInformation += "\nSlop：" + systemValues.slopNow ;
			showInformation += "\n坐标：\n"+ systemValues.positionNow;
			stepCountShowText.text = showInformation;
		}
		else 
		{
			stepCountShowText.text = "没有消息";
		}
	}
	void Start () 
	{
		try
		{
			showTitle();
		}
		catch
		{
			print ("初始化未完成，无输出");
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
