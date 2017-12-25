using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace socketServer.Codes.stages
{
    //掌管使用的加速度的轴的类，封装一层是为了保证有扩展性和输出

    class stepAxis
    {
        //每一种方法的简短说明信息
        private string[] methodInformations =
        {
            "使用手机加速度传感器Z轴进行计算",
            "使用手机加速度传感器Y轴进行计算",
            "使用手机加速度传感器X轴进行计算",
            "使用手机加速度传感器XYZ轴平方和开根号进行计算",
            "使用手机加速度传感器XYZ轴中方差最大的值进行计算",
        };
        public List<double> AZ(information theInformationController , Filter theFilter)
        {
            return theFilter.theFilerWork(theInformationController.accelerometerZ);
        }
        public List<double> AY(information theInformationController, Filter theFilter)
        {
            return theFilter.theFilerWork(theInformationController.accelerometerY);
        }
        public List<double> AX(information theInformationController, Filter theFilter)
        {
            return theFilter.theFilerWork(theInformationController.accelerometerX);
        }
        public List<double> ABXYZ(information theInformationController, Filter theFilter)
        {
            return theFilter.theFilerWork(theInformationController.getOperatedValues());
        }

        public List<double> XYZMaxVariance(information theInformationController, Filter theFilter)
        {
            List<double> Variances = new List<double>();
            List<double> AX = theFilter.theFilerWork(theInformationController.accelerometerX);
            List<double> AY = theFilter.theFilerWork(theInformationController.accelerometerY);
            List<double> AZ = theFilter.theFilerWork(theInformationController.accelerometerZ);
            List<List<double>> Axis = new List<List<double>>();
            Axis.Add(AX);
            Axis.Add(AY);
            Axis.Add(AZ);
            Variances.Add(getVariance(AX));
            Variances.Add(getVariance(AY));
            Variances.Add(getVariance(AZ));
            return Axis[getMaxIndex(Variances)];
        }

        //很简单的辅助方法：计算方差
        double getVariance(List<double> theValues)
        {
            double average = 0;
            double variance = 0;
            //计算平均数
            for (int i = 0; i < theValues.Count; i++)
                average += theValues[i];
            average /= theValues.Count;

            for (int i = 0; i < theValues.Count; i++)
            {
                variance +=  (theValues[i]- average)* (theValues[i] - average);
            }
            variance /= theValues.Count;

            return variance;
        }

        //选取最大的
        int getMaxIndex(List<double> theValues)
        {
            int index = 0;
            double max = -9999;
            for (int i = 0; i < theValues.Count; i++)
            {
                if (max < theValues[i])
                {
                    max = theValues[i];
                    index = i;
                }
            }
            return  index;
        }
   

        //返回对这种方法的说明
        public string getMoreInformation(int index)
        {
            return methodInformations[index];
        }
  

    }
}
