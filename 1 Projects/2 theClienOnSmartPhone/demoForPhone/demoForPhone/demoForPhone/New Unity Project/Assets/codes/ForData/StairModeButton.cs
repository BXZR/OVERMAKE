using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StairModeButton : MonoBehaviour {

	//客户端采集数据改变StairMode的按钮

	Text theText ;

	void Start () 
	{
		theText = this.GetComponentInChildren<Text> ();
		theText.text = systemValues.getStairModeStirng ();
	}


	public void changeStairMode()
	{
		systemValues.changeStairModeNow ();
		if(theText)
			theText.text = systemValues.getStairModeStirng ();
	}
		
}
