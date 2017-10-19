using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;




namespace socketServer
{

    //timerFlash就是每隔很短一段时间进行刷新的方法
    //withSaveedData是很长的一段时间统一处理的方法
    public enum workType { timerFlash, withSavedData}

    //整个程序的主逻辑处理单元
    public partial class MainWindow : Window
    {

        information theInformationController;//信息总控单元，必须要有
        theServer theServerController;//网络服务控制单元，必须有
        FileSaver theFileSaver;//保存到文件中的控制器
        DispatcherTimer tm;//刷新控制单元
        PeackSearcher thePeackFinder;//寻找峰谷的控制单元，用于步态分析
        Filter theFilter;//专门用于滤波的控制单元
        rotationAngel theAngelController;//角度控制单元
        position thePositionController;//最终定位的控制单元
        stepLength theStepLengthController;//用来确定步长的控制单元
        workType theWorkType = workType.timerFlash;//收集数据分析的模式
        pictureMaker thePictureMaker;//隔一段时间，做一张图片
        float stepTimer = 1f;//间隔多长时间进行一次计算

        public MainWindow()
        {
            InitializeComponent();

        }


        private void makeFlashController()
        {
            //方法切换在这里判断执行
            if (theWorkType == workType.timerFlash)//有一点强硬的阶段
            {
                tm.Tick += new EventHandler(flashQuitck);
                tm.Interval = TimeSpan.FromSeconds(0.6);
            }
            else if (theWorkType == workType.withSavedData)//个人更加推荐这种方法
            {
                tm.Tick += new EventHandler(withSavedData);
                tm.Interval = TimeSpan.FromSeconds(stepTimer);
            }
            tm.Start();
        }

        /*************************************************方法2（带缓冲，真正使用的）*************************************************************/
        //主要思路与方法1是差不多的，只不过是在一个比较长的时间之内判断多个波峰和波谷，
        //然后进行统一的计算
        //为此需要一个缓冲区
        void withSavedData(object sender, EventArgs e)
        {

            List<double> theFilteredAZ = theFilter.theFilerWork(theInformationController.accelerometerZ);
            List<double> theFilteredD = theFilter.theFilerWork(theInformationController.compassDegree,0.1f);
            int stepcounts =  thePeackFinder.countStepWithStatic(theFilteredAZ);//必要的一步，怎么也需要走一边来刷新缓存（也就是纪录波峰的下标）
            //根据下标获得需要的旋转角和步长
            //当下的步长的模型可以说完全不对，只能算做支撑架构运作的一个方式
            List<double> theStepAngeUse = new List<double>();
            List<double> theStepLengthUse = new List<double>();
            //计算移动的时候用的是去除不可能项的步数
            for (int i = 0; i < thePeackFinder.peackBuff.Count; i++)
            {

                theStepAngeUse.Add(theFilteredD[thePeackFinder.peackBuff[i]]);
                if(i>=1)
                theStepLengthUse.Add(theStepLengthController.getStepLength(theStepAngeUse[i-1],theStepAngeUse [i]));//这个写法后期需要大量的扩展，或者说这才是这个程序的核心所在
                else
                theStepLengthUse.Add(theStepLengthController.getStepLength());
            }

            SystemSave.allStepCount = SystemSave.stepCount + thePeackFinder.peackBuff.Count;
            theStepLabel.Content = "(带缓存计步方法)\n（当前分组）原始数据步数：" + PeackSearcher.TheStepCount + "    去除不可能项步数：" + thePeackFinder.peackBuff.Count;
            theStepLabel.Content += "\n历史存储步数：" + SystemSave.stepCount + "    总步数：" + SystemSave.allStepCount + "\n绘制图像： " + SystemSave.pictureNumber;
            theStepLabel.Content += "    当前分组数据条目： " + theInformationController.accelerometerY.Count + "    总数据条目："+ SystemSave.getValuesCount( theInformationController.accelerometerY.Count);
            POSITION.Text = thePositionController.getPositions(theStepAngeUse, theStepLengthUse);
            //先做thePositionController.getPositions(theStepAngeUse, theStepLengthUse);用来刷新内部缓存


            //两种绘制方法也算是各有千秋，所以给一个选项自行选择吧
            if (drawWithBufferCheck.IsChecked == true)
            {
                //实时绘制图像，重新绘制的方式
                drawPositionLine();
               // Console.WriteLine("sdf");
            }
            else
            {
                //实时绘制图像，但是并不重新绘制
                for (int u = savedIndex; u < thePositionController.theTransformPosition.Count; u++)
                {
                    savedIndex = thePositionController.theTransformPosition.Count - 1;
                    drawPositionLineOnTime(thePositionController.theTransformPosition[u].X, thePositionController.theTransformPosition[u].Y);
                }
            }


            //如果数据足够多，就需要保存成一张图像
            if (theInformationController.accelerometerY.Count > SystemSave.buffCount)
            {
                for (int i = 0; i < thePositionController.theTransformPosition.Count; i++)
                {
                    SystemSave.savedPositions.Add(thePositionController.theTransformPosition[i]);
                }

                savedIndex = 0;

                thePictureMaker.createPictureFromData(theInformationController);
                theInformationController.flashInformation();
                SystemSave.stepCount += thePeackFinder.peackBuff.Count;

            }
        }
        /*************************************************方法1（比较原始）*************************************************************/
        //判断一步依赖于一个假说：
        //人0.4秒内只能走一步
        //设定时间要比 tm.Interval = TimeSpan.FromSeconds(0.4);
        //争取让每一次计算都包含的是一步

