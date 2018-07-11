using socketServer.Codes;
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
                SystemSave.serverIP = comboBoxForIP.SelectionBoxItem.ToString();
                SystemSave.serverPort = Convert.ToInt32(ALLPort.Text);

                theServer theServerForAll = new socketServer.theServer(SystemSave.serverIP, SystemSave.serverPort);
                theServerForAll.setMode(2);
                SystemSave.SystemServerMode = 2;
                //制作显示用的label
                string informationS = "";
                informationS += theServerForAll.startTheServer();
                Log.saveLog(LogType.information, informationS);
                informationS += "\n------------------------------------------------------------";
                informationS += "\n正在侦听来自客户端的连接......";
                informationS += "\n一个新的连接到来时将会打开一个新的窗口进行实验";
                informationS += "\n进行实验时请尽量保持此窗口不关闭";
                informationS += "\n------------------------------------------------------------";
                informationS += "\n注意：\n每一个客户端窗口中的方法可分别选择\n但所有客户端窗口共享设定窗口中的内容";
                theShowLabel.Content = informationS;
                //为了防止乱按按钮，直接将button给禁了
                button.IsEnabled = false;
                button2.IsEnabled = false;
                comboBoxForIP.IsEnabled = false;
                ALLPort.IsEnabled = false;
            }
            catch (Exception E)
            {
                //恢复原先的数值
                SystemSave.SystemServerMode = 1;
                Log.saveLog(LogType.error, "多人模式下IPn/端口设置出现问题，开启失败");
                MessageBox.Show("IP和端口设置不正确\n请设置后重新尝试\n"+ E.Message);
            }
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("IP = " + SystemSave.serverIP);

            SystemSave.SystemServerMode = 1;
            SystemSave.serverIP = comboBoxForIP.SelectionBoxItem.ToString() ;
            Console.WriteLine("IP = " + SystemSave.serverIP);

            SystemSave.serverPort = Convert.ToInt32( ALLPort.Text);
            MainWindow aMainWindow =  new MainWindow();
            aMainWindow.pressStartButton();
            aMainWindow.Show();
            this.Close();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (SystemSave.theServrForAll != null)
            {
                SystemSave.theServrForAll.closeServer();
                SystemSave.theServrForAll = null;//取消引用防止多次关闭
            }
            //窗口关闭的时候做一次Log的保存
            Log.writeLogToFile();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                //这两种也是展示IP的方式，先留在这里
                //ALLIP.Text = SystemSave.serverIP;
                //ALLIP.Text = SystemSave.getIPAddress();

                ALLPort.Text = SystemSave.serverPort.ToString();

                List<string> IPS = SystemSave.getIPAddressAll();
                comboBoxForIP.Items.Clear();
                for (int i = 0; i < IPS.Count; i++)
                {
                    ComboBoxItem Item = new ComboBoxItem();
                    Item.Content = IPS[i];
                    comboBoxForIP.Items.Add(Item);
                }
                comboBoxForIP.SelectedIndex = 0;
            }
            catch
            {
                MessageBox.Show("初始化失败，原因很可能是.netframework不匹配，请安装最新版");
            }
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
           //为了防止出现同步设定的问题，这里使用了简单的单例模式的思想
            if (SystemSave. theSettingWindow == null)
            {
                SystemSave.theSettingWindow = new socketServer.Settings();
                SystemSave.theSettingWindow.startSet(null);
                SystemSave.theSettingWindow.Show();
            }
        }
    }
}
