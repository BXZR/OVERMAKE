using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace socketServer
{

    public enum UseDataType { accelerometerX, accelerometerY, accelerometerZ, gyroX, gyroY, gyroZ, magnetometerX, magnetometerY, magnetometerZ, compassDegree }
    //这个类用于预处理、缓存保存从客户端得到信息
    //此外，所有查看信息的方法都应该只在这一个类中出现
    
    //事实上，这个才是一个真正用于操作的类，server操作只是为了填充这个类，其他的类用于处理这个类
    class information
    {
        //所有的内容实际上都在这个类里面做一下缓存，存好之后处理

       public List<string> allInformation = new List<string>();//总的信息缓存
       public List<double> accelerometerY = new List<double>();//专门用于记录加速计Y轴的数据的数组
       public List<double> accelerometerZ = new List<double>();//专门用于记录加速计Z轴的数据的数组
       public List<double> accelerometerX = new List<double>();//专门用于记录加速计X轴的数据的数组
       public List <double> compassDegree = new List<double> ();//专门用于记录磁力计辅助数据的数组

       private List<double> theOperatedValue = new List<double>();//记录处理之后的数据，这是一个综合的加速度
        //引用放在这里是为了优化

        //除了直接调用传过来存储的数据，当然也可以做或得到经过一些前期处理得到的数据用于计算
        //思路： aUse = sqrt (ax*ax  + ay*ay + az*az)
        public List<double> getOperatedValues()//预处理方法1
        {
            theOperatedValue.Clear();//实际上这里有一个优化的机制就是动态的起始下标
            for (int i = 0; i < accelerometerZ.Count; i++)
            {
                
                double value = Math.Sqrt(accelerometerY[i] * accelerometerY[i] + accelerometerZ[i] * accelerometerZ[i] + accelerometerX[i] * accelerometerX[i]);
                theOperatedValue.Add(value);
            }
            return theOperatedValue;
        }

        //唯一对外开放的存储方法
        public void addInformation(string information , UseDataType theType)
        {
        
            if (theType == UseDataType.accelerometerY)
            {
                saveAY(information);
            }
            if (theType == UseDataType.accelerometerX)
            {
               saveAX(information);
            }
            if (theType == UseDataType.accelerometerZ)
            {
                saveAZ(information);
            }
            if (theType == UseDataType.compassDegree)
            {
                saveCD(information);
            }

        }

        //唯一对外开放的清理方法
        //进入下一个阶段的记录
        public void flashInformation()
        {
            allInformation.Clear();
            accelerometerY.Clear();
            accelerometerZ.Clear();
            accelerometerX.Clear();
            compassDegree.Clear();
            theOperatedValue.Clear();
        }

         //各种分量的存储小方法,私有，绝对要私有
        private void saveCD( string information)
        {
             string []splitInformation = information .Split(',');
            double theCDData = 0;
            for (int i = 0; i < splitInformation.Length; i++)
            {
                if (string.IsNullOrEmpty(splitInformation[i]) == true)
                {
                    continue;
                }
                else
                {
                    try
                    {

                        // Random D = new Random();
                        theCDData = Convert.ToDouble(splitInformation[i]);// +D.Next(20, 180);

                    }
                    catch
                    {
                        Console.WriteLine("信息不完整：" + splitInformation[i]);
                        theCDData = 0;
                    }
                }
                compassDegree.Add(theCDData +90);//这个是我用别人的手机指南针软件搞出来的角度与这个角度的差异，中间相差90度
            }
        }
    
       //各种分量的存储小方法,私有，绝对要私有
        private void saveAY( string information)
        {
             string []splitInformation = information .Split(',');
            double theAYData = 0;
            for(int i=0;i< splitInformation .Length ;i++)
            {
                if (string.IsNullOrEmpty(splitInformation[i]) == true)
                {
                    continue;
                }
                else
                {
                    try
                    {
                        theAYData = Convert.ToDouble(splitInformation[i]);
                    }
                    catch
                    {
                        theAYData = 0;
                    }
                    accelerometerY.Add(theAYData);
                }
            }
        }

        //各种分量的存储小方法,私有，绝对要私有
        private void saveAX(string information)
        {
            string[] splitInformation = information.Split(',');
            double theAXData = 0;
            for (int i = 0; i < splitInformation.Length; i++)
            {
                if (string.IsNullOrEmpty(splitInformation[i]) == true)
                {
                    continue;
                }
                else
                {
                    try
                    {
                        theAXData = Convert.ToDouble(splitInformation[i]);
                    }
                    catch
                    {
                        theAXData = 0;
                    }
                    accelerometerX.Add(theAXData);
                }
            }
        }

        //各种分量的存储小方法,私有，绝对要私有
        private void saveAZ(string information)
        {
            string[] splitInformation = information.Split(',');
            double theAZData = 0;
            for (int i = 0; i < splitInformation.Length; i++)
            {
                if (string.IsNullOrEmpty(splitInformation[i]) == true)
                {
                    continue;
                }
                else
                {
                    try
                    {
                        theAZData = Convert.ToDouble(splitInformation[i]);
                    }
                    catch
                    {
                        theAZData = 0;
                    }
                    accelerometerZ.Add(theAZData);
                }
            }
        }


    }
}
