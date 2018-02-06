// POSSouce.cpp : 定义控制台应用程序的入口点。
//

#include "stdafx.h"
#include "Position.h"
#include<iostream>

using namespace std;
int main()
{
	//示例使用方法，具体修改操作情况参见各个文件
	//初始化一个Position对象，构造参数为采样时间间隔，初始速度
	//构造参数也可以一个或者没有，一个参数的时候传入的是采样时间间隔
	//什么都没有的时候，初始速度为0，采样时间间隔为0.01秒
	//第三个参数传入的是缓冲区的大小，不传入就是默认100
	Position thePosition = Position(0.01,30,100);
	for (int i = 0; i < 900; i++)
	{
		//计算当前位移，传入的参数分别是加速度x,y,z  陀螺仪x,y,z  磁力计x,y,z
		//thePosition.CanculatePosition(1,2,3,4,5,6,7,8,9);
		thePosition.CanculatePositionWithBuffer(1, 2, 3, 4, 5, 6, 7, 8, 9);
		//输出计算过程中的一些信息
		//cout << thePosition.getInformation() << endl;
		//返回的是当前坐标值
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

