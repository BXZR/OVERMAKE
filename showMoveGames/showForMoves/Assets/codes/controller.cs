using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class controller : MonoBehaviour {

	public moveWithSocket theServer;
	public playerMoveWithWeb thePlayer;
	public void makeStart()
	{
		theServer = this.GetComponent <moveWithSocket> ();
		theServer.clientMain ();
		InvokeRepeating ("send" , 0.3f, 0.1f);
		InvokeRepeating ("flashPosition", 0.3f, 0.1f);//更新快一点，更加灵敏
	}

	private  void flashPosition()
	{
		thePlayer.flashPosition ();
	}
	private void send()
	{
		//print ("dddf");
		theServer.send ();

	}
}
