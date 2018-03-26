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
using Visifire.Charts;

namespace socketServer
{

    /// <summary>
    /// Interaction logic for ChartWindow.xaml
    /// </summary>
    public partial class ChartWindow : Window
    {
        public ChartWindow()
        {
            InitializeComponent();
        }




        #region 柱状图
        public void CreateChartColumn(List<double> theValues, List<string> XLabels = null,string titleName = "" , string SuffixIn = "m")
        {
            //创建一个图标
            Chart chart = new Chart();
            //设置图标的宽度和高度
            chart.Width = 800;
            chart.Height = 425;
            chart.Margin = new Thickness(5, 5, 10, 5);
            //是否启用打印和保持图片
            chart.ToolBarEnabled = false;
            //设置图标的属性
            chart.ScrollingEnabled = false;//是否启用或禁用滚动
            chart.View3D = true;//3D效果显示
            //创建一个标题的对象
            Title title = new Title();
            title.Text = titleName;
            title.Padding = new Thickness(0, 10, 5, 0);
            //向图标添加标题
            chart.Titles.Add(title);

            //初始化一个新的Axis
            Axis xaxis = new Axis();
 
            //设置Axis的属性
            //图表的X轴坐标按什么来分类，如时分秒
            xaxis.IntervalType = IntervalTypes.Number;
            xaxis.Interval = 1;
            chart.AxesX.Add(xaxis);

            Axis yAxis = new Axis();
            //设置图标中Y轴的最小值永远为0           
            yAxis.AxisMinimum = 0;
            //设置图表中Y轴的后缀 
            yAxis.Suffix = SuffixIn;
            chart.AxesY.Add(yAxis);


            // 创建一个新的数据线。               
            DataSeries dataSeries = new DataSeries();
            // 设置数据线的格式。
            dataSeries.LegendText = titleName;


            dataSeries.RenderAs = RenderAs.Column;//柱状图

            dataSeries.XValueType = ChartValueTypes.Auto;
            // 设置数据点              
            DataPoint dataPoint;
            for (int i = 0; i < theValues.Count; i++)
            {
                // 创建一个数据点的实例。                   
                dataPoint = new DataPoint();
                // 设置X轴点    
                if (XLabels == null)
                    dataPoint.AxisXLabel = i.ToString();
                else
                    dataPoint.AxisXLabel = XLabels[i];
                //设置Y轴点                   
                dataPoint.YValue = theValues[i];
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

            theChartGrid.Children.Add(gr);
        }
        #endregion





        #region 折线图
        public void CreateChartSpline(UseDataType IN, List<double> theValues ,string titleName = "")
        {
            //创建一个图标
            Chart chart = new Chart();

            //设置图标的宽度和高度
            chart.Width = 800;
            chart.Height = 425;
            chart.Margin = new Thickness(5, 5, 10, 5);
            //是否启用打印和保持图片
            chart.ToolBarEnabled = false;

            //设置图标的属性
            chart.ScrollingEnabled = false;//是否启用或禁用滚动
            chart.View3D = true;//3D效果显示

            //创建一个标题的对象
            Title title = new Title();

            title.Text = titleName;
            //设置标题的名称
            if (string.IsNullOrEmpty(titleName))
            {
                switch (IN)
                {
                    case UseDataType.accelerometerY:
                        {
                            title.Text = "选中轴的加速度";
                        }
                        break;
                    case UseDataType.compassDegree:
                        {
                            title.Text = "磁力计角度";
                        }
                        break;
                }
            }
           
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
            switch (IN)
            {
                case UseDataType.accelerometerY:
                    {
                        yAxis.Suffix = "m/s2";
                    }
                    break;
                case UseDataType.compassDegree:
                    {
                        yAxis.Suffix = "o";
                    }
                    break;
            }

           
            chart.AxesY.Add(yAxis);


            // 创建一个新的数据线。               
            DataSeries dataSeries = new DataSeries();
            // 设置数据线的格式。
            dataSeries.LegendText = titleName;


            dataSeries.RenderAs = RenderAs.Spline;//折线图

            dataSeries.XValueType = ChartValueTypes.Auto;
            // 设置数据点              
            DataPoint dataPoint;
            for (int i = 0; i < theValues.Count; i++)
            {
                // 创建一个数据点的实例。                   
                dataPoint = new DataPoint();
                // 设置X轴点                    
                dataPoint.XValue = (double)i / 4;
                //设置Y轴点                   
                dataPoint.YValue = theValues[i];
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

            theChartGrid.Children.Add(gr);
        }
        #endregion



        #region 折线图2
        //绘制对比曲线使用的折线图
        public void CreateChartSpline2(UseDataType IN, List<double> theValues , List<double>  theValues2 , string titleName = "")
        {
            //创建一个图标
            Chart chart = new Chart();

            //设置图标的宽度和高度
            chart.Width = 800;
            chart.Height = 425;
            chart.Margin = new Thickness(5, 5, 10, 5);
            //是否启用打印和保持图片
            chart.ToolBarEnabled = false;

            //设置图标的属性
            chart.ScrollingEnabled = false;//是否启用或禁用滚动
            chart.View3D = true;//3D效果显示

            //创建一个标题的对象
            Title title = new Title();

            title.Text = "表格标题";
            //设置标题的名称
            switch (IN)
            {
                case UseDataType.accelerometerZ:
                    {
                        title.Text = "选中轴的加速度";
                    }
                    break;
                case UseDataType.compassDegree:
                    {
                        title.Text = "磁力计角度";
                    }
                    break;
            }

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
            switch (IN)
            {
                case UseDataType.accelerometerY:
                    {
                        yAxis.Suffix = "m/s2";
                    }
                    break;
                case UseDataType.compassDegree:
                    {
                        yAxis.Suffix = "o";
                    }
                    break;
            }
            chart.AxesY.Add(yAxis);
            // 创建一个新的数据线。               
            DataSeries dataSeries = new DataSeries();
            DataSeries dataSeries2 = new DataSeries();
            // 设置数据线的格式。
            switch (IN)
            {
                case UseDataType.accelerometerZ:
                    {
                        dataSeries.LegendText = "加速度";
                        dataSeries2.LegendText = "加速度样本";
                    }
                    break;
                case UseDataType.compassDegree:
                    {
                        dataSeries.LegendText = "磁力计角度";
                    }
                    break;
            }


            dataSeries.RenderAs = RenderAs.Spline;//折线图
            dataSeries.XValueType = ChartValueTypes.Auto;

            dataSeries2.RenderAs = RenderAs.Spline;//折线图
            dataSeries2.XValueType = ChartValueTypes.Auto;
            // 设置数据点              
            DataPoint dataPoint;
            DataPoint dataPoint2;
            for (int i = 0; i < theValues.Count; i++)
            {
                // 创建一个数据点的实例。                   
                dataPoint = new DataPoint();
                dataPoint2 = new DataPoint();
                // 设置X轴点                    
                dataPoint.XValue = (double)i / 4;
                dataPoint2.XValue = (double)i / 4;
                //设置Y轴点                   
                dataPoint.YValue = theValues[i];
                dataPoint2.YValue = theValues2[i];
                dataPoint.MarkerSize = 8;
                dataPoint2.YValue = theValues2[i];
                dataSeries.DataPoints.Add(dataPoint);
                dataSeries2.DataPoints.Add(dataPoint2);
            }

            // 添加数据线到数据序列。                
            chart.Series.Add(dataSeries);
            chart.Series.Add(dataSeries2);

            //将生产的图表增加到Grid，然后通过Grid添加到上层Grid.           
            Grid gr = new Grid();
            gr.Children.Add(chart);

            theChartGrid.Children.Add(gr);
        }
        #endregion


    }
}
