using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class modeChangeButton : MonoBehaviour {

	//客户端采集数据改变StairMode的按钮

	Text theText ;
	public bool isStair = false;
	void Start () 
	{
		theText = this.GetComponentInChildren<Text> ();
		if(isStair)
			theText.text = systemValues.getStairModeStirng ();
		else
			theText.text = systemValues.getStepModeStirng ();
	}


	public void changeStairMode()
	{
		systemValues.changeStairModeNow ();
		if(theText)
			theText.text = systemValues.getStairModeStirng ();
	}

	public  void  changeStepMode()
	{
		systemValues.changeStepModeNow ();
		if (theText)
			theText.text = systemValues.getStepModeStirng ();
	}
}