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
        };
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

        //移动模式转真实的位移
        private double   transToHeightMove(int mode)
        {
            Console.WriteLine("ZMove Mode = "+ mode);
            // 1 或者 -1 用来判断向上走还是向下走
            //0表示根本就是平地
            //(这个部分现在还未完成，需要与其他模块的配合)
            return  SystemSave.StairHeight * (mode-1);
        }
    }
}
