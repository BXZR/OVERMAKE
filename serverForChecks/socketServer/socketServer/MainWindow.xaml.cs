using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace socketServer
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {

        information theInformationController;//信息总控单元，必须要有
        theServer theServerController;//网络服务控制单元，必须有
        FileSaver theFileSaver;//保存到文件中的控制器
        PeackSearcher thePeackFinder;//寻找峰谷的控制单元，用于步态分析

        public MainWindow()
        {
            InitializeComponent();
        }


        private void button1_Click(object sender, RoutedEventArgs e)
        {
            //saveInformation(informationSave);
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            //全世界的初始化都在这里完成
            theInformationController = new information();
            theServerController = new theServer(theInformationController);
            thePeackFinder = new PeackSearcher();
            theFileSaver = new FileSaver();

            //正式开始
            string showInformation = theServerController.startTheServer();
            MessageBox.Show(showInformation);
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
          //  serverSocket.Close();
           // serverSocket.Dispose();
        }

        private void button4_Click(object sender, RoutedEventArgs e)
        {

            string IS = theInformationController.accelerometerY.Count + "条数据\n";
            for (int i = 0; i < theInformationController.accelerometerY.Count; i++)
                IS += theInformationController.accelerometerY[i] + "\n";
            theFileSaver.saveInformation(IS);
            //MessageBox.Show(IS);

            //string IS = theInformationController.allInformation.Count + "条数据\n";
            //for (int i = 0; i < theInformationController.allInformation.Count; i++)
            //    IS += theInformationController.allInformation[i] + "\n";
            //MessageBox.Show(IS);


            int stepCount = thePeackFinder.countSteps( theInformationController .accelerometerY);
            MessageBox.Show("一共"+stepCount+"步");
        }

        private void button3_Click_1(object sender, RoutedEventArgs e)
        {
           string showInformation =  theServerController.closeServer();
           MessageBox.Show(showInformation);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            theServerController.closeServer();
        }


    }
}

        
        