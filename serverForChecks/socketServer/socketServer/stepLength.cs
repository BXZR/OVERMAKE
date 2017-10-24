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

        //外部方法1，必须对应methodType方法0，这个在mainWindow处会有判断
        public double getStepLength(double angelPast = 0, double angelNow = 0 )
        {
              return StepLengthMethod1(angelPast, angelNow);
        }

        //不论应用的是哪一个轴向，至少需要传入一个用来计算的轴向
        //indexPre 和 indexNow 指的是传入的theA的下标，需要算theA的方差，而这这两个下标就是范围
        public double getStepLength(int indexPre , int indexNow , List<double> theA , List<long> timeUse = null )
        {
            if ( indexNow <= indexPre || timeUse == null  )//也就是说传入的数值是错误的，或者数据不够
                return stepLengthBasic();//万金油
            else
            {
                double average = 0;
                for (int i = indexPre; i < indexNow; i++)
                {
                    average += theA[i];
                }
                average /= (indexNow - indexPre);
                //公式需要使用的参数 (为了保证清晰，分成多个循环来写)
                double VK = 0;
                for (int i = indexPre; i < indexNow; i++)
                {
                    VK += (theA[i] - average) * (theA[i] - average);
                }
                VK /= (indexNow - indexPre);

                double timestep = timeUse[indexNow] - timeUse[indexPre];
                //有除零异常说明时间非常短，可以认为根本就没走
                if(timestep  ==0)
                    return 0;//万金油
                double FK = (1000 / timestep);//因为时间戳是毫秒作为单位的

                double stepLength = 0.2 * VK + 0.3 * FK + 0.4;
               // Console.WriteLine("VK =" + VK + " FK =" + FK + " length = " + stepLength);
                if (stepLength > 2)//一步走两米，几乎不可能
                    return stepLengthBasic();//万金油
                else
                    return stepLength;
            }
        }

        //为了保证以后传入多个参数进行判断的情况，请保持这种模式
        private double StepLengthMethod1(double angelPast = 0 , double  angelNow = 0)
        {
            if (Math.Abs(angelPast - angelNow) > changeGate)
                return stepLengthBasic() / 2;
            else
                return stepLengthBasic();
        }

        //微软研究得到的平均步长
        //在这里是为了保证架构做的基础实现
        //后期打算用训练后的步长模型来做
        private double stepLengthBasic()
        {
            return 0.95;
        }
    }
}
