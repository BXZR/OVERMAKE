using socketServer.Codes.AcordUse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace socketServer.Codes
{
    class ZAxisMoveController
    {

        //每一种方法的简短说明信息
        private string[] methodInformations =
        {
            "不进行上下位移的计算",
            "使用决策树判断向上、向下还是平移",
            "使用人工神经网络判断向上、向下还是平移",
            "使用KNN判断向上、向下还是平移",
            "使用KMeans判断向上、向下还是平移"
        };


        public List<double> ZMoving(int SelectedIndex , List<int>indexBuff , information theInformationController)
        {
            Filter theFilter = new socketServer.Filter();
            List<double> theStairMode = new List<double>();
            //根本就不计算Z轴位移，其实也是另一种开关
            if (SelectedIndex == 0)
            {
   
                for (int i = 0; i < indexBuff.Count; i++)
                {
                    theStairMode.Add(0);
                }
            }
            //决策树的方法
            if (SelectedIndex == 1)
            {
                theStairMode = new List<double>();
                if (SystemSave.StairTree == null)
                {
                    theStairMode = noneMethod(indexBuff);
                }
                else
                {
                    //这些数据在一些复杂的方法中会用到，因此计算出来备用
                    List<double> ax = theFilter.theFilerWork(theInformationController.accelerometerX);
                    List<double> ay = theFilter.theFilerWork(theInformationController.accelerometerY);
                    List<double> az = theFilter.theFilerWork(theInformationController.accelerometerZ);
                    List<double> gx = theFilter.theFilerWork(theInformationController.gyroX);
                    List<double> gy = theFilter.theFilerWork(theInformationController.gyroY);
                    List<double> gz = theFilter.theFilerWork(theInformationController.gyroZ);

                    theStairMode = DecisitionTreeMethod(indexBuff, ax, ay, az, gx, gy, gz);
                }
            }
            //人工神经网络方法
            if (SelectedIndex == 2)
            {
                if (SystemSave.AccordANNforSLForZAxis == null)
                {
                    theStairMode = noneMethod(indexBuff);
                }
                else
                {
                    //这些数据在一些复杂的方法中会用到，因此计算出来备用
                    List<double> ax = theFilter.theFilerWork(theInformationController.accelerometerX);
                    List<double> ay = theFilter.theFilerWork(theInformationController.accelerometerY);
                    List<double> az = theFilter.theFilerWork(theInformationController.accelerometerZ);
                    List<double> gx = theFilter.theFilerWork(theInformationController.gyroX);
                    List<double> gy = theFilter.theFilerWork(theInformationController.gyroY);
                    List<double> gz = theFilter.theFilerWork(theInformationController.gyroZ);

                    theStairMode = ANNZMove(indexBuff, ax, ay, az, gx, gy, gz);
                }
            }
            //KNN方法
            if (SelectedIndex == 3)
            {
                if (SystemSave.theKNNControllerForStair == null)
                {
                    theStairMode = noneMethod(indexBuff);
                }
                else
                {
                    //这些数据在一些复杂的方法中会用到，因此计算出来备用
                    List<double> ax = theFilter.theFilerWork(theInformationController.accelerometerX);
                    List<double> ay = theFilter.theFilerWork(theInformationController.accelerometerY);
                    List<double> az = theFilter.theFilerWork(theInformationController.accelerometerZ);
                    List<double> gx = theFilter.theFilerWork(theInformationController.gyroX);
                    List<double> gy = theFilter.theFilerWork(theInformationController.gyroY);
                    List<double> gz = theFilter.theFilerWork(theInformationController.gyroZ);

                    theStairMode = KNNZMove(indexBuff, ax, ay, az, gx, gy, gz);
                }
            }
            //Means方法
            if (SelectedIndex == 4)
            {
                if (SystemSave.theKMeansForStair == null)
                {
                    theStairMode = noneMethod(indexBuff);
                }
                else
                {
                    //这些数据在一些复杂的方法中会用到，因此计算出来备用
                    List<double> ax = theFilter.theFilerWork(theInformationController.accelerometerX);
                    List<double> ay = theFilter.theFilerWork(theInformationController.accelerometerY);
                    List<double> az = theFilter.theFilerWork(theInformationController.accelerometerZ);
                    List<double> gx = theFilter.theFilerWork(theInformationController.gyroX);
                    List<double> gy = theFilter.theFilerWork(theInformationController.gyroY);
                    List<double> gz = theFilter.theFilerWork(theInformationController.gyroZ);

                    theStairMode = KMeansZMove(indexBuff, ax, ay, az, gx, gy, gz);
                }
            }
            return theStairMode;
        }

        //=====================================================================================//
        //返回对这种方法的说明
        public string getMoreInformation(int index)
        {
            return methodInformations[index];
        }
        //返回全部的方法说明
        public string[] getMoreInformation()
        {
            return methodInformations;
        }

        //专门用来控制Z轴高度上下移动的类
        public List<double> noneMethod(List<int> indexBuff)
        {
            List<double> theStairMode = new List<double>();
            for (int i = 0; i < indexBuff.Count; i++)
            {
                theStairMode.Add(0);
            }
            return theStairMode;
        }


        public List<double> DecisitionTreeMethod(List<int> indexBuff, List<double> ax, List<double> ay, List<double> az, List<double> gx, List<double> gy, List<double> gz)
        {
            List<double> theStairMode = new List<double>();
            for (int i = 0; i < indexBuff.Count; i++)
            {
                int mode = SystemSave.StairTree.searchModeWithTree(ax[indexBuff[i]], ay[indexBuff[i]], az[indexBuff[i]], gx[indexBuff[i]], gy[indexBuff[i]], gz[indexBuff[i]]);
                theStairMode.Add(transToHeightMove(mode));
            }
            return theStairMode;
        }

        public List<double> ANNZMove(List<int> indexBuff, List<double> ax, List<double> ay, List<double> az, List<double> gx, List<double> gy, List<double> gz)
        {
            List<double> theStairMode = new List<double>();
            for (int i = 0; i < indexBuff.Count; i++)
            {
                int mode = SystemSave.AccordANNforSLForZAxis.getModeWithANNForStair(ax[indexBuff[i]], ay[indexBuff[i]], az[indexBuff[i]], gx[indexBuff[i]], gy[indexBuff[i]], gz[indexBuff[i]]);
                theStairMode.Add(transToHeightMove(mode));
            }
            return theStairMode;
        }
        //KNN和ANN完全不是一个东西
        public List<double> KNNZMove(List<int> indexBuff, List<double> ax, List<double> ay, List<double> az, List<double> gx, List<double> gy, List<double> gz)
        {
            List<double> theStairMode = new List<double>();
            for (int i = 0; i < indexBuff.Count; i++)
            {
                int mode = SystemSave.theKNNControllerForStair.getKNNType(ax[indexBuff[i]], ay[indexBuff[i]], az[indexBuff[i]], gx[indexBuff[i]], gy[indexBuff[i]], gz[indexBuff[i]]);
                theStairMode.Add(transToHeightMove(mode));
            }
            return theStairMode;
        }
        //KNN和KMeans完全不是一个东西
        public List<double> KMeansZMove(List<int> indexBuff, List<double> ax, List<double> ay, List<double> az, List<double> gx, List<double> gy, List<double> gz)
        {
            List<double> theStairMode = new List<double>();
            for (int i = 0; i < indexBuff.Count; i++)
            {
                int mode = SystemSave.theKMeansForStair.getTypeWithKMeans(ax[indexBuff[i]], ay[indexBuff[i]], az[indexBuff[i]], gx[indexBuff[i]], gy[indexBuff[i]], gz[indexBuff[i]]);
                theStairMode.Add(transToHeightMove(mode));
            }
            return theStairMode;
        }

        //移动模式转真实的位移
        private double   transToHeightMove(int mode)
        {
           //传入的mode是012
           //0下1平2上
            Console.WriteLine("ZMove Mode = "+ mode);
            // 1 或者 -1 用来判断向上走还是向下走
            //0表示根本就是平地
            //(这个部分现在还未完成，需要与其他模块的配合)
            return  SystemSave.StairHeight * (mode-1);
        }
    }
}
