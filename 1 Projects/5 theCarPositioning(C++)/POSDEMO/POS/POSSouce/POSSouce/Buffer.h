#include "stdafx.h"

//�����ר���������������Լ�����������ع���
//������Ի�û���������������
class Buffer
{
public:

	Buffer();
	//��ʼ��������Ҫ���趨Buffer���ȵķ���
	Buffer(int bufferLength);
	//�洢����Buffer�ķ���
	void SetBuffer(double ax, double ay, double az, double gx, double gy, double gz, double mx, double my, double mz);
	//Buffer�ǲ����Ѿ����ˣ�������˾ͻ�������
	bool isBufferFull();
	//���¿�ʼ��������index�ع�0
	void FlashBuffer();
	//��û������ĳ��ȣ�����Ҳ˵���˻��������ȳ��˹����ʱ����Ҫֻ��
	int getBufferLength();
	//��û�����������,����һ��������
	//֮���Բ���ֱ�ӵ�public���飬�ǿ��ǵ��ǲ����ڷ���֮ǰ����һ���Ĵ��������˲���
	double * getAxFromBuff();
	double * getAyFromBuff();
	double * getAzFromBuff();
	double * getGxFromBuff();
	double * getGyFromBuff();
	double * getGzFromBuff();
	double * getMxFromBuff();
	double * getMyFromBuff();
	double * getMzFromBuff();
	//��ȡ����Buffer�е�ÿһ���ƶ��±��һ��Ԫ�أ���ϳ�double*����
	double * getDataFromAllBuffWithIndex(int index);
private:
	double * BuffAx;//��������Ax
	double * BuffAy;//��������Ay
	double * BuffAz;//��������Az
	double * BuffGx;//��������Gx
	double * BuffGy;//��������Gy
	double * BuffGz;//��������Gz
	double * BuffMx;//��������Gx
	double * BuffMy;//��������Gy
	double * BuffMz;//��������Gz
	int theIndexNow;//��ǰ���������±�
	int theBuffLength;//����������󳤶ȣ������һ��Ĭ�ϣ������������Ϊ�趨
};