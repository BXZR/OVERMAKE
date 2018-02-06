#include "stdafx.h"
#include "Position.h"
#include<math.h>
#include<iostream>
using namespace std;

//--------------------------------构造方法---------------------------------------//
//构造方法，初始位置为（0,0,0），采样时间间隔 0.01 , 初始速度 0
Position::Position()
{
	positionNowX = 0;
	positionNowY = 0;
	positionNowZ = 0;
	HeadingNow = 0;
	MoveLengthNow = 0;
	timeDuration = 0.01;
	speedForStart = 0;
	makeStart();//更新组件
}

//构造方法，初始位置为（0,0,0），采样时间间隔 time , 初始速度 0
Position::Position(double time)
{
	positionNowX = 0;
	positionNowY = 0;
	positionNowZ = 0;
	HeadingNow = 0;
	MoveLengthNow = 0;
	timeDuration = time;
	speedForStart = 0;
	makeStart();//更新组件
}

//构造方法，初始位置为（0,0,0），采样时间间隔 time , 初始速度 speed
Position::Position(double time, double speed)
{
	positionNowX = 0;
	positionNowY = 0;
	positionNowZ = 0;
	HeadingNow = 0;
	MoveLengthNow = 0;
	timeDuration = time;
	speedForStart =speed;
	makeStart();//更新组件
}


//--------------------------------设定位置方法---------------------------------------//
void Position::SetPosition(double X, double Y)
{
	positionNowX = X;
	positionNowY = Y;
}

void Position::SetPosition(double X, double Y , double Z)
{
	positionNowX = X;
	positionNowY = Y;
	positionNowZ = Z;
}

//--------------------------------方向计算方法---------------------------------------//
//核心，计算当前相对坐标的方法
//传入的参数分别是
//加速度x,y,z  陀螺仪x,y,z  磁力计x,y,z
void Position::CanculatePosition(double ax, double ay, double az, double gx, double gy, double gz, double mx, double my, double mz)
{
	//根据九轴信息计算当前的移动方向，并且记录
	HeadingNow =  theHeadingController.getHeading(gx, gy, gz ,ax,ay,az, mx, my, mz);
	//根据加速度信息计算当前的移动位移大小，并且记录
	MoveLengthNow = theMoveLengthController.getMoveLength(ax, ay, az,timeDuration);
	//计算X轴新坐标
	positionNowX += sin(HeadingNow) * MoveLengthNow;
	//计算Y轴新坐标
	positionNowY += cos(HeadingNow) * MoveLengthNow;

}


//--------------------------------辅助方法---------------------------------------//

//设定两个组件的初始数据的方法
//theMoveLengthController需要设定初始的速度
//theHeadingController需要计算采样用的时间间隔的一半
void Position::makeStart()
{
	theMoveLengthController = MoveLength(speedForStart);
	theHeadingController = Heading(timeDuration/2);
}

//辅助转化方法，double转string
string doubleToString(double num)
{
	char str[256];
	sprintf(str, "%lf", num);
	string result = str;
	return result;
}

//信息获取方法，文本版本的
string Position::getInformation()
{
	return "X: " + doubleToString(positionNowX) + " Y: " + doubleToString( positionNowY) + " Z: " + doubleToString(positionNowZ) + "\nheading: " + doubleToString( HeadingNow) + " moveLength: " + doubleToString( MoveLengthNow);

}

double * Position::getPosition()
{
	double * pos = new double[3];
	pos[0] = positionNowX;
	pos[1] = positionNowY;
	pos[2] = positionNowZ;
	return pos;
}



