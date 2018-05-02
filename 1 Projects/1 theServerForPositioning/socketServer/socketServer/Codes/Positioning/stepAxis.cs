using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace socketServer.Codes.stages
{
    //掌管使用的加速度的轴的类，封装一层是为了保证有扩展性和输出

   public  class stepAxis
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


        public List<double> stepAxisUse(int stepAxisUseIndex, information theInformationController, Filter theFilter, bool useFilter = true)
        {
            List<double> theFilteredAZ = new List<double>();
            switch (stepAxisUseIndex)//选择不同的轴向
            {
                case 0:
                    //基础方法:用Z轴加速度来做
                    theFilteredAZ = AZ(theInformationController, theFilter, useFilter);
                    break;
                case 1:
                    //实验用方法：X轴向
                    theFilteredAZ = AX(theInformationController, theFilter, useFilter);
                    break;
                case 2:
                    //实验用方法：X轴向
                    theFilteredAZ = AY(theInformationController, theFilter, useFilter);
                    break;
                case 3:
                    //基础方法:用三个轴的加速度平方和开根号得到
                    theFilteredAZ = ABXYZ(theInformationController, theFilter, useFilter);
                    break;
                case 4:
                    //基础方法:用三个轴的加速度最大方差得到
                    theFilteredAZ = XYZMaxVariance(theInformationController, theFilter, useFilter);
                    break;
            }
            return theFilteredAZ;
        }
        //--------------------------------------------------------------------------------------------------------//
        private List<double> AZ(information theInformationController , Filter theFilter ,bool useFilter = true)
        {
            if(useFilter)
                return theFilter.theFilerWork(theInformationController.accelerometerZ);
            return theInformationController.accelerometerZ;
        }
        private List<double> AY(information theInformationController, Filter theFilter, bool useFilter = true)
        {
            if (useFilter)
                return theFilter.theFilerWork(theInformationController.accelerometerY);
            return theInformationController.accelerometerY;
        }
        private List<double> AX(information theInformationController, Filter theFilter, bool useFilter = true)
        {
            if(useFilter)
                 return theFilter.theFilerWork(theInformationController.accelerometerX);
            return theInformationController.accelerometerX;
        }
        private List<double> ABXYZ(information theInformationController, Filter theFilter, bool useFilter = true)
        {
            List<double> work = theInformationController.getOperatedValues();
            if (useFilter)
                return theFilter.theFilerWork(work);
            return work;
        }

        private List<double> XYZMaxVariance(information theInformationController, Filter theFilter, bool useFilter = true)
        {
            List<double> Variances = new List<double>();
            List<double> AX; 
            List<double> AY;
            List<double> AZ;
            if (useFilter)
            {
                AX = theFilter.theFilerWork(theInformationController.accelerometerX);
                AY = theFilter.theFilerWork(theInformationController.accelerometerY);
                AZ = theFilter.theFilerWork(theInformationController.accelerometerZ);
            }
            else
            {
                AX = theInformationController.accelerometerX;
                AY = theInformationController.accelerometerY;
                AZ = theInformationController.accelerometerZ;
            }
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
        private double getVariance(List<double> theValues)
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
        private int getMaxIndex(List<double> theValues)
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
        //返回全部的方法说明
        public string[] getMoreInformation()
        {
            return methodInformations;
        }

    }
}
