﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace socketServer
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        public MainWindow theMainWindow = null;//保留引用，还需要调用方法的

        public Settings()
        {
            InitializeComponent();
        }



        public void startSet(MainWindow theIN)
        {
            theMainWindow = theIN;
            if (theMainWindow.isServerStarted() == false)
            {
                saveRestart.IsEnabled = false;//没有开启服务器就没必要重启
            }
            else
            {
                Tip.Content = "检测到服务器已经运行，不推荐修改IP设置。";
            }
        }

        private void button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                SystemSave.serverIP = ServerIPText.Text;
                SystemSave.serverPort = Convert.ToInt32(ServerPortText.Text);
                SystemSave.angleOffset = Convert.ToInt32(packageOffset.Text);
                SystemSave.zeroCrossOffset = Convert.ToDouble(ZeroLine.Text);
                this.Close();
            }
            catch
            {
                MessageBox.Show("输入格式错误");
            }
        }

        private void button_Copy_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SystemSave.serverIP = ServerIPText.Text;
                SystemSave.serverPort = Convert.ToInt32(ServerPortText.Text);
                SystemSave.angleOffset = Convert.ToInt32(packageOffset.Text);
                SystemSave.zeroCrossOffset = Convert.ToDouble(ZeroLine.Text);
                string information =  theMainWindow.makeClose();
                information += "\n-----------------------\n"+theMainWindow.makeStart();
                this.Close();
                MessageBox.Show(information);
               
            }
            catch (Exception eee)
            {
                MessageBox.Show("出错："+eee.Message);
            }
        }

        private void saveRestart_Loaded(object sender, RoutedEventArgs e)
        {
            ServerIPText.Text = SystemSave.serverIP;
            ServerPortText.Text = SystemSave.serverPort.ToString();
            packageOffset.Text = SystemSave.angleOffset.ToString();
            ZeroLine.Text  = SystemSave.zeroCrossOffset.ToString();
            getStartValue();
        }

        //第一次打开窗口的时候记录默认数值
        private static bool hasBasicValue = false;
        private static string ValueServerIPText;
        private static string ValueServerPortText;
        private static string ValuepackageOffset;
        private static string ValueZeroLine;
        void getStartValue()
        {
            if (hasBasicValue == false)
            {
              ValueServerIPText = SystemSave.serverIP;
              ValueServerPortText = SystemSave.serverPort.ToString();
              ValuepackageOffset = SystemSave.angleOffset.ToString();
              ValueZeroLine = SystemSave.zeroCrossOffset.ToString();
              hasBasicValue = true;//最初数值只会被记录一次
           }
        }

        //获取最初的数值
        void setStartValue()
        {
            ServerIPText.Text = ValueServerIPText ;
            ServerPortText.Text = ValueServerPortText ;
            packageOffset.Text = ValuepackageOffset ;
            ZeroLine.Text = ValueZeroLine ;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            setStartValue();
        }
    }
}
