using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace socketServer.Codes.DecisionTree
{
    public class theDecisionTree
    {

        //保存各个属性的数值
        //其实可以不用保存但是为了保证更多的东西可以看到，就做一些保留
        private List<double> AX = new List<double>();
        private List<double> AY = new List<double>();
        private List<double> AZ = new List<double>();
        private List<double> GX = new List<double>();
        private List<double> GY = new List<double>();
        private List<double> GZ = new List<double>();
        private List<double> SL = new List<double>();
        private List<double> SM = new List<double>();
        //因为是针对“程度”的分类，所以其实真正参与到计算的不是确切的数值，而是“分类”
        private List<int> AXMode = new List<int>();
        private List<int> AYMode = new List<int>();
        private List<int> AZMode = new List<int>();
        private List<int> GXMode = new List<int>();
        private List<int> GYMode = new List<int>();
        private List<int> GZMode = new List<int>();
        private List<int> SLMode = new List<int>();
        private List<int> StairMode = new List<int>();
        //这个是属性的集合，方便每一个属性被剥离出去
        private List<List<int>> MAP = new List<List<int>>();

        List<int> typesForAll = new List<int>();//有多少种类型
        List<int> countOfTypesForAll = new List<int>();//每一种类型有多少
        List<double> inforValues = new List<double>();//计算用的影响值
        List<string> titles = new List<string>();//标题集合
        public theDecisionTreeNode theRoot;//数根节点，这个是递归的根节点
        private bool isStarted = false;//是否构建的标志
        public bool IsStarted { get { return isStarted; } }//只读标记
        private double inforSL = 0;//用来选择的目标基准值
        //用来选择的目标mode(可以使 SLMode , StairMode 。。。。。)
        private List<int> aimMode = new List<int>();

        //传入需要的参数
        //通过决策树选择出来当前的移动的模式
        public int searchModeWithTree(double ax, double ay, double az, double gx, double gy, double gz)
        {
            int mode = 1;
            if (isStarted)
            {
                //这个顺序都是AX AY AZ GX GY GZ 顺序不可以打乱
                //量化成为类型或者说模式
                int axUse = SystemSave.getTypeIndexForData(ax);
                int ayUse = SystemSave.getTypeIndexForData(ay);
                int azUse = SystemSave.getTypeIndexForData(az);
                int gxUse = SystemSave.getTypeIndexForData(gx);
                int gyUse = SystemSave.getTypeIndexForData(gy);
                int gzUse = SystemSave.getTypeIndexForData(gz);
                List<int> VAL = new List<int>();
                VAL.Add(axUse);
                VAL.Add(ayUse);
                VAL.Add(azUse);
                VAL.Add(gxUse);
                VAL.Add(gyUse);
                VAL.Add(gzUse);
                mode = theRoot.searchLeafMode(VAL, titles);
            }
            return mode;
        }


        //根据数据集做一棵决策树然后处理
        //外部构建这个树的方法
        //treeType是作为目标list
        //设定treeType : 0 stepLength 1 StairMode 
        public void BuildTheTree(string path = "" , int treeType =0)
        {
            if (isStarted == false)
            {
               
                makeTitles();//建立titles集合
                initMap(path);//读数据
                makeTreeType(treeType);//设定决策树的type或者说根节点的目标
                makePartValues();//建立分类数据
                makeDictionary();//创建“字典”
                makeBasicValue();//计算基础数值
                theRoot = new theDecisionTreeNode("Root", -1);
                makeEffectValues();
                //全局根节点的做法有一点特殊
                theRoot.makeValues(MAP, inforValues, titles, -1, -1 ,null , aimMode , aimMode);
                makePoint(theRoot);
                isStarted = true;
            }
        }


        private void makeTreeType(int treeType = 0)
        {
            //设定决策树的type
            if (treeType == 0)
                aimMode = SLMode;
            else if (treeType == 1)
                aimMode = StairMode;
        }

        private void makeTitles()
        {
            titles = new List<string>();
            titles.Add("AX");
            titles.Add("AY");
            titles.Add("AZ");
            titles.Add("GX");
            titles.Add("GY");
            titles.Add("GZ");
        }

        //读取数据文件建立这个树所需要的基础数据
        private void initMap(string path = "")
        {
            if (string.IsNullOrEmpty(path))
                return;
            string infotmationGet = FileSaver.readFromTrainBase();
           // string infotmationGet = new FileSaver().readInformation(path);
            string[] line = infotmationGet.Split('\n');
            for (int i = 0; i < line.Length; i++)
            {
                string[] rows = line[i].Split(',');
                if (string.IsNullOrEmpty(line[i]))
                    continue;
                //一行的数据有可能无法被当做数据项使用，而且这种情况也很常见
                try
                {
                    // Console.WriteLine(rows[0] + "------" + i);
                    AX.Add(Convert.ToDouble(rows[0]));
                    AY.Add(Convert.ToDouble(rows[1]));
                    AZ.Add(Convert.ToDouble(rows[2]));
                    GX.Add(Convert.ToDouble(rows[3]));
                    GY.Add(Convert.ToDouble(rows[4]));
                    GZ.Add(Convert.ToDouble(rows[5]));
                    SL.Add(Convert.ToDouble(rows[15]));
                    SM.Add(Convert.ToInt32(rows[16]));
                }
                catch
                {
                    //这只是权宜之计
                    //AX.Add(Convert.ToDouble(0));
                    //AY.Add(Convert.ToDouble(0));
                    //AZ.Add(Convert.ToDouble(0));
                    //GX.Add(Convert.ToDouble(0));
                    //GY.Add(Convert.ToDouble(0));
                    //GZ.Add(Convert.ToDouble(0));
                    //SL.Add(Convert.ToDouble(0));
                    //SM.Add(Convert.ToInt32(0));
                    continue;//这一行被放弃
                }
            }
           // Console.WriteLine("Data loaded for tree");
        }
        //建立属性集合，用来拆分和标记
        private void makeDictionary()
        {
            MAP.Add(AXMode);
            MAP.Add(AYMode);
            MAP.Add(AZMode);
            MAP.Add(GXMode);
            MAP.Add(GYMode);
            MAP.Add(GZMode);
            // Console.WriteLine("Map built");
        }

        //基础数据转化为分类数据
        private void makePartValues()
        {
            for (int i = 0; i < AX.Count; i++)
            {
                //属性的分类数量
                AXMode.Add(SystemSave.getTypeIndexForData(AX[i]));
                AYMode.Add(SystemSave.getTypeIndexForData(AY[i]));
                AZMode.Add(SystemSave.getTypeIndexForData(AZ[i]));
                GXMode.Add(SystemSave.getTypeIndexForData(GX[i]));
                GYMode.Add(SystemSave.getTypeIndexForData(GY[i]));
                GZMode.Add(SystemSave.getTypeIndexForData(GZ[i]));
                //目标的分类数量
                SLMode.Add(SystemSave.getTypeIndexForStepLength(SL[i]));
                StairMode.Add(SystemSave.getTypeIndexForStair(SM[i]));
            }
            //Console.WriteLine("Part get");
        }

        //目标的infor参见公式
        //计算信息获取度
        private void makeBasicValue()
        {
            //我这里将数据分成了多种情况，也就是最终需要分成的类型数量，因此可以直接进行处理
            //但是为了保证可兼容性，在这里直接做复杂一点的处理
            //为了表达简单，用的方法相当土鳖

            //SLMode是目标，所以用这个来计算
            //其实仍然是从头到尾的遍历
            for (int i = 0; i < aimMode.Count; i++)
            {
                if (typesForAll.Contains(aimMode[i]) == false)
                {
                   // Console.WriteLine("aimMode =" + aimMode[i]);
                    typesForAll.Add(aimMode[i]);
                    countOfTypesForAll.Add(0);
                }
                else
                {
                    //否则对应index的countOfTypesForAll需要累加
                    countOfTypesForAll[typesForAll.IndexOf(aimMode[i])]++;
                }
            }
            //Console.WriteLine("get type count = " + typesForAll.Count);
            //Console.WriteLine("countOfTypesForAll = " + countOfTypesForAll.Count);

            /*
             * 这个是“对应index的countOfTypesForAll需要累加”的原始土鳖方法
             * 暂时保留之.......
             * 优势，简单易读
             * 劣势，效率太糟糕
            for (int i = 0; i < aimMode.Count; i++)
            {
                for (int j = 0; j < typesForAll.Count; j++)
                {
                    if (typesForAll[j] == aimMode[i])
                    {
                        countOfTypesForAll[j]++;
                    }
                }
            }
            */

            //for (int i = 0; i < countOfTypesForAll.Count; i++)
            //Console.WriteLine("countOfTypesForAll"+i+"  "+ countOfTypesForAll[i]);

            // Math.Log(125, 5);//以5为底，125的对数
            for (int i = 0; i < typesForAll.Count; i++)
            {
                double counts = (double)countOfTypesForAll[i];//做一下隐式转换
                double P = counts / (double)aimMode.Count;//频率当概率使用
                inforSL += -P * Math.Log(P, 2);
            }
            // Console.WriteLine("Basic Value = " + inforSL);
        }

        //--------------------------------------------------------------------------------------------------------------------------//
        //计算每一种属性对目标值的影响
        //全局唯一计算一次其余的分项就可以了
        private void makeEffectValues()
        {
            //这些是针对非目标属性的那些用来分辨的属性来做的
            //要选择当前所有还需要分的属性中最重要的那个属性
            //Map是当前属性存储的集合
            for (int mapIndex = 0; mapIndex < MAP.Count; mapIndex++)
            {
                List<int> types = new List<int>();//有多少种类型
                List<int> countOfTypes = new List<int>();//每一种类型有多少

                //types为map每一个元素的去重复
                //方法1 linQ
                types = MAP[mapIndex].Distinct().ToList(); 
                //方法2 土鳖赋值方法
                //for (int i = 0; i < MAP[mapIndex].Count; i++)
                //{
                //    if (types.Contains(MAP[mapIndex][i]) == false)
                //    {
                //        types.Add(MAP[mapIndex][i]);
                //    }
                //}

                //for (int i = 0; i < types.Count; i++)
                //   Console.WriteLine("branceType = "+ types[i]);

                for (int i = 0; i < types.Count; i++)
                    for (int j = 0; j < typesForAll.Count; j++)//因为要记录每一种的小项目中，针对大项目的数量，需要做百分比
                        countOfTypes.Add(0);

                // Console.WriteLine("countOfTypes: " + countOfTypes.Count);

                int startIndex = -1;
                int offsetIndex = -1;
                //每一种小的类型分别有多少
                for (int k = 0; k < MAP[mapIndex].Count; k++)
                {
                    //方法1 List的内置方法
                    startIndex = types.IndexOf(MAP[mapIndex][k]);
                    //方法2 土鳖查找法
                    //for (int i = 0; i < types.Count; i++)
                    //{
                    //    //对于每一个数据多需要做处理
                    //    if (MAP[mapIndex][k] == types[i])//如果当前数据由某一个类型
                    //    {
                    //        startIndex = i;//基础index
                    //                       //找到这个该数据在本小类中的类型
                    //                       //Console.WriteLine("theType of this :" + types[i]);
                    //    }
                    //}
                    //计算偏移量
                    //这与我的数据排布方式有关
                    //方法1 List的内置方法
                    offsetIndex = typesForAll.IndexOf(aimMode[k]);
                    //计算偏移量也可以用List的做法处理
                    //方法2 土鳖查找法
                    //for (int j = 0; j < typesForAll.Count; j++)
                    //{
                    //    //横向处理
                    //    if (aimMode[k] == typesForAll[j])
                    //    {
                    //        offsetIndex = j;
                    //        //找到这个该数据在本小类中的类型
                    //        //Console.WriteLine("All Type of this :" + typesForAll[j]);
                    //    }
                    //}
                    int indexUse = typesForAll.Count * startIndex + offsetIndex;
                    countOfTypes[indexUse]++;
                }

                //for (int i = 0; i < countOfTypes.Count; i++)
                //    Console.WriteLine("countOfTypes[" + i + "] = " + countOfTypes[i]);
                //每一种小的类型对于大的类型有多少
                //并开始套用公式////////////////////////////////////////////////////////////////////////////////////////////////////
                double inforValue = 0;
                if (SystemSave.DecisionTreeMethodID == 0)
                {
                    for (int i = 0; i < types.Count; i++)
                    {
                        double allcountForThisType = 0;
                        for (int j = 0; j < typesForAll.Count; j++)
                        {
                            allcountForThisType += (double)countOfTypes[i + j];//做一下隐式转换
                        }
                        double P1 = allcountForThisType / MAP[mapIndex].Count;
                        for (int j = 0; j < typesForAll.Count; j++)
                        {
                            double P2 = (double)countOfTypes[i + j] / allcountForThisType;//频率当概率使用
                            inforValue += -P1 * P2 * Math.Log(P2, 2);
                        }
                    }
                    inforValue = inforSL - inforValue;
                }
                //如果使用的是C4.5的方法就需要加上一步
                //，这个是目前看博客学到的公式方法，希望是对的....
                //这里将不同的方法的相同代码分别使用了，要不然代码虽简但是不好看
                if (SystemSave.DecisionTreeMethodID == 1)
                {
                    for (int i = 0; i < types.Count; i++)
                    {
                        double allcountForThisType = 0;
                        for (int j = 0; j < typesForAll.Count; j++)
                        {
                            allcountForThisType += (double)countOfTypes[i + j];//做一下隐式转换
                        }
                        double P1 = allcountForThisType / MAP[mapIndex].Count;
                        for (int j = 0; j < typesForAll.Count; j++)
                        {
                            double P2 = (double)countOfTypes[i + j] / allcountForThisType;//频率当概率使用
                            inforValue += -P1 * P2 * Math.Log(P2, 2);
                        }
                    }
                    inforValue = inforSL - inforValue;
                    inforValue /= inforSL;
                }
                if (SystemSave.DecisionTreeMethodID == 2)
                {
                    for (int i = 0; i < types.Count; i++)
                    {
                        double allcountForThisType = 0;
                        for (int j = 0; j < typesForAll.Count; j++)
                        {
                            allcountForThisType += (double)countOfTypes[i + j];//做一下隐式转换
                        }
                        double P1 = allcountForThisType / MAP[mapIndex].Count;
                        for (int j = 0; j < typesForAll.Count; j++)
                        {
                            double P2 = (double)countOfTypes[i + j] / allcountForThisType;//频率当概率使用
                            inforValue += P1 * (1-P2*P2);//目前的理解就是不同的方法其实就是不同的“熵”的算法而已
                        }
                    }
                }

                inforValues.Add(inforValue);
                //Console.WriteLine("mapIndex = " + mapIndex);
                //Console.WriteLine("inforVAlues: " + inforValue);
            }
        }

        //排序并且去除最大项作为节点
        private void makePoint(theDecisionTreeNode theFatherPoint = null)
        {
            if (theFatherPoint.MAP.Count == 0)
            {
               // Console.WriteLine("--------------------------------------------------");
               // for(int i =0; i< theFatherPoint.stepLengthMode.Count; i++ )
               // Console.WriteLine("SLMode for "+theFatherPoint.name +" is "+ theFatherPoint.stepLengthMode[i]);
                return;//全部拆分出去了就意味着书已经完成了（材料用光）
            }
            //如果已经是一种确定的了（分到这个节点的时候已经只会有一种情况了）
            //那么这一分支或许可以剪枝
            if (theFatherPoint.aimMode.Count == 1 && SystemSave.isCutForDecisionTree)
            {
                //Console.WriteLine("在" + theFatherPoint.name + "处剪枝");
                return;
            }

            //根本就没有排序，而是查找最大项实现的，因为目标只有最大项
            double max = -9999;
            int index = 0;
            for (int i = 0; i < theFatherPoint.inforValues.Count; i++)
            {
                if (max < theFatherPoint.inforValues[i])
                {
                    max = theFatherPoint.inforValues[i];
                    index = i;
                }
            }
            //找到所有的类型:对于这个影响能力最大的属性来说，他有多少种类型
            //每一种类型都将会是一个分支
            List<int> types = new List<int>();
            //types就是theFatherPoint.MAP[index]的去重复版本
            //方法1，linq
            types = theFatherPoint.MAP[index].Distinct().ToList();
            //方法2，循环检查（土鳖）
            //for (int i = 0; i < theFatherPoint.MAP[index].Count; i++)
            //{
            //    if (types.Contains(theFatherPoint.MAP[index][i]) == false)
            //    {
            //        types.Add(theFatherPoint.MAP[index][i]);
            //    }
            //} 
            //Console.WriteLine("------------------------------------------------");
            //Console.WriteLine("Father is "+ theFatherPoint.name);
            for (int i = 0; i < types.Count; i++)
            {
                //Console.WriteLine("Child is " + theFatherPoint.Titles[index] +"-"+types[i]);
                theDecisionTreeNode thePoint = new DecisionTree.theDecisionTreeNode(theFatherPoint.Titles[index], types[i]);
                theFatherPoint.childs.Add(thePoint);
                //将选取出的数据最大（影响最大的）从MAP中删除
                //并为这一项建立节点（每一个type都是一个节点）
                //材料用掉，这个最大的影响力的节点属性已经不会被这个属性的子节点使用了
                thePoint.makeValues(theFatherPoint, index , aimMode);
            }
            for (int i = 0; i < theFatherPoint.childs.Count; i++)
            {
                //递归创建决策树
                theDecisionTreeNode .nodeCountAll++;
                makePoint(theFatherPoint.childs[i]);
            }

        }


        //额外补充的小小方法
        //共多少个节点
        public int getNodeCount()
        {
            return theDecisionTreeNode.nodeCountAll;
        }
        //一共多少层
        public int getDepth()
        {
            return theDecisionTreeNode.maxDepth;
        }

     
    }
}
