﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class inputChange : MonoBehaviour {

	//使用面板回调来调用这个方法，并不常用，考虑泛用性的功能
	public void changeServerIP()
	{
		server.serverIP = this.GetComponent <InputField> ().text;
	}
	public  void  changeServerPort()
	{
		server.myProt = Convert.ToInt32(this.GetComponent <InputField> ().text);
	}
	public void changeServerHZ()
	{
		server.HZ = Convert.ToInt32 (this.GetComponent <InputField> ().text);
		server.HZ = Mathf.Clamp(server.HZ , 0 ,1000);
	}
}