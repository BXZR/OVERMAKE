using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace socketServer.Codes.Positioning
{
    //这个类专门用来处理GPS的信息
    //如果能够获得GPS信号并且做相关判别
    //在有GPS信号的时候能够判断GPS信号的效果
    //GPS能用就混合定位
   public  class GPSPosition
    {
        private List<GPS> theGPSPositions = new List<GPS>();

        //计算初始的GPS信号信息
        public void  makeGPSPosition( List<int> indexBuff , information theInformationController , Filter theFilter )
        {
            theGPSPositions = new List<GPS>();
            List<double> filteredGPSX = theFilter.theFilerWork( theInformationController.GPSPositionX);
            List<double> filteredGPSY = theFilter.theFilerWork( theInformationController.GPSPositionY);
            for (int i = 0; i < indexBuff.Count; i++)
            {
                theGPSPositions.Add(new Positioning.GPS(filteredGPSX[indexBuff[i]] , filteredGPSY[indexBuff[i]] ));
            }
        }

        //信息比对和矫正当前的GPS信息
        public void fixPositionWithGPS(position thePositionController)
        {
            List<transForm> thetransformPositions = thePositionController.theTransformPosition;
            //最简单程度的GPS更新当前的坐标--------------------------------------------------------
            //另外，这种更新位置式XY轴平面的
            //数据太少不适合这样的计算
            if (thetransformPositions.Count < 5 || thetransformPositions.Count != theGPSPositions.Count)
                return;


            //开始进行修正
            fixMethod1(thePositionController);
        }


        //修正方法1
        private void fixMethod1(position thePositionController)
        {
            //首尾如如果差距不是非常大，那么尾就换成GPS尾，这样实现GPS更新当前位置
            transForm P1 = thePositionController.theTransformPosition[0];
            transForm P2 = thePositionController.theTransformPosition[thePositionController.theTransformPosition.Count - 1];
            double distance1 = (P1.X - P2.X) * (P1.X - P2.X) + (P1.Y - P2.Y) * (P1.Y - P2.Y);

            double P2x = (theGPSPositions[0].GPSX - theGPSPositions[theGPSPositions.Count - 1].GPSX);
            double P2y = (theGPSPositions[0].GPSY - theGPSPositions[theGPSPositions.Count - 1].GPSY);
            double distance2 = P2x * P2x + P2y * P2y;
            //距离低于门限，用GPS最后一个信号更新最后一个transform的坐标
            if (Math.Abs(distance1 - distance2) < 2)
            {
                //真正的更新
                //这里更新的不是真实坐标，因为PDR全部都是相对坐标
                //这里用的是最终相对位移叠加到初试位置来做的
                P2.X = P1.X + P2x;
                P2.Y = P1.Y + P2y;
            }
        }
    }

    public class GPS
    {
        public double GPSX = 0;
        public double GPSY = 0;

        public GPS(double x, double y)
        {
            GPSX = x;
            GPSY = y;
        }
        public string getGPSPositionInformation()
        {
            return "GPS: (" + GPSX +"," +GPSY + ")";
        }
    }
}
