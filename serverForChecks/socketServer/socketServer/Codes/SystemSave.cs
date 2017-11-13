using System.Collections.Generic;
using System.Windows.Media;

namespace socketServer
{
    //这个类专门用来记录
    class SystemSave
    {

        //系统设置的相关参数
        public static string serverIP = "219.216.73.162";//程序服务器的IP
        public static int serverPort = 8886;//程序服务器的端口
        public static int lengthForBuffer = 2048;//服务器缓冲区大小（1024不够用）


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


        public static int getValuesCount (int valueNow = 0)//传入的是未计入分组的数据信息
        {
            return pictureNumber * buffCount + valueNow;
        }

        //每一个阶段的数据收集，系统面板也会有一些东西需要刷新的
        public static void makeFlash()
        {
           
        }

        //手机在口袋中的时候的偏差值
        public static int angleOffset = 130;
        //使用零点交叉方法判断走一步的时候的初始零点（因为不同手机有不同的零点偏差，我的手机就是-1）
        public static double zeroCrossOffset = -0.75;
    }
}
