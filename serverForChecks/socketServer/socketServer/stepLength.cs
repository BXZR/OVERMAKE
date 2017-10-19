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
        private double changeGate = 60;//转弯的阀值
        //如果转弯且角度差异大于一个阀值，返回的步长信息恐怕需要调整
        //为了保证以后传入多个参数进行判断的情况，请保持这种模式
        public double getStepLength(double angelPast = 0 , double  angelNow = 0)
        {
            if (Math.Abs(angelPast - angelNow) > changeGate)
                return stepLength1() / 2;
            else
                return stepLength1();
        }

        //微软研究得到的平均步长
        //在这里是为了保证架构做的基础实现
        //后期打算用训练后的步长模型来做
        private double stepLength1()
        {
            return 0.95;
        }
    }
}
