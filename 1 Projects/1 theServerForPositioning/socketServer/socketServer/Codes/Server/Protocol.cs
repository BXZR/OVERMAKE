using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace socketServer
{
    //这个类专门用来处理数据解析
    //解析的是从客户端发送给服务端的字符串
    class Protocol
    {

        //解析数据信息的标题
        public string getInformaitonTitle(string threInformationGet = "")
        {
           return  threInformationGet.Split(';')[0].Split('+')[0].Trim();
        }


        //对获取的字符串进行切割的做法打包方法
        //对获取数据的解析
         public  void getandMakeInformation(string information, information theInformationController)
        {
            //Console.WriteLine("\n------------------\n"+information+ "\n------------------\n");
            //其实这个方法有非常冗余的封装，但是为了保证可扩展性可读性，暂时先不改了
            //获取了消息information,处理过程需要新一层的封装了
            //把信息存入到缓存里面去
            //要保存多个数据，这里就需要做一下切分，也就是所谓的协议
            //暂定的协议： 
            //传输内容的大项目用';'切分
            //传输内容的小项目用','切分
            if (string.IsNullOrEmpty(information))
                return;

            string[] theSplited = information.Split(';');
            //因为信息的第一项是用来做报头了
            //多段的报头，操作标记，数位标记等等，中间以“+”分割
            if (theSplited[0].Split('+')[1]  == "A")
            {
                for (int i = 1; i < theSplited.Length; i++)
                {
                    //实际上下面所有的信息都会被存储，所以可以保证下标保持对应
                    switch (i)
                    {
                        //第一大项： Y轴加速度
                        case 1: { theInformationController.addInformation(UseDataType.accelerometerY, theSplited[1]); } break;
                        //第二大项： 直接从unity里面获取到的角度(最先先用这个做，后期自己优化，本项也可以作为一个基础对照项)
                        case 2: { theInformationController.addInformation(UseDataType.compassDegree, theSplited[2]); } break;//正北0度                                                                                 //第三大项： X轴加速度
                        //第三大项： X轴加速度
                        case 3: { theInformationController.addInformation(UseDataType.accelerometerX, theSplited[3]); } break;
                        //第四大项： Z轴加速度
                        case 4: { theInformationController.addInformation(UseDataType.accelerometerZ, theSplited[4]); } break;
                        //第五大项： X轴陀螺仪
                        case 5: { theInformationController.addInformation(UseDataType.gyroX, theSplited[5]); } break;
                        //第六大项： Y轴陀螺仪
                        case 6: { theInformationController.addInformation(UseDataType.gyroY, theSplited[6]); } break;
                        //第七大项： Z轴陀螺仪
                        case 7: { theInformationController.addInformation(UseDataType.gyroZ, theSplited[7]); } break;
                        //第八大项： X轴磁力计
                        case 8: { theInformationController.addInformation(UseDataType.magnetometerX, theSplited[8]); } break;
                        //第九大项： y轴磁力计
                        case 9: { theInformationController.addInformation(UseDataType.magnetometerY, theSplited[9]); } break;
                        //第十大项： z轴磁力计
                        case 10: { theInformationController.addInformation(UseDataType.magnetometerZ, theSplited[10]); } break;
                        //GPS
                        case 11: { theInformationController.addInformation(UseDataType.GPS, theSplited[11]); } break;
                        //时间戳
                        case 12: { theInformationController.addInformation(UseDataType.timeStamp, theSplited[12]); } break;
                        //AHRSZ信息
                        case 13: { theInformationController.addInformation(UseDataType.AHRSZ, theSplited[13]); /*Console.WriteLine("13 => " + theSplited[13]);*/ } break;
                        //IMU信息
                        case 14: { theInformationController.addInformation(UseDataType.IMUZ, theSplited[14]); /*Console.WriteLine("14 => " + theSplited[14]);*/ } break;
                    }
                }
            }
            else if (theSplited[0].Split('+')[1] == "B")
            {
                //如果网络带宽实在是不行，就考虑用这种分片的方法分着发送。
                //这一点在客户端上也留有接口
                /*
                for (int i = 1; i < theSplited.Length; i++)
                {
                    //实际上下面所有的信息都会被存储，所以可以保证下标保持对应
                    switch (i)
                    {
                        //GPS
                        case 1: { theInformationController.addInformation(UseDataType.GPS, theSplited[1]); } break;
                        //时间戳
                        case 2: { theInformationController.addInformation(UseDataType.timeStamp, theSplited[2]); } break;
                        //AHRSZ信息
                        case 3: { theInformationController.addInformation(UseDataType.AHRSZ, theSplited[3]); } break;
                        //IMU信息
                        case 4: { theInformationController.addInformation(UseDataType.IMUZ, theSplited[4]); } break;

                    }
                }
                */
                            }
        }

        //获得发送数据的内容并返回
        public  string makeSendToClients(MainWindow theWindow = null)
        {
            string sendString = "";
            if (theWindow == null)
            {
                sendString = SystemSave.allStepCount.ToString();
                sendString += ";" + SystemSave.stepLengthNow.ToString("f2");
                sendString += ";" + SystemSave.stepAngleNow.ToString("f2");
                sendString += ";" + SystemSave.slopNow.ToString("f2");
                sendString += ";" + SystemSave.heightNow.ToString("f2");
            }
            else
            {
                sendString += theWindow.allStepCount.ToString();
                sendString += ";" + theWindow.stepLengthNow.ToString("f2");
                sendString += ";" + theWindow.stepAngleNow.ToString("f2");
                sendString += ";" + theWindow.slopNow.ToString("f2");
                sendString += ";" + theWindow.heightNow.ToString("f2");
            }
            return sendString;
        }


        //客户端对服务端的操作是有一个集合的，而这个集合就是在这个地方
        //这种操作的信息格式与一般data的信息内容差不多
        //格式为 “信息头;操作内容;参数”
        public void clientOperateServer(string informartionIn, MainWindow theMainwindow)
        {
            string[] theSplited = informartionIn.Split(';');
            //数据长度不对，不允许操作
            if (theSplited.Length < 2)
                return;
            //获得操作类型
            string operateType = theSplited[1];
            //获得操作参数
            //string[] para = new  string[theSplited.Length-2];
            //theSplited.CopyTo(para ,2);
            //正式开始操作
            switch(operateType)
            {
                //绘制重定位，当前位置变为地图中心，消除掉轨迹内容，从当前位置开始重新定位
                case "flashPosition":
                    {
                        Console.WriteLine("start operte with client's conmmand");
                        theMainwindow.operateFlashPosition();
                        Console.WriteLine("theClientoperate: flashPosition");
                    }
                    break;
            }
        }


    }
}
