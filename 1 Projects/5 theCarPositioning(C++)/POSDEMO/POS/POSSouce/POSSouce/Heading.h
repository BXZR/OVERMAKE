#include "stdafx.h"

//这个类专门用来计算当前的移动方向
class Heading
{
public :
	//默认构造，全都是默认参数
	Heading();
	//构造方法，用来设定halfT的数值
	Heading(double halfT);
	double getHeading(double gx, double gy, double gz, double ax, double ay, double az, double mx, double my, double mz);
	double getHeading(double* Data);
private:
	//各种随着计算需要不断更新的参数
	double Kp ; 
	double Ki ; 
	double halfT ; //必须设置为采样频率的一半
	double q0 , q1 , q2 , q3 ; 
	double exInt , eyInt , ezInt ;
	//航向计算方法，AHRS
	double AHRSupdate(double gx, double gy, double gz, double ax, double ay, double az, double mx, double my, double mz);

};