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
        //返还这种状态的说明
        public virtual string getInformation()
        {
            return "";
        }
    }
}
