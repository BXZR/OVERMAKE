﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace socketServer
{
    //这个类用于保存客户端传过来的数据
    //这个类是专门在处理文件I/O的操作类，实际上trainBaseFile等等类也是用这个来做底层调用的
    class FileSaver
    {
        //内部方法得到保存用的文件名
        //主要是为了统一文件名的编辑过程
        private string makeFileName()
        {
            string  fileName = SystemSave.InformationFilePath + "information_"+DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss")  +".txt" ;
            return fileName;
        }

        //如果传入的是一个字符串List
        public void saveInformation(List<string> theList, string fileName = "")
        {
            if (string.IsNullOrEmpty(fileName))
                fileName = makeFileName();//如果没有指定就用默认的

            string saveString= "";
            for (int i = 0; i < theList.Count; i++)
            {
              //  Console.WriteLine(theList[i]);
                saveString += theList[i] + "\n";
            }
            saveInformation(saveString, fileName);
        }

        //单个信息写入适合这种方法
        bool locked  = false;
       public void saveInformation(string information , string fileName = "" )
        {
            if (string.IsNullOrEmpty(fileName))
                fileName = makeFileName();//如果没有指定就用默认的
            //Console.WriteLine(fileName +"---");
            FileStream aFile = new FileStream( fileName , FileMode.Append);
            StreamWriter sw = new StreamWriter(aFile);
            //Console.WriteLine(information);
            sw.Write(information);
            sw.Close();
            sw.Dispose();
        }

       //适合一口气写入所有缓冲区内容到文件的方法
       public void saveInformation2(string information, string fileName = "" )
        {
            if (string.IsNullOrEmpty(fileName))
                fileName = makeFileName();//如果没有指定就用默认的


            StreamWriter sw = new StreamWriter(fileName, true);
            sw.Write(information);
            sw.Close();
            sw.Dispose();
        }
        public string readInformation(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                fileName = "";//如果没有指定就用默认的

            string information;
            try
            {
                StreamReader sw = new StreamReader(fileName, true);
                information = sw.ReadToEnd();
                sw.Close();
                sw.Dispose();
            }
            catch
            {
                information = "";
            }
            return information;
        }

        public static string readFromFile(string path)
        {
            if (string.IsNullOrEmpty(path))
                return  "";//如果没有指定就用默认的

            string information;
            try
            {
                StreamReader sw = new StreamReader(path, true);
                information = sw.ReadToEnd();
                sw.Close();
                sw.Dispose();
            }
            catch
            {
                information = "";
            }
            return information;
        }

        public static string readFromTrainBase()
        {
            string path = SystemSave.TrainBasedFilePath;
            if (string.IsNullOrEmpty(path))
                return "";//如果没有指定就用默认的

            string information;
            try
            {
                StreamReader sw = new StreamReader(path, true);
                information = sw.ReadToEnd();
                sw.Close();
                sw.Dispose();
            }
            catch
            {
                information = "";
            }
            return information;
        }

        public static void deleteDirFiles(string path)
        {
            DirectoryInfo dir = new DirectoryInfo(path);
            FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();  //返回目录中所有文件和子目录
            foreach (FileSystemInfo i in fileinfo)
            {
                if (i is DirectoryInfo)            //判断是否文件夹
                {
                    DirectoryInfo subdir = new DirectoryInfo(i.FullName);
                    subdir.Delete(true);          //删除子目录和文件
                }
                else
                {
                    File.Delete(i.FullName);      //删除指定文件
                }
            }
        }
    }
}