// POSSouce.cpp : �������̨Ӧ�ó������ڵ㡣
//

#include "stdafx.h"
#include "Position.h"
#include<iostream>

using namespace std;
int main()
{
	//ʾ��ʹ�÷����������޸Ĳ�������μ������ļ�
	//��ʼ��һ��Position���󣬹������Ϊ����ʱ��������ʼ�ٶ�
	//�������Ҳ����һ������û�У�һ��������ʱ������ǲ���ʱ����
	//ʲô��û�е�ʱ�򣬳�ʼ�ٶ�Ϊ0������ʱ����Ϊ0.01��
	//����������������ǻ������Ĵ�С�����������Ĭ��100
	Position thePosition = Position(0.01,30,100);
	for (int i = 0; i < 900; i++)
	{
		//���㵱ǰλ�ƣ�����Ĳ����ֱ��Ǽ��ٶ�x,y,z  ������x,y,z  ������x,y,z
		//thePosition.CanculatePosition(1,2,3,4,5,6,7,8,9);
		thePosition.CanculatePositionWithBuffer(1, 2, 3, 4, 5, 6, 7, 8, 9);
		//�����������е�һЩ��Ϣ
		//cout << thePosition.getInformation() << endl;
		//���ص��ǵ�ǰ����ֵ
		//double * positionNow = thePosition.getPosition();
		//double XNow = positionNow[0];
		//double YNow = positionNow[1];
		//double ZNow = positionNow[2];
		//cout << "("<< XNow <<","<< YNow<<","<< ZNow <<")" << endl<< endl;
	}

	int  a;
	cin >> a;
    return 0;
}

