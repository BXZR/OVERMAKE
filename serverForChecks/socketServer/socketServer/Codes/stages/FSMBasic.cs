using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace socketServer.Codes.stages
{
    //状态检查有限状态机的基类
    //之所以不用接口是因为有一些参数恐怕用集成更为方便
    class FSMBasic
    {
        //状态转换方法，根据满足的要求转换到不同的FMSBasic
        public virtual FSMBasic ChangeState(double valueToCheck = 0)
        {
            return null;
        }
        //更加复杂的状态转换方法，根据满足的要求转换到不同的FMSBasic
        public virtual FSMBasic ChangeState(double valueToCheck, List<int> indexBuff , List<double> UsedZ)
        {
            return null;
        }
        //返还这种状态的说明
        public virtual string getInformation()
        {
            return "";
        }

        //检查是不是应该进入到上下楼梯的状态
        public bool getStairModeCheck(List<int> indexBuff, List<double> UsedZ)
        {
            if (indexBuff.Count < 3)
                return false;

            List<double> theValues = new List<double>();
           // Console.WriteLine("-------------------------------------------");
            for (int i = indexBuff.Count - 1; i >= indexBuff.Count - 3; i--)
            { 
               // Console.WriteLine("used[i] = "+UsedZ[i]);
                if (Math.Abs( UsedZ[i]) < 0.9)
                    return false;
                theValues.Add(UsedZ[i]);
            }
            double VS = MathCanculate.getVariance(theValues);
            if (VS < 0.05)
                return true;
            return true;

        }
    }
}
