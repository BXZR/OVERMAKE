#include "stdafx.h"

//�����ר���������㵱ǰ���ƶ�����
class Heading
{
public :
	//Ĭ�Ϲ��죬ȫ����Ĭ�ϲ���
	Heading();
	//���췽���������趨halfT����ֵ
	Heading(double halfT);
	double getHeading(double gx, double gy, double gz, double ax, double ay, double az, double mx, double my, double mz);
	double getHeading(double* Data);
private:
	//�������ż�����Ҫ���ϸ��µĲ���
	double Kp ; 
	double Ki ; 
	double halfT ; //��������Ϊ����Ƶ�ʵ�һ��
	double q0 , q1 , q2 , q3 ; 
	double exInt , eyInt , ezInt ;
	//������㷽����AHRS
	double AHRSupdate(double gx, double gy, double gz, double ax, double ay, double az, double mx, double my, double mz);

};