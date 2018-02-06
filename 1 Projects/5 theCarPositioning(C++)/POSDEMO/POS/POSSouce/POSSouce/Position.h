#include "stdafx.h"
#include "Heading.h"
#include "MoveLength.h"
#include "Buffer.h"
#include<string>
using namespace std;

//ר�ż�¼���洢��ǰλ�õ���
//Ҳ�������λģ�������
class Position
{
public :
	//���췽������ʼλ��Ϊ��0,0,0��������ʱ���� 0.01 , ��ʼ�ٶ� 0  ������ 100
	Position();
	//���췽������ʼλ��Ϊ��0,0,0��������ʱ���� time , ��ʼ�ٶ� 0  ������ 100
	Position(double time);
	//���췽������ʼλ��Ϊ��0,0,0��������ʱ���� time , ��ʼ�ٶ� speed  ������ 100
	Position(double time , double speed);
	//���췽������ʼλ��Ϊ��0,0,0��������ʱ���� time , ��ʼ�ٶ� speed  ������ bufferLength
	Position(double time, double speed , int  bufferLength);
	//�趨���������ݽ�����Ϣ�����趨λ��
	void SetPosition(double X, double Y);
	//�趨���������ݽ�����Ϣ�����趨λ��
	void SetPosition(double X, double Y, double Z);
	//���ķ��������㵱ǰ���������
	void CanculatePosition(double ax, double ay, double az, double gx, double gy, double gz, double mx, double my, double mz);
	//���ķ��������㵱ǰ��������꣨ʹ�û�������
	void CanculatePositionWithBuffer(double ax, double ay, double az, double gx, double gy, double gz, double mx, double my, double mz);
	//��ȡλ�ã����ص���һ������ֱ���x,y,z�����λ��
	double* getPosition();
	//��ȡ������Ϣ
	string getInformation();

private:
	double positionNowX;//��ǰ���X����
	double positionNowY;//��ǰ���Y����
	double positionNowZ;//��ǰ���Z���꣬�ƺ�û������󣬵��ǿ����������Űɻ���
	double HeadingNow;//��ǰ���ƶ�����
	double MoveLengthNow;//��ǰ��λ�ƾ���
	double timeDuration;//����ʱ����
	double speedForStart;//��ʼ�ٶ�
	int bufferLength;//��������С
	Heading theHeadingController;//�����ƶ�����Ŀ��Ƶ�Ԫ
	MoveLength theMoveLengthController;//�����ƶ�����Ŀ��Ƶ�Ԫ
	Buffer theBuffer;//������
	//������������ר���ö��ڻ��������������ض�����һ������ʱ���������ܻ����
	//˼����ǽ���õĴ����������ռ�������ͳһ����λ�ƴ�С
	//����뷨�������¼ٶ���
	//��ʱ����û�г������ٵ�ת��

	
	//�����ʼ��������������Ϊ��������Ը��ã�
	void makeStart();
};