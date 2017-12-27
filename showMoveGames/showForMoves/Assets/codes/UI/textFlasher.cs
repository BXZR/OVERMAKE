using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Net;

public class textFlasher : MonoBehaviour {

	//内嵌控件有时候无法输入，所以当IP不对的时候会很尴尬
	//所幸的是内嵌的内容都必定会在本地
	//多以直接赋值127.0.0.1也好

	public InputField textToChange;

	public void makeLocal()
	{
		textToChange.text =  getIPAddress();
	}

	public static string getIPAddress()
	{
		//这个获取IP地址的方法非常简单粗暴但是不是公网的IP
		//string IP = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName()).AddressList.FirstOrDefault<IPAddress>(a => a.AddressFamily.ToString().Equals("InterNetwork")).ToString();
		//真正使用的获得IP的手段是下面更细节的方法
		System.Net.IPAddress[] addressList = System.Net.Dns.GetHostByName(System.Net.Dns.GetHostName()).AddressList;

		//for (int i = 0; i < addressList.Length; i++) 
		//{
		//	print("IP["+i+"] = "+ addressList[i]);
		//}

		//string IP1 = addressList[0].ToString();
		//string IP2 = addressList[1].ToString();
		//print ("IP1 = "+IP1+"\nIP2 = "+ IP2);
		//print ("IP = "+IP);
		string IP = addressList[7].ToString();
		return IP;
	}
}
