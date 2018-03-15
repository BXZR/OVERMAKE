using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace socketServer.Codes.Learning
{
    //描述KNN“点”的做法
    class KNNPoint
    {
        public double ax;
        public double ay;
        public double az;
        public double gx;
        public double gy;
        public double gz;
        public double AIM;
        public double distance = 9999;
        public KNNPoint(double axIn, double ayIn, double azIn, double gxIn, double gyIn, double gzIn, double AIMIN)
        {
            ax = axIn;
            ay = ayIn;
            az = azIn;
            gx = gxIn;
            gy = gyIn;
            gz = gzIn;
            AIM = AIMIN;
        }
    }
    //这个类抓门用来使用AKK进行分类
    class KNN
    {
        //K近邻的参数K的大小
        int theKForKNN = 20;
        //用来保存历史数据的一些list
        List<KNNPoint> KNNPoints;
        List<int> typesInK;

        //这个方法有两种模式可以用
        //默认，forSL true 用做步长分类
        //forSL false 用作楼梯姿态分类
        public void makeKNN(int KIn = 20, string dataPath = "" , bool forSL = true)
        {
            theKForKNN = KIn;
            getData(dataPath , forSL);
        }


        //获得实用ANN分析出来的SL(步长)Type
        public int getKNNType(double ax, double ay, double az, double gx, double gy, double gz)
        {
            //重新计算所有的数据距离
            foreach (KNNPoint thePoint in KNNPoints)
                thePoint.distance = getDistance(ax, ay, az, gx, gy, gz, thePoint);
            //重新排序
            sortThePoints(KNNPoints, 0, KNNPoints.Count - 1);
            getTypesInK();
            return getMaxCountType(typesInK);
        }


        //获取int List里面出现次数最多的type
        int getMaxCountType(List<int> theTypes)
        {
            List<int> counts = new List<int>();
            for(int i = 0; i< SystemSave.CommonFormulaWeights.Count; i++)
                counts.Add(0);
            for (int i = 0; i < theTypes.Count; i++)
                counts[theTypes[i]]++;
            int maxCount = -999;
            int maxCountTypeIndex = 0;
            for (int i = 0; i < counts.Count; i++)
            {
                if (maxCount < counts[i])
                {
                    maxCount = counts[i];
                    maxCountTypeIndex = i;
                }
            }
            return maxCountTypeIndex;

        }

        //获得最近的K个数据的类型
        void getTypesInK()
        {
            typesInK = new List<int>();
            for (int i = 0; i < theKForKNN; i++)
            {
                typesInK.Add(SystemSave.getTypeIndex(KNNPoints[i].AIM));
            }
        }


        //对这些KNNPoint进行排序
        private void sortThePoints(List<KNNPoint> theP, int low, int high)
        {
            if (low >= high)
                return;

            int first = low;
            int last = high;
            KNNPoint keyValue = theP[low];
            while (low < high)
            {
                while (low < high && theP[high].distance >= keyValue.distance)
                    high--;
                theP[low] = theP[high];
                while (low < high && theP[low].distance <= keyValue.distance)
                    low++;
                theP[high] = theP[low];
            }
            theP[low] = keyValue;
            sortThePoints(theP, first, low - 1);
            sortThePoints(theP, low + 1, last);
    }


        //获得存储的数据
        //这个用于初始化就行
        private void getData(string dataPath,bool forsl = true)
        {
            if (string.IsNullOrEmpty(dataPath))
                return;

            KNNPoints = new List<Learning.KNNPoint>();
            string infotmationGet = FileSaver.readFromTrainBase();
            // string infotmationGet = new FileSaver().readInformation(dataPath);
            string[] line = infotmationGet.Split('\n');
            for (int i = 0; i < line.Length; i++)
            {
                string[] rows = line[i].Split(',');
                if (string.IsNullOrEmpty(line[i]))
                    continue;
                //一行的数据有可能无法被当做数据项使用，而且这种情况也很常见
                try
                {
                    // Console.WriteLine(rows[0] + "------" + i);
                    double ax = Convert.ToDouble(rows[0]);
                    double ay = Convert.ToDouble(rows[1]);
                    double az = Convert.ToDouble(rows[2]);
                    double gx = Convert.ToDouble(rows[3]);
                    double gy = Convert.ToDouble(rows[4]);
                    double gz = Convert.ToDouble(rows[5]);
                    double aim = 0;
                    if (forsl)
                        aim  = Convert.ToDouble(rows[15]);
                    else
                        aim = Convert.ToDouble(rows[16]);
                    KNNPoint thePoint =   new KNNPoint(ax,ay,az,gx,gy,gz,aim);
                    KNNPoints.Add(thePoint);
                }
                catch
                {
                    //这只是权宜之计
                    //AX.Add(Convert.ToDouble(0));
                    //AY.Add(Convert.ToDouble(0));
                    //AZ.Add(Convert.ToDouble(0));
                    //GX.Add(Convert.ToDouble(0));
                    //GY.Add(Convert.ToDouble(0));
                    //GZ.Add(Convert.ToDouble(0));
                    //SL.Add(Convert.ToDouble(0));
                    //SM.Add(Convert.ToInt32(0));
                    continue;//这一行被放弃
                }
            }
        }

        //获得K近邻的距离
        private double getDistance(double ax, double ay, double az, double gx, double gy, double gz, KNNPoint thePoint)
        {
            double value1 = (ax - thePoint.ax) * (ax - thePoint.ax);
            double value2 = (ay - thePoint.ay) * (ay - thePoint.ay);
            double value3 = (az - thePoint.az) * (az - thePoint.az);
            double value4 = (gx - thePoint.gx) * (gx - thePoint.gx);
            double value5 = (gy - thePoint.gy) * (gy - thePoint.gy);
            double value6 = (gz - thePoint.gz) * (gz - thePoint.gz);
            double whole = value1 + value2 + value3 + value4 + value5 + value6;
            return Math.Sqrt(whole);
        }

    }
}
