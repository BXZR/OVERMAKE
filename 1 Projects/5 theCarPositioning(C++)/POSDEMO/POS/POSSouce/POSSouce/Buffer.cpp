#include "stdafx.h"
#include"Buffer.h"

//--------------------------------���췽��---------------------------------------//
Buffer::Buffer()
{
	theBuffLength = 100;//Ĭ�ϻ�������С����100
	BuffAx = new double[theBuffLength];
	BuffAy = new double[theBuffLength];
	BuffAz = new double[theBuffLength];
	BuffGx = new double[theBuffLength];
	BuffGy = new double[theBuffLength];
	BuffGz = new double[theBuffLength];
	BuffMx = new double[theBuffLength];
	BuffMy = new double[theBuffLength];
	BuffMz = new double[theBuffLength];
	theIndexNow = 0;//�����趨��ǰ���±�
}
//�����ض��Ļ������Ĵ�С��Buffer
Buffer::Buffer(int bufferLength)
{
	theBuffLength = bufferLength;//Ĭ�ϻ�������С����100
	BuffAx = new double[theBuffLength];
	BuffAy = new double[theBuffLength];
	BuffAz = new double[theBuffLength];
	BuffGx = new double[theBuffLength];
	BuffGy = new double[theBuffLength];
	BuffGz = new double[theBuffLength];
	BuffMx = new double[theBuffLength];
	BuffMy = new double[theBuffLength];
	BuffMz = new double[theBuffLength];
	theIndexNow = 0;//�����趨��ǰ���±�
}

//--------------------------------��ؼ��ķ���---------------------------------------//
//�洢����Buffer�ķ���
void Buffer::SetBuffer(double ax, double ay, double az, double gx, double gy, double gz, double mx, double my, double mz)
{
	//���ǵ�ǰ�趨�ı������ƣ�������������˾ͱ���Ҫ���м��㣬���򶼲�����뻺������
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
//����ˢ�»�������λ��
void Buffer::FlashBuffer()
{
	theIndexNow = 0;
	//���ֻ�����������
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

//index��Ϊ��黺�����ǲ������˵ı��
bool Buffer::isBufferFull()
{
	return !(theIndexNow < theBuffLength);
}

double * Buffer::getDataFromAllBuffWithIndex(int index)
{
	//ֻ�о�������
	double * data = new double[9];
	//�������ͻ᷵��ȫ��0��������ʵ��һ���Ƕ����
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


//--------------------------------��ȡ����---------------------------------------//
//����Buffer�ķ����飬������ھ���ֱ�ӷ��صģ����Ǻ���һ��������صĴ���������Ҳ������
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