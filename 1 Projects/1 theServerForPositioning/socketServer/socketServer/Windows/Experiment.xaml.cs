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
    /// Interaction logic for Experiment.xaml
    /// </summary>
    public partial class Experiment : Window
    {

        //保留引用，因为所有的数据都是从这里出来的
        public MainWindow theMainWindow = null;

        public Experiment()
        {
            InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            if (theMainWindow == null)
            {
                MessageBox.Show("初始化未完成");
                return;
            }
            else//正式显示数据
            {
                //为了保证数据干净，要做一次滤波
                // List<double> theFiltered =  theFilter.theFilerWork(theInformationController.compassDegree);
                List<double> theFiltered = theMainWindow.theStepAngeUse;
                if (theFiltered == null)
                    return;
                //查看滤波后的数据
                // string IS = theFiltered.Count + "条数据\n";
                //for (int i = 0; i < theFiltered.Count; i++)
                //    IS += theFiltered[i] + "\n";
                // theFileSaver.saveInformation(IS);
                // MessageBox.Show("GET:\n"+IS);

                ChartWindow theChartWindow = new ChartWindow();
                theChartWindow.CreateChartSpline(UseDataType.compassDegree, theFiltered);
                theChartWindow.Show();
            }
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            //展示“滤波”之后的加速度的曲线
            List<double> theFiltered = theMainWindow. theFilteredAZ;
            // List<double> theFiltered = 
            if (theFiltered == null)
                return;
            //查看滤波后的数据
            //string IS = theFiltered.Count + "条数据\n";
            //for (int i = 0; i < theFiltered.Count; i++)
            //    IS += theFiltered[i] + "\n";
            // theFileSaver.saveInformation(IS);
            // MessageBox.Show("GET:\n"+IS);

            //滤波之后做各种分析，第一项是步态分析，判断走了多少步
            int stepCount = theMainWindow. thePeackFinder.countStepWithStatic(theFiltered);

            ChartWindow theChartWindow = new ChartWindow();

            theChartWindow.CreateChartSpline(UseDataType.accelerometerY, theFiltered, theMainWindow.stepCheckAxisUse.SelectionBoxItem.ToString());
            theChartWindow.Show();
            //MessageBox.Show("一共" + stepCount + "步");
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            new insectUnityPlay().Show();
        }
    }
}
