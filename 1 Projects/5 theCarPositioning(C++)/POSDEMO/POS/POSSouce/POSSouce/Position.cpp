#include "stdafx.h"
#include "Position.h"
#include<math.h>
#include<iostream>
using namespace std;

//--------------------------------���췽��---------------------------------------//
//���췽������ʼλ��Ϊ��0,0,0��������ʱ���� 0.01 , ��ʼ�ٶ� 0  ������ 100
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
	makeStart();//�������
}

//���췽������ʼλ��Ϊ��0,0,0��������ʱ���� time , ��ʼ�ٶ� 0  ������ 100
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
	makeStart();//�������
}

//���췽������ʼλ��Ϊ��0,0,0��������ʱ���� time , ��ʼ�ٶ� speed  ������ 100
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
	makeStart();//�������
}

//���췽������ʼλ��Ϊ��0,0,0��������ʱ���� time , ��ʼ�ٶ� speed ������ BufferLength
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
	makeStart();//�������
}

//--------------------------------�趨λ�÷���---------------------------------------//
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

//--------------------------------������㷽��---------------------------------------//
//���ģ����㵱ǰ�������ķ���
//����Ĳ����ֱ���
//���ٶ�x,y,z  ������x,y,z  ������x,y,z
void Position::CanculatePosition(double ax, double ay, double az, double gx, double gy, double gz, double mx, double my, double mz)
{
	//���ݾ�����Ϣ���㵱ǰ���ƶ����򣬲��Ҽ�¼
	HeadingNow =  theHeadingController.getHeading(ax, ay, az, gx, gy, gz , mx, my, mz);
	//���ݼ��ٶ���Ϣ���㵱ǰ���ƶ�λ�ƴ�С�����Ҽ�¼
	MoveLengthNow = theMoveLengthController.getMoveLength(ax, ay, az,timeDuration);
	//����X��������
	positionNowX += sin(HeadingNow) * MoveLengthNow;
	//����Y��������
	positionNowY += cos(HeadingNow) * MoveLengthNow;

}

//���ģ����㵱ǰ�������ķ���
//����Ĳ����ֱ���
//���ٶ�x,y,z  ������x,y,z  ������x,y,z
//�����һ������ʹ�û����������ӳٵļ���
//�ŵ㣺ӵ����ʷ���ݣ����Կ���ʹ�õ��㷨���Ӷ�
//ȱ�㣬������ʵʱ��,���ⳬ��ת���п��ܻ����
void Position::CanculatePositionWithBuffer(double ax, double ay, double az, double gx, double gy, double gz, double mx, double my, double mz)
{
	theBuffer.SetBuffer(ax, ay, az,  gx, gy, gz,mx, my, mz);
	//��Ϊ�в�����ʷ���ݣ�������һЩ������ʷ���ݵ������Ϳ���Ū��
	//ֻ���ڻ��������˵�ʱ��ſ��Խ��м���
	if (theBuffer.isBufferFull())
	{
		//����һ�׶εĵ�һ�����ݵ������㷽��Ļ�������
		double * getHeadingData = theBuffer.getDataFromAllBuffWithIndex(0);
		//���ݾ�����Ϣ���㵱ǰ���ƶ����򣬲��Ҽ�¼
		HeadingNow = theHeadingController.getHeading(getHeadingData);
		//���ݼ��ٶ���Ϣ���㵱ǰ���ƶ�λ�ƴ�С�����Ҽ�¼
		double * axData = theBuffer.getAxFromBuff();
		double * ayData = theBuffer.getAyFromBuff();
		double * azData = theBuffer.getAzFromBuff();
		int length = theBuffer.getBufferLength();
		MoveLengthNow = theMoveLengthController.getMoveLength(axData, ayData, azData, timeDuration , length);
		//����X��������
		positionNowX += sin(HeadingNow) * MoveLengthNow;
		//����Y��������
		positionNowY += cos(HeadingNow) * MoveLengthNow;
		theBuffer.FlashBuffer();//��ջ������ȴ��������ļ���
		std::cout << "canculate" << endl;
	}

}


//--------------------------------��������---------------------------------------//

//�趨��������ĳ�ʼ���ݵķ���
//theMoveLengthController��Ҫ�趨��ʼ���ٶ�
//theHeadingController��Ҫ��������õ�ʱ������һ��
void Position::makeStart()
{
	theMoveLengthController = MoveLength(speedForStart);
	theHeadingController = Heading(timeDuration/2);
	theBuffer = Buffer(bufferLength);
}

//����ת��������doubleתstring
string doubleToString(double num)
{
	char str[256];
	sprintf(str, "%lf", num);
	string result = str;
	return result;
}

//��Ϣ��ȡ�������ı��汾��
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



