using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace socketServer.Codes.DecisionTree
{
    class theDecisionTreeNode
    {
        public string name = "";
        public double effectValue = 0;
        public List<theDecisionTreeNode> childs = new List<theDecisionTreeNode> ();
        public List<List<int>> MAP = new List<List<int>>();
        public theDecisionTreeNode(string nameIn , double effectValueIn )
        {
            name = nameIn;
            effectValue = effectValueIn;
        }
        public void makeMap(List<List<int>> MAPIn)
        {
            MAP = new List<List<int>>();
            for (int i = 0; i < MAPIn.Count; i++)
            {
                MAP.Add(MAPIn[i]);
            }
        }


    }
}
