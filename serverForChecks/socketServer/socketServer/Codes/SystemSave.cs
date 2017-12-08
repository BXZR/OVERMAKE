﻿using socketServer.Codes.DecisionTree;
using socketServer.Windows;
using System.Collections.Generic;
using System.Windows.Media;

namespace socketServer
{
    //这个类专门用来记录
    class SystemSave
    {

        //系统设置的相关参数
        public static string serverIP = "219.216.78.119";//程序服务器的IP
        public static int serverPort = 8886;//程序服务器的端口
        public static int lengthForBuffer = 2048;//服务器缓冲区大小（1024不够用）
        public static theServer theServrForAll;//所有窗口共享唯一的一个网络服务器

        //默认初始坐标（,这个坐标首先可以在设置界面进行设置，若有条件就在使用一些定位的方法获得）
        public static double startPositionX = 0;
        public static double startPositionY = 0;
        public static double startPositionZ = 0; 

        //有些窗口应该保持单例模式
        public static Settings theSettingWindow = null;//全局唯一设定窗口
        public static Appendix theAppendixWindow = null;//全局唯一附录窗口

        //如果正在上楼梯，这个标记实际上需要根据数据得到
        //这个标记的作用是开/关Z轴的计算
        public static bool isStairsUp = false;
        public static double StairHeight = 0.18;//每一阶台阶的高度

        public static int stepCount = 0;//被保存下来的前几个阶段的步数
        public static int stepCount2 = 0;//被保存下来的前几个阶段的步数
        public static int allStepCount = 0;//总步数，去除了不可能项之后的总步数
        public static int pictureNumber = 0;//生成图像数量，也表示产生的分组的数量

        public static int buffCount = 400;//缓冲区大小(程序中信息缓冲区，并不是socket的缓冲区)
        //需要注意的是缓冲区的大小需要跟countUseX相同（至少在当前图像生成策略中如此）

        //图像的长和宽，也是规定的每一个阶段的数据的数量
        //此外，当前countUseX，countUseY还是根据数据生成图像的分辨率
        public static int countUseX = 400;//每400条数据做一张图片
        public static int countUseY = 300;//这个是重复的

        public static List<transForm> savedPositions = new List<transForm>();
        public static double stepAngleNow = 0;//记录最新的移动方向
        public static double stepLengthNow = 9.5f;//记录最新的移动步长
        public static double slopNow = 0.00;//当前记录的最新的slop
        //颜色设置
        public static Color theOldColor = Colors.Magenta;//绘制颜色（之前的旧轨迹）
        public static Color theNewColor = Colors.Black;//绘制颜色（当前的轨迹）
        public static Color theNewColor2 = Colors.Orange;//绘制颜色（当前的轨迹  ，方法2的颜色）
                                                         //应该还有一个对比用的颜色，但是在这里先不写

        //是不是增加角度偏移量
        public static bool UseHeadingOffset = false;
        public static int getValuesCount (int valueNow = 0)//传入的是未计入分组的数据信息
        {
            return pictureNumber * buffCount + valueNow;
        }

        //每一个阶段的数据收集，系统面板也会有一些东西需要刷新的
        public static void makeFlash()
        {
           
        }
        //波峰波谷判断走一步的方法中，间隔一定的数据量才能够判别唯一不，这个就是数据间隔量（高度滤波之后的）
        public static int peackThresholdForStepDetection = 2;
        //手机在口袋中的时候的偏差值
        public static int angleOffset = 130;
        //使用零点交叉方法判断走一步的时候的初始零点（因为不同手机有不同的零点偏差，我的手机就是-1）
        public static double zeroCrossOffset = -0.75;
        //是否使用buffer绘制路线图
        public static bool drawWithBuffer = true;
        //默认行人身高
        public static double Stature = 1.7234;
        //男性步长身高加权值
        public static double WeightForMale = 0.415;
        //女性步长身高加权值
        public static double WeightForFemale = 0.413;
        //是否是男人
        public static bool isMale = true;
        //最大加速度和最小加速度的差开四次根号的步长计算方法的参数K
        public static double stepLengthWeightForAMinus = 0.71;//注意论文中是0.41但是这个参数不太适合本工程
        //Scarlet步长估计方法的参数
        public static double stepLengthWeightForScarlet = 1.1;//注意论文中是0.81但是这个参数似乎不太适合与本工程
        //kim步长计算方法
        public static double stepLengthWeightForKim = 0.65;//论文中的是0.55
        //peack判步方法2需要在设定一些参数
        public static double maxAForStart = -0.85;
        public static double minAForStart = -1.15;
        public static double Dertshold = 0.2;
        public static double uperGateForStart = -0.85;
        public static double downGateForStart = -1.15;
        public static double uperGateForShow= -0.85;//记录后来的数值用以显示
        public static double downGateForShow= -1.15;//记录后来的数值用以显示
        //是否使用动态零线（这个领先就是判步方法中zerrocross的方法）
        //但是使用这个方法可能会造成没什么必要的额外计算
        public static bool isDynamicallyZeroLineForStepDection = false;

        //构建决策树所需要的文件路径
        //事实上所有的数据集都用这个文件
        public static string TrainBasedFile = "TrainBase/TrainBaseTree.txt";
        //决策树算法类型 0:IC3  1:C4.5
        public static int DecisionTreeMethodID = 0;
        public static bool isCutForDecisionTree = false;//决策树是不是要剪枝？默认不剪枝，用来对比
        public static theDecisionTree StepLengthTree = null;//步长方法中的决策树
        public static theDecisionTree StairTree = null;//判断走楼梯的方向的决策树

        //实验的时候手机的模式
        //0 手机是平放的，上下平移没有摆动
        //1 手机是随意按照一定规律摆动的
        public static int CanculateHeadingMode = 0;
        public static bool CHM1Sampled = false;
        public static int sampleTime = 5;//采样保证正确性
        public static double startAngleForCHM1 = 0;//起始的角度
        //计算偏移量的时候总该有一个准确的当前方向，但是没有就只能输入了
        //偏移量
        //得到这个偏移量之后就可以摆动移动了
        public static double headingOffsetFor3DHeading = 0;

        //腿长，在使用倒置钟摆步长计算方法的时候会用到
        private  static double legLength = 80;
        public static double legLengthInHeight = 0.455;//身高的0.455是腿长
        //这个参数可以考虑在后期扩展
        public static double getLegLength()
        {
            legLength = Stature * legLengthInHeight;
            return legLength;//这其实就是一个get方法
        }

        //滤波器的中平滑平均的数量，因为考虑到有积分的时候，所以需要把这个作为一参数来做
        //积分滤波后的数据的时候需要乘以这个参数
        public static int filterSmoothCount = 5;

        //路线图绘制缩放值，为了适应不同的路线大小每走一步的距离不一定就是那么大，要有缩放
        public static double routeLineScale = 5;
    }
}
