using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace socketServer
{
    //这个类用于步态的推测
    class stepModeCheck
    {
        //步长的slop参数
        //根据这个slop的数值可以推测人的移动模式为步行、停止、跑步等等
        //具体操作需要在此基础之上进一步处理，这里这个数值仅仅用于记录
      
        private int windowCount = 5;//窗口大小，如果是5就是查看最后五个数据

        public double getModeCheckWithWindow(List<double> AX, List<double> AY, List<double> AZ, int windowUse = 5)
        {
            double stepLengthSlop = 0;
            if (AX.Count < windowUse)
                return 0;
            double maxAX = findMax(AX, AX.Count - 1 - windowUse, AX.Count - 1 );
            double minAX = findMin(AX, AX.Count - 1 - windowUse, AX.Count - 1 );
            double maxAY = findMax(AY, AY.Count - 1 - windowUse, AY.Count - 1 );
            double minAY = findMin(AY, AY.Count - 1 - windowUse, AY.Count - 1 );
            double maxAZ = findMax(AZ, AZ.Count - 1 - windowUse, AZ.Count - 1 );
            double minAZ = findMin(AZ, AZ.Count - 1 - windowUse, AZ.Count - 1 );
            stepLengthSlop = Math.Sqrt((maxAX - minAX) * (maxAX - minAX) + (maxAY - minAY) * (maxAY - minAY) + (maxAZ - minAZ) * (maxAZ - minAZ));
            return stepLengthSlop;
        }

        public double  getModeCheckWithPeack(List<double> AX , List<double> AY , List<double> AZ, int startIndex , int endIndex)
        {
            double stepLengthSlop = 0;
            //Console.WriteLine("startIndex = " + startIndex);
            // Console.WriteLine("endIndex = " + endIndex);

            double maxAX = findMax(AX, startIndex, endIndex);
            double minAX = findMin(AX, startIndex, endIndex);
            double maxAY = findMax(AY, startIndex, endIndex);
            double minAY = findMin(AY, startIndex ,endIndex);
            double maxAZ = findMax(AZ, startIndex, endIndex);
            double minAZ = findMin(AZ, startIndex, endIndex);
            
            stepLengthSlop = Math.Sqrt((maxAX - minAX) * (maxAX - minAX) + (maxAY - minAY) * (maxAY - minAY) + (maxAZ - minAZ) * (maxAZ - minAZ));

            // Console.WriteLine("maxAX = " + maxAX);
            // Console.WriteLine("minAX = " + minAX);
            // Console.WriteLine("maxAY = " + maxAY);
            // Console.WriteLine("minAY = " + minAY);
            // Console.WriteLine("maxAZ = " + maxAZ);
            // Console.WriteLine("minAZ = " + minAZ);
            // Console.WriteLine("stepLengthSlop = " + stepLengthSlop);
            return   stepLengthSlop ;
        }
     
        //获取一段数组中最大的数值
        private double findMax (List<double> IN  , int startIndex , int endIndex)
        {
            //为了防止传入参数颠倒做的保护
            if (startIndex > endIndex)
            {
                int temp = startIndex;
                startIndex = endIndex;
                endIndex = temp;  
            }

            double max = -99999;
            for(int i = startIndex; i< endIndex; i++)
            {
                if (IN[i] > max)
                    max = IN[i];
            }
            return max;
        }
        //获取一段数组中最小的数值
        private double findMin(List<double> IN, int startIndex, int endIndex)
        {
            //为了防止传入参数颠倒做的保护
            if (startIndex > endIndex)
            {
                int temp = startIndex;
                startIndex = endIndex;
                endIndex = temp;
            }

            double min = 99999;
            for (int i = startIndex; i < endIndex; i++)
            {
                if (IN[i] < min)
                    min = IN[i];
            }
            return min;
        }


    }
}
