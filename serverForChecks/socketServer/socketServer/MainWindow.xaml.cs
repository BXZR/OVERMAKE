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
using Visifire.Charts;
using System.Windows.Threading;

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
        Filter theFilter;//专门用于滤波的控制单元
        DispatcherTimer tm = new DispatcherTimer();//刷新控制单元
        string stepCountShow = "";
        public MainWindow()
        {
            InitializeComponent();
         
        }


        #region 折线图
        public void CreateChartSpline(string name, List<double> theValues)
        {
            //创建一个图标
            Chart chart = new Chart();

            //设置图标的宽度和高度
            chart.Width = 580;
            chart.Height = 380;
            chart.Margin = new Thickness(100, 5, 10, 5);
            //是否启用打印和保持图片
            chart.ToolBarEnabled = false;

            //设置图标的属性
            chart.ScrollingEnabled = false;//是否启用或禁用滚动
            chart.View3D = true;//3D效果显示

            //创建一个标题的对象
            Title title = new Title();

            //设置标题的名称
            title.Text = name;
            title.Padding = new Thickness(0, 10, 5, 0);

            //向图标添加标题
            chart.Titles.Add(title);

            //初始化一个新的Axis
            Axis xaxis = new Axis();
            //设置Axis的属性
            //图表的X轴坐标按什么来分类，如时分秒
            xaxis.IntervalType = IntervalTypes.Seconds;
            //图表的X轴坐标间隔如2,3,20等，单位为xAxis.IntervalType设置的时分秒。
            xaxis.Interval = 1;
            //设置X轴的时间显示格式为7-10 11：20           
         //   xaxis.ValueFormatString = "MM秒";
            //给图标添加Axis            
            chart.AxesX.Add(xaxis);

            Axis yAxis = new Axis();
            //设置图标中Y轴的最小值永远为0           
            yAxis.AxisMinimum = -2;
            //设置图表中Y轴的后缀          
            yAxis.Suffix = "m/s2";
            chart.AxesY.Add(yAxis);


            // 创建一个新的数据线。               
            DataSeries dataSeries = new DataSeries();
            // 设置数据线的格式。               
            dataSeries.LegendText = "Y轴加速度";

            dataSeries.RenderAs = RenderAs.Spline;//折线图

            dataSeries.XValueType = ChartValueTypes.Auto;
            // 设置数据点              
            DataPoint dataPoint;
            for (int i = 0; i < theValues.Count; i++)
            {
                // 创建一个数据点的实例。                   
                dataPoint = new DataPoint();
                // 设置X轴点                    
                dataPoint.XValue = (double)i/4;
                //设置Y轴点                   
                dataPoint.YValue =  theValues [i];
                dataPoint.MarkerSize = 8;
                //dataPoint.Tag = tableName.Split('(')[0];
                //设置数据点颜色                  
                // dataPoint.Color = new SolidColorBrush(Colors.LightGray);                   
               // dataPoint.MouseLeftButtonDown += new MouseButtonEventHandler(dataPoint_MouseLeftButtonDown);
                //添加数据点                   
                dataSeries.DataPoints.Add(dataPoint);
            }

            // 添加数据线到数据序列。                
            chart.Series.Add(dataSeries);

 

            //将生产的图表增加到Grid，然后通过Grid添加到上层Grid.           
            Grid gr = new Grid();
            gr.Children.Add(chart);

            Chart.Children.Add(gr);
        }
        #endregion



        private void makeFlashController()
        {
          
            tm.Tick += new EventHandler(tm_Tick);
            tm.Interval = TimeSpan.FromSeconds(0.6);
            tm.Start();
        }

        void tm_Tick(object sender, EventArgs e)
        {
            List<double> theFiltered = theFilter.theFilerWork(theInformationController.accelerometerY);
            theStepLabel.Content = "一共走了" + thePeackFinder.countStepsWithTime(theFiltered);
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
            theFilter = new Filter();

            makeFlashController();

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

            //为了保证数据干净，要做一次滤波
            List<double> theFiltered = theFilter.theFilerWork(theInformationController.accelerometerY);

            //查看滤波后的数据
            string IS = theFiltered.Count + "条数据\n";
            for (int i = 0; i < theFiltered .Count; i++)
                IS += theFiltered[i] + "\n";
            theFileSaver.saveInformation(IS);
           // MessageBox.Show("GET:\n"+IS);

            //滤波之后做各种分析，第一项是步态分析，判断走了多少步
            int stepCount = thePeackFinder.countSteps(theFiltered);
            CreateChartSpline("Y加速度", theFiltered);

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




        private void button5_Click(object sender, RoutedEventArgs e)
        {
          
        }

        private void button6_Click(object sender, RoutedEventArgs e)
        {
            //为了保证数据干净，要做一次滤波
            List<double> theFiltered = theFilter.theFilerWork(theInformationController.compassDegree);

            //查看滤波后的数据
            string IS = theFiltered.Count + "条数据\n";
            for (int i = 0; i < theFiltered.Count; i++)
                IS += theFiltered[i] + "\n";
            theFileSaver.saveInformation(IS);

            CreateChartSpline("角度变化", theFiltered);
        }


    }
}

        
        