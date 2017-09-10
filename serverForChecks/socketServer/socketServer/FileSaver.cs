using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace socketServer
{
    //这个类用于保存客户端传过来的数据
    class FileSaver
    {

        //如果传入的是一个字符串List
        public void saveInformation(List<string> theList, string fileName = @"information.txt")
        {
            string saveString= "";
            for (int i = 0; i < theList.Count; i++)
                saveString += theList[i] + "\n";
            saveInformation(saveString, fileName);
        }

        //单个信息写入适合这种方法
       public void saveInformation(string information , string fileName = @"information.txt" )
        {
            FileStream aFile = new FileStream( fileName , FileMode.Append);
            StreamWriter sw = new StreamWriter(aFile);
            sw.Write(information);
            sw.Close();
            sw.Dispose();
        }

       //适合一口气写入所有缓冲区内容到文件的方法
       public void saveInformation2(string information, string fileName = @"information.txt" )
        {
            StreamWriter sw = new StreamWriter(fileName, true);
            sw.Write(information);
            sw.Close();
            sw.Dispose();
        }
    }
}
