using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AHRS  
{
	//这个类用来表述AHRS的各种做法，作为一般类存在（并非组件）
	//方法3  AHRS算法代码：磁力计+加计+陀螺版（来自网络有待进一步弄一波）

	double Kp = 2.0;                     // proportional gain governs rate of convergence to accelerometer/magnetometer
	double Ki = 0.005;                // integral gain governs rate of convergence of gyroscope biases
	double halfT = 0.025;                //必须设置为采样频率的一半
	double q0 = 1, q1 = 0, q2 = 0, q3 = 0;        // quaternion elements representing the estimated orientation
	double exInt = 0, eyInt = 0, ezInt = 0;        // scaled integral error
	////这个方法不可以每一次计算都会修改全局的数值，所以调用次数需要慎用
	public double AHRSupdate(double gx, double gy, double gz, double ax, double ay, double az, double mx, double my, double mz)
	{

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
		norm = Math.Sqrt(ax * ax + ay * ay + az * az);
		ax = ax / norm;
		ay = ay / norm;
		az = az / norm;
		norm = Math.Sqrt(mx * mx + my * my + mz * mz);
		mx = mx / norm;
		my = my / norm;
		mz = mz / norm;

		// compute reference direction of flux
		hx = 2 * mx * (0.5 - q2q2 - q3q3) + 2 * my * (q1q2 - q0q3) + 2 * mz * (q1q3 + q0q2);
		hy = 2 * mx * (q1q2 + q0q3) + 2 * my * (0.5 - q1q1 - q3q3) + 2 * mz * (q2q3 - q0q1);
		hz = 2 * mx * (q1q3 - q0q2) + 2 * my * (q2q3 + q0q1) + 2 * mz * (0.5 - q1q1 - q2q2);
		bx = Math.Sqrt((hx * hx) + (hy * hy));
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
		//exInt = exInt + ex * Ki;
		//eyInt = eyInt + ey * Ki;
		//ezInt = ezInt + ez * Ki;
		exInt = exInt + ex * Ki* halfT;
		eyInt = eyInt + ey * Ki* halfT;
		ezInt = ezInt + ez * Ki* halfT;
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
		norm = Math.Sqrt(q0 * q0 + q1 * q1 + q2 * q2 + q3 * q3);
		q0 = q0 / norm;
		q1 = q1 / norm;
		q2 = q2 / norm;
		q3 = q3 / norm;

		//四元数转换欧拉角
		double roll = Math.Atan2(2.0f * (q0 * q1 + q2 * q3), 1 - 2.0f * (q1 * q1 + q2 * q2))* 57.3;
		double pitch = Math.Asin(2.0f*(q0* q2 - q1* q3)) * 57.3;
	//	double yaw = Math.Atan2(2.0f * (q1 * q2 - q0 * q3), 2.0f * (q0 * q0 + q1 * q1) - 1) * 57.3;
		double yaw  = Math.Atan2(-2 * q1 * q2 - 2 * q0 * q3, 2 * q2 * q2 + 2 * q3 * q3 - 1) * 57.3;  
		return yaw; //返回偏航角
	}
	//这是第二种AHRS的方法
	public double AHRSupdate2(double gx, double gy, double gz, double ax, double ay, double az, double mx, double my, double mz)
	{

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
		norm = Math.Sqrt(ax * ax + ay * ay + az * az);
		ax = ax / norm;
		ay = ay / norm;
		az = az / norm;
		norm = Math.Sqrt(mx * mx + my * my + mz * mz);
		mx = mx / norm;
		my = my / norm;
		mz = mz / norm;

		// compute reference direction of flux
		hx = 2 * mx * (0.5 - q2q2 - q3q3) + 2 * my * (q1q2 - q0q3) + 2 * mz * (q1q3 + q0q2);
		hy = 2 * mx * (q1q2 + q0q3) + 2 * my * (0.5 - q1q1 - q3q3) + 2 * mz * (q2q3 - q0q1);
		hz = 2 * mx * (q1q3 - q0q2) + 2 * my * (q2q3 + q0q1) + 2 * mz * (0.5 - q1q1 - q2q2);
		bx = Math.Sqrt((hx * hx) + (hy * hy));
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
		norm = Math.Sqrt(q0 * q0 + q1 * q1 + q2 * q2 + q3 * q3);
		q0 = q0 / norm;
		q1 = q1 / norm;
		q2 = q2 / norm;
		q3 = q3 / norm;

		//四元数转换欧拉角
		double roll = Math.Atan2(2.0f * (q0 * q1 + q2 * q3), 1 - 2.0f * (q1 * q1 + q2 * q2))* 57.3;
		double pitch = Math.Asin(2.0f*(q0* q2 - q1* q3)) * 57.3;
		double yaw = Math.Atan2(2.0f * (q1 * q2 - q0 * q3), 2.0f * (q0 * q0 + q1 * q1) - 1) * 57.3;
		return yaw+180; //返回偏航角
	}

}
