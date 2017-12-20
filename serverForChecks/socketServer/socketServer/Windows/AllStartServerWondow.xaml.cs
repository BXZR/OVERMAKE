using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

namespace socketServer.Windows
{
    /// <summary>
    /// Interaction logic for AllStartServerWondow.xaml
    /// </summary>
    public partial class AllStartServerWondow : Window
    {
        public AllStartServerWondow()
        {
            InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SystemSave.serverIP = ALLIP.Text;
                SystemSave.serverPort = Convert.ToInt32(ALLPort.Text);

                theServer theServerForAll = new socketServer.theServer(ALLIP.Text, Convert.ToInt32(ALLPort.Text));
                theServerForAll.setMode(2);
                SystemSave.SystemServerMode = 2;
                //制作显示用的label
                string informationS = "";
                informationS += theServerForAll.startTheServer();
                informationS += "\n------------------------------------------------------------";
                informationS += "\n正在侦听来自手机客户端的连接......";
                informationS += "\n一个新的连接到来时将会打开一个新的窗口进行实验";
                informationS += "\n进行实验时请尽量保持此窗口不关闭";
                informationS += "\n------------------------------------------------------------";
                informationS += "\n注意：\n每一个客户端窗口中的方法可分别选择\n但所有客户端窗口共享设定窗口中的内容";
                theShowLabel.Content = informationS;
                //为了防止乱按按钮，直接将button给禁了
                button.IsEnabled = false;
                button2.IsEnabled = false;
            }
            catch (Exception E)
            {
                //恢复原先的数值
                SystemSave.SystemServerMode = 1;
                MessageBox.Show("IP和端口设置不正确\n请设置后重新尝试\n"+ E.Message);
            }
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            SystemSave.SystemServerMode = 1;
            SystemSave.serverIP = ALLIP.Text ;
            SystemSave.serverPort = Convert.ToInt32( ALLPort.Text);
            MainWindow aMainWindow =  new MainWindow();
            aMainWindow.pressStartButton();
            aMainWindow.Show();
            this.Close();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (SystemSave.theServrForAll != null)
                SystemSave.theServrForAll.closeServer();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //ALLIP.Text = SystemSave.serverIP;
            ALLPort.Text = SystemSave.serverPort.ToString();
            ALLIP.Text = SystemSave.getIPAddress();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            ALLIP.Text = SystemSave.getIPAddress();
        }
    }
}
