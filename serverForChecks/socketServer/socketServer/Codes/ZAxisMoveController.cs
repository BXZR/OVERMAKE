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
        };
        //返回对这种方法的说明
        public string getMoreInformation(int index)
        {
            return methodInformations[index];
        }


        //专门用来控制Z轴高度上下移动的类
        public List<int> noneMethod(List<int> indexBuff)
        {
            List<int> theStairMode = new List<int>();
            for (int i = 0; i < indexBuff.Count; i++)
            {
                theStairMode.Add(0);
            }
            return theStairMode;
        }

        public List<int> DecisitionTreeMethod(List<int> indexBuff, List<double> ax, List<double> ay, List<double> az, List<double> gx, List<double> gy, List<double> gz)
        {
            List<int> theStairMode = new List<int>();
            for (int i = 0; i < indexBuff.Count; i++)
            {
                int mode = SystemSave.StairTree.searchModeWithTree(ax[indexBuff[i]], ay[indexBuff[i]], az[indexBuff[i]], gx[indexBuff[i]], gy[indexBuff[i]], gz[indexBuff[i]]);
                theStairMode.Add(mode);
            }
            return theStairMode;
        }
    }
}
