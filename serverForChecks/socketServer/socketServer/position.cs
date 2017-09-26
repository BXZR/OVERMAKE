using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace socketServer
{

    //用来记录的类
    class transForm
    {
        public double X;
        public double Y;
        public transForm(double XIn, double YIn)
        {
            X = XIn;
            Y = YIn; 
        }
    }

    //这个类专门用来计算坐标
    class position
    {
        public double positionX = 0;
        public double positionY = 0;

        public List<transForm> theTransformPosition = new List<transForm>();//真正用来记录坐标的工具
    /*****************************方法1，立即计算的方法*****************************************/
        //最基础的架构公式
        public void calculationPosition ( double angel , double stepLength )
        {
            positionX = positionX + Math.Sin(angel) * stepLength;
            positionY = positionY + Math.Cos(angel) * stepLength;
        }

        public string getPosition()
        {
            return " (" + positionX.ToString("f4") + " , " + positionY.ToString("f4") + ")"; 
        }

        /******************************方法2，先把顶点存到缓冲区的方法*********************************************************/
        //给出打包的角度何必步长的缓存
        //难点就在于怎么构建这个缓存
        //初步思路是在隔一段时间统一计算中通过波峰获得波峰的下标，这也会是角度的下标
        //此外步长现在是一个立即数，有方法获得
        //为此还需要一个波峰的下标的缓冲区，并且花时间建立两个List
        //但是这种方法大幅度减缓了周期，看上去还算合算
        public string getPositions(List<double> angels, List<double> stepLengths)
        {
            
            List<double> XSave = new List<double>();
            List<double> YSave = new List<double>();

            string theInformation = "角度： 0.0000 步长： 0.9500 坐标： （0.0000,0.0000）\n";
            for (int i = 0; i < angels .Count; i++)
            {
                double XAdd = Math.Sin(getRadianFromDegree(angels[i])) * stepLengths[i];
                double YAdd = Math.Cos(getRadianFromDegree(angels[i])) * stepLengths[i];
                // Console.WriteLine("--"+XAdd+"--"+YAdd+"--");
                // theInformation += "角度： " + angels[i].ToString("f4") +"\n移动：( " + XAdd.ToString("f4") + " , " + YAdd.ToString("f4") + " )\n------------------\n";
                positionX += XAdd;
                positionY += YAdd;
                XSave.Add(positionX);
                YSave.Add(positionY);
            }

           // theTransformPosition.Clear();
            for (int i = 0; i < XSave.Count; i++)
            {
                theTransformPosition.Add(new transForm(XSave[i] , YSave[i]));
                theInformation += "角度： " + angels[i].ToString("f4") + " 步长： "+stepLengths [i]+"坐标：  (" + XSave[i].ToString("f4") + " , " + YSave[i].ToString("f4") + ") \n";
            }
            return theInformation;
        }

        // 角度转弧度 π/180×角度
        double getRadianFromDegree(double degree)
        {
            return degree * Math.PI / 180;
        }
        // 弧度变角度 180/π×弧度
        double getDegreeFromRadian(double radian)
        {
            return radian * 180 / Math.PI;
        }
    }
}
