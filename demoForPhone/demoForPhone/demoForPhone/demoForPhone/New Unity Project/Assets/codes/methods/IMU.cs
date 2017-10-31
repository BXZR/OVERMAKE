using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class IMU  {

	double Kp = 2.0;                     // proportional gain governs rate of convergence to accelerometer/magnetometer
	double Ki = 0.005;                // integral gain governs rate of convergence of gyroscope biases
	double halfT = 0.025;                //必须设置为采样频率的一半
	double q0 = 1, q1 = 0, q2 = 0, q3 = 0;        // quaternion elements representing the estimated orientation
	double exInt = 0, eyInt = 0, ezInt = 0;        // scaled integral error

	double  gyroG = 0.0610351; //网上盗来的参数

	double yawNow = 0;//初始值的Raw
	bool inited = false;//是否初始化了

//----------------------------------------------这个方法暂时不真正使用----------------------------------------------------------------//
	public  double  MahonyAHRSupdateIMU(double gx, double gy, double gz, double ax, double ay,double  az) {
		double recipNorm;
		double halfvx, halfvy, halfvz;
		double halfex, halfey, halfez;
		double qa, qb, qc;

		//如果加速计各轴的数均是0，那么忽略该加速度数据。否则在加速计数据归一化处理的时候，会导致除以0的错误。
		// Compute feedback only if accelerometer measurement valid (avoids NaN in accelerometer normalisation)
		if(!((ax == 0.0f) && (ay == 0.0f) && (az == 0.0f))) 
		{

			//把加速度计的数据进行归一化处理。其中invSqrt是平方根的倒数，使用平方根的倒数而不是直接使用平方根的原因是使得下面的ax，ay，az的运算速度更快。通过归一化处理后，ax，ay，az的数值范围变成-1到+1之间。
			// Normalise accelerometer measurement
			recipNorm = Math.Sqrt(ax * ax + ay * ay + az * az);
			ax /= recipNorm;
			ay /= recipNorm;
			az /= recipNorm;

			//根据当前四元数的姿态值来估算出各重力分量。用于和加速计实际测量出来的各重力分量进行对比，从而实现对四轴姿态的修正。
			// Estimated direction of gravity and vector perpendicular to magnetic flux
			halfvx = q1 * q3 - q0 * q2;
			halfvy = q0 * q1 + q2 * q3;
			halfvz = q0 * q0 - 0.5f + q3 * q3;

			//使用叉积来计算估算的重力和实际测量的重力这两个重力向量之间的误差。
			// Error is sum of cross product between estimated and measured direction of gravity
			halfex = (ay * halfvz - az * halfvy);
			halfey = (az * halfvx - ax * halfvz);
			halfez = (ax * halfvy - ay * halfvx);

			//把上述计算得到的重力差进行积分运算，积分的结果累加到陀螺仪的数据中，用于修正陀螺仪数据。积分系数是Ki，如果Ki参数设置为0，则忽略积分运算。
			// Compute and apply integral feedback if enabled

			if( Ki  > 0.0f) {
				exInt += Ki * halfex ; // integral error scaled by Ki
				eyInt  += Ki  * halfey ;
				ezInt += Ki  * halfez ;
				gx += exInt; // apply integral feedback
				gy += eyInt;
				gz += ezInt;
			}
			else {
				exInt = 0.0f; // prevent integral windup
				eyInt = 0.0f;
				ezInt  = 0.0f;
			}

			//把上述计算得到的重力差进行比例运算。比例的结果累加到陀螺仪的数据中，用于修正陀螺仪数据。比例系数为Kp。
			// Apply proportional feedback
			gx += Kp * halfex;
			gy += Kp * halfey;
			gz += Kp * halfez;
		}

		//通过上述的运算，我们得到了一个由加速计修正过后的陀螺仪数据。接下来要做的就是把修正过后的陀螺仪数据整合到四元数中。
		// Integrate rate of change of quaternion
		gx *= (0.5f ); // pre-multiply common factors
		gy *= (0.5f );
		gz *= (0.5f );
		qa = q0;
		qb = q1;
		qc = q2;
		q0 += (-qb * gx - qc * gy - q3 * gz);
		q1 += (qa * gx + qc * gz - q3 * gy);
		q2 += (qa * gy - qb * gz + q3 * gx);
		q3 += (qa * gz + qb * gy - qc * gx);

		//把上述运算后的四元数进行归一化处理。得到了物体经过旋转后的新的四元数。
		// Normalise quaternion
		recipNorm = Math.Sqrt(q0 * q0 + q1 * q1 + q2 * q2 + q3 * q3);
		q0 /= recipNorm;
		q1 /= recipNorm;
		q2 /= recipNorm;
		q3 /= recipNorm;

		//四元数转换欧拉角
		double roll = Math.Atan2(2.0f * (q0 * q1 + q2 * q3), 1 - 2.0f * (q1 * q1 + q2 * q2))* 57.3;
		double pitch = Math.Asin(2.0f*(q0* q2 - q1* q3)) * 57.3;
		double yaw = Math.Atan2(2.0f * (q1 * q2 - q0 * q3), 2.0f * (q0 * q0 + q1 * q1) - 1) * 57.3;
		//double Yaw=  Math.Atan2 (2.0f * (q0 * q1 + q2 * q3), q0*q0 - q1*q1 - q2*q2 + q3*q3 )*57.3;
		//double yaw2 = Math.Atan2(2.0 * q1* q2 + 2 * q0 * q3, -2.0 * q2*q2 - 2 * q3 * q3+ 1)*  57.3; // yaw
		if(yaw<0)  yaw+=360.0;  //将 -+180度  转成0-360度
		return yaw; //返回偏航角

	}
//-------------------------------------------------------------------------------------------------------------------------//

	public double IMUupdate(double gx, double gy, double gz, double ax, double ay, double az , double heading ) 
	{

		if (inited == false) 
		{
			yawNow = heading;
			inited = true;
			return yawNow;
		}

		double norm;
		double vx, vy, vz;
		double ex, ey, ez;         

		// normalise the measurements
		norm = Math.Sqrt(ax*ax + ay*ay + az*az);      
		ax = ax / norm;
		ay = ay / norm;
		az = az / norm;      
		//把加计的三维向量转成单位向量。


		// estimated direction of gravity
		vx = 2*(q1*q3 - q0*q2);
		vy = 2*(q0*q1 + q2*q3);
		vz = q0*q0 - q1*q1 - q2*q2 + q3*q3;

		//这是把四元数换算成《方向余弦矩阵》中的第三列的三个元素。
		//根据余弦矩阵和欧拉角的定义，地理坐标系的重力向量，转到机体坐标系，正好是这三个元素。
		//所以这里的vx\y\z，其实就是当前的欧拉角（即四元数）的机体坐标参照系上，换算出来的重力单位向量。


		// error is sum of cross product between reference direction of field and direction measured by sensor
		ex = (ay*vz - az*vy);
		ey = (az*vx - ax*vz);
		ez = (ax*vy - ay*vx);

		//axyz是机体坐标参照系上，加速度计测出来的重力向量，也就是实际测出来的重力向量。
		//axyz是测量得到的重力向量，vxyz是陀螺积分后的姿态来推算出的重力向量，它们都是机体坐标参照系上的重力向量。
		//那它们之间的误差向量，就是陀螺积分后的姿态和加计测出来的姿态之间的误差。
		//向量间的误差，可以用向量叉积（也叫向量外积、叉乘）来表示，exyz就是两个重力向量的叉积。
		//这个叉积向量仍旧是位于机体坐标系上的，而陀螺积分误差也是在机体坐标系，而且叉积的大小与陀螺积分误差成正比，正好拿来纠正陀螺。（你可以自己拿东西想象一下）由于陀螺是对机体直接积分，所以对陀螺的纠正量会直接体现在对机体坐标系的纠正。

		// integral error scaled integral gain
		exInt = exInt + ex*Ki;
		eyInt = eyInt + ey*Ki;
		ezInt = ezInt + ez*Ki;

		// adjusted gyroscope measurements
		gx = gx + Kp*ex + exInt;
		gy = gy + Kp*ey + eyInt;
		gz = gz + Kp*ez + ezInt;
		//用叉积误差来做PI修正陀螺零偏

		// integrate quaternion rate and normalise
		q0 = q0 + (-q1*gx - q2*gy - q3*gz)*halfT;
		q1 = q1 + (q0*gx + q2*gz - q3*gy)*halfT;
		q2 = q2 + (q0*gy - q1*gz + q3*gx)*halfT;
		q3 = q3 + (q0*gz + q1*gy - q2*gx)*halfT;  
		//四元数微分方程

		// normalise quaternion
		norm =  Math.Sqrt(q0*q0 + q1*q1 + q2*q2 + q3*q3);
		q0 = q0 / norm;
		q1 = q1 / norm;
		q2 = q2 / norm;
		q3 = q3 / norm;
		//四元数规范化

		//四元数转换欧拉角
		double roll = Math.Atan2(2.0f * (q0 * q1 + q2 * q3), 1 - 2.0f * (q1 * q1 + q2 * q2))* 57.3;
		double pitch = Math.Asin(2.0f*(q0* q2 - q1* q3)) * 57.3;
		//double yaw = Math.Atan2(2.0f * (q1 * q2 - q0 * q3), 2.0f * (q0 * q0 + q1 * q1) - 1) * 57.3;
		//double yaw2 = Math.Atan2(2.0 * q1* q2 + 2 * q0 * q3, -2.0 * q2*q2 - 2 * q3 * q3+ 1)*  57.3; // yaw
	//double yaw=  Math.Atan2 (2.0f * (q0 * q1 + q2 * q3), q0*q0 - q1*q1 - q2*q2 + q3*q3 )*57.3;


		//一个非常鬼畜的问题就是imu修正的算法其实对于yaw也就是偏航角是没有修正的
		//得到的偏航角实际上只有在长时间内才会保持一个不错的精度
		//没有结合地磁，方向的变化趋势是对的，但是效果还是不太好
		double yaw =  - Math.Atan2 (2 * q1*q2 - 2*q0*q3 , -2 *q1 *q1 - 2*q3*q3 +1)*57.3;
		if (yaw > 360)
			yaw -= 360;
		if (yaw < 0)
			yaw += 360;
		//下面这个方法其实是从网上查到的一个算是比较通用的做法，引测对于短时间内的数据精确度不好
		//长时间的数据也不是很理想，相对于AHRS来说暂时效果较弱
		yawNow += gz * gyroG * 0.002f;
		return yaw; //返回偏航角
		//return yawNow; //返回偏航角
	}
}
