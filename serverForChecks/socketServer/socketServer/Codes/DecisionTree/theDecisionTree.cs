using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace socketServer.Codes.DecisionTree
{
    class theDecisionTree
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
        //因为是针对“程度”的分类，所以其实真正参与到计算的不是确切的数值，而是“分类”
        private List<int> AXMode = new List<int>();
        private List<int> AYMode = new List<int>();
        private List<int> AZMode = new List<int>();
        private List<int> GXMode = new List<int>();
        private List<int> GYMode = new List<int>();
        private List<int> GZMode = new List<int>();
        private List<int> SLMode = new List<int>();
       
        //这个是属性的集合，方便每一个属性被剥离出去
        private List<List<int>> MAP = new List<List<int>>();

        List<int> typesForAll = new List<int>();//有多少种类型
        List<int> countOfTypesForAll = new List<int>();//每一种类型有多少
        List<double> inforValues = new List<double>();//计算用的影响值
        List<string> titles = new List<string>();//标题集合
        theDecisionTreeNode theRoot;//数根节点，这个是递归的根节点
 

        private  double inforSL = 0;//用来选择的目标基准值

        //根据数据集做一棵决策树然后处理
        //外部构建这个树的方法
        public void BuildTheTree(string path = "")
        {
            makeTitles();//建立titles集合
            initMap(path);//读数据
            makePartValues();//建立分类数据
            makeDictionary();//创建“字典”
            makeBasicValue();//计算基础数值
            theRoot = new theDecisionTreeNode("Root",-1);
            makeEffectValues();
            theRoot.makeValues(MAP , inforValues , titles  , - 1);
            makePoint(theRoot);
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
            string infotmationGet = new FileSaver().readInformation(path);
            string[] line = infotmationGet.Split('\n');
            for (int i = 0; i < line.Length; i++)
            {

                string[] rows = line[i].Split(',');
                if (string.IsNullOrEmpty(line[i]))
                    continue;

               // Console.WriteLine(rows[0] + "------" + i);
                AX.Add(Convert.ToDouble(rows[0]));
                AY.Add(Convert.ToDouble(rows[1]));
                AZ.Add(Convert.ToDouble(rows[2]));
                GX.Add(Convert.ToDouble(rows[3]));
                GY.Add(Convert.ToDouble(rows[4]));
                GZ.Add(Convert.ToDouble(rows[5]));
                SL.Add(Convert.ToDouble(rows[6]));
            }
            Console.WriteLine("Data loaded");
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
            Console.WriteLine("Map built");
        }

        //基础数据转化为分类数据
        private void makePartValues()
        {
            for (int i = 0; i < AX.Count; i++)
            {
                AXMode.Add(getTypeIndex(AX[i]));
                AYMode.Add(getTypeIndex(AY[i]));
                AZMode.Add(getTypeIndex(AZ[i]));
                GXMode.Add(getTypeIndex(GX[i]));
                GYMode.Add(getTypeIndex(GY[i]));
                GZMode.Add(getTypeIndex(GZ[i]));
                SLMode.Add(getTypeIndex(SL[i]));
            }
            Console.WriteLine("Part get");
        }

        //目标的infor参见公式
        private void makeBasicValue()
        {
            //我这里将数据分成了四种情况，因此可以直接进行处理
            //但是为了保证可兼容性，在这里直接做复杂一点的处理
            //为了表达简单，用的方法相当土鳖

            //SLMode是目标，所以用这个来计算
            for (int i = 0; i < SLMode.Count; i++)
            {
                if (typesForAll.Contains(SLMode[i]) == false)
                {
                    //Console.WriteLine("SLMode =" + SLMode[i]);
                    typesForAll.Add(SLMode[i]);
                    countOfTypesForAll.Add(0);
                }
            }
           // Console.WriteLine("get type count = " + typesForAll.Count);
           // Console.WriteLine("countOfTypesForAll = " + countOfTypesForAll.Count);

            for (int i = 0; i < SLMode.Count; i++)
            {
                for (int j = 0; j < typesForAll.Count; j++)
                {
                    if (typesForAll[j] == SLMode[i])
                    {
                        countOfTypesForAll[j]++;
                    }
                }
            }

           //for (int i = 0; i < countOfTypesForAll.Count; i++)
                //Console.WriteLine("countOfTypesForAll"+i+"  "+ countOfTypesForAll[i]);

            // Math.Log(125, 5);//以5为底，125的对数
            for (int i = 0; i < typesForAll.Count; i++)
            {
                double counts =(double) countOfTypesForAll[i];//做一下隐式转换
                double P = counts / (double)SLMode.Count;//频率当概率使用
                inforSL += -P * Math.Log(P, 2);
            }
            Console.WriteLine("Basic Value = " + inforSL);
        }

        //--------------------------------------------------------------------------------------------------------------------------//
        //计算每一种属性对目标值的影响
        //全局唯一计算一次其余的分项就可以了
        private void makeEffectValues()
        {
            //要选择当前所有还需要分的属性中最重要的那个属性
            //Map是当前属性存储的集合
            for (int mapIndex = 0; mapIndex < MAP.Count; mapIndex++)
            {
                List<int> types = new List<int>();//有多少种类型
                List<int> countOfTypes = new List<int>();//每一种类型有多少
                
                for (int i = 0; i < MAP[mapIndex].Count; i++)
                {
                    if (types.Contains(MAP[mapIndex][i]) == false)
                    {
                        types.Add(MAP[mapIndex][i]);
                    }
                }

                //for (int i = 0; i < types.Count; i++)
                //   Console.WriteLine("branceType = "+ types[i]);

                for (int i = 0; i < types.Count; i++)
                    for(int j = 0; j < typesForAll.Count; j++)//因为要记录每一种的小项目中，针对大项目的数量，需要做百分比
                    countOfTypes.Add(0);

                Console.WriteLine("countOfTypes: " + countOfTypes.Count);

                int startIndex = -1;
                int offsetIndex = -1;
                //每一种小的类型分别有多少
                for (int k = 0; k < MAP[mapIndex].Count; k++)
                {
                    for (int i = 0; i < types.Count; i++)
                    {
                        //对于每一个数据多需要做处理
                        if(MAP[mapIndex][k] == types[i])//如果当前数据由某一个类型
                        {
                            startIndex = i;//基础index
                           //找到这个该数据在本小类中的类型
                            //Console.WriteLine("theType of this :" + types[i]);
                        }
                    }
                    //计算偏移量
                    //这与我的数据排布方式有关
                    for (int j = 0; j < typesForAll.Count; j++)
                    {
                        //横向处理
                        if (SLMode[k] == typesForAll[j])
                        {
                            offsetIndex = j;
                            //找到这个该数据在本小类中的类型
                            //Console.WriteLine("All Type of this :" + typesForAll[j]);
                        }
                    }
                    int indexUse = typesForAll.Count * startIndex + offsetIndex;
                    countOfTypes[indexUse]++;
                }

                //for (int i = 0; i < countOfTypes.Count; i++)
                //    Console.WriteLine("countOfTypes[" + i + "] = " + countOfTypes[i]);
                //每一种小的类型对于大的类型有多少
                //并开始套用公式
                double inforValue = 0;
                for (int i = 0; i < types.Count; i++)
                {
                    double allcountForThisType = 0;
                    for (int j = 0; j < typesForAll.Count; j++)
                    {
                        allcountForThisType += (double)countOfTypes[i+j];//做一下隐式转换
                    }
                    double P1 = allcountForThisType / MAP[mapIndex].Count;
                    for (int j = 0; j< typesForAll.Count; j++)
                    {

                        double P2 = (double)countOfTypes[i + j] / allcountForThisType;//频率当概率使用
                        inforValue += -P1*P2 * Math.Log(P2, 2);
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
                return;//全部拆分出去了就意味着书已经完成了（材料用光）

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
            for (int i = 0; i < theFatherPoint.MAP[index].Count; i++)
            {
                if (types.Contains(theFatherPoint.MAP[index][i]) == false)
                {
                    types.Add(theFatherPoint.MAP[index][i]);
                }
            }
            Console.WriteLine("------------------------------------------------");
            Console.WriteLine("Father is "+ theFatherPoint.name+"-"+theFatherPoint.effectValue );
            for (int i = 0;i < types.Count; i++)
            {
                Console.WriteLine("Child is " + theFatherPoint.Titles[index] +"-"+types[i]);
                theDecisionTreeNode thePoint = new DecisionTree.theDecisionTreeNode(theFatherPoint.Titles[index], types[i]);
                theFatherPoint.childs.Add(thePoint);
                //将选取出的数据最大（影响最大的）从MAP中删除
                //并为这一项建立节点（每一个type都是一个节点）
                //材料用掉，这个最大的影响力的节点属性已经不会被这个属性的子节点使用了
                thePoint.makeValues(theFatherPoint.MAP, theFatherPoint.inforValues, theFatherPoint.Titles ,index);
            }
            for (int i = 0; i < theFatherPoint.childs.Count; i++)
            {
              //递归创建决策树
               makePoint(theFatherPoint.childs[i]);
            }
        }




        //数据分成四类
        private int getTypeIndex(double Value = 0)
        {
            if (Value < 0.25)
                return 1;
            if (Value >= 0.25 && Value < 0.5)
                return 2;
            if (Value >= 0.5 && Value  < 0.75)
                return 3;
            else
                return 4;
        }
    }
}
