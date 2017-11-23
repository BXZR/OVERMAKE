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
        public List<double> inforValues = new List<double>();
        public  List<string > Titles  = new List<string> ();
        public theDecisionTreeNode(string nameIn , double effectValueIn )
        {
            name = nameIn;
            effectValue = effectValueIn;
        }
        public void makeValues(List<List<int>> MAPIn , List<double> inforValuesIn , List<string > titlesIn,int indexNotUse  )
        {
            MAP = new List<List<int>>();
            inforValues = new List<double>();
            Titles = new List<string>();
            for (int i = 0; i < MAPIn.Count; i++)
            {
                if(i != indexNotUse)
                     MAP.Add(MAPIn[i]);
            }
            for (int i = 0; i < inforValuesIn.Count; i++)
            {
                if (i != indexNotUse)
                    inforValues.Add(inforValuesIn[i]);
            }
            for (int i = 0; i < titlesIn.Count; i++)
            {
                if (i != indexNotUse)
                    Titles.Add(titlesIn[i]);
            }

        }

    }
}
