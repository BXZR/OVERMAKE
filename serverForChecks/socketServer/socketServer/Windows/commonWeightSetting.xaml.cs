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

namespace socketServer.Windows
{
    /// <summary>
    /// Interaction logic for commonWeightSetting.xaml
    /// </summary>
    public partial class commonWeightSetting : Window
    {
        public commonWeightSetting()
        {
            InitializeComponent();
        }

        private ListBoxItem theItemToChange;

        public void makeStart(ListBoxItem theItem)
        {
            //保留引用直接修改
            theItemToChange = theItem;
            string information = (string)(theItem).Content;
            information = information.Replace(" ", "");
            string[] values = information.Split(',');
            double AValue = Convert.ToDouble(values[0].Split('=')[1]);
            double BValue = Convert.ToDouble(values[1].Split('=')[1]);
            double CValue = Convert.ToDouble(values[2].Split('=')[1]);
            AT.Text = AValue.ToString("f2");
            BT.Text = BValue.ToString("f2");
            CT.Text = CValue.ToString("f2");
        }
        private void button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //仅仅是用来检查是不是符合格式的
                double VCheck1  = Convert.ToDouble(AT.Text);
                double VCheck2  = Convert.ToDouble(BT.Text);
                double VCheck3 = Convert.ToDouble(CT.Text);

                theItemToChange.Content = string.Format("α = {0} , β = {1} , γ = {2}", VCheck1.ToString("f2"), VCheck2.ToString("f2"), VCheck3.ToString("f2"));
                this.Close();
            }
            catch
            {
                MessageBox.Show("输入格式似乎不对，请输入数字");
            }
        }
    }
}
