using System;
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
        }
    }
}
