#include "stdafx.h"
//ר����������λ�ƾ������
//���㷵�ص���һ��ʱ���ڵ�λ�ƵĴ�С��û�з���
class MoveLength
{
 public :
	//����λ�ƴ�С������Ĭ�Ϲ̶��ٶ�ʹ��
	double getMoveLength();
	//����λ�ƴ�С������ʹ��ʱ������������ʱ��
	double getMoveLength(double ax ,double ay ,double az);
	//����λ�ƴ�С������ֱ�Ӹ�������Ƶ��
	double getMoveLength(double ax, double ay, double az,double timeUse);
	//����λ�ƴ�С������ֱ�Ӹ�������Ƶ��,�����ǻ���������
	//��Ϊ������ǻ��������飬������Ҫһ��������Ϊѭ�����
	double getMoveLength(double* ax, double* ay, double* az, double timeUse, int length);
	//���죬���ٶ���0
	MoveLength();
	//���죬���ٶ���VZero
	MoveLength(double VZero);
private :
	//������ֵĸ�������1�����Բе������������Ժ���չ�˴��ķ���
	double canculateLengthMethod1(double AUse, double timeUse);
	//��ǰ���ƶ��ٶȼ�¼
	double VNow;
};