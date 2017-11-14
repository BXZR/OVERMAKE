using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace socketServer
{
    //这个类专门用来处理步长
    class stepLength
    {
        private double changeGate = 60;//转弯的阀值
        //如果转弯且角度差异大于一个阀值，返回的步长信息恐怕需要调整

        //外部方法0，必须对应methodType方法0，这个在mainWindow处会有判断
        //同时也是外部方法1，传参就是方法1
        public double getStepLength1(double angelPast = 0, double angelNow = 0 )
        {
              return StepLengthMethod1(angelPast, angelNow);
        }

        //外部方法2，用重载做出的区分
        //不论应用的是哪一个轴向，至少需要传入一个用来计算的轴向
        //indexPre 和 indexNow 指的是传入的theA的下标，需要算theA的方差，而这这两个下标就是范围
        //论文公式方法
        public double getStepLength2(int indexPre , int indexNow , List<double> theA , List<long> timeUse = null )
        {
            if (indexNow >= theA .Count || indexPre >= theA.Count || indexNow <= indexPre || timeUse == null  )//也就是说传入的数值是错误的，或者数据不够
                return stepLengthBasic();//万金油
            else
            {
                //Console.WriteLine("timeUseCount = "+ timeUse.Count);
               // Console.WriteLine("theACount = " + theA.Count);
                double average = 0;
                for (int i = indexPre; i < indexNow; i++)
                {
                    average += theA[i];
                }
                average /= (indexNow - indexPre);
                //公式需要使用的参数 (为了保证清晰，分成多个循环来写)
                double VK = 0;
                for (int i = indexPre; i < indexNow; i++)
                {
                    double minus = (theA[i] - average) * (theA[i] - average);
                    VK += minus;
                   
                }
                //Console.WriteLine("VK = " + VK);
                VK /= (indexNow - indexPre);
                //Console.WriteLine("VK = " + VK);

                long timestep = timeUse[indexNow] - timeUse[indexPre];
                //有除零异常说明时间非常短，可以认为根本就没走
                if(timestep  ==0)
                    return 0;//万金油
                double FK = (1000 / timestep);//因为时间戳是毫秒作为单位的

                double stepLength = 0.9 * VK + 0.4 * FK + 0.3;
                //Console.WriteLine("VK =" + VK + " FK =" + FK + " length = " + stepLength);
                if (stepLength > 2)//一步走两米，几乎不可能
                    return stepLengthBasic();//万金油
                else
                    return stepLength;
            }
        }


        //外部方法3，男女身高加权
        public double getStepLength3()
        {
            //男女加权不同，仅此而已
            if (SystemSave.isMale)
                return SystemSave.WeightForMale * SystemSave.Stature;
            return SystemSave.WeightForFemale* SystemSave.Stature;
        }

        //方法4，纵向加速度差值开四次根号的方法
        public double getStepLength4(int indexPre, int indexNow, List<double> theA)
        {
            double stepLength = 0;
            double aMax = -9999;
            double aMin = 9999;
            for (int i = indexPre; i < indexNow; i++)
            {
                if (aMax < theA[i])
                    aMax = theA[i];
                if (aMin > theA[i])
                    aMin = theA[i];
                //这个公式的思路是是aMax - aMin，结果开四次根号最后乘以参数K  
            }

            stepLength = SystemSave.stepLengthWeightForAMinus * Math.Pow((aMax - aMin) , 0.25);
            // Console.WriteLine("step length :" + stepLength);
            // Console.WriteLine("AMax :" + aMax);
            // Console.WriteLine("AMin :" + aMin);
            // Console.WriteLine("minus :" + Math.Pow((aMax - aMin), 0.25));
            return stepLength;
        }

        //方法5，对加速度做平均然后除以阶段加速度的极差的做法
        public double getStepLength5(int indexPre, int indexNow, List<double> theA)
        {
            //为了预防除零异常
            if (indexPre >= indexNow)
                return StepLengthMethod1();
            double stepLength = 0;
            double aMax = -9999;
            double aMin = 9999;
            double aAverage = 0;
            for (int i = indexPre; i < indexNow; i++)
            {
                aAverage += theA[i];
                if (aMax < theA[i])
                    aMax = theA[i];
                if (aMin > theA[i])
                    aMin = theA[i];
                //这个公式的思路是是aMax - aMin，结果开四次根号最后乘以参数K  
            }
            //为了防止除零异常和负数步长
            if(aMin >= aMax)
                return StepLengthMethod1();

            //scarlet步长计算公式
            aAverage /= (indexNow-indexPre);
            stepLength = SystemSave.stepLengthWeightForScarlet * (aAverage - aMin) / (aMax - aMin);
            return stepLength;

        }
        //方法6 算法也是一种加速度和步长的关系的算法，单纯的加速度平均开三次根号的做法
        public double getStepLength6(int indexPre, int indexNow, List<double> theA)
        {
            //为了预防除零异常
            if (indexPre >= indexNow)
                return StepLengthMethod1();
            double stepLength = 0;
            double aAverage = 0;
            for (int i = indexPre; i < indexNow; i++)
            {
                aAverage += Math.Abs( theA[i]);
            }
            aAverage /= (indexNow - indexPre);
            stepLength = SystemSave.stepLengthWeightForKim  * Math.Pow(aAverage, 0.33);
           // Console.WriteLine("Kim average = " + aAverage);
           // Console.WriteLine("Kim = " + stepLength);
            return stepLength;
        }

        //为了保证以后传入多个参数进行判断的情况，请保持这种模式
        private double StepLengthMethod1(double angelPast = 0 , double  angelNow = 0)
        {
            if (Math.Abs(angelPast - angelNow) > changeGate)
                return stepLengthBasic() / 2;
            else
                return stepLengthBasic();
        }

        //微软研究得到的平均步长
        //在这里是为了保证架构做的基础实现
        //后期打算用训练后的步长模型来做
        private double stepLengthBasic()
        {
            return 0.6;
        }
    }
}
