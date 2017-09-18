using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace socketServer
{
    //这个类专门用来计算坐标
    class position
    {
        public double positionX = 0;
        public double positionY = 0;

        
    /*****************************方法1，立即计算的方法*****************************************/
        //最基础的架构公式
        public void calculationPosition ( double angel , double stepLength )
        {
            positionX = positionX + Math.Cos(angel) * stepLength;
            positionY = positionY + Math.Sin(angel) * stepLength;
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
            positionX = 0;
            positionY = 0;
        List<double> XSave = new List<double>();
            List<double> YSave = new List<double>();

            string theInformation = "";
            for (int i = 0; i < angels .Count; i++)
            {
                positionX = positionX + Math.Cos(angels[i]) * stepLengths[i];
                positionY = positionY + Math.Sin(angels[i]) * stepLengths[i];
                XSave.Add(positionX);
                YSave.Add(positionY);
            }
            for (int i = 0; i < XSave.Count; i++)
            {
                theInformation  += " (" + XSave[i].ToString("f4") + " , " + YSave[i].ToString("f4") + ") \n";
            }
            return theInformation;
        }
    }
}
