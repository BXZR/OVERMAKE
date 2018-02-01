using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace socketServer.Codes
{
    //计算器操作类
    //提供静态计算方法
    class MathCanculate
    {

        //计算两个GPS信号之间的距离
        //单位是米
        public static  double DistanceForGPS(double long1, double lat1, double long2, double lat2)
        {
            double a, b, R;
            R = 6378137; //地球半径
            lat1 = lat1 * Math.PI / 180.0;
            lat2 = lat2 * Math.PI / 180.0;
            a = lat1 - lat2;
            b = (long1 - long2) * Math.PI / 180.0;
            double d;
            double sa2, sb2;
            sa2 = Math.Sin(a / 2.0);
            sb2 = Math.Sin(b / 2.0);
            d = 2 * R * Math.Asin(Math.Sqrt(sa2 * sa2 + Math.Cos(lat1) * Math.Cos(lat2) * sb2 * sb2));
            return d;
        }


        // 角度转弧度 π/180×角度
        public static double getRadianFromDegree(double degree)
        {
            return degree * Math.PI / 180;
        }
        // 弧度变角度 180/π×弧度
        public static double getDegreeFromRadian(double radian)
        {
            return radian * 180 / Math.PI;
        }

        //计算平均数
        public static double getAverage(List<double> values)
        {
            if (values.Count == 0)
                return 0;

            double average = 0;
            for (int i = 0; i < values.Count; i++)
                average += values[i];
            return average / values.Count;
        }
        public static double getAverage(List<double> values , int indexPre , int indexNow)
        {
            if (indexNow <= indexPre)
                return 0;


            double average = 0;
            for (int i = indexPre; i < indexNow; i++)
            {
                average += values[i];
            }
            return average /= (indexNow - indexPre);
        }
        //计算方差
        public static double getVariance(List<double> values)
        {
            double average = 0;
            for (int i = 0; i < values.Count; i++)
            {
                average += values[i];
            }
            average /= values.Count;
            //公式需要使用的参数 (为了保证清晰，分成多个循环来写)
            double VK = 0;
            for (int i = 0; i < values.Count; i++)
            {
                double minus = (values[i] - average) * (values[i] - average);
                VK += minus;

            }
            //Console.WriteLine("VK = " + VK);
            VK /= values.Count;
            return VK;
        }

        public static double getVariance(List<double> values, int indexPre, int indexNow)
        {
            if (indexNow <= indexPre)
            {
                //Console.WriteLine("pre index should be lower than now index , so swap them.");
                //return 0;//交换是一个看上去更人性化的解决方案
                int temp = indexPre;
                indexPre = indexNow;
                indexNow = temp;
            }
            double average = 0;
            for (int i = indexPre; i < indexNow; i++)
            {
                average += values[i];
            }
            average /= (indexNow - indexPre);
            //公式需要使用的参数 (为了保证清晰，分成多个循环来写)
            double VK = 0;
            for (int i = indexPre; i < indexNow; i++)
            {
                double minus = (values[i] - average) * (values[i] - average);
                VK += minus;

            }
            //Console.WriteLine("VK = " + VK);
            VK /= (indexNow - indexPre);
            return VK;
        }

        //排序
        public static List<double> SortValues(List<double> theP)
        {
            quickSort(theP, 0, theP.Count-1);
            return theP;
        }

       private static  void quickSort(List<double> theP, int low, int high)
        {
            if (low >= high)
                return;

            int first = low;
            int last = high;
            double keyValue = theP[low];
            while (low < high)
            {
                while (low < high && theP[high]>= keyValue)
                    high--;
                theP[low] = theP[high];
                while (low < high && theP[low] <= keyValue)
                    low++;
                theP[high] = theP[low];
            }
            theP[low] = keyValue;
            quickSort(theP, first, low - 1);
            quickSort(theP, low + 1, last);
        }

    }
}
