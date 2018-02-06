#include "stdafx.h"
#include"Heading.h"
#include<math.h>

//--------------------------------构造方法---------------------------------------//
//默认构造，全都是默认参数
Heading::Heading()
{
	Kp = 2.0;
	Ki = 0.005;
	halfT = 0.005; //必须设置为采样频率的一半
	q0 = 1, q1 = 0, q2 = 0, q3 = 0;
	exInt = 0, eyInt = 0, ezInt = 0;
} 
Heading::Heading(double halfTNew)
{
	Kp = 2.0;
	Ki = 0.005;
	halfT = halfTNew; //必须设置为采样频率的一半
	q0 = 1, q1 = 0, q2 = 0, q3 = 0;
	exInt = 0, eyInt = 0, ezInt = 0;
}

//统一外包计算航向的方法，在这里切换算法
double Heading::getHeading(double gx, double gy, double gz, double ax, double ay, double az, double mx, double my, double mz)
{
	return AHRSupdate( gx,  gy, gz,  ax, ay, az,mx, my, mz );
}
//--------------------------------航向计算方法---------------------------------------//
//航向计算方法，AHRS
double Heading::AHRSupdate(double gx, double gy, double gz, double ax, double ay, double az, double mx, double my, double mz)
{
	//Console.WriteLine(string .Format("{0},{1},{2},{3},{4},{5},{6},{7},{8}" , gx,gy,gz,ax,ay,az,mx,my,mz));
	double norm;
	double hx, hy, hz, bx, bz;
	double vx, vy, vz, wx, wy, wz;
	double ex, ey, ez;

	// auxiliary variables to reduce number of repeated operations
	double q0q0 = q0 * q0;
	double q0q1 = q0 * q1;
	double q0q2 = q0 * q2;
	double q0q3 = q0 * q3;
	double q1q1 = q1 * q1;
	double q1q2 = q1 * q2;
	double q1q3 = q1 * q3;
	double q2q2 = q2 * q2;
	double q2q3 = q2 * q3;
	double q3q3 = q3 * q3;

	// normalise the measurements
	norm = sqrt(ax * ax + ay * ay + az * az);
	ax = ax / norm;
	ay = ay / norm;
	az = az / norm;
	norm = sqrt(mx * mx + my * my + mz * mz);
	mx = mx / norm;
	my = my / norm;
	mz = mz / norm;

	// compute reference direction of flux
	hx = 2 * mx * (0.5 - q2q2 - q3q3) + 2 * my * (q1q2 - q0q3) + 2 * mz * (q1q3 + q0q2);
	hy = 2 * mx * (q1q2 + q0q3) + 2 * my * (0.5 - q1q1 - q3q3) + 2 * mz * (q2q3 - q0q1);
	hz = 2 * mx * (q1q3 - q0q2) + 2 * my * (q2q3 + q0q1) + 2 * mz * (0.5 - q1q1 - q2q2);
	bx = sqrt((hx * hx) + (hy * hy));
	bz = hz;

	// estimated direction of gravity and flux (v and w)
	vx = 2 * (q1q3 - q0q2);
	vy = 2 * (q0q1 + q2q3);
	vz = q0q0 - q1q1 - q2q2 + q3q3;
	wx = 2 * bx * (0.5 - q2q2 - q3q3) + 2 * bz * (q1q3 - q0q2);
	wy = 2 * bx * (q1q2 - q0q3) + 2 * bz * (q0q1 + q2q3);
	wz = 2 * bx * (q0q2 + q1q3) + 2 * bz * (0.5 - q1q1 - q2q2);

	// error is sum of cross product between reference direction of fields and direction measured by sensors
	ex = (ay * vz - az * vy) + (my * wz - mz * wy);
	ey = (az * vx - ax * vz) + (mz * wx - mx * wz);
	ez = (ax * vy - ay * vx) + (mx * wy - my * wx);

	// integral error scaled integral gain
	exInt = exInt + ex * Ki;
	eyInt = eyInt + ey * Ki;
	ezInt = ezInt + ez * Ki;

	// adjusted gyroscope measurements
	gx = gx + Kp * ex + exInt;
	gy = gy + Kp * ey + eyInt;
	gz = gz + Kp * ez + ezInt;

	// integrate quaternion rate and normalise
	q0 = q0 + (-q1 * gx - q2 * gy - q3 * gz) * halfT;
	q1 = q1 + (q0 * gx + q2 * gz - q3 * gy) * halfT;
	q2 = q2 + (q0 * gy - q1 * gz + q3 * gx) * halfT;
	q3 = q3 + (q0 * gz + q1 * gy - q2 * gx) * halfT;

	// normalise quaternion
	norm = sqrt(q0 * q0 + q1 * q1 + q2 * q2 + q3 * q3);
	q0 = q0 / norm;
	q1 = q1 / norm;
	q2 = q2 / norm;
	q3 = q3 / norm;
	// Console.WriteLine(string .Format ("q0 = {0} , q1 = {1} , q2 = {2} , q3 = {3}" , q0,q1,q2,q3));
	//四元数转换欧拉角

	double roll = atan2(2.0f * (q0 * q1 + q2 * q3), 1 - 2.0f * (q1 * q1 + q2 * q2)) * 57.3;
	double pitch = asin(2.0f * (q0 * q2 - q1 * q3)) * 57.3;
	double yaw = atan2(2.0f * (q1 * q2 - q0 * q3), 2.0f * (q0 * q0 + q1 * q1) - 1) * 57.3;
	//Console.WriteLine("yaw = " + (yaw));
	return (yaw); //返回偏航角
}

