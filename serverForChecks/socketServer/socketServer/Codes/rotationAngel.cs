using socketServer.Codes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace socketServer
{
    //这个类专门用来控制旋转信息的处理
    class rotationAngel
    {
        private double theAngelNow = 0;//总记录的角度
        private double changeGate = 5;//如果变化超过这个数目就认为改变了

        //每一种方法的简短说明信息
        private string[] methodInformations =
        {
            "直接读取电子罗盘角度的方法",
            "无视短时间内的较小的方向偏移，方向更平滑",
            "客户端实现加速计、陀螺仪、磁力计融合算法(AHRS)",
             "根据不同的情况切换使用AHRS和IMU的数值",
             "在客户端实现的加速计、陀螺仪融合算法(IMU)",
             "根据电子罗盘的原理判断方向（并不正确）",
            "在服务端实现的加速计、陀螺仪、磁力计融合算法(并不正确)",
            "在服务端实现的加速计、陀螺仪融合算法(并不正确)",
           
        };


        //判断变化，超过阀值就认为有所改变了
        public double getAngelNow(double angelIn)
        {
            if (Math.Abs(theAngelNow - angelIn) > changeGate)
            {
                theAngelNow = angelIn;
            }
            return theAngelNow;
        }

        //微软提出的一种更准一点的判断转向了的方法
        public double getAngelNow(List<double> IN)
        {
            if (IN.Count <= 1)
                return theAngelNow;

            double ALL = 0;
            for (int i = 0; i < IN.Count; i++)
            {
                ALL += IN[i];
            }
            double average = ALL / IN.Count - 1;
            if (Math.Abs(theAngelNow - average) > changeGate)
            {
                theAngelNow = IN[IN.Count - 1];
            }
            return theAngelNow;
        }

        //--------------------------------------------------实际上这两种方法应该方法客户端来做，在服务器中采样不充分所以出错写在这里只是为了做一个对比--------------------------------------------------------------------------------------//

        //方法3  AHRS算法代码：磁力计+加计+陀螺版（来自网络有待进一步弄一波）

        double Kp = 2.0;                     // proportional gain governs rate of convergence to accelerometer/magnetometer
        double Ki = 0.005;                // integral gain governs rate of convergence of gyroscope biases
        double halfT = 0.025;                //必须设置为采样频率的一半
        double q0 = 1, q1 = 0, q2 = 0, q3 = 0;        // quaternion elements representing the estimated orientation
        double exInt = 0, eyInt = 0, ezInt = 0;        // scaled integral error
        ////这个方法不可以每一次计算都会修改全局的数值，所以调用次数需要慎用
        public double AHRSupdate(double gx, double gy, double gz, double ax, double ay, double az, double mx, double my, double mz)
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
            // Console.WriteLine(string .Format ("q0 = {0} , q1 = {1} , q2 = {2} , q3 = {3}" , q0,q1,q2,q3));
            //四元数转换欧拉角

            double roll = Math.Atan2(2.0f * (q0 * q1 + q2 * q3), 1 - 2.0f * (q1 * q1 + q2 * q2)) * 57.3;
            double pitch = Math.Asin(2.0f * (q0 * q2 - q1 * q3)) * 57.3;
            double yaw = Math.Atan2(2.0f * (q1 * q2 - q0 * q3), 2.0f * (q0 * q0 + q1 * q1) - 1) * 57.3;
            //Console.WriteLine("yaw = " + (yaw));
            return (yaw); //返回偏航角
        }

        //因为本程序的机制是多从重复计算，而这个程序每一次计算都会改变状态，所以需要一个方法在运行一整个阶段的上面方法之后做一个清空
        public void makeMehtod3Clear()
        {
            q0 = 1; q1 = 0; q2 = 0; q3 = 0;        // quaternion elements representing the estimated orientation
            exInt = 0; eyInt = 0; ezInt = 0;        // scaled integral error
        }

        public double IMUupdate(double gx, double gy, double gz, double ax, double ay, double az)
        {
            double norm;
            double vx, vy, vz;
            double ex, ey, ez;

            // normalise the measurements
            norm = Math.Sqrt(ax * ax + ay * ay + az * az);
            ax = ax / norm;
            ay = ay / norm;
            az = az / norm;
            // 把加计的三维向量转成单位向量。

            // estimated direction of gravity
            vx = 2 * (q1 * q3 - q0 * q2);
            vy = 2 * (q0 * q1 + q2 * q3);
            vz = q0 * q0 - q1 * q1 - q2 * q2 + q3 * q3;

            /*这是把四元数换算成《方向余弦矩阵》中的第三列的三个元素。
            根据余弦矩阵和欧拉角的定义，地理坐标系的重力向量，转到机体坐标系，正好是这三个元素。
            所以这里的vx\y\z，其实就是当前的欧拉角（即四元数）的机体坐标参照系上，换算出来的重力单位向量。
            */
            // error is sum of cross product between reference direction of field and direction measured by sensor
            ex = (ay * vz - az * vy);
            ey = (az * vx - ax * vz);
            ez = (ax * vy - ay * vx);

            /*axyz是机体坐标参照系上，加速度计测出来的重力向量，也就是实际测出来的重力向量。
            axyz是测量得到的重力向量，vxyz是陀螺积分后的姿态来推算出的重力向量，它们都是机体坐标参照系上的重力向量。
            那它们之间的误差向量，就是陀螺积分后的姿态和加计测出来的姿态之间的误差。
            向量间的误差，可以用向量叉积（也叫向量外积、叉乘）来表示，exyz就是两个重力向量的叉积。
            这个叉积向量仍旧是位于机体坐标系上的，而陀螺积分误差也是在机体坐标系，而且叉积的大小与陀螺积分误差成正比，正好拿来纠正陀螺。（你可以自己拿东西想象一下）由于陀螺是对机体直接积分，所以对陀螺的纠正量会直接体现在对机体坐标系的纠正。
            */

            // integral error scaled integral gain
            exInt = exInt + ex * Ki;
            eyInt = eyInt + ey * Ki;
            ezInt = ezInt + ez * Ki;

            // adjusted gyroscope measurements
            gx = gx + Kp * ex + exInt;
            gy = gy + Kp * ey + eyInt;
            gz = gz + Kp * ez + ezInt;

            // 用叉积误差来做PI修正陀螺零偏

            // integrate quaternion rate and normalise
            q0 = q0 + (-q1 * gx - q2 * gy - q3 * gz) * halfT;
            q1 = q1 + (q0 * gx + q2 * gz - q3 * gy) * halfT;
            q2 = q2 + (q0 * gy - q1 * gz + q3 * gx) * halfT;
            q3 = q3 + (q0 * gz + q1 * gy - q2 * gx) * halfT;
            // 四元数微分方程

            // normalise quaternion
            norm = Math.Sqrt(q0 * q0 + q1 * q1 + q2 * q2 + q3 * q3);
            q0 = q0 / norm;
            q1 = q1 / norm;
            q2 = q2 / norm;
            q3 = q3 / norm;
            //四元数规范化

            double X = Math.Atan2(2 * (q1 * q2 + q3 * q0), q3 * q3 - q0 * q0 - q1 * q1 + q2 * q2) / 3.14 * 180;
            double Y = Math.Asin(-2 * (q0 * q2 - q3 * q1)) / 3.14 * 180;
            double Z = Math.Atan2(2 * (q0 * q1 + q3 * q2), q3 * q3 + q0 * q0 - q1 * q1 - q2 * q2) / 3.14 * 180;

            //Console.WriteLine("Z = " + -Z);
            return Z;
        }

        //论文方法中一个选择的方法
        //其实第一步是检查IGRF做偏差
        //如果小于权值，直接用IMU
        //如果大于权值，判断AHRS和IMU的差异值
        //差异值大于权值用AHRS
        //差异值小还是用AHRS
        double IGRFGate = 0;
        double EDYawGate = 3;
        public double AHRSIMUSelect(int indexPre, int indexNow, List<double> AHRSValues, List<double> IMUValues)
        {
            double degreeNow = 0;

            double AHRSAverage = MathCanculate.getAverage(AHRSValues , indexPre,indexNow);
            double IMUAverage = MathCanculate.getAverage(IMUValues, indexPre, indexNow);
           
            //Console.WriteLine("Average1 = " + AHRSAverage);
           // Console.WriteLine("Average2 = " + IMUAverage);

            double EDYaw = 0;
            for (int i = indexPre; i < indexNow; i++)
            {
                double VL = (AHRSValues[i] - AHRSAverage) - (IMUValues[i] - IMUAverage);
                EDYaw += VL * VL;
            }
            EDYaw = Math.Sqrt(EDYaw);
            Console.WriteLine("EDYaw = " + EDYaw);
            //返回数值
            if (EDYaw > EDYawGate)
            {
                Console.WriteLine("Select AHRS");
                return AHRSValues[indexNow];
            }
            Console.WriteLine("Select IMU");
            return IMUValues[indexNow];
        }
        //返回对这种方法的说明
        public string getMoreInformation(int index)
        {
            return methodInformations[index];
        }

        //来自博客的方法
        //使用加速计和磁力计做的heading
        //电子罗盘倾斜补偿航偏角计算
        //算是比较贴近底层的东西了
        //需要注意的是 unity 的XYZ轴 和AHRS/IMU的轴向不同
        //虽然在这里直接使用unity的数据所以没有事
        //目前为止这个方法非常的不稳定也并不准确（201711301356）
        public double getYawWithAM(double AX, double AY, double AZ, double MX, double MY, double MZ)
        {
            double heading = 0;

            double A = Math.Atan(AX / Math.Sqrt(AY * AY + AZ * AZ)) / 3.14 * 180;
            double B = Math.Atan(AY / Math.Sqrt(AX * AX + AZ * AZ)) / 3.14 * 180;
            //Console.WriteLine("A = " + A + " B = " + B);

            double HY = MY * Math.Cos(B) + MX * Math.Sin(B) * Math.Sin(A) - MZ * Math.Cos(A) * Math.Sin(B);
            double HX = MX * Math.Cos(A) + MZ * Math.Sin(A);
            Console.WriteLine("HX = " + HX + " HY = " + HY);

            heading = Math.Atan(HY / HX) / 3.14 * 180;

            //看网上的说法还需要分开做一个象限的计算
            if (HX > 0 && HY > 0)
            {
                heading = 360 - heading;
            }
            else if (HX > 0 && HY < 0)
            {
                heading = 180 - heading;
            }
            else if (HX < 0)
            {
                heading = -heading;
            }
            else if (HX == 0 && HY < 0)
            {
                heading = 90;
            }
            else if (HX == 0 && HY > 0)
            {
                heading = 270;
            }
            Console.WriteLine("Heading = " + heading);

            return heading;
        }
    }
}
