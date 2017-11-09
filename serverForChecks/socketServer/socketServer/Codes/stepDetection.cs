﻿using System;
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

        public  void  stepDetectionExtra1(List<double> AZValues)
        {

            if (isSampled == false)
            {
                makeSample(AZValues);
               
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
                        Console.WriteLine("---------" + peackBuff.Count);
                        Console.WriteLine("判断走了一步");
                        peackBuff.Add(i);  
                    }
                }
            }
        }

        bool showed = false;

        //对比两个序列的相似程度方法1（最土鳖的数值方法）
        //返回真则认为两组数据差不多
        bool contrast1(List<double> data1, List<double> data2)
        {
            int sameCount = 0;
            for(int i = 0; i < data1.Count; i++)
            {
                //在这里减少1是因为实验用的手机传感器存在偏差1
                if ( Math.Abs(  Math.Abs(data1[i])-1 ) < 0.01 || Math.Abs(data1[i]) < 0.01)
                    continue;

                    double checkUp = Math.Abs(data1[i] - data2[i]) ;
                    double checkValue = Math.Abs(checkUp / (Math.Abs(data2[i])));
                    Console.WriteLine("Value --- "+data1[i] +" --- "+ data2[i] +" --- ");
                    Console.WriteLine("checkValue = " + checkValue);
                    if (checkValue > minusGate)
                    sameCount++;
            }
            Console.WriteLine("sameCount2  = " + sameCount);
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

            double dataAverage1 = 0;//数据的平均数
            double dataAverage2 = 0;//数据平均数2
            for (int i = 0; i < data1.Count; i++)
            {
                dataAverage1 += data1[i];
                dataAverage2 += data2[i];
            }

            dataAverage1 /= data1.Count;
            dataAverage2 /= data2.Count;
             
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
            Console.WriteLine("data1Down = "+ data1Down / data1.Count);
            if (down == 0 ||  ( data1Down/data1 .Count) < 0.07 )//不可比较的话统统返回false
                return false;

            double pearsonValue = up / down;
            Console.WriteLine("pearsonValue = " + pearsonValue);
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

        //这个类的统一外部刷新接口，因为有分组的存在，这个是非常必要的
        public  void makeFlash()
        {
            peackBuff.Clear();
        }

    }
}