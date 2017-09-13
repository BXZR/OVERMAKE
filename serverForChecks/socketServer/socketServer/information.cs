using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace socketServer
{

    public enum UseDataType { accelerometerX, accelerometerY, accelerometerZ, gyroX, gyroY, gyroZ, magnetometerX, magnetometerY, magnetometerZ, compassDegree }
    //这个类用于处理、缓存保存从客户端得到信息
    //此外，所有查看信息的方法都应该只在这一个类中出现
    
    //事实上，这个才是一个真正用于操作的类，server操作只是为了填充这个类，其他的类用于处理这个类
    class information
    {
        //所有的内容实际上都在这个类里面做一下缓存，存好之后处理

        public List<string> allInformation = new List<string>();//总的信息缓存
       public List<double> accelerometerY = new List<double>();//专门用于记录加速计Y轴的数据的数组
       public List <double> compassDegree = new List<double> ();//专门用于记录磁力计辅助数据的数组
        //唯一对外开放的存储方法
        public void addInformation(string information , UseDataType theType)
        {
        
            if (theType == UseDataType.accelerometerY)
            {
                saveAY(information);
            }
            if (theType == UseDataType.compassDegree)
            {
                saveCD(information);
            }

        }

         //各种分量的存储小方法,私有，绝对要私有
        private void saveCD( string information)
        {
             string []splitInformation = information .Split(',');
            double theCDData = 0;
            for(int i=0;i< splitInformation .Length ;i++)
            {
                try
                {

                   // Random D = new Random();
                    theCDData = Convert.ToDouble(splitInformation[i]);// +D.Next(20, 180);
                        
                }
                catch
                {
                    theCDData = 0;
                }
                compassDegree.Add(theCDData);
            }
        }
    
       //各种分量的存储小方法,私有，绝对要私有
        private void saveAY( string information)
        {
             string []splitInformation = information .Split(',');
            double theAYData = 0;
            for(int i=0;i< splitInformation .Length ;i++)
            {
                try
                {
               
                    theAYData  = Convert.ToDouble( splitInformation[i]);
                }
                catch
                {
                    theAYData = 0;
                }
                accelerometerY.Add(theAYData);
            }
        }
    }
}
