using socketServer.Codes.stages;
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

        FSMBasic theStage = new StageStance();//当前状态的推断，使用的是有限状态机
        private int windowCount = 5;//窗口大小，如果是5就是查看最后五个数据
        public double slopNow = 0;
        //获得走路的最新的slope数值使用
        //可以在后面用来判断行走姿态，算是为后面做的一个简单的准备工作，留下接口
        //实际上这个也分为两种
        //一种是每出现一个周期检查一下这个周期的slope的数值
        //一种是钉死窗口滑动，检查这个窗口的数值
        public List<int> stepModeCheckUse(List<int> indexBuff, Filter theFilter, information theInformationController ,List<double> theFilteredAZ)
        {

            if (indexBuff.Count > 1)
            {
                List<double> X = theFilter.theFilerWork(theInformationController.accelerometerX);
                List<double> Y = theFilter.theFilerWork(theInformationController.accelerometerY);
                List<double> Z = theFilter.theFilerWork(theInformationController.accelerometerZ);
                List<int> toDelete = new List<int>();
                for (int i = 0; i < indexBuff.Count-1; i++)
                {
                    double slopeWithPeack = getModeCheckWithPeack( X, Y, Z, indexBuff[i], indexBuff[i+1]);
                    this.slopNow = slopeWithPeack;
                    SystemSave.slopNow = slopeWithPeack;
                    theStage = theStage.ChangeState(slopeWithPeack, indexBuff, theFilteredAZ);
                    if (theStage is StageStance)//如果判断为站立状态就删除这个坐标
                        toDelete.Add(i+1);
                }
                for (int i = 0; i < toDelete.Count; i++)
                    indexBuff.Remove(toDelete[i]);
                //string stateInformate = "[" + theStage.getInformation() + "]";
                return indexBuff;
            }
            else
            {
                //string stateInformate = "[" + theStage.getInformation() + "]";
                return  indexBuff;
            }

            return indexBuff;
        }

//====================================================================================================================================//
        private double getModeCheckWithWindow(List<double> AX, List<double> AY, List<double> AZ, int windowUse = 5)
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

        private double  getModeCheckWithPeack(List<double> AX , List<double> AY , List<double> AZ, int startIndex , int endIndex)
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

            //Console.WriteLine("maxAX = " + maxAX);
            //Console.WriteLine("minAX = " + minAX);
            //Console.WriteLine("maxAY = " + maxAY);
            //Console.WriteLine("minAY = " + minAY);
            //Console.WriteLine("maxAZ = " + maxAZ);
            //Console.WriteLine("minAZ = " + minAZ);
            //Console.WriteLine("stepLengthSlop = " + stepLengthSlop);
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
