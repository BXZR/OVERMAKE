#include "stdafx.h"
#include "Position.h"
#include<math.h>
#include<iostream>
using namespace std;

//--------------------------------构造方法---------------------------------------//
//构造方法，初始位置为（0,0,0），采样时间间隔 0.01 , 初始速度 0  缓冲区 100
Position::Position()
{
	positionNowX = 0;
	positionNowY = 0;
	positionNowZ = 0;
	HeadingNow = 0;
	MoveLengthNow = 0;
	timeDuration = 0.01;
	speedForStart = 0;
	bufferLength = 100;
	makeStart();//更新组件
}

//构造方法，初始位置为（0,0,0），采样时间间隔 time , 初始速度 0  缓冲区 100
Position::Position(double time)
{
	positionNowX = 0;
	positionNowY = 0;
	positionNowZ = 0;
	HeadingNow = 0;
	MoveLengthNow = 0;
	timeDuration = time;
	speedForStart = 0;
	bufferLength = 100;
	makeStart();//更新组件
}

//构造方法，初始位置为（0,0,0），采样时间间隔 time , 初始速度 speed  缓冲区 100
Position::Position(double time, double speed)
{
	positionNowX = 0;
	positionNowY = 0;
	positionNowZ = 0;
	HeadingNow = 0;
	MoveLengthNow = 0;
	timeDuration = time;
	speedForStart =speed;
	bufferLength = 100;
	makeStart();//更新组件
}

//构造方法，初始位置为（0,0,0），采样时间间隔 time , 初始速度 speed 缓冲区 BufferLength
Position::Position(double time, double speed , int BufferLength)
{
	positionNowX = 0;
	positionNowY = 0;
	positionNowZ = 0;
	HeadingNow = 0;
	MoveLengthNow = 0;
	timeDuration = time;
	speedForStart = speed;
	bufferLength = BufferLength;
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
	HeadingNow =  theHeadingController.getHeading(ax, ay, az, gx, gy, gz , mx, my, mz);
	//根据加速度信息计算当前的移动位移大小，并且记录
	MoveLengthNow = theMoveLengthController.getMoveLength(ax, ay, az,timeDuration);
	//计算X轴新坐标
	positionNowX += sin(HeadingNow) * MoveLengthNow;
	//计算Y轴新坐标
	positionNowY += cos(HeadingNow) * MoveLengthNow;

}

//核心，计算当前相对坐标的方法
//传入的参数分别是
//加速度x,y,z  陀螺仪x,y,z  磁力计x,y,z
//特殊的一点在于使用缓冲区进行延迟的计算
//优点：拥有历史数据，所以可以使用的算法更加多
//缺点，开销和实时性,另外超速转弯有可能会出错
void Position::CanculatePositionWithBuffer(double ax, double ay, double az, double gx, double gy, double gz, double mx, double my, double mz)
{
	theBuffer.SetBuffer(ax, ay, az,  gx, gy, gz,mx, my, mz);
	//因为有不少历史数据，所以有一些依赖历史数据的做法就可以弄了
	//只有在缓冲区满了的时候才可以进行计算
	if (theBuffer.isBufferFull())
	{
		//用这一阶段的第一个数据当做计算方向的基础数据
		double * getHeadingData = theBuffer.getDataFromAllBuffWithIndex(0);
		//根据九轴信息计算当前的移动方向，并且记录
		HeadingNow = theHeadingController.getHeading(getHeadingData);
		//根据加速度信息计算当前的移动位移大小，并且记录
		double * axData = theBuffer.getAxFromBuff();
		double * ayData = theBuffer.getAyFromBuff();
		double * azData = theBuffer.getAzFromBuff();
		int length = theBuffer.getBufferLength();
		MoveLengthNow = theMoveLengthController.getMoveLength(axData, ayData, azData, timeDuration , length);
		//计算X轴新坐标
		positionNowX += sin(HeadingNow) * MoveLengthNow;
		//计算Y轴新坐标
		positionNowY += cos(HeadingNow) * MoveLengthNow;
		theBuffer.FlashBuffer();//清空缓冲区等待接下来的计算
		std::cout << "canculate" << endl;
	}

}


//--------------------------------辅助方法---------------------------------------//

//设定两个组件的初始数据的方法
//theMoveLengthController需要设定初始的速度
//theHeadingController需要计算采样用的时间间隔的一半
void Position::makeStart()
{
	theMoveLengthController = MoveLength(speedForStart);
	theHeadingController = Heading(timeDuration/2);
	theBuffer = Buffer(bufferLength);
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



