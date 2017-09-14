using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace socketServer
{
    //这个类专门用于处理滤波过程
    //处理的是用于步态检测用的y轴加速度
    class Filter
    {


     //唯一对外平滑方法
    public List <double> theFilerWork(List<double> IN)
    {
        IN = theFliterMethod1(IN);
        IN = GetKalMan(IN);
        IN = theFliterMethod2(IN);
        return IN;
    }

    //滤波方法1
    //    一阶滞后滤波法
    //A、方法：
    //    取a=0~1
    //    本次滤波结果=（1-a）*本次采样值+a*上次滤波结果
    //B、优点：
    //    对周期性干扰具有良好的抑制作用
    //    适用于波动频率较高的场合
    //C、缺点：
    //    相位滞后，灵敏度低
    //    滞后程度取决于a值大小
    //    不能消除滤波频率高于采样频率的1/2的干扰信号

        double theValueA = 0.4f;
        private List<double> theFliterMethod1(List<double> IN)
        {
            for (int i = 1; i < IN.Count; i++)
            {
                IN[i] = IN[i] * (1 - theValueA) + theValueA * IN[i - 1];
            }
             return IN;
        }

     //滤波方法2
    //A、方法：
    //    连续取N个采样值进行算术平均运算
    //    N值较大时：信号平滑度较高，但灵敏度较低
    //    N值较小时：信号平滑度较低，但灵敏度较高
    //    N值的选取：一般流量，N=12；压力：N=4
    //B、优点：
    //    适用于对一般具有随机干扰的信号进行滤波
    //    这样信号的特点是有一个平均值，信号在某一数值范围附近上下波动
    //C、缺点：
    //    对于测量速度较慢或要求数据计算速度较快的实时控制不适用
    //    比较浪费RAM

        int smoothCount = 5;//平滑的方法（用几个数据的平均数来代替这几个数据）
        private List<double> theFliterMethod2(List<double> IN)
        {
            List<double> OUT = new List<double>();
            int countUse = 1;//计数器，为了明显用1作为开头了
            double numPlus = 0;//这几个数的总和
            for (int i = 1; i < IN.Count; i++)
            {
                countUse++;
                numPlus += IN[i];
                if(countUse == smoothCount || (i == IN .Count -1 && countUse !=0))//到了采样的时候了
                {
                    OUT.Add(numPlus / countUse);
                    numPlus = 0;
                    countUse = 0;
                }
            }
            return OUT;
        }

        //滤波方法3，究极的卡尔曼滤波

        double[] CanShu = { 23, 9, 16, 16, 1, 0, 0, 0 };

        double[] Observ = { 22, 24, 24, 25, 24, 26, 21, 26, };

        //double[] Observ = { 25, 26 };

        public List<double> GetKalMan(List<double> IN)
        {
            List<double> outList = new List<double>();
            
            double KamanX = CanShu[0];

            double KamanP = CanShu[1];

            double KamanQ = CanShu[2];

            double KamanR = CanShu[3];

            double KamanY = CanShu[4];

            double KamanKg = CanShu[5];

            double KamanSum = CanShu[6];

            for (int i = 0; i < IN.Count; i++)
            {

                KamanY = KamanX;

                KamanP = KamanP + KamanQ;

                KamanKg = KamanP / (KamanP + KamanR);

                KamanX = (KamanY + KamanKg * ( IN[i] - KamanY));

                KamanSum += KamanX;

                outList.Add(KamanX);

               //this.richTextBox1.Text += KamanX.ToString() + "\n";

                KamanP = (1 - KamanKg) * KamanP;



            }

           // Average = KamanSum / Observe.Length;

            return outList;

        }

    }
}
