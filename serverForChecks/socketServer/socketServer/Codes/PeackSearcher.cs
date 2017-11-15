using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace socketServer
{
    //这个类用于寻找波峰和波谷
    //也就是用于判断走了一步
    //这个类之前可以考虑用一个滤波类做一下

    class PeackSearcher
    {

        private static int theCount = 0;//波峰的个数
        private static int theCount2 = 0;//波谷的个数
        private static int theStepCount = 0;//被计算出来的步数
        public static int TheStepCount { get { return theStepCount; } } //可以直接访问的只读的步数
        public static int TheCount { get { return theCount; } } //可以直接访问的只读波峰的个数
        public static int TheCount2 { get { return theStepCount; } } //可以直接访问的只读波谷的个数

        public static int changeCount = 0;//步数变化的次数
        private double canReachGate = 0.1f;//没有超过这个加速度就认为是抖动，无视之
        //返回是否走了一步，用的是内部的变量，实际上也应该怎么做，不推荐外部调用
 

        int stepCountSave = 0;//步数保存并用于判断是不是走出了一步
        //事实上这个变量只在这个方法中使用

        public List<int> peackBuff = new List<int>();//保存波峰的下标的缓冲区

        public bool countCheck(List<double> wave)
        {
            if (wave.Count < 1)
                return false ;


            int stepNow = countStepWith(wave);
            if (stepCountSave  < stepNow )
            {
                theStepCount = stepNow;
                stepCountSave = stepNow;
                changeCount++;
                return true;
            }

            return false;
        }


        //基础方法,内部方法，并不修改那些变量
        //返回步数
        private int countStepWith(List<double> wave)
        {
            int CS1 = 0;
            int CS2 = 0;
            if (wave.Count < 1)
                return 0;
            int direction = wave[0] > 0 ? -1 : 1;
            for (int i = 0; i < wave.Count - 1; i++)
            {
                double minus = wave[i + 1] - wave[i];

                if ( Math .Abs(minus ) > canReachGate   && minus * direction > 0)//放弃突变的情况
                {

                    direction *= -1;
                    if (direction == 1)
                    {
                        CS1++;
                        //"波峰"
                    }
                    else
                    {
                        CS2++;
                        //"波谷"
                    }
                }
            }
            //用波峰波谷的数量平均值来做似乎比较好，暂时从个人的逻辑来说
            return  CS1;
        }

        //附带缓冲区记录
        //基础方法，可以通过这个方法来修改共有变量
        //返回步数
        //传入的是加速度传感器的Y轴的数值
        //这个方法被应用于peack step detection里面中
        public int countStepWithStatic(List <double> wave)
        {
           peackBuff.Clear();//每一次都重新计算，这个方法整体可以考虑大优化
           theCount = 0;//波峰的个数
           theCount2 = 0;//波谷的个数

            if (wave.Count < 1)
                return 0;
           int direction = wave[0] > 0? -1:1;
          for(int i=0;i< wave .Count -1;i++) 
          {
                if (Math.Abs(wave[i]) < canReachGate)
                    continue;
                 double minus = wave[i+1]-wave[i];

                if (Math.Abs(minus) > canReachGate  && minus * direction > 0)//放弃突变的情况
                {
                    direction *= -1;
                    if (direction == 1)
                    {
                        theCount++;
                        //"波峰"
                        //在一定时间间隔之内只有可能有一步
                        // SystemSave. peackThresholdForStepDetection掌管这个数据间隔量
                        if ( peackBuff.Count < 3 || ( i- peackBuff[peackBuff .Count -1] ) > SystemSave.peackThresholdForStepDetection)
                        peackBuff.Add(i);
                    }
                    else
                    {
                        theCount2++; 
                        //"波谷"
                    }
                }
          }
            theStepCount = theCount;//记录这个步数
         //用波峰波谷的数量平均值来做似乎比较好，暂时从个人的逻辑来说
          return theCount;
        }

    }
}
