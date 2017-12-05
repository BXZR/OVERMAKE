using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace socketServer.Codes.DecisionTree
{
    public class theDecisionTreeNode
    {
        public string name = "";
        public int Mode = 0;//这个节点所代表的模式，对应的属性Mode
        //这个节点所代表的步长的模式，因为可能有抖动导致SLMode未必唯一，所以使用list存储
        public List<int> aimMode = new List<int>();
        //如果分到叶子节点还没有分完，就需要使用多数表决的方法，也因此需要有一个缓冲区来实现数量的保存
        public List<int> aimModeCount = new List<int>();
        public List<theDecisionTreeNode> childs = new List<theDecisionTreeNode>();
        public List<List<int>> MAP = new List<List<int>>();
        public List<double> inforValues = new List<double>();
        public List<string> Titles = new List<string>();
        public int depth = 0;//这个节点在树中的深度
        private string thePart = "";//表现为AXAYAZGXGYGZ 用来在搜索的时候作为标记出现
        public static int maxDepth = 0;//最深的层数是？
        public static int nodeCountAll = 0;//总共的节点个数

        //选择叶节点计数数量最多的aimMode
        private int getLeafAimMode()
        {
            int index = -11;
            int maxCount = -9999;
            for (int i = 0; i < aimModeCount.Count; i++)
            {
                if (aimModeCount[i] > maxCount)
                {
                    maxCount = aimModeCount[i];
                    index = i;
                }
            }
            return aimMode[index];
        }

        //增加aimMode的时候对结果进行计数
        private void freshAimModeCount(int modeIn)
        {
            int indexUse = -1;
            for (int i = 0; i < aimMode.Count; i++)
            {
                if (aimMode[i] == modeIn)
                {
                    indexUse = i;
                    break;
                }
            }
            if (indexUse >= 0)
            {
                aimModeCount[indexUse]++;
            }
        }

        public int searchLeafMode(List<int> values , List<string> titles )
        {
            int theModeReturn = 0;
            if (this.childs.Count == 0)
            {
                theModeReturn = getLeafAimMode();
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

        //modesForCheck是这个节点对应的属性的mode集合
        public void makeValues(theDecisionTreeNode father , int indexNotUse , List<int> aimMode)
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

            //从自身代表的属性的mode找到所有father中有的stepLengthMode，这些对应的就是这个节点对应的steoLengthMode
            for (int i = 0; i < father.MAP[indexNotUse].Count; i++)
            {
                if(father.MAP[indexNotUse][i] == this.Mode && father.aimMode.Contains(aimMode[i]))
                {
                    if (this.aimMode.Contains(aimMode[i]) == false)
                    {
                        this.aimMode.Add(aimMode[i]);
                        this.aimModeCount.Add(0);
                    }
                    freshAimModeCount(aimMode[i]);
                }
            }
            //如果出现矛盾的情况就放宽要求
            if (this.aimMode.Count == 0)
            {
                Console.WriteLine(this.name+"出现矛盾项");
                for (int i = 0; i < father.MAP[indexNotUse].Count; i++)
                {
                    if (father.MAP[indexNotUse][i] == this.Mode)
                    {
                        if (this.aimMode.Contains(aimMode[i]) == false)
                        { 
                            this.aimMode.Add(aimMode[i]);
                            this.aimModeCount.Add(0);
                        }
                        freshAimModeCount(aimMode[i]);
                    }
                }
            }

            this.depth = father.depth + 1;
            if (this.depth > maxDepth)
                maxDepth = depth;

        }

        //这是一个简单的方法，实际上更通用
        //放在这里是为了更容易理解
        //目前这个方法仅用于树根的初始化使用
        //theModesOfThis是选定属性的属性mode集合
        //stepLengthMode时候对应数据全部的stepLength的mode集合
        //fatherStepLengthMode是来自father的stepLengthMode
        public void makeValues(List<List<int>> MAPIn , List<double> inforValuesIn , List<string > titlesIn,int indexNotUse , int depth, List<int> theModesOfThis, List<int> stepLengthMode , List<int> fatherStepLengthMode)
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

            //从自身代表的属性的mode找到所有father中有的steoLengthMode，这些对应的就是这个节点对应的stepLengthMode
            //theModesOfThis 如果为空，就说明是root节点，还没有选定属性，所以没有theModesOfThis
            //但是跟节点包含所有的信息
            if (theModesOfThis != null)
            {
                for (int i = 0; i < theModesOfThis.Count; i++)
                {
                    if (theModesOfThis[i] == this.Mode && fatherStepLengthMode.Contains(stepLengthMode[i]))
                    {
                        //保证不重复
                        if (this.aimMode.Contains(stepLengthMode[i]) == false)
                        { 
                            this.aimMode.Add(stepLengthMode[i]);
                            this.aimModeCount.Add(0);
                        }
                        freshAimModeCount(stepLengthMode[i]);
                    }
                }
                //如果出现矛盾的情况就放宽要求
                if (this.aimMode.Count == 0)
                {
                    for (int i = 0; i < theModesOfThis.Count; i++)
                    {
                        if (theModesOfThis[i] == this.Mode)
                        {
                            if (this.aimMode.Contains(stepLengthMode[i]) == false)
                            { 
                                this.aimMode.Add(stepLengthMode[i]);
                                this.aimModeCount.Add(0);
                            }
                            freshAimModeCount(stepLengthMode[i]);
                        }
                    }
                }
            }
            else // 如果传入空，说明这个真的是最根本的根节点了，这个节点一定有所有的信息
            {
                  for(int i =0; i < fatherStepLengthMode.Count; i++)
                    {
                       if (this.aimMode.Contains(fatherStepLengthMode[i]) == false)
                       { 
                        this.aimMode.Add(fatherStepLengthMode[i]);
                        this.aimModeCount.Add(0);
                       }
                    freshAimModeCount(fatherStepLengthMode[i]);
                }
            }
            this.depth = depth+1;
            if (this.depth > maxDepth)
                maxDepth = depth;

        }

    }
}
