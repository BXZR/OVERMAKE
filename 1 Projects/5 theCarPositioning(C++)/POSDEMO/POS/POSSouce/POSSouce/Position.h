#include "stdafx.h"
#include "Heading.h"
#include "MoveLength.h"
#include<string>
using namespace std;

//专门记录，存储当前位置的类
//也是这个定位模块的主类
class Position
{
public :
	//构造方法，初始位置为（0,0,0），采样时间间隔 0.01 , 初始速度 0
	Position();
	//构造方法，初始位置为（0,0,0），采样时间间隔 time , 初始速度 0
	Position(double time);
	//构造方法，初始位置为（0,0,0），采样时间间隔 time , 初始速度 speed
	Position(double time , double speed);
	//设定方法，根据矫正信息重新设定位置
	void SetPosition(double X, double Y);
	//设定方法，根据矫正信息重新设定位置
	void SetPosition(double X, double Y, double Z);
	//核心方法，计算当前的相对坐标
	void CanculatePosition(double ax, double ay, double az, double gx, double gy, double gz, double mx, double my, double mz);
	//获取位置，返回的是一个数组分别是x,y,z的相对位移
	double* getPosition();
	//获取描述信息
	string getInformation();

private:
	double positionNowX;//当前相对X坐标
	double positionNowY;//当前相对Y坐标
	double positionNowZ;//当前相对Z坐标，似乎没这个需求，但是看造型先留着吧还是
	double HeadingNow;//当前的移动方向
	double MoveLengthNow;//当前的位移距离
	double timeDuration;//采样时间间隔
	double speedForStart;//初始速度
	Heading theHeadingController;//计算移动方向的控制单元
	MoveLength theMoveLengthController;//计算移动距离的控制单元
	
	//组件初始化方法（个人认为这样灵活性更好）
	void makeStart();
};