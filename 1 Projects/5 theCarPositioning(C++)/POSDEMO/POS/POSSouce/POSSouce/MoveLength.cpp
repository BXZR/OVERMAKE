#include "stdafx.h"
#include "MoveLength.h"
#include<time.h>//��ʱû�ã��Ժ�������ʱ���

//--------------------------------���췽��---------------------------------------//
//ˢ�·��������ٶ���0
MoveLength::MoveLength()
{
	VNow = 0;
}

//ˢ�·��������ٶȸ�����
MoveLength::MoveLength(double VZ)
{
	VNow = VZ;
}

//--------------------------------����ʹ�ü�����뷽��---------------------------------------//
//����ǹ̶��ٶȣ����Կ���ֱ��ʹ���������
//���ص���һ���̶���ֵ���ƶ�����
double MoveLength::getMoveLength()
{
	return 1;
}

//ax,ay,az�ֱ���������ٶ�
//���ڻ�û�ж�����һ���ᣬ���Բ���ȫ������������
//demo�Ļ�����ax��Ϊ����Ļ�׼
//���������Ҫ����ʱ����������й�ƽ̨��һֱȷ���Ĳ���Ƶ����ʵ���Ǻ��Ƽ���ô��
double MoveLength::getMoveLength(double ax , double ay , double az)
{
	return 1;
}

//timeUse�ǲ���Ƶ��,ax,ay,az�ֱ���������ٶ�
//���ڻ�û�ж�����һ���ᣬ���Բ���ȫ������������
//demo�Ļ�����ax��Ϊ����Ļ�׼
double MoveLength::getMoveLength(double ax, double ay, double az ,double timeUse)
{
	return canculateLengthMethod1( ax , timeUse);
}


//--------------------------------�ڲ�ʹ�ø�������---------------------------------------//
//���ֵļ��㷽��1��������������Ϊ��������ʱ����ʹ��
//AUse�ǵ�ǰ����ʹ�õĶ�Ӧ����ļ��ٶ���ֵ
//timeUse�ǲ�����ʲô���������õ���ʱ��Ҳ����AUse�ĳ���ʱ��
double MoveLength::canculateLengthMethod1(double AUse, double timeUse)
{
	VNow += AUse * timeUse;
	double Length = VNow * timeUse;
	return Length;
}



 