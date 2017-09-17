using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace socketServer
{
    //这个类专门用来控制旋转信息的处理
    class rotationAngel
    {
        private double theAngelNow = 0;//总记录的角度
        private double changeGate = 5;//如果变化超过这个数目就认为改变了

        //判断变化，超过阀值就认为有所改变了
        public  double getAngelNow(double angelIn)
        {
            if (Math.Abs(theAngelNow - angelIn) > changeGate)
            {
                theAngelNow = angelIn;
            }
            return theAngelNow;
        }

        //微软提出的一种更准一点的判断转向了的方法
        public double getAngelNow(List<double> IN)
        {
            if (IN.Count <=1)
                return theAngelNow;

            double ALL = 0;
            for (int i = 0; i < IN.Count; i++)
            {
                ALL += IN[i];
            }
            double average = ALL / IN.Count - 1;
            if (Math.Abs(theAngelNow - average) > changeGate)
            {
                theAngelNow = IN[IN.Count -1];
            }
            return theAngelNow;
        }
    }
}
