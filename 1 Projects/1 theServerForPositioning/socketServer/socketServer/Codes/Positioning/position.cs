using socketServer.Codes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace socketServer
{

    //用来记录的类
   public class transForm
    {
        public double X;
        public double Y;
        public double Z = 0;//Z是垂直于XY平面的轴，变现为高度的移动
        //需要注意的是这里设定的是XY平面是地面，这与Unity中的设定是不一样的
        public double heading;
        public transForm(double XIn, double YIn , double headingIn)
        {
            X = XIn;
            Y = YIn;
            heading = headingIn;
        }
        public transForm(double XIn, double YIn, double ZIn , double headingIn)
        {
            X = XIn;
            Y = YIn;
            Z = ZIn;
            heading = headingIn;
        }
        public string toString()
        {
            return ("X = " + X.ToString("f2") + " Y = " + Y.ToString("f2") );
        }
        public string toStringWithHeading()
        {
            return ("X = " + X.ToString("f2") + " Y = " + Y.ToString("f2") + " heading = "+ heading.ToString("f2"));
        }
        public string toStringFull()
        {
            return ("X = " + X.ToString("f2") + " Y = " + Y.ToString("f2") + " Z = " +Z.ToString("f2") + " heading = " + heading);
        }
    }

    //这个类专门用来计算坐标
    class position
    {
        public double positionX = 0;
        public double positionY = 0;
        public double positionZ = 0;
        public List<transForm> theTransformPosition = new List<transForm>();//真正用来记录坐标的工具
    /*****************************方法1，立即计算的方法*****************************************/
        //最基础的架构公式
        public void calculationPosition ( double angel , double stepLength , double ZMove = 0)
        {
            positionX = positionX + Math.Sin(angel) * stepLength;
            positionY = positionY + Math.Cos(angel) * stepLength;
            positionZ = ZMove;
        }

        //获取到当前的坐标
        public string getPosition()
        {
            return " (" + positionX.ToString("f4") + " , " + positionY.ToString("f4") + " , "  + positionZ.ToString("f4")+ ")"; 
        }

        /******************************方法2，先把顶点存到缓冲区的方法*********************************************************/
        //给出打包的角度何必步长的缓存
        //难点就在于怎么构建这个缓存
        //初步思路是在隔一段时间统一计算中通过波峰获得波峰的下标，这也会是角度的下标
        //此外步长现在是一个立即数，有方法获得
        //为此还需要一个波峰的下标的缓冲区，并且花时间建立两个List
        //但是这种方法大幅度减缓了周期，看上去还算合算
        public string getPositions(List<double> angels, List<double> stepLengths, List<double> stairMove = null)
        {
            if (SystemSave.savedPositions.Count > 0)
            {
                //获得最后的一个坐标信息
                int index = SystemSave.savedPositions.Count-1;
                positionX = SystemSave.savedPositions[index].X;
                positionY = SystemSave.savedPositions[index].Y;
                positionZ = SystemSave.savedPositions[index].Z;
            }
            else
            {
                //否则就是默认初始坐标(0)
                //这是一个了累加展示的程序，因此展示相对坐标就好
                // positionX = SystemSave.startPositionX;
                // positionY = SystemSave.startPositionY;
                // positionZ = SystemSave.startPositionZ;
                // 真正使用的
                positionX = 0;
                positionY = 0;
                positionZ = 0;
            }
            List<double> XSave = new List<double>();
            List<double> YSave = new List<double>();
            List<double> ZSave = new List<double>();
            List<double> headingSave = new List<double>();
            string theInformation = "详细定位信息  初始坐标（" + SystemSave.startPositionX.ToString("f4") + " , " +
               SystemSave.startPositionY.ToString("f4") + " , " + SystemSave.startPositionZ.ToString("f4") + "）";
              theInformation  += "\n----------------------------------------------------------------------------------------\n";
            for (int i = 0; i < angels .Count; i++)
            {
                double XAdd = Math.Sin(MathCanculate.getRadianFromDegree(angels[i])) * stepLengths[i];
                double YAdd = Math.Cos(MathCanculate.getRadianFromDegree(angels[i])) * stepLengths[i];
                // Console.WriteLine("--"+XAdd+"--"+YAdd+"--");
                // theInformation += "角度： " + angels[i].ToString("f4") +"\n移动：( " + XAdd.ToString("f4") + " , " + YAdd.ToString("f4") + " )\n------------------\n";
                positionX += XAdd;
                positionY += YAdd;
                //这个需要使用决策树判断是不是上楼梯型的stepLength
                //最后根据已经得到的siderStep来获得（这可能是一个立即数）
                if (stairMove != null)//如果判断Z轴向的移动(一般来说平面计算就有老所事情要做)
                {
                    positionZ += stairMove[i];
                }
                XSave.Add(positionX);
                YSave.Add(positionY);
                ZSave.Add(positionZ);
                headingSave.Add(angels[i]);
            }

            theTransformPosition.Clear();
            for (int i = 0; i < XSave.Count; i++)
            {
                theTransformPosition.Add(new transForm(XSave[i] , YSave[i], ZSave[i], headingSave[i]));
                //最终展示的时候除了偏移量还需要加上真实初始的坐标
                theInformation += "角度： " + angels[i].ToString("f4") + "  步长： " + stepLengths[i].ToString("f4");
                theInformation += "  坐标： (" + (XSave[i] + SystemSave.startPositionX).ToString("f4") + " , ";
                theInformation += (YSave[i] + SystemSave.startPositionY).ToString("f4") + " , ";
                theInformation += (ZSave[i]+SystemSave.startPositionZ).ToString("f4") + ") \n";
            }
            return theInformation;
        }

        //矫正更新最终的位置
        //其中Z轴不一定更新
        //有一点问题就是两种绘制方式的协调并且调整位置不算新走的一步
        public void setPosition(double X, double Y, double Z = 0)
        {
            for (int i = 0; i < theTransformPosition.Count; i++)
            {
                SystemSave.savedPositions.Add(theTransformPosition[i]);
            }
            int indexForLast = SystemSave.savedPositions.Count - 1;
            transForm newPosition = new transForm(X, Y, Z, SystemSave.savedPositions[indexForLast].heading);
            SystemSave.savedPositions[indexForLast] = newPosition;
            theTransformPosition[theTransformPosition.Count - 1] = newPosition;
        }


    }
}
