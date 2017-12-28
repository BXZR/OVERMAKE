using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace socketServer.Codes.stages
{
    class StageRun : FSMBasic
    {
        private double theValue = 0;
        public override FSMBasic ChangeState(double valueToCheck = 0)
        {
            if (valueToCheck <= 0.2)
                return new StageStance();
            if (valueToCheck < 0.9 && valueToCheck > 0.2)
                return new StageWalk();
            return this;
        }

        public override FSMBasic ChangeState(double valueToCheck, List<int> indexBuff, List<double> usedZ)
        {
            if (getStairModeCheck(indexBuff, usedZ))
            { Console.WriteLine("开始上下楼梯"); return new StageStair(); }
            if (valueToCheck <= 0.2)
                return new StageStance();
            if (valueToCheck < 0.9 && valueToCheck > 0.2)
                return new StageWalk();
            return this;
        }

        public override string getInformation()
        {
            return "正在奔跑";
        }
    }
}
