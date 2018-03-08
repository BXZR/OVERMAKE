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
            Bitmap bmp = new Bitmap(SystemSave.countUseX, SystemSave.countUseY);

            for (int i = 0; i < bmp.Height; i++)
                for (int j = 0; j < bmp.Width; j++)
                {
                    Color c = Color.FromArgb
                     (
                        255 ,
                        //(int)theInformationController.compassDegree[j]* 255 / 360,
                        ((int)theInformationController.accelerometerX[j]+7) * 10,
                        ((int)theInformationController.accelerometerY[j]+7)* 10,
                        ((int)theInformationController.accelerometerZ[j]+7)* 10
                         
                      );
                   // Console.WriteLine(c.R  +" "+ c.G +" "+ c.B +"");
                    bmp.SetPixel(j, i, c);

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
            string pictureName = "dataPictureComplex" + SystemSave.pictureNumber + ".jpeg";
            SystemSave.pictureNumber++;

            //千万注意读写路径问题
            //此外在这里需要深拷贝，否则会报错（GDI+）
            Bitmap bmp = new Bitmap(SystemSave.countUseX, SystemSave.countUseY);
            //以后可以用这种套路做扩展
            for (int i = 0; i < bmp.Height; i += 2)
            {
                for (int j = 0; j < bmp.Width; j++)
                {
                    Color c = Color.FromArgb
                     (
                        255,
                        //(int)theInformationController.compassDegree[j]* 255 / 360,
                        ((int)theInformationController.accelerometerX[j] + 7) * 10,
                        ((int)theInformationController.accelerometerY[j] + 7) * 10,
                        ((int)theInformationController.accelerometerZ[j] + 7) * 10
                      );
                    // Console.WriteLine(c.R  +" "+ c.G +" "+ c.B +"");
                    bmp.SetPixel(j, i, c);
                }
                //for (int j = 0; j < bmp.Width; j++)
                //{
                //    Color c = Color.FromArgb
                //     (
                //        255,
                //        //(int)theInformationController.compassDegree[j]* 255 / 360,
                //        (int)(theInformationController.compassDegree[j] * 200f / 360f),
                //        (int)(theInformationController.compassDegree[j] * 200f/360f),
                //        255
                //      );
                //    // Console.WriteLine(c.R  +" "+ c.G +" "+ c.B +"");
                //    bmp.SetPixel(j, i+1, c);
                //}
            }
            Bitmap imgSave = new Bitmap(bmp);
            string savePath = path + pictureName;
            imgSave.Save(savePath, System.Drawing.Imaging.ImageFormat.Jpeg);//指定图片格式   
            imgSave.Dispose();
        }



        //概念性的方法，当然需要优化
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
        public void ScaleCanvasBack(System.Windows.Controls.Canvas theCanvas  , float scale)
        {
            theCanvas.Children.Clear();

            //RenderTargetBitmap bmp = new RenderTargetBitmap(100, 100, 96, 96, System.Windows.Media.PixelFormats.Pbgra32);
           // bmp.Render(theCanvas);
           // var enc = new System.Windows.Media.Imaging.PngBitmapEncoder();
           // enc.Frames.Add(System.Windows.Media.Imaging.BitmapFrame.Create(bmp));

            //using (var stm = System.IO.File.Create("B.png"))
           // {
           //     enc.Save(stm);
           // }
            

           // System.Windows.Media.ImageBrush ib = new System.Windows.Media.ImageBrush();
           // ib.ImageSource = bmp;
          //  theCanvas.Background = ib;
        }

 
    }
}