        //一种实时的方法
        //很短的时间之内就检查一次
        //原始思路，这是一个非常鲁莽的方法，效率也不高
        //思路就是在一个非常短的时间之内最多只可能走出一步，也就是说即使发现了多个波峰，也只认为走出了一步
        void flashQuitck(object sender, EventArgs e)
        {
            List<double> theFilteredAY = theFilter.theFilerWork(theInformationController.accelerometerY);
            //统一用内部方法来做步态分析，统一修改并节约代码
            if (thePeackFinder.countCheck(theFilteredAY))
            {
                theStepLabel.Content = "（不带缓存）一共走了" + PeackSearcher.TheStepCount + "/" + PeackSearcher.changeCount + "步";
                //判断出了走了步，所以需要进行定位了
                List<double> theFilteredD = theFilter.theFilerWork(theInformationController.compassDegree);
                double theStepLength = theStepLengthController.getStepLength();
                double theDegree = theAngelController.getAngelNow(theFilteredD);
                thePositionController.calculationPosition(theDegree, theStepLength);
                POSITION.Text += "\n角度： " + theDegree.ToString("f4") + " 步长： " + theStepLength.ToString("f4") + " 坐标： " + thePositionController.getPosition();
            }
        }

        /******************************按钮事件控制单元***********************************************/

        //保存数据控制的按钮
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            string saveStringUse = "";
            for (int i = 0; i < SystemSave.savedPositions.Count; i++)
                saveStringUse += SystemSave.savedPositions[i].toString() + "\n";
            theFileSaver.saveInformation(saveStringUse);
        }

        //startServer按钮控制单元
        private void button2_Click(object sender, RoutedEventArgs e)
        {
            //全世界的初始化都在这里完成
            theInformationController = new information();
            theServerController = new theServer(theInformationController);
            thePeackFinder = new PeackSearcher();
            theFileSaver = new FileSaver();
            theFilter = new Filter();
            theAngelController = new rotationAngel();
            tm = new DispatcherTimer();
            thePositionController = new position();
            theStepLengthController = new stepLength();
            thePictureMaker = new pictureMaker(); 
            theWorkType = workType.withSavedData;//选择工作模式（可以考虑在界面给出选择）
            //相关工程更新
            makeFlashController();

            //正式开始
            string showInformation = theServerController.startTheServer();
            MessageBox.Show(showInformation);
        }


        //closeServer按钮控制单元
        private void button3_Click_1(object sender, RoutedEventArgs e)
        {
            string showInformation = theServerController.closeServer();
            MessageBox.Show(showInformation);
        }



