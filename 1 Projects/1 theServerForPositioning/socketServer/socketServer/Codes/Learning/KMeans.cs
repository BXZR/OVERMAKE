using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace socketServer.Codes.Learning
{
    class KMeansPoint
    {
        public double ax;
        public double ay;
        public double az;
        public double gx;
        public double gy;
        public double gz;
        public double AIM;
        public double distance = 998;
        public KMeansPoint (double axIn, double ayIn, double azIn, double gxIn, double gyIn, double gzIn, double AIMIN)
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

    //这个类专门用来表述KMeans的分类做法
    class KMeans
    {
        //因为哟普数据种类不全的情况，所以为了保靠，所有的数据都会按照这个方式重新排布和组织一次
        //记录所有的点
        private List<KMeansPoint> thePoints = new List<KMeansPoint>();
        private List<int> aimSave = new List<int>();
        //所有平均点
        private List<KMeansPoint> averagePoints = new List<KMeansPoint> ();

        public int getTypeWithKMeans(double ax, double ay, double az, double gx, double gy, double gz)
        {
            double distanceMain = 99999;
            int theAimType = 0;
            for (int i = 0; i < averagePoints.Count; i++)
            {
                double thedistance = getDistance(ax, ay, az, gx, gy, gz, averagePoints[i]);
                if (thedistance < distanceMain)
                {
                    distanceMain = thedistance;
                    theAimType = (int )averagePoints[i].AIM;
                }
            }
            return theAimType;
        }


        //获得K近邻的距离
        private double getDistance(double ax, double ay, double az, double gx, double gy, double gz, KMeansPoint thePoint)
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

        public void builtKMeans(string path, TypeCheckClass AIMCheckClass)
        {
            if (string.IsNullOrEmpty(path))
                return;

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
                    int aim = 0;

                    //------------------------------------分支--------------------------------------------------//
                    switch (AIMCheckClass)
                    {
                        //分类步长
                        case TypeCheckClass.StepLength: {  aim = (int)Convert.ToDouble(rows[15]); }break;
                        //分类Z轴移动状态
                        case TypeCheckClass.ZMove: { aim = (int)Convert.ToDouble(rows[16]); } break;
                        //分类这一步是不是真的存在
                        case TypeCheckClass.StepType: { aim = (int)Convert.ToDouble(rows[17]); } break;
                    }
                    //------------------------------------------------------------------------------------------//

                    KMeansPoint thePoint = new KMeansPoint(ax, ay, az, gx, gy, gz, aim);
                    thePoints.Add(thePoint);
                    if (aimSave.Contains(aim) == false)
                        aimSave.Add(aim);
                }
                catch
                {
                    //这只是权宜之计
                    continue;//这一行被放弃
                }
            }
            canculateAverage();
        }


        //计算各种类型的平均点
        private void canculateAverage()
        {
            averagePoints = new List<Learning.KMeansPoint>();
            for (int i = 0; i < aimSave.Count; i++)
            {
                double ax = 0;
                double ay = 0;
                double az = 0;
                double gx = 0;
                double gy = 0;
                double gz = 0;
                int count = 0;//count不会成为0
                for (int j = 0; j < thePoints.Count; j++)
                {
                    if (thePoints[j].AIM == aimSave[i])
                    {
                        count++;
                        ax += thePoints[j].ax;
                        ay += thePoints[j].ay;
                        az += thePoints[j].az;
                        gx += thePoints[j].gy;
                        gy += thePoints[j].gy;
                        gz += thePoints[j].gz;
                    }
                }
                KMeansPoint theAveragePoint = new KMeansPoint(ax/count, ay/count,az/count,gx/count,gy/count,gz/count,aimSave[i]);
                averagePoints.Add(theAveragePoint);
           }
        }
    }
}
