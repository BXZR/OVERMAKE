using socketServer.Codes;
using socketServer.Codes.AcordUse;
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
            "准备多种一般性公式的参数，使用决策树选择参数计算",
            "使用已有训练集线性回归得到一般公式参数",
            "使用已有训练集使用ANN的做法进行估计",
            "根据男女身高进行比例计算得到步长",
            "根据身高进行更加复杂的步长估计",
            "纵向加速度差值开四次根号的方法",
            "加速度做平均然后除以阶段加速度的极差的做法",
            "加速度平均开三次根号的方法",
            "利用腿和重心移动的关系推断步长",
            "加速度积分是为步长(平放)"
        };




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

                double VK = MathCanculate.getVariance(theA, indexNow, indexPre);

                long timestep = timeUse[indexNow] - timeUse[indexPre];
                //有除零异常说明时间非常短，可以认为根本就没走
                if (timestep == 0)
                    return 0;//万金油
                             // Console.WriteLine("timeStep is "+ timestep);
                double FK = ((double)1000 / timestep);//因为时间戳是毫秒作为单位的

                double stepLength = SystemSave.CommonFormulaWeights[0][0] * VK + SystemSave.CommonFormulaWeights[0][1] * FK + SystemSave.CommonFormulaWeights[0][2];
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

        //身高加权，更复杂的公式
        public double getStepLength11(int indexPre, int indexNow, List<long> timeUse)
        {
            long timestep = timeUse[indexNow] - timeUse[indexPre];
            //有除零异常说明时间非常短，可以认为根本就没走
            if (timestep == 0)
                return 0;//万金油
            double SF = ((double)1000 / timestep);//因为时间戳是毫秒作为单位的
            //Console.WriteLine("SF = "+ SF);
            double SL = 0.7 + SystemSave.StaturaMethod2_A * (SystemSave.Stature - 1.75) + SystemSave.StaturaMethod2_B * (SF - 1.79) * SystemSave.Stature / 1.75;
            return SL;
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
        //实际上这是一个公用的方法，不论用什么方法，只需要传入mode就可以使用systemSave储存的参数了
        public double getStepLength2WithMode(int indexPre, int indexNow, List<double> theA, List<long> timeUse = null, int modeUse = 1)
        {
            int indexUse = modeUse;
            // Console.WriteLine("method");
            if (indexNow >= theA.Count || indexPre >= theA.Count || indexNow <= indexPre || timeUse == null)//也就是说传入的数值是错误的，或者数据不够
                return stepLengthBasic();//万金油
            else
            {
                //Console.WriteLine("timeUseCount = "+ timeUse.Count);
                // Console.WriteLine("theACount = " + theA.Count);
                double VK = MathCanculate.getVariance(theA, indexNow, indexPre);

                long timestep = timeUse[indexNow] - timeUse[indexPre];
                //有除零异常说明时间非常短，可以认为根本就没走
                if (timestep == 0)
                    return 0;//万金油
                             // Console.WriteLine("timeStep is "+ timestep);
                double FK = ((double)1000 / timestep);//因为时间戳是毫秒作为单位的

                double stepLength = SystemSave.CommonFormulaWeights[indexUse][0] * VK + SystemSave.CommonFormulaWeights[indexUse][1] * FK + SystemSave.CommonFormulaWeights[indexUse][2];
                //Console.WriteLine("VK =" + VK + " FK =" + FK + " length = " + stepLength);
                if (stepLength > 2)//一步走两米，几乎不可能
                    return stepLengthBasic();//万金油
                else
                    return stepLength;
            }
        }


        //外部方法9，用重载做出的区分
        //不论应用的是哪一个轴向，至少需要传入一个用来计算的轴向
        //indexPre 和 indexNow 指的是传入的theA的下标，需要算theA的方差，而这这两个下标就是范围
        //论文公式方法
        //使用训练集线性回归得到公式
        private AccordNotNetUse accordUsing = null;
        public double getStepLengthWithKLinear(int indexPre, int indexNow, List<double> theA, List<long> timeUse = null)
        {
            // Console.WriteLine("method");
            if (indexNow >= theA.Count || indexPre >= theA.Count || indexNow <= indexPre || timeUse == null)//也就是说传入的数值是错误的，或者数据不够
                return stepLengthBasic();//万金油
            else
            {
                if (accordUsing == null)
                {
                    //因为只有步长使用了线性回归，所以直接在这里进行判断和处理也就可以了
                    //这里与ANN和决策树的不同在于ANN和决策树收到面板中设定的公式族群数量的制约，需要刷新
                    //但是线性回归的方法完全根据tranBase文件来的，得到参数之后就不更改了，所以直接放在这里似乎更方便
                    accordUsing = new Codes.AccordNotNetUse();
                    accordUsing.BuildWeights();
                }

                double VK = MathCanculate.getVariance(theA, indexNow, indexPre);

                long timestep = timeUse[indexNow] - timeUse[indexPre];
                //有除零异常说明时间非常短，可以认为根本就没走
                if (timestep == 0)
                    return 0;//万金油
                             // Console.WriteLine("timeStep is "+ timestep);
                double FK = ((double)1000 / timestep);//因为时间戳是毫秒作为单位的

                double stepLength = accordUsing.linearStepLength(VK, FK);
                //Console.WriteLine("VK =" + VK + " FK =" + FK + " length = " + stepLength);
                if (stepLength > 2)//一步走两米，几乎不可能
                    return stepLengthBasic();//万金油
                else
                    return stepLength;
            }
        }

        //使用accord的人工神经网络的做法
        //实际上这里有很大的空间进行重构
        public double getStepLengthWithANN(int indexPre, int indexNow, List<double> theA, List<long> timeUse = null)
        {
            // Console.WriteLine("method");
            if (indexNow >= theA.Count || indexPre >= theA.Count || indexNow <= indexPre || timeUse == null)//也就是说传入的数值是错误的，或者数据不够
                return stepLengthBasic();//万金油
            else
            {
                double VK = MathCanculate.getVariance(theA, indexNow, indexPre);

                long timestep = timeUse[indexNow] - timeUse[indexPre];
                //有除零异常说明时间非常短，可以认为根本就没走
                if (timestep == 0)
                    return 0;//万金油
                             // Console.WriteLine("timeStep is "+ timestep);
                double FK = ((double)1000 / timestep);//因为时间戳是毫秒作为单位的

                int indexUse = SystemSave.AccordANNforSL.getModeWithANNForSL(VK, FK);
                double stepLength = SystemSave.CommonFormulaWeights[indexUse][0] * VK + SystemSave.CommonFormulaWeights[indexUse][1] * FK + SystemSave.CommonFormulaWeights[indexUse][2];

                if (stepLength > 2)//一步走两米，几乎不可能
                    return stepLengthBasic();//万金油
                else
                    return stepLength;
            }
        }

        //为了保证以后传入多个参数进行判断的情况，请保持这种模式
        private double StepLengthMethod1(double angelPast = 0, double angelNow = 0)
        {
            if (Math.Abs(angelPast - angelNow) > SystemSave.changeGateForImmediate2)
                return stepLengthBasic() * SystemSave.percentWhenMeetChangeGate;
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
        //大腿移动扇形方法
        public double getStepLength8(int indexPre, int indexNow, List<double> AZ, List<long> timeStep)
        {
            double stepLength = 0;
            //获得中心的数值，淡然如果纠结于细节的话其实这种方法并不正确
            //因为不同的判步方法和波形导致的积分并不正确
            int idexHalf = (indexNow - indexPre) / 2 + indexPre;
            double S = 0;
            double V = 0;
            //Console.WriteLine("indexPre = "+ indexPre +" indexHalf = "+ idexHalf);
            for (int i = indexPre; i < idexHalf; i++)
            {
                if (i == 0)
                    continue;

                double Dt = timeStep[i] - timeStep[i - 1];
                //有除零异常说明时间非常短，可以认为根本就没走
                if (Dt <= 0)
                    return stepLengthBasic();//万金油返回值

                double timeUse = (Dt / 1000);//因为时间戳是毫秒作为单位的
                //简单的保险,因为第一个数据会超级大
                if (timeUse > 2)
                {
                    timeUse = 0.2;
                }
                //加速度的二重积分就是距离
                //Console.WriteLine("timeUse = "+ timeUse);
                V += AZ[i] * timeUse;
                S += V * timeUse + 0.5 * AZ[i] * timeUse * timeUse;
                //Console.WriteLine("S = " + S +"   V = "+V ) ;
            }

            S = Math.Abs(S);//取绝对值
            //stepLength = 2 * Math.Sqrt( 2 * S * SystemSave.getLegLength() - S*S) *1.25;
            double beSqrt = (2 * S * SystemSave.getLegLength() - S * S);
            //Console.WriteLine("S = " + S + "  beSqrt = " + beSqrt + "  leg = " + SystemSave.getLegLength());

            //一个很土的防护措施
            if (beSqrt < 0)
            {
                Log.saveLog(LogType.error, "腿长步长计算方法计算中出现负数开根号");
                return stepLengthBasic();
            }
            stepLength = 2 * Math.Sqrt(beSqrt);
            //Console.WriteLine("step Length with leg = "+stepLength);
            return stepLength;
        }

        //有关积分步长的方法组--------------------------------------------------------------------------
        public double VNowForCar = 0;
        public double VNowForCarSave = 0;
        //单纯的加速度二次积分方法求位移这个实际上更加推荐在车上使用
        //如果不滤波，indexPre和indexNow差距太小，会使得步长计算得0
        public double getStepLength9(int indexPre, int indexNow, List<double> A, List<long> timeStep)
        {
            //数据前期处理（保存空间先做出来）
            List<double> speed = new List<double>();
            List<double> AValue = new List<double>();
            List<long> times = new List<long>();
            for (int i = indexPre; i < indexNow; i++)
            {
                //从下面实验猜测滚雪球速度爆炸的错误原因：
                //利用这个门限（0.75）可以有效防止静止的时候速度的变化
                //在不动的状态之下速度有变化是因为来自传感器的神抖动（很小的误差加速度的积累造成惊人误差）
                //手机静止下来但是同样还是有速度，或许应该更多的在于采样频率和传感器本身的特性
                if (Math.Abs (A[i]) > 0.4)
                    AValue.Add(A[i]);//对于Z轴手机会自动为-1，需要注意
                else
                    AValue.Add(0);

                //正在考虑用时间戳来做还是用约定好的时间来做，似乎各有利弊
                //在这里时间的影响很惊人
                //时间戳的方法原理上应该是更值得使用的，老变化的问题应该是来自与手机机器的问题
                //times.Add(timeStep[i]);
                times.Add((long)((double)i*0.05*1000));
            }
            //Console.WriteLine("theSpeed is = " + VNowForCar);

            //积分计算
            //最后一个参数0用来切换不同的积分方法
            //实际上这是一种伪二重积分，但是采样时间足够短并且要求精度不是很高的时候原则上是可以用的
            double VADD = IntegralController.getInstance().makeIntegral(AValue, times, 3);
            Console.WriteLine("VADD = "+ VADD);
            VNowForCar += VADD;
            for (int i = indexPre; i < indexNow; i++)
            {
                //重新纪录速度
                speed.Add(VNowForCar);
            }
            double SL = IntegralController.getInstance().makeIntegral(speed, times, 3);

            return SL;
        }
        //如果是开车就需要做一下这个额外操作
        public void makeFlashForCar()
        {
            VNowForCarSave = VNowForCar;
        }
        //速度“归位”
        public void flashSpeedForIntegral(bool flashZero = false)
        {
            if(flashZero == false)
                VNowForCar = VNowForCarSave ;
            else
                VNowForCar = 0; //如在行人模式之下，仅仅是需要积分两步之间的加速度的积分，所以速度不需要累加
        }
        //有关积分步长的方法组--------------------------------------------------------------------------

        //微软研究得到的平均步长
        //在这里是为了保证架构做的基础实现
        //后期打算用训练后的步长模型来做
        private double stepLengthBasic()
        {
            return SystemSave.stepLengthForImmediate;
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
            //0,1,2
            int stairMode = new Random(DateTime.Now.Millisecond).Next(0, 3);
           // Console.WriteLine("Stair Mode Random is "+stairMode);
            return stairMode ;
        }


        //返回对这种方法的说明
        public string getMoreInformation(int index)
        {
            return methodInformations[index];
        }
        //返回全部的方法说明
        public string[] getMoreInformation()
        {
            return methodInformations;
        }

    }
}
