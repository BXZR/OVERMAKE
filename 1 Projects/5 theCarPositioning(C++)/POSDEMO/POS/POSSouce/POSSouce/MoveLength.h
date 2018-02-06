#include "stdafx.h"
//专门用来计算位移距离的类
//计算返回的是一段时间内的位移的大小，没有方向
class MoveLength
{
 public :
	//计算位移大小方法，默认固定速度使用
	double getMoveLength();
	//计算位移大小方法，使用时间戳来计算持续时间
	double getMoveLength(double ax ,double ay ,double az);
	//计算位移大小方法，直接给出采样频率
	double getMoveLength(double ax, double ay, double az,double timeUse);
	//计算位移大小方法，直接给出采样频率,参数是缓冲区数组
	//因为传入的是缓冲区数组，所以需要一个长度作为循环标记
	double getMoveLength(double* ax, double* ay, double* az, double timeUse, int length);
	//构造，初速度是0
	MoveLength();
	//构造，初速度是VZero
	MoveLength(double VZero);
private :
	//计算积分的辅助方法1，最脑残的样条方法，以后扩展此处的方法
	double canculateLengthMethod1(double AUse, double timeUse);
	//当前的移动速度记录
	double VNow;
};