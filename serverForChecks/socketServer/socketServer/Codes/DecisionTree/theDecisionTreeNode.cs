using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace socketServer.Codes.DecisionTree
{
   public  class theDecisionTreeNode
    {
        public string name = "";
        public int  Mode = 0;//这个节点所代表的模式
        public List<theDecisionTreeNode> childs = new List<theDecisionTreeNode> ();
        public List<List<int>> MAP = new List<List<int>>();
        public List<double> inforValues = new List<double>();
        public  List<string > Titles  = new List<string> ();
        public int depth = 0;//这个节点在树中的深度
        private string thePart = "";//表现为AXAYAZGXGYGZ 用来在搜索的时候作为标记出现
        public  static int maxDepth = 0;//最深的层数是？
        public static int nodeCountAll = 0;//总共的节点个数
        public int searchLeafMode(List<int> values , List<string> titles)
        {
            int theModeReturn = 0;
            if (this.childs.Count == 0)
            {
                theModeReturn = Mode;
                return theModeReturn;
            }
            else
            {
                int valuesIndex = 0;//用来使用判断的数据的index
                for (int i = 0; i < titles.Count; i++)
                {
                    if (titles[i] == this.thePart)
                    {
                        valuesIndex = i;
                        break;
                    }
                }
                //选择对应的数
                //加入这个节点是AX-1
                //那么他应该使用AX的mode作为切分点
                double modeValue = values[valuesIndex];
                for (int i = 0; i < this.childs.Count; i++)
                {
                    if (childs[i].Mode == modeValue)
                    {
                        theModeReturn = childs[i].searchLeafMode(values, titles);
                        break;
                    }
                }
                return theModeReturn;
            }

    
        }

        public theDecisionTreeNode(string nameIn ,int   ModeIn )
        {
            thePart = nameIn;
            Mode = ModeIn;
            name = nameIn+"-"+ Mode;
        }

        public void makeValues(theDecisionTreeNode father , int indexNotUse)
        {
            MAP = new List<List<int>>();
            inforValues = new List<double>();
            Titles = new List<string>();
            for (int i = 0; i < father.MAP.Count; i++)
            {
                if (i != indexNotUse)
                    MAP.Add(father.MAP[i]);
            }
            for (int i = 0; i < father.inforValues.Count; i++)
            {
                if (i != indexNotUse)
                    inforValues.Add(father.inforValues[i]);
            }
            for (int i = 0; i < father.Titles.Count; i++)
            {
                if (i != indexNotUse)
                    Titles.Add(father.Titles[i]);
            }
            this.depth = father.depth + 1;
            if (this.depth > maxDepth)
                maxDepth = depth;

        }

        //这是一个简单的方法，实际上更通用
        //放在这里是为了更容易理解
        //目前这个方法仅用于树根的初始化使用
        public void makeValues(List<List<int>> MAPIn , List<double> inforValuesIn , List<string > titlesIn,int indexNotUse , int depth )
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
            this.depth = depth+1;
            if (this.depth > maxDepth)
                maxDepth = depth;

        }

    }
}
