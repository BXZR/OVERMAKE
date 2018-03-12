using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace socketServer.Codes
{
    //这个类专门用来记录LOG文件

    //log的标题
    public enum LogType { information , error }
    class Log
    {
        private static FileSaver theFileSaver;//文件读写单元
        private static List<string> theLogBuffer;
        private static int LogBufferMaxCount = 20;//超过这些log自动保存

        private static void makeStartIfNull()
        {
            if (theLogBuffer == null)
                theLogBuffer = new List<string>();
            if (theFileSaver == null)
                theFileSaver = new socketServer.FileSaver();
        }

        public static void saveLog(LogType theType, string theInformation)
        {
            makeStartIfNull();
            string saveString = "[" + theType.ToString() + " " + DateTime.Now.ToString("yyyy:MM:dd:hh:mm:ss") + "]: " + theInformation;
            theLogBuffer.Add(saveString);
            if (theLogBuffer.Count > LogBufferMaxCount)
                writeLogToFile();
        }

        public static void writeLogToFile()
        {
            try
            {
                theFileSaver.saveInformation(theLogBuffer, SystemSave.SystemLogPath);
                theLogBuffer.Clear();
            }
            catch
            {
                Console.WriteLine("暂时无法保存日志，因为初始化还没有完成");
            }
        }
    }
}
