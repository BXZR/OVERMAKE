using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerMoceWithWeb : MonoBehaviour {

	//根据网络传输的信息来判断向着哪一个方向走多少距离
	private Vector3 aimPosition;//记录上一步的目标
	float speed= 2f;//这才是在游戏中真正移动的速度
	Animator theAnimator;//动画控制单元
	//传过来的速度作为移动的步长而存在

	public float speedScale = 3f;//在场景中适当放大效果
	void Start () 
	{
		theAnimator = this.GetComponent <Animator> ();
		aimPosition = this.transform.root.position+ new Vector3 (0,0,2);
		InvokeRepeating ("flashPosition", 0.2f, 0.05f);//更新快一点，更加灵敏
	}
	public void flashPosition()
	{
		if (systemValues.canFlashPosition) 
		{
			
			this.transform.rotation= Quaternion.Euler(0, (float)systemValues.stepAngle, 0);
			//			int valueADD = 1;//正负号标记
			//			//如果角度是0——— 90或者 270 ——360就是1
			//			//如果是 90 ——270 就是-1
			//			if (systemValues.stepAngle > 90 && systemValues.stepAngle < 270)
			//				valueADD = -1;
			systemValues.canFlashPosition = false;
			Vector3 aimPositionNow = this.transform.root.position + this.transform .forward *(float)systemValues.stepLengthNow* speedScale ;//最后秤上的一点加成是因为真实世界和游戏世界的坐标没有加矫正
			if(aimPosition != aimPositionNow)//如果来了一个新的目标
			{
				if (Vector3.Distance (aimPosition, this.transform.root.transform.position) < 0.02f)
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
			systemValues.canFlashPosition = false;
		}
	}

	private  void animationControl()
	{
		if (Vector3.Distance (aimPosition, this.transform.root.transform.position) < 0.1f) 
		{
			theAnimator.Play ("idle");
		} 
		else if (systemValues.slopNow <= 0.8f) 
		{
			theAnimator.Play ("walk");
		} 
		else
		{
			theAnimator.Play ("run");
		}
	}
	void Update () 
	{
		this.transform.root .position = Vector3.Lerp(this.transform .root .transform.position  , aimPosition , speed*Time.deltaTime);
		animationControl ();
	}
}
