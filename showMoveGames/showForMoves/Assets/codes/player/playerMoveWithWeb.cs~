using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerMoveWithWeb : MonoBehaviour {

	//根据网络传输的信息来判断向着哪一个方向走多少距离
	private Vector3 aimPosition;//记录上一步的目标
	float speed= 2f;//这才是在游戏中真正移动的速度
	Animator theAnimator;//动画控制单元
	//传过来的速度作为移动的步长而存在
	void Start () 
	{
		theAnimator = this.GetComponent <Animator> ();
		aimPosition = this.transform.root.position+ new Vector3 (0,0,2);
	}
	public void flashPosition()
	{
		if (systemValues.canFlashPosition) 
		{
			systemValues.canFlashPosition = false;
			this.transform.rotation= Quaternion.Euler(0, (float)systemValues.stepAngle, 0);
//			int valueADD = 1;//正负号标记
//			//如果角度是0——— 90或者 270 ——360就是1
//			//如果是 90 ——270 就是-1
//			if (systemValues.stepAngle > 90 && systemValues.stepAngle < 270)
//				valueADD = -1;
			
			Vector3 aimPositionNow = this.transform.root.position + this.transform .forward *(float)systemValues.stepLengthNow;
			if(aimPosition != aimPositionNow)//如果来了一个新的目标
			{
				if (Vector3.Distance (aimPosition, this.transform.root.transform.position) < 0.01f)
				{
					aimPosition = aimPositionNow;
				}
				else
				{
					//跳变
					this.transform.root.transform.position = aimPosition;
					aimPosition = aimPositionNow;
				}
			}
		}
	}

	private  void animationControl()
	{
		if (Vector3.Distance (aimPosition, this.transform.root.transform.position) < 0.01f)
		{
			theAnimator.Play ("idle");
		} 
		else
		{
			theAnimator.Play("walk");
		}
	}
	void Update () 
	{
		this.transform.root .position = Vector3.Lerp(this.transform .root .transform.position  , aimPosition , speed*Time.deltaTime);
		animationControl ();
	}
}
