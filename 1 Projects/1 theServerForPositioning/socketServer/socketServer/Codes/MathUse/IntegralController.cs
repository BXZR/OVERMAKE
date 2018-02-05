using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace socketServer.Codes
{
    class IntegralController
    {
        //保持单例
        private static IntegralController theIntergral;
        public static IntegralController getInstance()
        {
            if (theIntergral == null)
                theIntergral = new Codes.IntegralController();
            return theIntergral;
        }

        public string getIntegralInformation(int mode = 0)
        {
            string infotmationReturn = "";
            switch (mode)
            {
                case 0: { infotmationReturn = "样条积分方法"; } break;
                case 1: { infotmationReturn = "辛普森积分方法"; } break;
                case 2: { infotmationReturn = "辛普森积分方法形式2"; } break;
                case 3: { infotmationReturn = "样条积分方法形式2"; } break;
                case 4: { infotmationReturn = "取平均数的积分方法(误差大)"; } break;
                default: { infotmationReturn = "样条积分方法"; } break;
            }
            return infotmationReturn;
        }

        //这个类专门用来处理积分积分相关的问题
        //采样时间足够短并且要求精度不是很高的时候原则上是可以用的
        public double makeIntegral(List<double> values, List<long> timeSteps , int mode = 0)
        {
            double allValue = 0;

            switch (mode)
            {
                case 0: { allValue = DemoSimpleValues(values , timeSteps); } break;
                case 1: { allValue = DemoSimpson(values, timeSteps); } break;
                case 2: { allValue = Simpson(values, timeSteps); } break;
                case 3: { allValue = DemoSimpleValues2(values, timeSteps); } break;
                case 4: { allValue = AverageWithError(values, timeSteps); } break;
                default:{ allValue = DemoSimpleValues(values, timeSteps); }break;
            }

            return allValue;
        }

        //各种积分方法(都是单次积分，如果有需要就多次调用也就是多次积分了)
        //样条方法进行积分(DEMO)-----------------------------------------------------------------------------------
        private double DemoSimpleValues(List<double> values, List<long> timeStep)
        {
            double allvalues = 0;
            long timeUse = timeStep[timeStep.Count-1] - timeStep[0];
            double time = ((double)timeUse / 1000) / timeStep.Count;//因为时间戳是毫秒作为单位的

            for (int i = 0; i < timeStep.Count; i++)
                allvalues += values[i] * time;

            return allvalues;
        }

        //辛普森方法进行积分(DEMO)-----------------------------------------------------------------------------------
        private double DemoSimpson(List<double> values, List<long> timeStep)
        {
            int n = 4;
            double h = timeStep.Count / (2*n);
            double allValue = values[values.Count-1] + values[0];

            for (int i = 1; i <= (2*n -1); i += 2)
                allValue += 4 * values[0 + (int)(i * h)];

            for (int i = 2; i <= (2 * n - 2) ; i += 2)
                allValue += 2 * values[0 + (int)(i * h)];

            //切换到真实的H
            h = (double)(timeStep[timeStep.Count - 1] - timeStep[0]) / ((2 * n)*1000);
            allValue *= h / 3;

            Console.WriteLine("h = " + h);
            Console.WriteLine("allValue = " + allValue);

            return allValue;
        }

        //辛普森方法进行积分(网上的第二种说法)-----------------------------------------------------------------------------------
        private double Simpson(List<double> values, List<long> timeStep)
        {
            int n = 4;
            //因为这里真实计算的时间，而获得数据使用的是下标，所以需要一些切换
            double h = timeStep.Count / n;
            double allValue = values[0] - values[values.Count - 1];
            for (int  i = 1; i <= n; i++)
            {
                allValue += 4 * values[(int)(0 + h * i - h / 2)];
                allValue += 2 * values[(int)(0 + h * i)];
            }
            //切换到真实的H
            h = (double)(timeStep[timeStep.Count - 1] - timeStep[0]) / (n * 1000);
            allValue *= h / 6;
            return allValue;
        }

        //针对速度的加速度积分方法(看造型也是样条的方法)
        private double DemoSimpleValues2(List<double> values, List<long> timeStep)
        {
            double allvalues = 0;

            long timeUse = timeStep[timeStep.Count - 1] - timeStep[0];
            //Console.WriteLine("timeUse = " + timeUse);
            double time = ((double)timeUse / 1000) / timeStep.Count;//因为时间戳是毫秒作为单位的
            //Console.WriteLine("time = " + time);
            //看上去就是附带做了一次一阶线性滤波
            //或者也可以直接理解为梯形法
            for (int i = 1; i < timeStep.Count; i++)
                allvalues += (values[i]+ values[i-1])* time * 0.5; 

            return allvalues;
        }

        //带误差的方法，取所有获得的数据取平均计算矩形面积
        //简单粗暴误差大，但是如果能够去除掉足够小的噪声的话，或许还是可以用的
        private double AverageWithError(List<double> values, List<long> timeStep)
        {
            long timeUse = timeStep[timeStep.Count - 1] - timeStep[0];
            double theAverage = 0;

            if (values.Count == 0)
                return 0;

            for (int i = 0; i < values.Count; i++)
                theAverage += values[i];
            theAverage /= values.Count;

            return (theAverage * ((double)timeUse / 1000));
        }

   }
}
