using socketServer.Codes.DecisionTree;
using socketServer.Windows;
using System.Collections.Generic;
using System.Windows.Media;

namespace socketServer
{
    //这个类专门用来记录
    class SystemSave
    {

        //系统设置的相关参数
        public static string serverIP = "219.216.78.111";//程序服务器的IP
        public static int serverPort = 8886;//程序服务器的端口
        public static int lengthForBuffer = 2048;//服务器缓冲区大小（1024不够用）
        public static theServer theServrForAll;//所有窗口共享唯一的一个网络服务器
        public static int SystemModeInd = 0;//系统模式，有实验模式和实际模式
        public static int SystemServerMode = 1;//1 单人使用 2 多人使用
        public static int FilterMode = 1;//滤波模式 0 不用滤波 1 一般滤波 2 复杂滤波
        public static double systemFlashTimer = 0.5;//系统绘制和计算的时间间隔，时间越短刷新越快灵敏越高开销越大
        //两种模式的区别就是实验模式之下在原地晃手机就可以移动，但是这种情况在实际模式之下不被允许


        //默认初始坐标（,这个坐标首先可以在设置界面进行设置，若有条件就在使用一些定位的方法获得）
        public static double startPositionX = 0;
        public static double startPositionY = 0;
        public static double startPositionZ = 0; 

        //有些窗口应该保持单例模式
        public static Settings theSettingWindow = null;//全局唯一设定窗口
        public static Appendix theAppendixWindow = null;//全局唯一附录窗口

        //如果正在上楼梯，这个标记实际上需要根据数据得到
        //这个标记的作用是开/关Z轴的计算
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
        public static double heightNow = 0.00;//当前记录的最新Z轴向移动

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

   
        //波峰波谷判断走一步的方法中，间隔一定的数据量才能够判别唯一不，这个就是数据间隔量（高度滤波之后的）
        public static int peackThresholdForStepDetection = 2;
        //手机在口袋中的时候的偏差值
        public static int angleOffset = 130;
        //使用零点交叉方法判断走一步的时候的初始零点（因为不同手机有不同的零点偏差，我的手机就是-1）
        public static double zeroCrossOffset = -0.75;
        //是否使用buffer绘制路线图
        public static bool drawWithBuffer = true;
        //默认行人路长
        public static double stepLengthForImmediate = 0.6;
        //转弯测试阀值
        public static double changeGateForImmediate2 = 60;//转弯的阀值
        //如果转弯且角度差异大于一个阀值，返回的步长信息恐怕需要调整
        //调整的参数如下
        public static double percentWhenMeetChangeGate = 0.4;
        //默认行人身高
        public static double Stature = 1.7234;
        //男性步长身高加权值
        public static double WeightForMale = 0.415;
        //女性步长身高加权值
        public static double WeightForFemale = 0.413;
        //是否是男人
        public static bool isMale = true;
        //身高公式2中可变参数A
        public static double StaturaMethod2_A = 0.371;
        //身高公式2中可变参数A
        public static double StaturaMethod2_B = 0.227;
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
        public static string TrainBasedFile = "TrainBase/TrainBase.txt";
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
        //如果变化超过这个数目就认为改变了
        //实际上就是一个来自微软的简单平滑的思路参数
        public static double MSHeadingGate = 5;
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

        //获得本地IP地址的方法
        public static string getIPAddress()
        {
            //这个获取IP地址的方法非常简单粗暴但是不是公网的IP
            // string IP = Dns.GetHostEntry(Dns.GetHostName()).AddressList.FirstOrDefault<IPAddress>(a => a.AddressFamily.ToString().Equals("InterNetwork")).ToString();
            //真正使用的获得IP的手段是下面更细节的方法
            System.Net.IPAddress[] addressList = System.Net.Dns.GetHostByName(System.Net.Dns.GetHostName()).AddressList;
            string IP1 = addressList[0].ToString();
            string IP2 = addressList[1].ToString();
            return IP1;
        }

        public static void makeFlash()
        {

        }
        //数据分类方法
        //其实就是为了适应决策树而进行的属性离散化
        public static int getTypeIndex(double Value = 0)
        {
            //if (Value < 0.25)
            //    return 1;
            //if (Value >= 0.25 && Value < 0.5)
            //    return 2;
            //if (Value >= 0.5 && Value < 0.75)
            //    return 3;
            //else
            //    return 4;
            //-----------------------------------------------------------
            //if (Value < 0.5)
            //    return 1;
            //else
            //    return 2;
            //按照下面公式的数量进行分类
            //分类，同时分类的结果是选择的公式参数组的下标
            double caluetoCheck = 1.0 / CommonFormulaWeights.Count;
            double checker = caluetoCheck;
            for (int i = 0; i < CommonFormulaWeights.Count; i++)
            {
                if (Value < checker)
                    return i;
                checker += caluetoCheck;
            }
            return CommonFormulaWeights.Count - 1;
        }

        //所有的分类都是数组形式的 0 1 2
        //0 下楼
        //1 平走
        //2 上楼
        public static int getTypeIndexForStair(double Value = 0)
        {
            if (Value > 1)
                return 2;
            if (Value == 1)
                return 1;

                return 0;
        }
        //一般步长计算方法参数族群
        //使用决策树,神经网络等等方案选择出来模式，使用这一套模式的参数来做
        //public static  double[] afas = { 0.7, 0.8, 0.9, 1.0 };
        //public static double[] betas = { 0.3, 0.4, 0.5, 0.6 };
        //public static double[] gamas = { 0.1, 0.2, 0.3, 0.4 };
        public static List<double []> CommonFormulaWeights = new List<double[]>()
        {
            new double []{ 0.7,0.3,0.4},
            new double []{ 0.8,0.4,0.2 },
            new double []{ 0.9,0.5,0.3 },
            new double []{ 1.0, 0.6,0.4 },
        };

        //ANN的隐层层数
        public static int accordANNHiddenLayerCount = 5;
        //整体ANN训练的次数
        public static int accordANNTrainTime = 10;
    }
}
