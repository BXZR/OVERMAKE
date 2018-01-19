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

        //这个类专门用来处理积分积分相关的问题
        //采样时间足够短并且要求精度不是很高的时候原则上是可以用的
        public double makeIntegral(List<double> values, List<long> timeSteps , int mode = 0)
        {
            double allValue = 0;

            switch (mode)
            {
                case 0: { allValue = SimpleValues(values , timeSteps); } break;
                case 1: { allValue = Simpson(values, timeSteps); } break;
                default:{ allValue = SimpleValues(values, timeSteps); }break;
            }

            return allValue;
        }

        //各种积分方法(都是单次积分，如果有需要就多次调用也就是多次积分了)
        //样条方法进行积分-----------------------------------------------------------------------------------
        private double SimpleValues(List<double> values, List<long> timeStep)
        {
            double allvalues = 0;

            long timeUse = timeStep[timeStep.Count-1] - timeStep[0];
            double time = ((double)timeUse / 1000) / timeStep.Count;//因为时间戳是毫秒作为单位的

            int lowerACount = 0;
            for (int i = 0; i < timeStep.Count; i++)
                allvalues += values[i] * time;

            return allvalues;
        }

        //辛普森方法进行积分-----------------------------------------------------------------------------------
        private double Simpson(List<double> values, List<long> timeStep)
        {
            int n = 4;
            double h = timeStep.Count / n;
            double ans = values[values.Count-1] + values[0];

            for (int i = 1; i < n; i += 2)
                ans += 4 * values[0+ (int)(i * h)];

            for (int i = 2; i < n; i += 2)
                ans += 2 * values[0 + (int)(i * h)];

           return ans * h / 3;
        }

   }
}
