using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace socketServer.Codes
{
    //有关车的计算都在这个类里面
    //这个类不会很大，但是很难.....

    class CarCanculater
    {
        //积分车速
        public double VNowForCar = 0;
        //积分车速的分段保存
        public double VNowForCarSave = 0;

        //如果是开车就需要做一下这个额外操作
        public void makeFlashForCar()
        {
            VNowForCarSave = VNowForCar;
        }
        //速度“归位”
        public void flashSpeedForIntegral(bool flashZero = false)
        {
            if (flashZero == false)
                VNowForCar = VNowForCarSave;
            else
                VNowForCar = 0; //如在行人模式之下，仅仅是需要积分两步之间的加速度的积分，所以速度不需要累加
        }


//--------------------------------------------数据分段计算策略---------------------------------------------//

        //单纯地从数据量，也就是时间上面进行拆分
        public List<int> stepDectionExtrationForCar(List<double> AZValues)
        {
            List<int> indexs = new List<int>();
            for (int i = 0; i < AZValues.Count; i++)
            {
                //40在这里也算是也各参数，显示用的参数
                if (i % 20 == 0)
                    indexs.Add(i);
            }
            //Console.WriteLine("car step count = "+ indexs.Count);
            return indexs;
        }

//--------------------------------------------车速处理---------------------------------------------//
        //对于车来说，积分是一个很难的事情，因为所有的数据都不得不使用
        public double getCarSpeed(int indexPre, int indexNow, List<double> A, List<long> timeStep)
        {
            //数据前期处理（保存空间先做出来）
            List<double> speed = new List<double>();
            List<double> AValue = new List<double>();
            List<long> times = new List<long>();

            //看一下平均数(这可能是造成积分误差一个很大的因素)
            double AAverage = 0;
            double AAverage2 = 0;

            for (int i = indexPre; i < indexNow; i++)
            {
                //从下面实验猜测滚雪球速度爆炸的错误原因：
                //利用这个门限（0.75）可以有效防止静止的时候速度的变化
                //在不动的状态之下速度有变化是因为来自传感器的神抖动（很小的误差加速度的积累造成惊人误差）
                //手机静止下来但是同样还是有速度，或许应该更多的在于采样频率和传感器本身的特性
                double toAdd = 0;
                if (Math.Abs(A[i]) > 0.02)//简单去除一下扰动
                    toAdd = A[i];//对于Z轴手机会自动为-1，需要注意

                //正在考虑用时间戳来做还是用约定好的时间来做，似乎各有利弊
                //在这里时间的影响很惊人
                //时间戳的方法原理上应该是更值得使用的，老变化的问题应该是来自与手机机器的问题
                //times.Add(timeStep[i]);
                times.Add((long)((double)i * 0.05 * 1000));
                AValue.Add(toAdd);

                AAverage += A[i];
                AAverage2 += toAdd;
            }
            AAverage /= A.Count;
            AAverage2 /= A.Count;

            //这里说明gate可以滤掉一部分小数据，但是很显然这是一个非常粗浅的做法
            //Console.WriteLine("AAverage = "+ AAverage);
            //Console.WriteLine("AAverage2 = " + AAverage2);
            //Console.WriteLine("theSpeed is = " + VNowForCar);

            //积分计算
            //最后一个参数0用来切换不同的积分方法
            //实际上这是一种伪二重积分，但是采样时间足够短并且要求精度不是很高的时候原则上是可以用的
            double VADD = IntegralController.getInstance().makeIntegral(AValue, times, 3);
            //Console.WriteLine("VADD = "+ VADD);
            double VNowForCar = VADD;
            for (int i = indexPre; i < indexNow; i++)
            {
                //重新纪录速度
                speed.Add(VNowForCar);
            }
            double SL = IntegralController.getInstance().makeIntegral(speed, times, 3);

            return SL;
        }

    }
}
