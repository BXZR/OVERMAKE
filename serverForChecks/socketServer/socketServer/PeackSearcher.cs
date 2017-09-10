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
        //返回步数
        public int countSteps(List <double> wave)
        {
            int count = 0;
            int direction = wave[0] > 0? -1:1;
          for(int i=0;i< wave .Count -1;i++) 
          {
              double minus = wave[i+1]-wave[i];
               if(minus * direction>0  )//放弃突变的情况
               {

                    direction*=-1;
                    if(direction == 1)
                    {
                        count++;
                       //"波峰"
                    } 
                    else 
                    {
                       // count++;
                       //"波谷"
                     }
              }
          }
          return count;
        }

      /*                  这个是从网上抄过来的高斯方法，需要前期处理，暂时先放在这里                               */

        //测试论文代码寻峰方法
        double Gauss(int i, int m, int H)//高斯函数
        {
            double a = i;
            double b = H;
            double c = 4 *   Math.Log((double)2) * (a / b) * (a / b);
            return    Math .Exp (-c);
        }//H为半高宽，2m+1为窗宽

        double SimilarGaussConstant(int m, int H)//类高斯常数
        {
            double sum = 0;
            for (int i = -m; i <= m; i++)
                sum += Gauss(i, m, H);
            return sum / (2 * m + 1);
        }

        double SimilarGauss(int i, int m, int H)//类高斯函数
        {
            return Gauss(i, m, H) - SimilarGaussConstant(m, H);
        }

        double SimilarGauss2(int i, int m, int H)//类高斯函数的平方
        {
            return SimilarGauss(i, m, H) * SimilarGauss(i, m, H);
        }

        double IsPossiblePeak(int j, int m, int H, double [] argA)
        {//可能峰区判断函数
            double a = 0;
            double b = 0;
            for (int i = -m; i <= m; i++)
            {
                a += SimilarGauss(i, m, H) * argA[j + i];
                b += SimilarGauss2(i, m, H) * argA[j + i];
            }
            if (b != 0)
                return a /   Math .Sqrt(b);
            else
                return 0;
        }//argA为待分析数组

      public  int PeakSearch2(int n,int m,int H,double []argA,double C)
        //n是道数
        //H是峰的半宽
        //m是起始道数
        //两参数可指定为常数
        {
            //寻峰函数
            double a=0;//argB寻峰结果数组
            int num=0;//C为预先给定的常数
            bool first=true;
            bool isP=false;
            int numb=0;
            for(int i=m;i<n-m;i++)
            {
                numb++;
                double x = IsPossiblePeak(i,m,H,argA);
                if(IsPossiblePeak(i,m,H,argA)>C)
                {
                    if(first==true)
                    {
                        first=false;
                        isP=true;
                        a=IsPossiblePeak(i,m,H,argA);
                    }
                    else
                    {
                        if(a<IsPossiblePeak(i,m,H,argA))
                        {
                            a=IsPossiblePeak(i,m,H,argA);
                        }
                    }
                }
                else
                {
                    if(isP==true)
                    {
                        first=true;
                        isP=false;
                        num++;
                    }
                }
            }
            return num;
        }

    }
}
