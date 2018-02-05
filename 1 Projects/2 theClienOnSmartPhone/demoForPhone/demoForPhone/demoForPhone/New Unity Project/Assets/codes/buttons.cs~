using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class buttons : MonoBehaviour {

    //一些按钮的回调方法放在这里（因为与UI的关联太紧密）
	public void changePause()
	{
		//为了保证同时使用，有一些参数上的优化不做
		systemValues.isPaused = !systemValues.isPaused;
		if (systemValues.isPaused)
			this.GetComponentInChildren<Text> ().text = "paused";
		else
			this.GetComponentInChildren<Text> ().text = "play";
	}
}
