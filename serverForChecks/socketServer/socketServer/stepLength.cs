using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace socketServer
{
    //这个类专门用来处理步长
    class stepLength
    {
        public double getStepLength()
        {
            return stepLength1();
        }

        //微软研究得到的平均步长
        //在这里是为了保证架构
        private double stepLength1()
        {
            return 9.5;
        }
    }
}
