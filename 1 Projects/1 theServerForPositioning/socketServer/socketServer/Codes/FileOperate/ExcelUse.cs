using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace socketServer.Codes.FileOperate
{
    //这个类专门用于导出生excel数据
    class ExcelUse
    {
        //用的是最土鳖的方法，IO流方法
        //注意用"\t"分隔
        //获得informationController中所有的数据
        //这个最好是打上保存用的时间戳，每一次保存都是一个文件
        //数据表项可能就会有重复，但是这是故意的保护
        public string  getExcelString(information theInformationController)
        {
            StringBuilder strbu = new StringBuilder();
            //写入标题
            strbu.Append("accelerometerX" + "\t");
            strbu.Append("accelerometerY" + "\t");
            strbu.Append("accelerometerZ" + "\t");
            strbu.Append("gyroX" + "\t");
            strbu.Append("gyroY" + "\t");
            strbu.Append("gyroZ" + "\t");
            strbu.Append("magnetometerX" + "\t");
            strbu.Append("magnetometerY" + "\t");
            strbu.Append("magnetometerZ" + "\t");
            strbu.Append("GPS(AxisX)" + "\t");
            strbu.Append("GPS(AxisY)" + "\t");
            strbu.Append("timeStamp" + "\t");
            //加入换行字符串
            strbu.Append(Environment.NewLine);

            //写入内容
            for(int i = 0; i < theInformationController.accelerometerX.Count; i++)
            {
                string dataClip = "";
                dataClip += theInformationController.accelerometerX[i] + "\t";
                dataClip += theInformationController.accelerometerY[i] + "\t";
                dataClip += theInformationController.accelerometerZ[i] + "\t";
                dataClip += theInformationController.gyroX[i] + "\t";
                dataClip += theInformationController.gyroY[i] + "\t";
                dataClip += theInformationController.gyroZ[i] + "\t";
                dataClip += theInformationController.magnetometerX[i] + "\t";
                dataClip += theInformationController.magnetometerY[i] + "\t";
                dataClip += theInformationController.magnetometerZ[i] + "\t";
                dataClip += theInformationController.GPSPositionX[i] + "\t";
                dataClip += theInformationController.GPSPositionY[i] + "\t";
                dataClip += theInformationController.timeStep[i] + "\t";
                strbu.Append(dataClip);
                strbu.Append(Environment.NewLine);
            }
            return strbu.ToString();
        }
    }
}
