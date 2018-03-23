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


        private void button3_Click_1(object sender, RoutedEventArgs e)
        {
            SLDataGrid.CanUserAddRows = false;
            List<classForStepLengthShow> DataForSLMethod = new List<Windows.classForStepLengthShow>();
            //计算最后五步所有的计算步长的方法得到的步长
            for (int i = 0; i < theMainWindow.StepLengthMethod.Items.Count; i++)
            {
                System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
                stopwatch.Start(); //  开始监视代码
                //----------------------------------------------------------------------------------------------------------------
                theMainWindow.theStepLengthUse.Clear();
                theMainWindow.stepLengthGet(i, theMainWindow.indexBuff, theMainWindow.theFilteredAZ);
                //----------------------------------------------------------------------------------------------------------------
                stopwatch.Stop(); //  停止监视
                TimeSpan timeSpan = stopwatch.Elapsed; //  获取总时间
                double hours = timeSpan.TotalHours; // 小时
                double minutes = timeSpan.TotalMinutes;  // 分钟
                double seconds = timeSpan.TotalSeconds;  //  秒数
                double milliseconds = theMainWindow.theStepLengthUse.Count == 0 ? 0 : timeSpan.TotalMilliseconds / (double)theMainWindow.theStepLengthUse.Count;  //  毫秒数


                Windows.classForStepLengthShow DATA = new Windows.classForStepLengthShow();
                DATA.MethodName = theMainWindow.StepLengthMethod.Items[i].ToString().Split(':')[1];
                DATA.MethodInformaton = theMainWindow.TheStepLengthController.getMoreInformation(i);
                DATA.AverageTimeUse = milliseconds.ToString("f5");

                int count = theMainWindow.theStepLengthUse.Count;
                for (int j = 0; j < 5; j++)
                {
                    if (j < count)
                    {
                        switch (j)
                        {
                            case 0: { DATA.Step1SL = theMainWindow.theStepLengthUse[j].ToString("f2"); } break;
                            case 1: { DATA.Step2SL = theMainWindow.theStepLengthUse[j].ToString("f2"); } break;
                            case 2: { DATA.Step3SL = theMainWindow.theStepLengthUse[j].ToString("f2"); } break;
                            case 3: { DATA.Step4SL = theMainWindow.theStepLengthUse[j].ToString("f2"); } break;
                            case 4: { DATA.Step5SL = theMainWindow.theStepLengthUse[j].ToString("f2"); } break;
                        }
                    }
                    else
                    {
                        switch (j)
                        {
                            case 0: { DATA.Step1SL = "---"; } break;
                            case 1: { DATA.Step2SL = "---"; } break;
                            case 2: { DATA.Step3SL = "---"; } break;
                            case 3: { DATA.Step4SL = "---"; } break;
                            case 4: { DATA.Step5SL = "---"; } break;
                        }
                    }
                }
                DataForSLMethod.Add(DATA);
            }

            SLDataGrid.ItemsSource = DataForSLMethod ;
  
        }
    }



    class classForStepLengthShow
    {
        private string methodName = "S34343SASD";
        private string step1SL = "1";
        private string step2SL = "0.55";
        private string step3SL = "0.54";
        private string step4SL = " ";
        private string step5SL =  " ";
        private string averageTimeUse = " ";
        private string methodInformaton = "";


        public string MethodName { get { return methodName; } set { methodName = value; } }
        public string Step1SL { get { return step1SL; } set { step1SL = value; } }
        public string Step2SL { get { return step2SL; } set { step2SL = value; } }
        public string Step3SL { get { return step3SL; } set { step3SL = value; } }
        public string Step4SL { get { return step4SL; } set { step4SL = value; } }
        public string Step5SL { get { return step5SL; } set { step5SL = value; } }
        public string AverageTimeUse { get { return averageTimeUse; } set { averageTimeUse = value; } }
        public string MethodInformaton { get { return methodInformaton; } set { methodInformaton = value; } }

    }


}
