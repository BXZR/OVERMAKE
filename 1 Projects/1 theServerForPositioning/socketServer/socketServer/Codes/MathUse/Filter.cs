using socketServer.Codes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace socketServer
{
    //这个类专门用于处理滤波过程
    //处理的是用于步态检测用的y轴加速度
   public  class Filter
    {

        //每一种方法的简短说明信息
        private string[] methodInformations =
        {
            "不进行滤波",
            "一阶平均+卡尔曼滤波",
            "卡尔曼滤波",
            "巴特沃斯滤波",

        };

        public string getInformation(int index)
        {
            return methodInformations[index];
        }


        //滤波方法对不同的数据类型可以有同名方法
        //对外平滑方法
        public List <double> theFilerWork(List<double> IN , float  theValueUse = 0.4f)
    {
            // for (int i = 0; i < IN .Count; i++)
             //    Console.WriteLine("++++++-" + IN[i]);

            List<double> outList = new List<double> ();

            switch (SystemSave.FilterMode)
            {
                case 0:{ outList = IN;} break;
                case 1:
                    {
                        for (int i = 0; i < IN.Count; i++)
                            outList.Add(IN[i]);

                        outList = theFliterMethodAverage(outList ,SystemSave.filterSmoothCount);
                        //outList = GetKalMan(outList);
                    }
                    break;
                case 2:
                    {
                        for (int i = 0; i < IN.Count; i++)
                            outList.Add(IN[i]);

                        //outList = theFliterMethod1(outList, theValueUse);
                        outList = theFliterMethodAverage(outList ,SystemSave.filterSmoothCount);
                        outList = GetKalMan(outList);
                        //outList = ButterworthFilter(outList);
                    }
                    break;
                case 3:
                    {
                        //Console.WriteLine("filter method3 ");
                        for (int i = 0; i < IN.Count; i++)
                            outList.Add(IN[i]);

                       // outList = theFliterMethod1(outList, theValueUse);
                        //倒叙滤波的效果似乎更好一点，但是更加基于贪心的做法
                        outList = theFliterMethodAverage(outList, SystemSave.filterSmoothCount);
                        //outList = GetKalMan(outList);
                        outList = ButterworthFilter(outList);
                    }
                    break;
            }

            // for (int i = 0; i < outList.Count; i++)
            //     Console.WriteLine("-----"+outList[i]);

            return outList;
    }

        //最简单粗暴的滤波接口
        public List<double> theFilerWorkSample(List<double> IN)
        {
            List<double> outList = new List<double>();
            for (int i = 0; i < IN.Count; i++)
                outList.Add(IN[i]);
            outList = theFliterMethodAverage(outList, SystemSave.filterSmoothCount);
            for(int i =0; i< outList.Count;i++)
            {
                if (Math.Abs(outList[i]) < 0.01)
                    outList[i] = 0;
            }
            return outList;
        }

        //对外平滑方法
        //对时间做复杂的滤波会出问题，暂时先用简单的方略
        public List<long> theFilerWork(List<long> IN, float theValueUse = 0.4f)
        {
           // for (int i = 0; i < IN .Count; i++)
           //     Console.WriteLine("++++++-" + IN[i]);

            List<long> outList = new List<long>();

            switch (SystemSave.FilterMode)
            {
                case 0: { outList = IN; } break;
                case 1:
                    {
                        for (int i = 0; i < IN.Count; i++)
                            outList.Add(IN[i]);

                        outList = theFliterMethodAverage(outList, SystemSave.filterSmoothCount);
                    }
                    break;
                case 2:
                    {
                        for (int i = 0; i < IN.Count; i++)
                            outList.Add(IN[i]);

                       // outList = theFliterMethod1(outList, theValueUse);
                        outList = theFliterMethodAverage(outList, SystemSave.filterSmoothCount);
                    }
                    break;
                case 3:
                    {
                        //Console.WriteLine("filter method3 ");
                        for (int i = 0; i < IN.Count; i++)
                            outList.Add(IN[i]);

                        outList = theFliterMethod1(outList, theValueUse);
                        //倒叙滤波的效果似乎更好一点，但是更加基于贪心的做法
                        outList = theFliterMethodAverage(outList, SystemSave.filterSmoothCount);
                    }
                    break;
            }

          // for (int i = 0; i < outList.Count; i++)
          //     Console.WriteLine("-----"+outList[i]);
            return outList;
        }




        //-------------------------------------------------------------------------------//
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

        //之前的数据占40%的影响，这在step滤波的时候很有用，但是在角度的时候确实不好
        private List<double> theFliterMethod1(List<double> IN , float theValueA = 0.4f)
        {
            for (int i = 1; i < IN.Count; i++)
            {
                IN[i] = IN[i] * (1 - theValueA) + theValueA * IN[i - 1];
            }
             return IN;
        }
        private List<long> theFliterMethod1(List<long> IN, float theValueA = 0.4f)
        {
            for (int i = 1; i < IN.Count; i++)
            {
                IN[i] =(long)( IN[i] * (1 - theValueA) + theValueA * IN[i - 1]);
            }
            return IN;
        }

        //滤波方法2----------------------------------------------------------------------------------------------
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

        //int smoothCount = 5;//平滑的方法（用几个数据的平均数来代替这几个数据）
        private List<double> theFliterMethodAverage(List<double> IN, int smoothCount = 5)
        {
            List<double> OUT = new List<double>();
            int countUse = 0;//计数器，为了明显用1作为开头了
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
        //对于时间戳来说貌似没有必要狠狠滤波，做一次平均就可以了
        private List<long> theFliterMethodAverage(List<long> IN, int smoothCount = 5)
        {
            List<long> OUT = new List<long>();
            int countUse = 0;//计数器
            long numPlus = 0;//这几个数的总和
            for (int i = 1; i < IN.Count; i++)
            {
                countUse++;
                numPlus += IN[i];
                if (countUse == smoothCount || (i == IN.Count - 1 && countUse != 0))//到了采样的时候了
                {
                    OUT.Add(numPlus / countUse);
                    numPlus = 0;
                    countUse = 0;
                }
            }
            return OUT;
        }

        //平均方法的倒叙的滤波，每一次新加一个数据，就用新数据来算平均，同时放掉一个旧数据，也就是所谓的倒叙的滤波
        private List<double> theFliterMethodAverageRevert(List<double> IN, int smoothCount = 5)
        {
            List<double> OUT = new List<double>();
            int countUse = 0;//计数器，为了明显用1作为开头了
            double numPlus = 0;//这几个数的总和
            for (int i = IN.Count ; i >= 0 ; i--)
            {
                countUse++;
                numPlus += IN[i];
                if (countUse == smoothCount || (i == 0 && countUse != 0))//到了采样的时候了
                {
                    OUT.Add(numPlus / countUse);
                    numPlus = 0;
                    countUse = 0;
                }
            }
            return OUT;
        }
        //平均方法的倒叙的滤波，每一次新加一个数据，就用新数据来算平均，同时放掉一个旧数据，也就是所谓的倒叙的滤波
        //对于时间戳来说貌似没有必要狠狠滤波，做一次平均就可以了
        private List<long> theFliterMethodAverageRevert(List<long> IN, int smoothCount = 5)
        {
            List<long> OUT = new List<long>();
            int countUse = 0;//计数器，为了明显用1作为开头了
            long numPlus = 0;//这几个数的总和
            for (int i = IN.Count; i >= 0; i--)
            {
                countUse++;
                numPlus += IN[i];
                if (countUse == smoothCount || (i == 0 && countUse != 0))//到了采样的时候了
                {
                    OUT.Add(numPlus / countUse);
                    numPlus = 0;
                    countUse = 0;
                }
            }
            return OUT;
        }

        //滤波方法3，究极的卡尔曼滤波
        //http://blog.sciencenet.cn/blog-1060307-727434.html
        double[] CanShu = { 2, -1, 2, 2, 1, 0, 0, 0 };
        //double[] Observ = { 22, 24, 24, 25, 24, 26, 21, 26, };
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
        long[] CanShuLong = { 23, 9, 16, 16, 1, 0, 0, 0 };
        long[] ObservLong = { 22, 24, 24, 25, 24, 26, 21, 26, };
        //double[] Observ = { 25, 26 };
        public List<long> GetKalMan(List<long> IN)
        {
            List<long> outList = new List<long>();
            double KamanX = CanShuLong[0];
            double KamanP = CanShuLong[1];
            double KamanQ = CanShuLong[2];
            double KamanR = CanShuLong[3];
            double KamanY = CanShuLong[4];
            double KamanKg = CanShuLong[5];
            double KamanSum = CanShuLong[6];
            for (int i = 0; i < IN.Count; i++)
            {
                KamanY = KamanX;
                KamanP = KamanP + KamanQ;
                KamanKg = KamanP / (KamanP + KamanR);
                KamanX = (KamanY + KamanKg * (IN[i] - KamanY));
                KamanSum += KamanX;
                outList.Add((long)KamanX);
                //this.richTextBox1.Text += KamanX.ToString() + "\n";
                KamanP = (1 - KamanKg) * KamanP;
            }
            // Average = KamanSum / Observe.Length;
            return outList;
        }


        //----------------------巴特沃斯滤波----------------------------//

        double[] InSave = new double[] { 0, 0 };
        double[] OutSave = new double[] { 0, 0 };
        double[] BWeights = new double[] { 0, 0, 0 };
        double[] AWeights = new double[] { 0 , 0 }; 
        public List<double> ButterworthFilter(List<double> In)
        {
            //制作公式参数
            makeButterworthWeights(20, 30);
            //制作数据副本
            List<double> outList = new List<double>();
            //处理数据
            for (int i = 0; i < In.Count; i++)
            {
                double value = BWeights[0] * In[i] + BWeights[1] * InSave[1] + BWeights[2] * InSave[0] - AWeights[0] * OutSave[1] - AWeights[1] * OutSave[0];

                //输入序列保存 
                InSave[0] = InSave[1];
                InSave[1] = In[i];
                //输出序列保存
                OutSave[0] = OutSave[1];
                OutSave[1] =  value;

                outList.Add(value);
               // Console.WriteLine(string.Format("In = {0} Out = {1}" , In[i] , value));
            }
            return outList;
        }
        public List<long> ButterworthFilter(List<long> In)
        {
            //制作公式参数
            makeButterworthWeights(20, 30);
            //制作数据副本
            List<long> outList = new List<long>();
            //处理数据
            for (int i = 0; i < In.Count; i++)
            {
                long value =(long)( BWeights[0] * In[i] + BWeights[1] * InSave[1] + BWeights[2] * InSave[0] - AWeights[0] * OutSave[1] - AWeights[1] * OutSave[0]);

                //输入序列保存 
                InSave[0] = InSave[1];
                InSave[1] = In[i];
                //输出序列保存
                OutSave[0] = OutSave[1];
                OutSave[1] = value;

                outList.Add(value);
                // Console.WriteLine(string.Format("In = {0} Out = {1}" , In[i] , value));
            }
            return outList;
        }

        private bool madeButterworthWeights = false;
        public void makeButterworthWeights(float sample_freq, float cutoff_freq)
        {
            if (madeButterworthWeights == false)
            {
                BWeights = new double[] { 0, 0, 0 };
                AWeights = new double[] { 0, 0 };

                try
                {
                    float fr = 0;
                    float ohm = 0;
                    float c = 0;

                    fr = sample_freq / cutoff_freq;
                    ohm = (float)Math.Tan((float)Math.PI / fr);
                    c = 1.0f + 2.0f * (float)Math.Cos((float)Math.PI / 4.0f) * ohm + ohm * ohm;

                    BWeights[0] = ohm * ohm / c;
                    BWeights[1] = 2.0f * BWeights[0];
                    BWeights[2] = BWeights[0];
                    AWeights[0] = 2.0f * (ohm * ohm - 1.0f) / c;
                    AWeights[1] = (1.0f - 2.0f * (float)Math.Cos((float)Math.PI / 4.0f) * ohm + ohm * ohm) / c;
                    madeButterworthWeights = true;

                   // Console.WriteLine(string.Format("B1 = {0} B2 = {1} B3 = {2} A1 = {3} A2 = {4}", BWeights[0], BWeights[1], BWeights[2], AWeights[0], AWeights[1]));
                }
                catch (Exception W)
                {
                    Console.WriteLine(W.Message);
                }
            }
        }

    }
}
