using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace socketServer.Codes.stages
{
    //在楼梯上的状态
    class StageStair : FSMBasic
    {
        public override FSMBasic ChangeState(double valueToCheck, List<int> indexBuff, List<double> UsedZ)
        {
            if (valueToCheck <= 0.2)
                return new StageStance();
            if (valueToCheck >= 0.9)
                return new StageRun();
            if (valueToCheck < 0.9 && valueToCheck > 0.2)
                return new StageWalk();
            return this;
        }

        public override string getInformation()
        {
            return "楼梯行走";
        }
    }
}
