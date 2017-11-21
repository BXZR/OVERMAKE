using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace socketServer.Codes.stages
{
    //站立状态
    class StageStance : FSMBasic
    {
        private double theValue = 0;
        public override FSMBasic ChangeState(double valueToCheck = 0)
        {
            if (valueToCheck >= 0.9)
            { Console.WriteLine("开始奔跑"); return new StageRun(); }
            else if( valueToCheck > 0.2 && valueToCheck < 0.9)
            { Console.WriteLine("开始行走"); return new StageWalk();}
            return this;
        }
        public override string getInformation()
        {
            return "正在站立";
        }
    }
}
