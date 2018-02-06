#include "stdafx.h"
#include"Buffer.h"

//--------------------------------构造方法---------------------------------------//
Buffer::Buffer()
{
	theBuffLength = 100;//默认缓冲区大小就是100
	BuffAx = new double[theBuffLength];
	BuffAy = new double[theBuffLength];
	BuffAz = new double[theBuffLength];
	BuffGx = new double[theBuffLength];
	BuffGy = new double[theBuffLength];
	BuffGz = new double[theBuffLength];
	BuffMx = new double[theBuffLength];
	BuffMy = new double[theBuffLength];
	BuffMz = new double[theBuffLength];
	theIndexNow = 0;//重新设定当前的下标
}
//带有特定的缓冲区的大小的Buffer
Buffer::Buffer(int bufferLength)
{
	theBuffLength = bufferLength;//默认缓冲区大小就是100
	BuffAx = new double[theBuffLength];
	BuffAy = new double[theBuffLength];
	BuffAz = new double[theBuffLength];
	BuffGx = new double[theBuffLength];
	BuffGy = new double[theBuffLength];
	BuffGz = new double[theBuffLength];
	BuffMx = new double[theBuffLength];
	BuffMy = new double[theBuffLength];
	BuffMz = new double[theBuffLength];
	theIndexNow = 0;//重新设定当前的下标
}

//--------------------------------最关键的方法---------------------------------------//
//存储进入Buffer的方法
void Buffer::SetBuffer(double ax, double ay, double az, double gx, double gy, double gz, double mx, double my, double mz)
{
	//这是当前设定的保护机制，如果缓冲区满了就必须要进行计算，否则都不会进入缓冲区了
	if (theIndexNow < theBuffLength)
	{
		BuffAx[theIndexNow] = ax;
		BuffAy[theIndexNow] = ay;
		BuffAz[theIndexNow] = az;
		BuffGx[theIndexNow] = gx;
		BuffGy[theIndexNow] = gy;
		BuffGz[theIndexNow] = gz;
		BuffMx[theIndexNow] = mx;
		BuffMy[theIndexNow] = my;
		BuffMz[theIndexNow] = mz;
		theIndexNow++;
	}
}
//重新刷新缓冲区的位置
void Buffer::FlashBuffer()
{
	theIndexNow = 0;
	//各种缓冲区的清零
	memset(BuffAx , 0 , theBuffLength);
	memset(BuffAy, 0, theBuffLength);
	memset(BuffAz, 0, theBuffLength);
	memset(BuffGx, 0, theBuffLength);
	memset(BuffGy, 0, theBuffLength);
	memset(BuffGz, 0, theBuffLength);
	memset(BuffMx, 0, theBuffLength);
	memset(BuffMy, 0, theBuffLength);
	memset(BuffMz, 0, theBuffLength);
}

//index作为检查缓冲区是不是满了的标记
bool Buffer::isBufferFull()
{
	return !(theIndexNow < theBuffLength);
}

double * Buffer::getDataFromAllBuffWithIndex(int index)
{
	//只有九轴数据
	double * data = new double[9];
	//如果超界就会返回全是0，但是其实这一步是多余的
	if (index >= theIndexNow)
		return data;

	data[0] = BuffAx[index];
	data[1] = BuffAy[index];
	data[2] = BuffAz[index];
	data[3] = BuffGx[index];
	data[4] = BuffGy[index];
	data[5] = BuffGz[index];
	data[6] = BuffMx[index];
	data[7] = BuffMy[index];
	data[8] = BuffMz[index];
	
	return data;
}

int Buffer::getBufferLength()
{
	return theBuffLength;
}


//--------------------------------获取方法---------------------------------------//
//返回Buffer的方法组，这个现在就是直接返回的，但是后面一定会有相关的处理，因此这个也先留下
double * Buffer::getAxFromBuff()
{
	return BuffAx;
}
double * Buffer::getAyFromBuff()
{
	return BuffAy;
}
double * Buffer::getAzFromBuff()
{
	return BuffAz;
}
double * Buffer::getGxFromBuff()
{
	return BuffGx;
}
double * Buffer::getGyFromBuff()
{
	return BuffGy;
}
double * Buffer::getGzFromBuff()
{
	return BuffGz;
}
double * Buffer::getMxFromBuff()
{
	return BuffMx;
}
double * Buffer::getMyFromBuff()
{
	return BuffMy;
}
double * Buffer::getMzFromBuff()
{
	return BuffMz;
}