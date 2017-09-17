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
    }
}
