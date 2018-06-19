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
        


    }


    public class GPS
    {
        public float GPSX = 0;
        public float GPSY = 0;

        public string getGPSPositionInformation()
        {
            return "GPS: (" + GPSX +"," +GPSY + ")";
        }
    }
}
