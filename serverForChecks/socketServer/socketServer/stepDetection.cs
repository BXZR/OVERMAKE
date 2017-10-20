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
        //-----------------------这个方法最大的难点就是对第一步的采样------------------------//
        private  int indexForStep = 3;//记录下一个数值的坐标，每一次从上一步开始比对就可以了
        //此外还表示1条的数据被抛弃了
        private  double minusGate = 0.4;//如果数据差异百分比超过10%就认为数据是不一样的
        private  int countBetweenTwoStep = 3;//两步之间最少的数据量

        public  bool isSampled = false;//是否已经采样完毕
        public List<double> sample = new List<double>();//被采集的样本（波峰检测单元做第一个波形的检测）

        private int dataDropCount = 5;//前几个数据不要了

        private void makeSample(List<double> wave)
        {
            sample = new List<double>();
            if (wave.Count < dataDropCount)
                return; //前几个数据不要了，会有很大的误差

            int stepNumber = 0;
            int direction = wave[dataDropCount] > 0 ? -1 : 1;
            for (int i = 3; i < wave.Count - 1; i++)
            {
                double minus = wave[i + 1] - wave[i];
                sample.Add(wave[i]);
                if (minus * direction > 0)//放弃突变的情况
                {
                    direction *= -1;
                    if (direction == 1)
                    {
                        //"波峰"
                        stepNumber++;

                        //判断出来一个波峰+波谷就认为结束了
                        if (stepNumber >=2)
                        {
                            isSampled = true;
                            Console.WriteLine("sample over");
                            countBetweenTwoStep = sample.Count;

                            for (int y = 0; y < sample.Count; y++)
                                Console.WriteLine("采样结果" + sample[y]);


                            Console.WriteLine("countBetweenTwoStep: " + countBetweenTwoStep);
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

        public  void  stepDetectionExtra1(List<double> AZValues)
        {

            if (isSampled == false)
            {
                makeSample(AZValues);
            }
            else
            {
                //int i = AZValues.Count - countBetweenTwoStep;
                //if (i <= indexForStep)
                //    return;

                ////包装到两个队列中使用各种方法来判断相似程度
                //List<double> data1 = new List<double>();
                ////固定样本，并且原本样本可以用data1替换（时机合适的话）
                //for ( ;  i < AZValues.Count; i++)
                //{
                //    data1.Add(AZValues[i]);
                //}


                //if (contrast1(data1 , sample))
                //{
                //    Console.WriteLine("判断走了一步");
                //    peackBuff.Add(AZValues.Count - 1);
                //    indexForStep = AZValues.Count;//做一个缓冲标记
                //}

                peackBuff = new List<int>();
                for (int i = dataDropCount; i < AZValues.Count; i+= countBetweenTwoStep)
                {
                    if ((i + countBetweenTwoStep) > AZValues.Count)
                    {
                        break;//数据不够就不进行计算了
                    }
                    List<double> data1 = new List<double>();
                    for (int j = i; j < i + countBetweenTwoStep; j++)
                    {
                        data1.Add(AZValues[j]);
                    }
                    if (contrast1(data1, sample))
                    {
                        Console.WriteLine("判断走了一步");
                        peackBuff.Add(i);
                        indexForStep = AZValues.Count;//做一个缓冲标记
                    }
                }
            }
        }

        //对比两个序列的相似程度方法1（最土鳖的数值方法）
        //返回真则认为两组数据差不多
        bool contrast1(List<double> data1, List<double> data2)
        {
            int sameCount = 0;
            for(int i = 0; i < data1.Count; i++)
            {
                if ( Math.Abs(  Math.Abs(data1[i])-1 ) < 0.01 || Math.Abs(data1[i]) < 0.01)
                    continue;
                //为了预防除零异常，分子分母分别加上了一个1
                    double checkUp = Math.Abs(Math.Abs(data1[i]) - Math.Abs(data2[i])) ;
                    double checkValue = Math.Abs(checkUp / (Math.Abs(data2[i])));
                     Console.WriteLine("Value --- "+Math.Abs(data1[i]) +" --- "+ Math.Abs(data2[i]));
                    Console.WriteLine("checkValue = " + checkValue);
                    if (checkValue > minusGate)
                    sameCount++;
            }
            Console.WriteLine("sameCount2  = " + sameCount);
            if (sameCount >= countBetweenTwoStep / 2)
                return true;
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

        //这个类的统一外部刷新接口，因为有分组的存在，这个是非常必要的
        public  void makeFlash()
        {
            indexForStep = 0;
            peackBuff.Clear();
        }

    }
}
