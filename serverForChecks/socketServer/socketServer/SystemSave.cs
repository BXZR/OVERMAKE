using System.Collections.Generic;
using System.Windows.Media;

namespace socketServer
{
    //这个类专门用来记录
    class SystemSave
    {
        public static int stepCount = 0;//被保存下来的前几个阶段的步数
        public static int stepCount2 = 0;//被保存下来的前几个阶段的步数
        public static int allStepCount = 0;//总步数，去除了不可能项之后的总步数
        public static int pictureNumber = 0;//生成图像数量，也表示产生的分组的数量

        public static int buffCount = 400;//缓冲区大小
        //需要注意的是缓冲区的大小需要跟countUseX相同（至少在当前图像生成策略中如此）

        //图像的长和宽，也是规定的每一个阶段的数据的数量
        //此外，当前countUseX，countUseY还是根据数据生成图像的分辨率
        public static int countUseX = 400;//每400条数据做一张图片
        public static int countUseY = 300;//这个是重复的

        public static List<transForm> savedPositions = new List<transForm>();

        //颜色设置
        public static Color theOldColor = Colors.Magenta;//绘制颜色（之前的旧轨迹）
        public static Color theNewColor = Colors.Black;//绘制颜色（当前的轨迹）
        public static Color theNewColor2 = Colors.Orange;//绘制颜色（当前的轨迹  ，方法2的颜色）
        //应该还有一个对比用的颜色，但是在这里先不写



        public static int getValuesCount (int valueNow = 0)//传入的是未计入分组的数据信息
        {
            return pictureNumber * buffCount + valueNow;
        }
    }
}
