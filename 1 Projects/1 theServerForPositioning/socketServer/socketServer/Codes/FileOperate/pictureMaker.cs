using socketServer.Codes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace socketServer
{
 
    //这个类将数据进行排布，做成图像，供给tensorflow模块识别
    //关于实时性的问题还需要好好想
    class pictureMaker
    {


        //暂定是每一行代表400组数据
        //一共重复300行

        //真正用的方法
        //输入的是整体控制集合，
        public void createPictureFromData(information theInformationController, string path = @"DataForPDR/DataImage/")
        {
            if (theInformationController.accelerometerX.Count < SystemSave. countUseX)
                return;//数据不足就不处理


            Log.saveLog(LogType.information, "保存一张数据生成图");
            string pictureName = "dataPicture" + SystemSave.pictureNumber +".jpeg";
            SystemSave.pictureNumber++;

            //千万注意读写路径问题
            //此外在这里需要深拷贝，否则会报错（GDI+）
            int heightForData = SystemSave.BuffCount / SystemSave.countUseX;
            Bitmap bmp = new Bitmap(SystemSave.countUseX, SystemSave.countUseY);
            int moreCount = SystemSave.countUseY / heightForData;//重复的行数
       
            for(int  k = 0; k < moreCount; k++ )
              for (int i = 0; i < heightForData ; i++)
                for (int j = 0; j < bmp.Width; j++)
                {
                        int R = MathCanculate.Clamp(((int)theInformationController.accelerometerX[j] + 7) * 10, 0, 255);
                        int G = MathCanculate.Clamp(((int)theInformationController.accelerometerY[j] + 7) * 10, 0, 255);
                        int B = MathCanculate.Clamp(((int)theInformationController.accelerometerZ[j] + 7) * 10, 0, 255);
                    Color c = Color.FromArgb( 255 , R ,G ,B);
                    bmp.SetPixel(j, i+k* heightForData, c);
                }
            Bitmap imgSave = new Bitmap(bmp);
            string savePath = path + pictureName;
            imgSave.Save(savePath, System.Drawing.Imaging.ImageFormat.Jpeg);//指定图片格式   
            imgSave.Dispose();
        }

        //真正用的方法
        //输入的是整体控制集合，这可能是以后会用到的更加复杂的方法
        //需要考虑数据排布
        public void createPictureFromDataComplex(information theInformationController, string path = @"DataForPDR/DataImage/")
        {
            if (theInformationController.accelerometerX.Count < SystemSave.countUseX)
                return;//数据不足就不处理

            Log.saveLog(LogType.information, "保存一张数据生成图");
            string pictureName = "";
            //这只是一个假说，很短时间内的内的几步不会变化特别大，所以可以认为步长是在同一个type类型中
            //这样这个图片就算是打上了标记，卷积分类的时候有了依据
            //具体卷积做法有可能需要自行处置，并且这个部分是需要在额外的延迟线程中进行计算
            int typeIndex = SystemSave.getTypeIndex(SystemSave.stepLengthNow);
            pictureName = typeIndex + "-dataPictureComplex" + SystemSave.pictureNumber + ".jpeg";

            SystemSave.pictureNumber++;

            //千万注意读写路径问题
            //此外在这里需要深拷贝，否则会报错（GDI+）
            int heightForData = SystemSave.BuffCount * 4 / SystemSave.countUseX; //因为这里要分四行分别画不同的东西
            int moreCount = SystemSave.countUseY / heightForData;//重复的行数
            Bitmap bmp = new Bitmap(SystemSave.countUseX, SystemSave.countUseY);

            //第一行绘制加速度信息
            for (int k = 0; k < moreCount; k++)
                for (int i = 0; i < heightForData; )
                { 
                    for (int j = 0; j < bmp.Width; j++)
                     {
                        int R = MathCanculate.Clamp(((int)theInformationController.accelerometerX[j] + 7) * 10, 0, 255);
                        int G = MathCanculate.Clamp(((int)theInformationController.accelerometerY[j] + 7) * 10, 0, 255);
                        int B = MathCanculate.Clamp(((int)theInformationController.accelerometerZ[j] + 7) * 10, 0, 255);
                        Color c = Color.FromArgb(255, R, G, B);
                         bmp.SetPixel(j, i + k * heightForData, c);
                     }
                i++;//换到下一行
                //第二行绘制陀螺仪信息
                for (int j = 0; j < bmp.Width; j++)
                {
                        int R = MathCanculate.Clamp(((int)theInformationController.gyroX[j] + 50) * 2, 0, 255);
                        int G = MathCanculate.Clamp(((int)theInformationController.gyroY[j] + 50) * 2, 0, 255);
                        int B = MathCanculate.Clamp(((int)theInformationController.gyroZ[j] + 50) * 2, 0, 255);
                    Color c = Color.FromArgb (  255, R ,G ,B );
                    bmp.SetPixel(j, i + k * heightForData, c);
                }
                i++;//换到下一行
                //第三行绘制磁力计信息
                for (int j = 0; j < bmp.Width; j++)
                {
                        int R = MathCanculate.Clamp(((int)theInformationController.magnetometerX[j] + 50), 0, 255);
                        int G = MathCanculate.Clamp(((int)theInformationController.magnetometerY[j] + 50), 0, 255);
                        int B = MathCanculate.Clamp(((int)theInformationController.magnetometerZ[j] + 50), 0, 255);
                    Color c = Color.FromArgb ( 255, R ,G ,B );
                    bmp.SetPixel(j, i + k * heightForData, c);
                }
                i++;//换到下一行
                //第四行绘制compass heading , AHRS heading , IMU heading 
                for (int j = 0; j < bmp.Width; j++)
                {
                    int R = MathCanculate.Clamp((int)(theInformationController.compassDegree[i] * 255 / 360), 0, 255);
                    int G = MathCanculate.Clamp((int)(theInformationController.AHRSZFromClient[i] * 255 / 360), 0, 255);
                    int B = MathCanculate.Clamp((int)(theInformationController.IMUZFromClient[i] * 255 / 360), 0, 255);

                    Color c = Color.FromArgb(255, R ,G ,B );
                    // Console.WriteLine(c.R  +" "+ c.G +" "+ c.B +"");
                    bmp.SetPixel(j, i + k * heightForData, c);
                }
                i++;//换到下一行
            }
            Bitmap imgSave = new Bitmap(bmp);
            string savePath = path + pictureName;
            imgSave.Save(savePath, System.Drawing.Imaging.ImageFormat.Jpeg);//指定图片格式   
            imgSave.Dispose();
        }

        //概念性的方法，当然需要优化 （DEMO）
        //angle输入的是角度值
        public void createPictureFromData(double angle = 0 , double AX =0, double AY  =0, double AZ =0, string path = @"DataForPDR/DataImage/", string pictureName = "demo.bmp")
        {
               //千万注意读写路径问题
               //此外在这里需要深拷贝，否则会报错（GDI+）
               Bitmap bmp = new Bitmap(700, 550);
      
                for (int i = 0; i < bmp.Width; i++)
                    for (int j = 0; j < bmp.Height; j++)
                    {
                        Color c = Color.FromArgb((int)angle / 360, (int)AX * 100, (int)AY * 100, (int)AZ * 100);
                        bmp.SetPixel(i, j, c);
                    }
                Bitmap imgSave = new Bitmap(bmp);
                string savePath = path + pictureName;
                imgSave.Save(savePath, System.Drawing.Imaging.ImageFormat.Bmp );//指定图片格式   
                imgSave.Dispose();

        }

//------------------------------------------------------------------------------------------------------------------------------------------------------//
//此外保存canvas的方法也是保存在这里的
        public void saveCanvasPicture(System.Windows.Controls.Canvas theCanvas , String fileName)
        {
             System.Windows.Size theSize = new System.Windows.Size((int)theCanvas.ActualWidth, (int)theCanvas.ActualHeight);
             theCanvas.Measure(theSize);
             theCanvas.Arrange(new System.Windows.Rect(theSize));
            var rtb = new RenderTargetBitmap
                (
                (int)(theSize.Width*1.0), //width
                (int)(theSize.Height*1.2), //height
                (int) 100,
                (int) 100,
                System.Windows.Media.PixelFormats.Pbgra32 // pixelformat
                );
            rtb.Render(theCanvas);

            var enc = new System.Windows.Media.Imaging.PngBitmapEncoder();
            enc.Frames.Add(System.Windows.Media.Imaging.BitmapFrame.Create(rtb));

            using (var stm = System.IO.File.Create(fileName))
            {
                enc.Save(stm);
            }
        }

        //修改Canvas图片的分辨率，但是要保证Canvas不变化，所以需要使用bitmmap来做
        //目前暂时也没什么好方法来处理
        //牵扯的东西比较多
        //1 背景图的放大缩小，尤其是缩小
        //2 背景图的拖拽移动和当前的绘制方式是不一样的，所以暂时不能用
        public void ScaleCanvasBack(System.Windows.Controls.Canvas theCanvas  , float scale)
        {
            theCanvas.Children.Clear();

            //RenderTargetBitmap bmp = new RenderTargetBitmap(100, 100, 96, 96, System.Windows.Media.PixelFormats.Pbgra32);
            //bmp.Render(theCanvas);
            //var enc = new System.Windows.Media.Imaging.PngBitmapEncoder();
            //enc.Frames.Add(System.Windows.Media.Imaging.BitmapFrame.Create(bmp));

            //using (var stm = System.IO.File.Create("B.png"))
            //{
            //    enc.Save(stm);
            //}

            //System.Windows.Media.ImageBrush ib = new System.Windows.Media.ImageBrush();
            //ib.ImageSource = bmp;
            //theCanvas.Background = ib;
        }

    }
}