        //窗口自动关闭的时候也做一次自动的关闭
        private void Window_Closed(object sender, EventArgs e)
        {
            if (theServerController != null)
                theServerController.closeServer();
        }


        //非自动测试触发的按钮
        private void button4_Click(object sender, RoutedEventArgs e)
        {

            //为了保证数据干净，要做一次滤波
            List<double> theFiltered = theFilter.theFilerWork(theInformationController.accelerometerY);
            if (theFiltered == null)
                return;
            //查看滤波后的数据
            //string IS = theFiltered.Count + "条数据\n";
            //for (int i = 0; i < theFiltered.Count; i++)
            //    IS += theFiltered[i] + "\n";
            // theFileSaver.saveInformation(IS);
            // MessageBox.Show("GET:\n"+IS);

            //滤波之后做各种分析，第一项是步态分析，判断走了多少步
            int stepCount = thePeackFinder.countStepWithStatic(theFiltered);

            ChartWindow theChartWindow = new ChartWindow();
            theChartWindow.CreateChartSpline(UseDataType.accelerometerY, theFiltered);
            theChartWindow.Show();
            MessageBox.Show("一共" + stepCount + "步");
        }

        //处理旋转方向的按钮逻辑
        private void button6_Click(object sender, RoutedEventArgs e)
        {
            //为了保证数据干净，要做一次滤波
            List<double> theFiltered = theFilter.theFilerWork(theInformationController.compassDegree);
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

        private void button_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(demoForTensorFlow.DEMO());
        }



        //这个方法辅助用的字段两个：
        private double  X1Save = 0;
        private double  Y1Save = 0;
        private int savedIndex = 0;
        //实时的动态绘制路线图 (方法2,单纯地累加，误差一定会有，但是相对可控性提高)
        private void drawPositionLineOnTime(double X2 , double Y2)
        {
            //绘制圆心
            var ellipse = new Ellipse()
            {
                Width = 10,
                Height = 10,
                Fill = new SolidColorBrush(Colors.Red)
            };
            Canvas.SetLeft(ellipse, theCanvas.Width / 2);
            Canvas.SetTop(ellipse, theCanvas.Height / 2);
            theCanvas.Children.Add(ellipse);

            Line drawLine = new Line();
            drawLine.X1 = theCanvas.Width / 2 + X1Save * 5;//怕跑出范围，所以就缩小了一些
            drawLine.Y1 = theCanvas.Height / 2 + Y1Save * 5;//怕跑出范围，所以就缩小了一些
            drawLine.X2 = theCanvas.Width / 2 + X2 * 5;//怕跑出范围，所以就缩小了一些
            drawLine.Y2 = theCanvas.Height / 2 + Y2* 5;//怕跑出范围，所以就缩小了一些
            //保存字段值
            X1Save = X2;
            Y1Save = Y2;
            drawLine.Stroke = new SolidColorBrush(SystemSave.theNewColor2);
            drawLine.StrokeThickness = 2;
            theCanvas.Children.Add(drawLine);


        }

