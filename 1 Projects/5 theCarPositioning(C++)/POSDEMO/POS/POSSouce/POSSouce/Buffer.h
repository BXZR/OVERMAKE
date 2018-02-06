#include "stdafx.h"

//这个类专门用来做缓冲区以及缓冲区的相关管理
//对外可以获得缓冲区的所有数据
class Buffer
{
public:

	Buffer();
	//初始化，很重要，设定Buffer长度的方法
	Buffer(int bufferLength);
	//存储进入Buffer的方法
	void SetBuffer(double ax, double ay, double az, double gx, double gy, double gz, double mx, double my, double mz);
	//Buffer是不是已经满了，如果满了就会进入计算
	bool isBufferFull();
	//重新开始缓冲区，index回归0
	void FlashBuffer();
	//获得缓冲区的长度，这里也说明了缓冲区长度除了构造的时候，需要只读
	int getBufferLength();
	//获得缓冲区的内容,这是一个方法组
	//之所以不用直接的public数组，是考虑到是不是在返回之前还有一定的处理，例如滤波？
	double * getAxFromBuff();
	double * getAyFromBuff();
	double * getAzFromBuff();
	double * getGxFromBuff();
	double * getGyFromBuff();
	double * getGzFromBuff();
	double * getMxFromBuff();
	double * getMyFromBuff();
	double * getMzFromBuff();
	//获取各个Buffer中的每一个制定下标的一个元素，结合成double*返回
	double * getDataFromAllBuffWithIndex(int index);
private:
	double * BuffAx;//缓冲区，Ax
	double * BuffAy;//缓冲区，Ay
	double * BuffAz;//缓冲区，Az
	double * BuffGx;//缓冲区，Gx
	double * BuffGy;//缓冲区，Gy
	double * BuffGz;//缓冲区，Gz
	double * BuffMx;//缓冲区，Gx
	double * BuffMy;//缓冲区，Gy
	double * BuffMz;//缓冲区，Gz
	int theIndexNow;//当前缓冲区的下标
	int theBuffLength;//缓冲区的最大长度，这个有一个默认，但是最好是认为设定
};