#include "stdafx.h"
#include "MoveLength.h"
#include<time.h>//暂时没用，以后用来打时间戳

//--------------------------------构造方法---------------------------------------//
//刷新方法，初速度是0
MoveLength::MoveLength()
{
	VNow = 0;
}

//刷新方法，初速度给出来
MoveLength::MoveLength(double VZ)
{
	VNow = VZ;
}

//--------------------------------对外使用计算距离方法---------------------------------------//
//如果是固定速度，可以考虑直接使用这个方法
//返回的是一个固定数值的移动距离
double MoveLength::getMoveLength()
{
	return 1;
}

//ax,ay,az分别是三轴加速度
//现在还没有定是哪一个轴，所以参数全部都传过来了
//demo的话，用ax作为计算的基准
//这个方法需要计算时间戳，但是有关平台和一直确定的采样频率其实不是很推荐这么做
double MoveLength::getMoveLength(double ax , double ay , double az)
{
	return 1;
}

//timeUse是采样频率,ax,ay,az分别是三轴加速度
//现在还没有定是哪一个轴，所以参数全部都传过来了
//demo的话，用ax作为计算的基准
double MoveLength::getMoveLength(double ax, double ay, double az ,double timeUse)
{
	return canculateLengthMethod1( ax , timeUse);
}


//--------------------------------内部使用辅助方法---------------------------------------//
//积分的计算方法1，样条方法，作为雏形先暂时这样使用
//AUse是当前积分使用的对应的轴的加速度数值
//timeUse是不管用什么方法反正得到的时间差，也就是AUse的持续时间
double MoveLength::canculateLengthMethod1(double AUse, double timeUse)
{
	VNow += AUse * timeUse;
	double Length = VNow * timeUse;
	return Length;
}



 