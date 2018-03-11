using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OperateServer : MonoBehaviour {

	//这个类专门用来处理客户端对服务端的控制
    //机制还是传输各种字符串
	//传输的过程仍然是在server里面使用的，这里只是返回用于传输的字符串
	//这样做是为了方式扩展多个操作的时候出现问题

	//index为操作表需要传输到额信息的下标
	//extraInformation为可能需要添加的额外项目，默认是空的
	public string getSentInformation(int index ,string extraInformation = "")
	{
		string returnInformation = "operate;"+ operates [index];
		if (string.IsNullOrEmpty (extraInformation) == false)
			returnInformation += ";" + extraInformation;
		return returnInformation;
	}

	//记录当前的操作信息
	private string[] operates = new string[] {
		//消除服务端的绘制信息，但是当前位置不变化
		"flashPosition"
	};
}
	