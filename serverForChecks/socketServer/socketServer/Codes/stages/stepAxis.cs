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
        //返回对这种方法的说明
        public string getMoreInformation(int index)
        {
            return methodInformations[index];
        }


    }
}
