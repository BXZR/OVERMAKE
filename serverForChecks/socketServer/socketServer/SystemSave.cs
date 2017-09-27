using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace socketServer
{
    //这个类专门用来记录
    class SystemSave
    {
        public static int stepCount = 0;
        public static int pictureNumber = 0;

        //图像的长和宽，也是规定的每一个阶段的数据的数量
        public static int countUseX = 400;//每400条数据做一张图片
        public static int countUseY = 300;//这个是重复的
        public static List<transForm> savedPositions = new List<transForm>();

    }
}
