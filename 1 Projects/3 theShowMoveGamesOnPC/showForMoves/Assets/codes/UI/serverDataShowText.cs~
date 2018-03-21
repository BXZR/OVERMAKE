using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class serverDataShowText : MonoBehaviour {

    //这个是用来显示来自server传过来的数据的Label
	//显示的时机由控制器controller来控制

	private Text theShowtext;

	void Start () 
	{
		systemValues.theServerDataLabel = this;
		theShowtext = this.GetComponent<Text> ();
	}
	
	//显示相关内容
	public void showText(string information)
	{
		if (theShowtext) 
		{
			theShowtext.text = information;
		}
	}

	//直接显示约定的内容
	public void showText()
	{
		if (theShowtext) 
		{
			string information = "";

			information += "步数： "+  systemValues.stepCountNow ;
			information += "\n当前步长： "+  systemValues.stepLengthNow ;
			information += "\n当前方向： "+  systemValues.stepAngle;
			//information += "Slop： "+  systemValues.slopNow;
			//information += "当前高度： "+  systemValues.height;
			information += "\n相对坐标： "+   systemValues.thePosition;

			theShowtext.text = information;
		}
	}
}
