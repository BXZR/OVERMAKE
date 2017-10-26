﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public void createPictureFromData(information theInformationController, string path = @"img/")
        {
            if (theInformationController.accelerometerX.Count < SystemSave. countUseX)
                return;//数据不足就不处理



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
        public void createPictureFromDataComplex(information theInformationController, string path = @"img/")
        {
            if (theInformationController.accelerometerX.Count < SystemSave.countUseX)
                return;//数据不足就不处理

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
        public void createPictureFromData(double angle = 0 , double AX =0, double AY  =0, double AZ =0, string path = @"img/" , string pictureName = "demo.bmp")
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

 
    }
}
