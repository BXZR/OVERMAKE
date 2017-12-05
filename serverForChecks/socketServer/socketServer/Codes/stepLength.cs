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

        //每一种方法的简短说明信息
        private string[] methodInformations =
        {
            "步长设定为固定的数值",
            "步长设定为固定的数值，大幅度转向的时候步长减半",
            "根据步频和加速度方差计算的一般性公式",
            "根据男女身高进行比例计算得到步长",
            "纵向加速度差值开四次根号的方法",
            "加速度做平均然后除以阶段加速度的极差的做法",
            "加速度平均开三次根号的方法",
            "准备多种一般性公式的参数，使用决策树选择参数计算",
             "利用腿和重心移动的关系推断步长"
        };


        private double changeGate = 60;//转弯的阀值
        //如果转弯且角度差异大于一个阀值，返回的步长信息恐怕需要调整

        //外部方法0，必须对应methodType方法0，这个在mainWindow处会有判断
        //同时也是外部方法1，传参就是方法1
        public double getStepLength1(double angelPast = 0, double angelNow = 0)
        {
            return StepLengthMethod1(angelPast, angelNow);
        }

        //外部方法2，用重载做出的区分
        //不论应用的是哪一个轴向，至少需要传入一个用来计算的轴向
        //indexPre 和 indexNow 指的是传入的theA的下标，需要算theA的方差，而这这两个下标就是范围
        //论文公式方法
        public double getStepLength2(int indexPre, int indexNow, List<double> theA, List<long> timeUse = null)
        {
            // Console.WriteLine("method");
            if (indexNow >= theA.Count || indexPre >= theA.Count || indexNow <= indexPre || timeUse == null)//也就是说传入的数值是错误的，或者数据不够
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
                if (timestep == 0)
                    return 0;//万金油
                             // Console.WriteLine("timeStep is "+ timestep);
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
            return SystemSave.WeightForFemale * SystemSave.Stature;
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

            stepLength = SystemSave.stepLengthWeightForAMinus * Math.Pow((aMax - aMin), 0.25);
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
            if (aMin >= aMax)
                return StepLengthMethod1();

            //scarlet步长计算公式
            aAverage /= (indexNow - indexPre);
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
                aAverage += Math.Abs(theA[i]);
            }
            aAverage /= (indexNow - indexPre);
            stepLength = SystemSave.stepLengthWeightForKim * Math.Pow(aAverage, 0.33);
            // Console.WriteLine("Kim average = " + aAverage);
            // Console.WriteLine("Kim = " + stepLength);
            return stepLength;
        }

        //论文公式方法，算法7
        //使用决策树选择出来模式，使用这一套模式的参数来做
        private double[] afas = { 0.7, 0.8, 0.9, 1.0 };
        private double[] betas = { 0.3, 0.4, 0.5, 0.6 };
        private double[] gamas = { 0.1, 0.2, 0.3, 0.4 };
        public double getStepLength2WithDecisionTree(int indexPre, int indexNow, List<double> theA, List<long> timeUse = null, int modeUse = 1)
        {
            int indexUse = 0;
            // Console.WriteLine("method");
            if (indexNow >= theA.Count || indexPre >= theA.Count || indexNow <= indexPre || timeUse == null)//也就是说传入的数值是错误的，或者数据不够
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
                if (timestep == 0)
                    return 0;//万金油
                             // Console.WriteLine("timeStep is "+ timestep);
                double FK = (1000 / timestep);//因为时间戳是毫秒作为单位的

                double stepLength = afas[indexUse] * VK + betas[indexUse] * FK + gamas[indexUse];
                //Console.WriteLine("VK =" + VK + " FK =" + FK + " length = " + stepLength);
                if (stepLength > 2)//一步走两米，几乎不可能
                    return stepLengthBasic();//万金油
                else
                    return stepLength;
            }
        }



        //为了保证以后传入多个参数进行判断的情况，请保持这种模式
        private double StepLengthMethod1(double angelPast = 0, double angelNow = 0)
        {
            if (Math.Abs(angelPast - angelNow) > changeGate)
                return stepLengthBasic() / 2;
            else
                return stepLengthBasic();
        }


        //算法8 用腿长和倒置钟摆的思路来做，挺有意思的一个思路
        //但是这个方法需要实现输入腿长
        //并且需要非常细致的积分出来高度的最大位移
        //所以这种方法看上去也不是非常好的自适应方法
        //参考电子测量技术基于IOS平台的步长计算方案与实现

        //这个方法可以做很多的扩展
        //例如目前在选择加速度的积分的时候只是简单地做一般加速度的积分，没有考虑波形
        //但是如果是用波峰波谷，则可以用上升和下降来做，也是使用peacksearch的类似方法，凡是貌似有一点不值得
        public double getStepLength8(int indexPre , int indexNow , List<double> AZ , List<long> timeStep)
        {
            double stepLength = 0;
            //获得中心的数值，淡然如果纠结于细节的话其实这种方法并不正确
            //因为不同的判步方法和波形导致的积分并不正确
            int idexHalf = (indexNow - indexPre) / 2 + indexPre;
            double S = 0;
            double V = 0;
            //Console.WriteLine("indexPre = "+ indexPre +" indexHalf = "+ idexHalf);
            for (int i = indexPre ; i < idexHalf; i++)
            {
                if (i == 0)
                    continue;

                double Dt = timeStep[i] - timeStep[i-1];
                //有除零异常说明时间非常短，可以认为根本就没走
                if (Dt <= 0)
                    return stepLengthBasic();//万金油返回值

                double timeUse = (Dt/1000);//因为时间戳是毫秒作为单位的
                //简单的保险,因为第一个数据会超级大
                if (timeUse > 2)
                {
                    timeUse = 0.2;
                }

                //Console.WriteLine("timeUse = "+ timeUse);
                V += AZ[i] * timeUse;
                S += V * timeUse + 0.5 * AZ[i] * timeUse  * timeUse;
                //Console.WriteLine("S = " + S +"   V = "+V ) ;
            }
            S = Math.Abs(S);//取绝对值
            stepLength = 2 * Math.Sqrt( 2 * S * SystemSave.getLegLength() - S*S) *1.25;
            //Console.WriteLine("step Length with leg = "+stepLength);
            return stepLength;
        }

        //微软研究得到的平均步长
        //在这里是为了保证架构做的基础实现
        //后期打算用训练后的步长模型来做
        private double stepLengthBasic()
        {
            return 0.6;
        }
        
        //返回训练用的"真实步长"，用来构建决策树
        public double getRandomStepLength()
        {
            //测试用的真实的步长还没有办法拿到，所以用了一个随机数处理

            return 0.5 + new Random(DateTime.Now .Millisecond).NextDouble()*0.6 -0.3;
        }
        //返回训练用的“真实走楼梯的模式”
        public int getRandomStairMode()
        {
            int stairMode = 1 - new Random(DateTime.Now.Millisecond).Next(0, 3);
           // Console.WriteLine("Stair Mode Random is "+stairMode);
            return stairMode ;
        }


        //返回对这种方法的说明
        public string getMoreInformation(int index)
        {
            return methodInformations[index];
        }
    }
}