        //实时的动态绘制路线图 (方法1,每一次都重新绘制，所以会自带修正效果)
        private void drawPositionLine()
        {
            theCanvas.Children.Clear();
            var ellipse = new Ellipse()
            {
                Width = 10,
                Height = 10,
                Fill = new SolidColorBrush(Colors.Red)
            };

            Canvas.SetLeft(ellipse, theCanvas.Width / 2);
            Canvas.SetTop(ellipse , theCanvas.Height / 2);
            theCanvas.Children.Add(ellipse);

            for (int u = 0; u < SystemSave.savedPositions .Count -1; u++)
            {
                Line drawLine = new Line();
                drawLine.X1 = theCanvas.Width / 2 + SystemSave.savedPositions[u].X * 5;//怕跑出范围，所以就缩小了一些
                drawLine.Y1 = theCanvas.Height / 2 + SystemSave.savedPositions[u].Y * 5;//怕跑出范围，所以就缩小了一些
                drawLine.X2 = theCanvas.Width / 2 + SystemSave.savedPositions[u+1].X * 5;//怕跑出范围，所以就缩小了一些
                drawLine.Y2 = theCanvas.Height / 2 + SystemSave.savedPositions[u+1].Y * 5;//怕跑出范围，所以就缩小了一些
                drawLine.Stroke = new SolidColorBrush(SystemSave.theOldColor);
                drawLine.StrokeThickness = 2;
                theCanvas.Children.Add(drawLine);
            }

            for (int u = 0; u < thePositionController.theTransformPosition.Count-1; u++)
            {
                Line drawLine = new Line();
                drawLine.X1 = theCanvas.Width / 2 + thePositionController.theTransformPosition[u].X *5;//怕跑出范围，所以就缩小了一些
                drawLine.Y1 = theCanvas.Height / 2 + thePositionController.theTransformPosition[u].Y * 5;//怕跑出范围，所以就缩小了一些
                drawLine.X2 = theCanvas.Width / 2 + thePositionController.theTransformPosition[u+1].X * 5;//怕跑出范围，所以就缩小了一些
                drawLine.Y2 = theCanvas.Height / 2 + thePositionController.theTransformPosition[u+1].Y * 5;//怕跑出范围，所以就缩小了一些
                drawLine.Stroke = new SolidColorBrush(SystemSave.theNewColor);
                drawLine.StrokeThickness = 2;
                theCanvas.Children.Add(drawLine);
            }
        }

////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        void drawEclipse()
        {
            var circle = new Ellipse()
            {
                Width = 100,
                Height = 100,
                Fill = new SolidColorBrush(Colors.White)
            };
            var ellipse = new Ellipse()
            {
                Width = 50,
                Height = 100,
                Fill = new SolidColorBrush(Colors.White)
            };
            Canvas.SetLeft(ellipse, 100);
            theCanvas.Children.Add(circle);
            theCanvas.Children.Add(ellipse);
        }

        void drawLine()
        {
            Line s = new Line();


            s.X1 = theCanvas.Width / 2;
            s.X2 = 0;
            s.Y1 = theCanvas.Height / 2;
            s.Y2 = 0;
            s.Stroke = new SolidColorBrush(Colors.Black);

            s.StrokeThickness = 3;
            theCanvas.Children.Clear();
            theCanvas.Children.Add(s);
        }
         

        void draw1()
        {
            theCanvas.Children.Clear();
            var polygon2PointsCollection = new PointCollection();
            polygon2PointsCollection.Add(new Point() { X = 0 , Y = 0});

            polygon2PointsCollection.Add(new Point() { X = 50, Y = 50 });
            polygon2PointsCollection.Add(new Point() { X = 50, Y = 100 });
            polygon2PointsCollection.Add(new Point() { X = 100, Y = 50 });
            var polygon2 = new Polygon()
            {
                Stroke = new SolidColorBrush(Colors.Black),
                Points = polygon2PointsCollection,
                // Fill = new SolidColorBrush(Colors.White)
            };
            Canvas.SetLeft(polygon2, 100);
            theCanvas.Children.Add(polygon2);
        }

        //设置路径颜色的方法
        Color SetColor()
        {

            System.Windows.Forms.ColorDialog MyDialog = new System.Windows.Forms.ColorDialog();
            MyDialog.AllowFullOpen = true;
            MyDialog.ShowHelp = true;

            MyDialog.ShowDialog();
            
            //因为用到的是两个系统的Color，所以需要做一个转换
            //说实话这个地方是有一点冗余....
            Color newColor = new Color();
            newColor.R = MyDialog.Color.R;
            newColor.G = MyDialog.Color.G;
            newColor.B = MyDialog.Color.B;
            newColor.A = MyDialog.Color.A;
            SystemSave.theOldColor = newColor;
            SystemSave.theNewColor2 = newColor;
            return newColor;
        }

        private void button7_Click(object sender, RoutedEventArgs e)
        {
            thePictureMaker = new pictureMaker();
            thePictureMaker.createPictureFromData();
        }

        private void button5_Click(object sender, RoutedEventArgs e)
        {
            button5.Background = new SolidColorBrush(SetColor());
        }
    }
}

        
        