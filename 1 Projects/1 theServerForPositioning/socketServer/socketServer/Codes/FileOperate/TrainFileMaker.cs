using socketServer.Codes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace socketServer
{
    //这个类用于制作训练用的数据集
    class TrainFileMaker
    {
        Random theRandom = new Random();
        Filter theFilter = new Filter();

        //保存每一步所有的数据，这个是目前为止最通用的方法（不包含GPS）
        //算是线管数据的全存储，训练的饿的时候挑出来自己用的就好
        public List<string> getSaveTrainFile(List<int> indexBuff, List<double> theA, information theInformationController , stepLength theStepLengthController)
        {
            //在这里用list是为了和fileSave做配合，没有必要每一次都设计格式
            //传入一个list使用统一的格式比较好
            List<string> informationToSave = new List<string>();
            //开销很大...慎用....
            List<double> AX = theFilter.theFilerWork(theInformationController.accelerometerX);
            List<double> AY = theFilter.theFilerWork(theInformationController.accelerometerY);
            List<double> AZ = theFilter.theFilerWork(theInformationController.accelerometerZ);
            List<double> GX = theFilter.theFilerWork(theInformationController.gyroX);
            List<double> GY = theFilter.theFilerWork(theInformationController.gyroY);
            List<double> GZ = theFilter.theFilerWork(theInformationController.gyroZ);
            List<double> MX = theFilter.theFilerWork(theInformationController.magnetometerX);
            List<double> MY = theFilter.theFilerWork(theInformationController.magnetometerY);
            List<double> MZ = theFilter.theFilerWork(theInformationController.magnetometerZ);
            List<double> compass = theFilter.theFilerWork(theInformationController.compassDegree);
            List<double> AHRS = theFilter.theFilerWork(theInformationController.AHRSZFromClient);
            List<double> IMU = theFilter.theFilerWork(theInformationController.IMUZFromClient);
            //List<long> timeUse = theFilter.theFilerWork(theInformationController.timeStep, 0.4f, true, theInformationController.accelerometerZ.Count);
            List<long> timeUse = theFilter.theFilerWork(theInformationController.timeStep);
            List<double> StairMode = theFilter.theFilerWork(theInformationController.StairType);
            List<double> stepMode = theFilter.theFilerWork(theInformationController.StepType);
            //加工成字符串
            for (int i = 1; i < indexBuff.Count; i++)
            {
                //StairMode , StepMode是从客户端传入的人手工获取的当前行走状态 StairMode 0 1 2   StepMode 0 1
                //Console.WriteLine("0, 1, 2, 3, 4, 5, 6, 7, 8, 9,  10,  11, 12,13,14, 15, 16   ,17");
                //Console.WriteLine("AX,AY,AZ,GX,GY,GZ,MX,MY,MZ,Com,AHRS,IMU,VK,FK,FSL,RSL,StairMode , StepMode");
                string informationUse = "";
                //下面这些数据是真实的，可以直接用-----------------------------------------------------------------------------------------------------------
                informationUse += AX[indexBuff[i]].ToString("f3") + "," + AY[indexBuff[i]].ToString("f3") + "," + AZ[indexBuff[i]].ToString("f3") + ",";
                informationUse += GX[indexBuff[i]].ToString("f3") + "," + GY[indexBuff[i]].ToString("f3") + "," + GZ[indexBuff[i]].ToString("f3") + ",";
                informationUse += MX[indexBuff[i]].ToString("f3") + "," + MY[indexBuff[i]].ToString("f3") + "," + MZ[indexBuff[i]].ToString("f3") + ",";
                informationUse += compass[indexBuff[i]].ToString("f3") + "," + AHRS[indexBuff[i]].ToString("f3") + "," + IMU[indexBuff[i]].ToString("f3")+",";
                double VK = MathCanculate.getVariance(theA, indexBuff[i - 1], indexBuff[i]);
                double timestep = timeUse[indexBuff[i]] - timeUse[indexBuff[i - 1]];
                double FK = timestep == 0 ? 0f : (1000 / timestep);//因为时间戳是毫秒作为单位的
                double[] weights = SystemSave.CommonFormulaWeights[0];
                double StepLengthInfered = weights[0] * VK + weights[1] * FK + weights[2];//据此推断出来的步长
                informationUse += VK.ToString("f3") + "," + FK.ToString("f3") + "," + SystemSave.getTypeIndexForStepLength( StepLengthInfered ) + ",";
                //下面这些数据是随机生成的的，仅仅可以用作实验-------------------------------------------------------------------------------------------------
                // informationUse += theStepLengthController.getRandomStepLength().ToString("f3") + "," + theStepLengthController.getRandomStairMode();
                informationUse += theStepLengthController.getRandomStepLength().ToString("f3") + ",";
                informationUse += StairMode[indexBuff[i]].ToString("f0") +"," + stepMode[indexBuff[i]].ToString("f0");

                //Console.WriteLine("StartMode[indexBuff[i]] = " + StartMode[indexBuff[i]]);
                
                if (i < indexBuff.Count - 1)
                    informationUse += ",";
                informationToSave.Add(informationUse);
            }
            return informationToSave;
        }


        //以后真正需要使用的生成数据集合的方法
        //但是适合使用GPS的情况
        public string getSaveTrainFile(int indexPre, int indexNow,
            List<double> theA, List<double> theGPSX, List<double> theGPSY, List<long> timeUse = null)
        {

            if (indexNow <= indexPre || timeUse == null)//也就是说传入的数值是错误的，或者数据不够
                return "---";//万金油
            else
            {
                double VK = MathCanculate.getVariance(theA, indexPre, indexNow);

                double timestep = timeUse[indexNow] - timeUse[indexPre];
                //有除零异常说明时间非常短，可以认为根本就没走
                if (timestep == 0)
                    return "---";//万金油
                double FK = (1000 / timestep);//因为时间戳是毫秒作为单位的

                //这个是最基本的模型，当然会改但是架构就是这样了
                //double stepLength = 0.2 * VK + 0.3 * FK + 0.4;
                //存储训练用的参数
                try
                {
                    double x1 = theGPSX[indexPre];
                    double y1 = theGPSY[indexPre];
                    double x2 = theGPSX[indexNow];
                    double y2 = theGPSY[indexNow];
                    // Console.WriteLine(string.Format("x1 = {0} , y1 = {1} , x2 = {2} , y2 = {3}" , x1,y1,x2,y2));
                    double stepLength = MathCanculate.DistanceForGPS(x1, y1, x2, y2);
                    //这是根据希望得到的公式而做的
                    string saveStringItem = VK.ToString("f3") + "," + FK.ToString("f3") + "," + stepLength.ToString("f3");
                    return saveStringItem;
                    // Console.WriteLine(saveStringItem);
                }
                catch(Exception E)
                {
                    Log.saveLog(LogType.error, "TrainBase构建出错:"+E.Message);
                    return "";
                } 
              
            }
        }



    }
}
