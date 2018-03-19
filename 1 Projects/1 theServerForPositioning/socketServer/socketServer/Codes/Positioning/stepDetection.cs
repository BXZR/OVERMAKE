using socketServer.Codes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace socketServer
{
   //这个类介绍静态的一些额外的判断走了一步的方法
   //系统默认使用的是使用PeachSearcher做的判断方法
   //这个类的方法用于效果的对比，并且所有的内容都是静态（不收管理管理器约束并且随时更换）
   
   //这个类必须要有的：下标缓冲
    class stepDetection
    {
        public  List<int> peackBuff = new List<int>();//走了一步时候的下标，用于获取其他集合的数据，必要

        //来自一篇论文
        //主要思想是检查重复元素的信息，如果信息被认为是重复的，就认为走了一步
        //-----------------------这个方法的难点:对第一步的采样，判断是否匹配------------------------//

        private  double minusGate = 0.4;//如果数据差异百分比超过10%就认为数据是不一样的
        private  int countBetweenTwoStep = 3;//两步之间最少的数据量

        public  bool isSampled = false;//是否已经采样完毕
        public List<double> sample = new List<double>();//被采集的样本（波峰检测单元做第一个波形的检测）

        private int dataDropCount = 1;//前几个数据不要了
        private int indexForSTART= 0;//记录的初始的波峰的下标

        //每一种方法的简短说明信息
        private string[] methodInformations =
        {
            "一段时间内出现唯一的波峰意味着一步的发生",
            "限制波峰波谷差值的波峰波谷判步方法",
            "在一个峰值的限定内两次经过峰值判断走出一步",
            "对第一步进行采样，从而对后续数据进行匹配",
            "根据数据连续两次的经过零点来判断走出一步"
        };

        private void makeSample(List<double> wave)
        {
            if (wave.Count < dataDropCount || isSampled )
                return; //前几个数据不要了，会有很大的误差

            sample = new List<double>();
            int stepNumber = 0;
            int direction = wave[dataDropCount] > 0 ? -1 : 1;
            for (int i = dataDropCount ; i < wave.Count - 1; i++)
            {
                double minus = wave[i + 1] - wave[i];

                //获得第一个波峰之间的数据用作样本
                if (stepNumber == 1 )
                {
                   sample.Add(wave[i]);
                }
                if (minus * direction > 0)//放弃突变的情况
                {
                    direction *= -1;
                    if (direction == 1)
                    {
                        //"波峰"
                        stepNumber++;
                        if (stepNumber == 1)
                        {
                            sample.Add(wave[i]);//初始波峰的下标也需要记录下来
                            indexForSTART = i;//重新纪录抛弃项目，这也是后续计算的开始位置
                        }
                        //判断出来一个波峰+波谷就认为结束了
                        if (stepNumber >=2)
                        {
                            Console.WriteLine("sample over");
                            countBetweenTwoStep = sample.Count;

                            for (int y = 0; y < sample.Count; y++)
                                Console.WriteLine("采样结果" + sample[y]);

                            //采样图像显示
                            //ChartWindow sampleShow = new socketServer.ChartWindow();
                            //sampleShow.Show();
                            //sampleShow.CreateChartSpline(UseDataType.accelerometerZ, sample);
                            //Console.WriteLine("countBetweenTwoStep: " + countBetweenTwoStep);
                            isSampled = true;
                            break;
                        }
                    }
                    else
                    {
                        //"波谷"
                    }
                }
            }
       }

        //最基本的波峰波谷的方法
        public List<int> stepDectionExtration0(List<double> AZValues, PeackSearcher PK)
        {
            int stepCount = PK.countStepWithStatic(AZValues);
            return PK.peackBuff;
        }

        public List<int> stepDectionExtration3(List<double> AZValues, PeackSearcher PK)
        {
            int stepCount = PK.coutStepWithStatic2(AZValues);
            return PK.peackBuff;
        }
        //带波峰波谷距离阀值的方法
        public List<int> stepDectionExtration4(List<double> AZValues, PeackSearcher PK)
        {
            int stepCount = PK.countStepWithStatic3(AZValues);
            return PK.peackBuff;
        }

        //采样波峰波谷的方法（说实话这是一种非常理想方法但是实际中使用还需要花很长时间磨合）
        public  List<int> stepDetectionExtra1(List<double> AZValues)
        {

            if (isSampled == false)
            {
                makeSample(AZValues);
                return new List<int>();
            }
            else
            {
               // Console.WriteLine("第一个波峰的位置：" + dataDropCount);
                peackBuff = new List<int>();//记录下标的位置
                for (int i = indexForSTART; i < AZValues.Count; i+= countBetweenTwoStep/2)
                {
                    if ((i + countBetweenTwoStep) > AZValues.Count)
                    {
                        //数据不够就不进行计算了
                        //数据采集完成就可以继续计算
                        break;
                    }
                    List<double> data1 = new List<double>();
                    for (int j = i; j < i + countBetweenTwoStep; j++)
                    {
                        //Console.WriteLine("AZ: " + AZValues[j]);
                        data1.Add(AZValues[j]);
                    }
                    if (contrast2(data1, sample))
                    {
                       // Console.WriteLine("---------" + peackBuff.Count);
                       // Console.WriteLine("判断走了一步");
                        peackBuff.Add(i);  
                    }
                }
                return peackBuff;
            }
        }

        //0cross方法，零点交叉
        //SystemSave.zeroCrossOffset记录零点信息
        //这个方法超级灵敏，可能会产生各种各样的额外步数
        //可以考虑通过修改SystemSave.zeroCrossOffset把水平线数值修改，可以有不同的效果
        public List<int> stepDetectionExtra2(List<double> AZValues)
        {
            List<int> buff = new List<int>();
            int overCount = 0;//过零点的次数
           // Console.WriteLine("--------------------------------------");
            //因为震荡的关系需要做一下间隔，一小段时间内只可能走一步
            int savedIndex = -1;//及记录的当前下标
            int steps = 3;//间隔这些数据才有可能被称为一步

            for (int i = 1 ;i < AZValues.Count; i++)
            {
                //Console.WriteLine(AZValues [i] +" is the AZValue");
                double checkUseValue = (AZValues[i - 1] - SystemSave.zeroCrossOffset) * (AZValues[i] - SystemSave.zeroCrossOffset);
                //Console.WriteLine("checkUse is "+ checkUseValue);
                if (checkUseValue < 0 )//变号就算是过零点
                    overCount++;
                if (overCount > 0 && overCount % 2 == 0)
                {
                    if ((i - savedIndex) > steps)
                    {
                        savedIndex = i;
                        buff.Add(i);
                    }
                }
                //Console.WriteLine("OverCount: " + overCount);
            }
            return buff;
        }


        bool showed = false;
        //对比两个序列的相似程度方法1（最土鳖的数值方法）
        //返回真则认为两组数据差不多
       private bool contrast1(List<double> data1, List<double> data2)
        {
            int sameCount = 0;
            for(int i = 0; i < data1.Count; i++)
            {
                //在这里减少1是因为实验用的手机传感器存在偏差1
                if ( Math.Abs(  Math.Abs(data1[i])-1 ) < 0.01 || Math.Abs(data1[i]) < 0.01)
                    continue;

                    double checkUp = Math.Abs(data1[i] - data2[i]) ;
                    double checkValue = Math.Abs(checkUp / (Math.Abs(data2[i])));
                   // Console.WriteLine("Value --- "+data1[i] +" --- "+ data2[i] +" --- ");
                   // Console.WriteLine("checkValue = " + checkValue);
                    if (checkValue > minusGate)
                    sameCount++;
            }
            //Console.WriteLine("sameCount2  = " + sameCount);
            if (sameCount >= countBetweenTwoStep / 2)
            {
                if (showed == false)
                { 
                ChartWindow show = new socketServer.ChartWindow();
                show.Show();
                show.CreateChartSpline2(UseDataType.accelerometerZ, data1, data2);
                    showed = true;
                }
                return true;
            }
            return false;
        }

        //对比两个序列的相似程度方法2（皮尔逊相关系数的方法）
        //返回真则认为两组数据差不多
        /*
            0.8-1.0     极强相关
            0.6-0.8     强相关
            0.4-0.6     中等程度相关
            0.2-0.4     弱相关
            0.0-0.2     极弱相关或无相关 
         */
        private int showPWindowCount = 2;//匹配成功显示的曲线窗口数量
        bool contrast2(List<double> data1, List<double> data2)
        {
            if (data1.Count == 0)
                return false;

            double dataAverage1 = MathCanculate.getAverage(data1);//数据的平均数
            double dataAverage2 = MathCanculate.getAverage(data2);//数据的平均数2

            //分成多段来写，保持清晰
            double up = 0;//分子
            double  data1Down = 0;//分母计算因子
            double data2Down = 0;//分母计算因子
            for (int i = 0; i < data1.Count; i++)
            {
                up += (data1[i] - dataAverage1) * (data2[i] - dataAverage2);
                data1Down += (data1[i] - dataAverage1) * (data1[i] - dataAverage1);
                data2Down += (data2[i] - dataAverage2) * (data2[i] - dataAverage2);
            }
            double down = Math .Sqrt(  data1Down * data2Down);
           // Console.WriteLine("data1Down = "+ data1Down / data1.Count);
            if (down == 0 ||  ( data1Down/data1 .Count) < 0.07 )//不可比较的话统统返回false
                return false;

            double pearsonValue = up / down;
           // Console.WriteLine("pearsonValue = " + pearsonValue);
            if (Math.Abs( pearsonValue)  > 0.65)//一个折中的小数值（数值要求并不是很严格）
            {
                //if (showPWindowCount >0 )
                //{
                //    ChartWindow show = new socketServer.ChartWindow();
                //    show.Show();
                //    show.CreateChartSpline2(UseDataType.accelerometerZ, data1, sample);
                //    showPWindowCount--;
                //}
                return true;
            }
            return false;
        }


        //对比两个字符串的相似度
        private static decimal getsimilaritywith(string sourcestring, string str)
        {
            decimal kq = 2;
            decimal kr = 1;
            decimal ks = 1;

            char[] ss = sourcestring.ToCharArray();
            char[] st = str.ToCharArray();

            //获取交集数量
            int q = ss.Intersect(st).Count();
            int s = ss.Length - q;
            int r = st.Length - q;

            return kq * q / (kq * q + kr * r + ks * s);
        }

        //检测这个移动是不是真的移动，也就是说在原地晃手机的时候是否允许被判断走了一步
        //在实验的时候原地晃手机是可以的，但是在实际使用的时候原地晃手机不可以这样，者可以通过一个模式进行判断
        public List<int> FixedStepCalculate(information theInformationController, Filter theFilter , List<int> indexBuff)
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
                Console.WriteLine(Variances[1]);
                if (Variances[1] < gate)
                    toRemove.Add(indexBuff[i]);
            }
            for (int i = 0; i < toRemove.Count; i++)
            {
                indexBuff.Remove(toRemove[i]);
            }
            //Console.WriteLine("indexBuff Count after= " + indexBuff.Count);
            return  indexBuff;
        }

        //这个类的统一外部刷新接口，因为有分组的存在，这个是非常必要的
        public  void makeFlash()
        {
            peackBuff.Clear();
        }

        //返回对这种方法的说明
        public string getMoreInformation(int index)
        {
            return methodInformations[index];
        }
        //返回全部的方法说明
        public string[] getMoreInformation()
        {
            return methodInformations;
        }
    }
}
