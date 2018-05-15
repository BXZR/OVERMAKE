using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace socketServer.Codes.Positioning
{
    //判断走一步之后结合其他数据的“滤步”
    //判步滤镜，用于剔除indexBuff中不能被认为是一步的那些记录
    //顺带判断当前的状态也用这个类来做
    //作为对原生数据的处理结果的一种判定

    public class StepFilter
    {
        //分类方法也就是stepFilter的方法之一
        private stepModeCheck theSlopChecker = new stepModeCheck();

        //每一种方法的简短说明信息
        private string[] methodInformations =
        {
            "判断走一步之后不再进行额外分类剔除",
            "其他轴的变化如果不够大，这一步将会被剔除",
            "使用有限状态机对亦不进行分类，剔除静止步"
        };
        //返回全部的方法说明
        public string[] getMoreInformation()
        {
            return methodInformations;
        }
        public string getInformation(int index)
        {
            return methodInformations[index];
        }

       //-------------------------------------------------------------两大核心功能--------------------------------------------------------------------------------//
        //过滤掉多余的步子
        public List<int> FilterStep(information theInformationController, Filter theFilter, List<int> indexBuff, List<double> filteredAZ , int methodID )
        {
            switch (methodID)
            {
                case 0: { return indexBuff; }break;
                case 1: { return FixedStepCalculate(theInformationController, theFilter, indexBuff); } break;
                case 2: { indexBuff = theSlopChecker.stepModeCheckUse(indexBuff , theFilter , theInformationController, filteredAZ); };break;
            }
            return indexBuff; 
        }

        //当前推断的行动状态信息
        public string stateInformation(int methodID)
        {
            string information = "";
            switch (methodID)
            {
                case 0: { information =  "没有状态信息"; }break;
                case 1: { information = "没有状态信息"; } break;
                //这一项最多只能是行走状态，而不会有站立状态，因为已经站立状态已经被剔除了
                case 2: { information = theSlopChecker.theStage.getInformation(); } break;
            }
            return information;
        }

//================================================================================================================================================//

        //方法1，多轴方差比照的做法
        //检测这个移动是不是真的移动，也就是说在原地晃手机的时候是否允许被判断走了一步
        //在实验的时候原地晃手机是可以的，但是在实际使用的时候原地晃手机不可以这样，者可以通过一个模式进行判断
        private List<int> FixedStepCalculate(information theInformationController, Filter theFilter, List<int> indexBuff)
        {
            //Console.WriteLine("-------------------------------------------");
            // Console.WriteLine("indexBuff Count pre= " + indexBuff.Count);
            List<int> toRemove = new List<int>();
            for (int i = 1; i < indexBuff.Count; i++)
            {
                List<double> theX = theFilter.theFilerWork(theInformationController.accelerometerX);
                List<double> theY = theFilter.theFilerWork(theInformationController.accelerometerY);
                List<double> theZ = theFilter.theFilerWork(theInformationController.accelerometerZ);
                double XVariance = MathCanculate.getVariance(theX, indexBuff[i - 1], indexBuff[i]);
                double YVariance = MathCanculate.getVariance(theY, indexBuff[i - 1], indexBuff[i]);
                double ZVariance = MathCanculate.getVariance(theZ, indexBuff[i - 1], indexBuff[i]);
                List<double> Variances = new List<double>();
                Variances.Add(XVariance);
                Variances.Add(YVariance);
                Variances.Add(ZVariance);
                Variances = MathCanculate.SortValues(Variances);
                //如果第二大的项目方法不够大，就认为是原地踏步，这个方法可以在后期扩展
                //也必须扩展
                double gate = 0.1;
                //Console.WriteLine(Variances[1]);
                if (Variances[1] < gate)
                    toRemove.Add(indexBuff[i]);
            }
            for (int i = 0; i < toRemove.Count; i++)
            {
                indexBuff.Remove(toRemove[i]);
            }
            //Console.WriteLine("indexBuff Count after= " + indexBuff.Count);
            return indexBuff;
        }

    }
}
