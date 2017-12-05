using socketServer.Codes.DecisionTree;
using socketServer.Codes.stages;
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
        DispatcherTimer tm;//刷新控制单元这是一个组件，是针对时间的刷新
        PeackSearcher thePeackFinder;//寻找峰谷的控制单元，用于步态分析
        Filter theFilter;//专门用于滤波的控制单元
        rotationAngel theAngelController;//角度控制单元
        position thePositionController;//最终定位的控制单元
        stepLength theStepLengthController;//用来确定步长的控制单元
        workType theWorkType = workType.timerFlash;//收集数据分析的模式
        pictureMaker thePictureMaker;//隔一段时间，做一张图片
        stepDetection stepExtra;//额外的判断走了一步的方法集合
        stepModeCheck theStepModeCheckController;//推断行走状态：战力，行走，奔跑用 的控制单元
        TrainFileMaker theTrainFileMake;//制作数据集的控制单元
        float stepTimer = 0.5f;//间隔多长时间进行一次计算（计算时间间隔越短自然越灵敏，但是开销也就越大）
        stepAxis theStepAxis;//用来判定使用哪一个轴向的封装
        FSMBasic theStage = new StageStance();//当前状态的推断，使用的是有限状态机


        //公有的存储空间
        List<double> theStepAngeUse = new List<double>();
        List<double> theStepLengthUse = new List<double>();
        List<double> theFilteredD = new List<double>();
        List<int> theStairMode = new List<int>();

        int stepcounts = 0;//当前阶段的步数
        List<int> indexBuff = new List<int>();//确认一步的下标存储
        List<double> theFilteredAZ = new List<double>();//当前使用的轴
        public MainWindow()
        {
            InitializeComponent();

        }

        //整体工作模式
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
            //-----------------------------------------------获取计算用数据-----------------------------------------------//
            //刷新存储空间
            theFilteredAZ = stepCheckAxis();
            theFilteredD = theFilter.theFilerWork(theInformationController.compassDegree, 0.1f);
            theStepAngeUse = new List<double>();
            theStepLengthUse = new List<double>();
            //theStairMode = new List<int>();
            //实现方法里面对这个list已经做出了相关操作，这里没必要做了
            stepcounts = 0;
            //判断走了一步并更新到缓存
            stepDectionUse();
            //当前移动的步数
            stepcounts = indexBuff.Count;
            //获得移动方向
            headingAngleGet();
            //计算步长
            stepLengthGet(indexBuff, theFilteredAZ);
            //计算上下楼梯的模式
            StairCheck(indexBuff);
            //计算坐标并获得显示的文本
            POSITION.Text = thePositionController.getPositions(theStepAngeUse, theStepLengthUse , theStairMode);
            //更新slope的数值
            stepModeCheck(indexBuff);
            //制作输出显示的内容
            makeLabelMehtod(stepcounts);
            //绘制路线图
            drawPicturesShow();

            //如果数据足够多，就需要保存成一张图像,同时做刷新处理和一些额外的补充计算
            if (theInformationController.accelerometerY.Count > SystemSave.buffCount)
            makeFlash();
        }

        //额外补充计算////////////////////////////////////////////////////////////////////////////////////
        public void canculateExtra()
        {
            //使用动态零线方法
            if (SystemSave.isDynamicallyZeroLineForStepDection)
            {
                //预防除零异常
                if (indexBuff.Count == 0)
                    return;

                double ZeroAverage = 0;
                for (int i = 0; i < indexBuff.Count; i++)
                {
                    ZeroAverage += theFilteredAZ[indexBuff[i]];
                }
                ZeroAverage /= indexBuff.Count;
                SystemSave.zeroCrossOffset = ZeroAverage;
            }
        }


        //刷新操作////////////////////////////////////////////////////////////////////////////////////
        void makeFlash()
        {
            //额外补充计算
            canculateExtra();
            //---------------------------------保存训练用的数据---------------------------------------------//
            saveTrainBase(theFilteredAZ);
            //------------------------------------------------------------------------------------------//
            for (int i = 0; i < thePositionController.theTransformPosition.Count; i++)
            {
                SystemSave.savedPositions.Add(thePositionController.theTransformPosition[i]);
            }
            //方法1的刷新和存储
            savedIndex = 0;
            //制作信息图像
            thePictureMaker.createPictureFromData(theInformationController);
            // thePictureMaker.createPictureFromDataComplex(theInformationController);//暂时先不必要用特别复杂的图像生成方法，会卡
            theInformationController.flashInformation();
            SystemSave.stepCount += thePeackFinder.peackBuff.Count;
            //方法2的刷新和存储
            SystemSave.stepCount2 += stepExtra.peackBuff.Count;
            stepExtra.makeFlash();
            SystemSave.makeFlash();
  
        }


        //绘制路线图/////////////////////////////////////////////////////////////////////////////////
        void drawPicturesShow()
        {
            //两种绘制方法也算是各有千秋，所以给一个选项自行选择吧
            if (SystemSave.drawWithBuffer)
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
                    drawPositionLineOnTime(
                        thePositionController.theTransformPosition[u].X, 
                        thePositionController.theTransformPosition[u].Y,
                        thePositionController.theTransformPosition[u].heading
                        );
                }
                //修正箭头位置和方向
                flashHeadingPicture(thePositionController.theTransformPosition);
            }
        }

        //更换使用的判断走看了一步的轴的方法///////////////////////////////////////////////////////////
        List<double> stepCheckAxis()
        {
            List<double> theFilteredAZ = new List<double>();
            switch (stepCheckAxisUse.SelectedIndex)//选择不同的轴向
            {
                case 0:
                    //基础方法:用Z轴加速度来做
                    theFilteredAZ = theStepAxis.AZ(theInformationController , theFilter);  
                    break;
                case 1:
                    //实验用方法：X轴向
                    theFilteredAZ = theStepAxis.AX(theInformationController, theFilter);
                    break;
                case 2:
                    //实验用方法：X轴向
                    theFilteredAZ = theStepAxis.AY(theInformationController, theFilter);
                    break;
                case 3:
                    //基础方法:用三个轴的加速度平方和开根号得到
                    theFilteredAZ = theStepAxis.ABXYZ(theInformationController, theFilter);
                    break;
            }
            return theFilteredAZ;
        }

        //判断走了一步的方法///////////////////////////////////////////////////////////
        void stepDectionUse()
        {
            if (stepCheckMethod.SelectedIndex == 0)
            {
                //方法1：波峰波谷大法，我个人推荐的方法
                //必要的一步，怎么也需要走一边来刷新缓存（也就是纪录波峰的下标）
                //根据下标获得需要的旋转角和步长
                //当下的步长的模型可以说完全不对，只能算做支撑架构运作的一个方式
                //计算移动的时候用的是去除不可能项的步数
                indexBuff = stepExtra.stepDectionExtration0(theFilteredAZ, thePeackFinder);

            }
            else if (stepCheckMethod.SelectedIndex == 1)
            {
                indexBuff = stepExtra.stepDectionExtration3(theFilteredAZ, thePeackFinder);
            }
            else if (stepCheckMethod.SelectedIndex == 2)
            {
                //方法3：重复性判断方法，相对比较严格（感觉不是人能用的）
                indexBuff = stepExtra.stepDetectionExtra1(theFilteredAZ);
            }
            else if (stepCheckMethod.SelectedIndex == 3)
            {
                //方法4零点交叉
                indexBuff = stepExtra.stepDetectionExtra2(theFilteredAZ);
            }
        }

        //判断走楼梯的模式的方法
        //开销很大...
        void StairCheck(List<int> indexBuff)
        {
            if (SystemSave.isStairsUp == false)
            {
                theStairMode = null;
                return;
            }
            theStairMode = new List<int>();
            //这些数据在一些复杂的方法中会用到，因此计算出来备用
            List<double> ax = theFilter.theFilerWork(theInformationController.accelerometerX);
            List<double> ay = theFilter.theFilerWork(theInformationController.accelerometerY);
            List<double> az = theFilter.theFilerWork(theInformationController.accelerometerZ);
            List<double> gx = theFilter.theFilerWork(theInformationController.gyroX);
            List<double> gy = theFilter.theFilerWork(theInformationController.gyroY);
            List<double> gz = theFilter.theFilerWork(theInformationController.gyroZ);
            for (int i = 0; i < indexBuff.Count; i++)
            {
                int mode = SystemSave.StairTree.searchModeWithTree(ax[indexBuff[i]], ay[indexBuff[i]], az[indexBuff[i]], gx[indexBuff[i]], gy[indexBuff[i]], gz[indexBuff[i]]);
                theStairMode.Add(mode);
            }
        }


        //获取步长的方法//////////////////////////////////////////////////////////////////////////
        //这个方法在多个整体方法中是共用的
        //实际上获得步长的方法就只在这里进行计算，因为小方法很多，的也是在这里进行分类的
        void stepLengthGet(List<int> indexBuff, List<double> AZUse)
        {
            //这些数据在一些复杂的方法中会用到，因此计算出来备用
            List<double> ax = theFilter.theFilerWork(theInformationController.accelerometerX);
            List<double> ay = theFilter.theFilerWork(theInformationController.accelerometerY);
            List<double> az = theFilter.theFilerWork(theInformationController.accelerometerZ);
            List<double> gx = theFilter.theFilerWork(theInformationController.gyroX);
            List<double> gy = theFilter.theFilerWork(theInformationController.gyroY);
            List<double> gz = theFilter.theFilerWork(theInformationController.gyroZ);

            for (int i = 0; i < indexBuff.Count; i++)
            {
                //方法0，最土鳖的方法直接就是立即数
                if (StepLengthMethod.SelectedIndex == 0)
                {
                    theStepLengthUse.Add(theStepLengthController.getStepLength1());
                }
                //方法1
                else if (StepLengthMethod.SelectedIndex == 1)
                {
                    if (i >= 1)
                    {
                        theStepLengthUse.Add(theStepLengthController.getStepLength1(theStepAngeUse[i - 1], theStepAngeUse[i]));//这个写法后期需要大量的扩展，或者说这才是这个程序的核心所在
                    }
                    else
                        theStepLengthUse.Add(theStepLengthController.getStepLength1());
                }
                //方法2，一般公式
                else if (StepLengthMethod.SelectedIndex == 2)
                {

                    if (i >= 1)
                    {
                        // for (int v = 0; v < theInformationController.timeStep.Count; v++)
                        //    Console.WriteLine(theInformationController.timeStep[v]);
                        List<long> timeUse = theFilter.theFilerWork(theInformationController.timeStep, 0.4f, true, theInformationController.accelerometerZ.Count);

                        double stepLength = theStepLengthController.getStepLength2(indexBuff[i - 1], indexBuff[i], AZUse, timeUse);
                        theStepLengthUse.Add(stepLength);//这个写法后期需要大量的扩展，或者说这才是这个程序的核心所在
                    }
                    else
                        theStepLengthUse.Add(theStepLengthController.getStepLength1());
                }
                //方法3，身高相关
                else if (StepLengthMethod.SelectedIndex == 3)
                {
                    theStepLengthUse.Add(theStepLengthController.getStepLength3());
                }
                //方法4，加速度开四次根号 Square  acceleration formula  （Weinberg approach）
                else if (StepLengthMethod.SelectedIndex == 4)
                {
                    if (i >= 1)
                        theStepLengthUse.Add(theStepLengthController.getStepLength4(indexBuff[i - 1], indexBuff[i], AZUse));
                    else
                        theStepLengthUse.Add(theStepLengthController.getStepLength1());
                }
                //方法5 说是可以克服每一个行人的不同特征，其实就是加速度平均上的计算 （Scarlet approach）
                else if (StepLengthMethod.SelectedIndex == 5)
                {
                    if (i >= 1)
                        theStepLengthUse.Add(theStepLengthController.getStepLength5(indexBuff[i - 1], indexBuff[i], AZUse));
                    else
                        theStepLengthUse.Add(theStepLengthController.getStepLength1());
                }
                //方法6，加速度平均开三次根号的做法
                else if (StepLengthMethod.SelectedIndex == 6)
                {
                    if (i >= 1)
                        theStepLengthUse.Add(theStepLengthController.getStepLength6(indexBuff[i - 1], indexBuff[i], AZUse));
                    else
                        theStepLengthUse.Add(theStepLengthController.getStepLength1());
                }
                //方法7，决策树选择公式参数的方法
                else if (StepLengthMethod.SelectedIndex == 7)
                {
                    if (i >= 1)
                    {
                        // for (int v = 0; v < theInformationController.timeStep.Count; v++)
                        //    Console.WriteLine(theInformationController.timeStep[v]);
                        if (SystemSave.StepLengthTree == null || SystemSave.StepLengthTree.IsStarted == false)
                        {
                            theStepLengthUse.Add(theStepLengthController.getStepLength1());
                        }
                        else
                        {
                            List<long> timeUse = theFilter.theFilerWork(theInformationController.timeStep, 0.4f, true, theInformationController.accelerometerZ.Count);
                            int mode = SystemSave.StepLengthTree.searchModeWithTree(ax[indexBuff[i]], ay[indexBuff[i]], az[indexBuff[i]], gx[indexBuff[i]], gy[indexBuff[i]], gz[indexBuff[i]]);
                           // Console.WriteLine("modeNow = " + mode);
                            double stepLength = theStepLengthController.getStepLength2WithDecisionTree(indexBuff[i - 1], indexBuff[i], AZUse, timeUse, mode);
                            theStepLengthUse.Add(stepLength);//这个写法后期需要大量的扩展，或者说这才是这个程序的核心所在
                        }
                    }
                    else
                    {
                        theStepLengthUse.Add(theStepLengthController.getStepLength1());
                    }
                }
                //方法7，使用关于腿长的倒置钟摆的方法（很好玩的方法）
                else if (StepLengthMethod.SelectedIndex == 8)
                {
                    if (i >= 1)
                    {
                        List<long> timeUse2 = theFilter.theFilerWork(theInformationController.timeStep, 0.4f, true, theInformationController.accelerometerZ.Count);
                        theStepLengthUse.Add(theStepLengthController.getStepLength8(indexBuff[i - 1], indexBuff[i], AZUse, timeUse2));
                    }
                    else
                        theStepLengthUse.Add(theStepLengthController.getStepLength1());
                }
            }
            //记录最新的移动步长
            if (theStepLengthUse.Count > 0)
                SystemSave.stepLengthNow = theStepLengthUse[theStepLengthUse.Count - 1];
        }


        //移动方向的“滤镜”
        //如果选择了额外的heading计算的方案，可能需要额外的计算。
        //这些计算统一在这里处理

        private double headingOffsetExtraCanculate(double heading , bool Can = true)
        {
            //当然也可以选择不计算
            if (Can == false)
            {
                return heading;
            }


            if (SystemSave.UseHeadingOffset)
            {
                heading += SystemSave.angleOffset;
            }
            if (SystemSave.CanculateHeadingMode ==1)
            {
                if (SystemSave.sampleTime > 0 && SystemSave.CHM1Sampled == false)
                {
                    SystemSave.headingOffsetFor3DHeading = get3DHeadingPffset();
                    if (SystemSave.headingOffsetFor3DHeading > -1000)
                    {
                        SystemSave.sampleTime--;
                        if (SystemSave.sampleTime < 0)
                        {
                            SystemSave.CHM1Sampled = true;
                        }
                    }
                }
                //Console.WriteLine("3D heading offset is Added");
                return heading + SystemSave.headingOffsetFor3DHeading;
            }
            return heading;
        }

        //获取移动方向的方法/////////////////////////////////////////////////////
        //注意这里的if和for的顺序和steoLength结构不一样
        void headingAngleGet()
        {
            List<double> AX = theFilter.theFilerWork(theInformationController.accelerometerX);
            List<double> AY = theFilter.theFilerWork(theInformationController.accelerometerY);
            List<double> AZ = theFilter.theFilerWork(theInformationController.accelerometerZ);
            List<double> GX = theFilter.theFilerWork(theInformationController.gyroX);
            List<double> GY = theFilter.theFilerWork(theInformationController.gyroY);
            List<double> GZ = theFilter.theFilerWork(theInformationController.gyroZ);
            List<double> MX = theFilter.theFilerWork(theInformationController.magnetometerX);
            List<double> MY = theFilter.theFilerWork(theInformationController.magnetometerY);
            List<double> MZ = theFilter.theFilerWork(theInformationController.magnetometerZ);

            if (HeadingMehtod.SelectedIndex == 0)
            {
                //记录移动的方向 （方法1直接读取电子罗盘的信息）
                for (int i = 0; i < indexBuff.Count; i++)
                {
                    double degree = theFilteredD[indexBuff[i]];
                    degree = headingOffsetExtraCanculate(degree);
                    theStepAngeUse.Add(degree);
                }

            }
            else if (HeadingMehtod.SelectedIndex == 1)
            {
                //方法2 微软建议滤波法(这个方法其实也是一种读取电子罗盘的方法，只不过更加复杂一点点)
                if (indexBuff.Count <= 1)//数据量不够就直接用方法1
                {
                    for (int i = 0; i < indexBuff.Count; i++)
                    {
                        double degree = theFilteredD[indexBuff[i]];
                        degree = headingOffsetExtraCanculate(degree);
                        theStepAngeUse.Add(degree);
                    }
                }
                else
                {
                    theStepAngeUse.Add(theFilteredD[indexBuff[0]]);//第一个是定的
                    for (int j = 1; j < indexBuff.Count; j++)
                    {
                        List<double> checkUse = new List<double>();
                        int preIndex = indexBuff[j - 1];
                        int nowIndex = indexBuff[j];
                        for (int i = preIndex; i <= nowIndex; i++)
                            checkUse.Add(theFilteredD[i]);

                        double degree = theAngelController.getAngelNow(checkUse);
                        degree = headingOffsetExtraCanculate(degree);
                        theStepAngeUse.Add(degree);
                    }
                }
            }
            else if (HeadingMehtod.SelectedIndex == 2)
            {
                List<double> AHRSZ = theFilter.theFilerWork(theInformationController.AHRSZFromClient, 0.1f);
                for (int i = 0; i < indexBuff.Count; i++)
                {
                    double degree = AHRSZ[indexBuff[i]];
                    degree = headingOffsetExtraCanculate(degree);
                    theStepAngeUse.Add(degree);
                }
            }
            else if (HeadingMehtod.SelectedIndex == 3)
            {
                List<double> IMUZ = theFilter.theFilerWork(theInformationController.IMUZFromClient, 0.1f);
                try
                {
                    for (int i = 0; i < indexBuff.Count; i++)
                    {
                        //Console.WriteLine("--------" + AHRSZ[indexBuff[i]] + "--------" + IMUZ[indexBuff[i]]);
                        double degree = IMUZ[indexBuff[i]];
                        degree = headingOffsetExtraCanculate(degree);
                        theStepAngeUse.Add(degree);
                    }

                }
                catch
                {
                    // Console.WriteLine("imuMethod crashed using compass reading");
                    for (int i = 0; i < indexBuff.Count; i++)
                    {
                        double degree = theFilteredD[indexBuff[i]];
                        degree = headingOffsetExtraCanculate(degree);
                        theStepAngeUse.Add(degree);
                    }
                }
            }
            else if (HeadingMehtod.SelectedIndex == 4)
            {
                List<double> AHRSZ = theFilter.theFilerWork(theInformationController.AHRSZFromClient, 0.1f);
                List<double> IMUZ = theFilter.theFilerWork(theInformationController.IMUZFromClient, 0.1f);
                //记录移动的方向
                for (int i = 0; i < indexBuff.Count; i++)
                {
                    if (i <= 1)
                    {
                        double degree = AHRSZ[indexBuff[i]];
                        degree = headingOffsetExtraCanculate(degree, false);
                        theStepAngeUse.Add(degree);
                    }
                    else
                    {
                        double degree = theAngelController.AHRSIMUSelect(indexBuff[i - 1], indexBuff[i], AHRSZ, IMUZ);
                        degree = headingOffsetExtraCanculate(degree, false);
                        theStepAngeUse.Add(degree);
                    }
                }
            }

            else if (HeadingMehtod.SelectedIndex == 5)
            {
                for (int i = 0; i < indexBuff.Count; i++)
                {
                    int indexUse = indexBuff[i];
                    double degree = theAngelController.getYawWithAM(
                        AX[indexUse], AY[indexUse], AZ[indexUse], MX[indexUse], MY[indexUse], MZ[indexUse]
                        );
                    degree = headingOffsetExtraCanculate(degree, false);
                    theStepAngeUse.Add(degree);
                }
            }
            else if (HeadingMehtod.SelectedIndex == 6)
            {
                //方法3，AHRS方法

                double ax = 0, ay = 0, az = 0, gx = 0, gy = 0, gz = 0, mx = 0, my = 0, mz = 0;
                // Console.WriteLine(AX.Count +"-------------");

                for (int i = 0; i < indexBuff.Count; i++)
                {
                    double degree = 0;
                    ax = AX[indexBuff[i]];
                    ay = AY[indexBuff[i]];
                    az = AZ[indexBuff[i]];
                    gx = GX[indexBuff[i]];
                    gy = GY[indexBuff[i]];
                    gz = GZ[indexBuff[i]];
                    mx = MX[indexBuff[i]];
                    my = MY[indexBuff[i]];
                    mz = MZ[indexBuff[i]];
                    degree = theAngelController.AHRSupdate(gx, gy, gz, ax, ay, az, mx, my, mz);
                    degree = headingOffsetExtraCanculate(degree, false);
                    theStepAngeUse.Add(degree);
                }
                // theAngelController.makeMehtod3Clear();
            }
            else if (HeadingMehtod.SelectedIndex == 7)
            {
                //方法3，IMU方法
                double ax = 0, ay = 0, az = 0, gx = 0, gy = 0, gz = 0;//, mx = 0, my = 0, mz = 0;
                // Console.WriteLine(AX.Count +"-------------");

                for (int i = 0; i < indexBuff.Count; i++)
                {
                    double degree = 0;
                    ax = AX[indexBuff[i]];
                    ay = AY[indexBuff[i]];
                    az = AZ[indexBuff[i]];
                    gx = GX[indexBuff[i]];
                    gy = GY[indexBuff[i]];
                    gz = GZ[indexBuff[i]];
                    //mx = MX[indexBuff[i]];
                    //my = MY[indexBuff[i]];
                    //mz = MZ[indexBuff[i]];
                    degree = theAngelController.IMUupdate(gx, gy, gz, ax, ay, az);
                    degree = headingOffsetExtraCanculate(degree, false);
                    theStepAngeUse.Add(degree);
                }
                // theAngelController.makeMehtod3Clear();
            }
         
            //记录最新的移动方向
            if (theStepAngeUse.Count > 0)
            SystemSave.stepAngleNow = theStepAngeUse[theStepAngeUse.Count - 1];

        }

        //保存训练用的数据////////////////////////////////////////////////////////////////////////////////////
        void saveTrainBase(List<double> theFilteredAZ)
        {
            List<string> theTrainBase = new List<string>();
           // List<double> FilteredX = theFilter.theFilerWork(theInformationController.GPSPositionX);
           //List<double> FilteredY = theFilter.theFilerWork(theInformationController.GPSPositionY);
            //Console.WriteLine("V = "+indexBuff.Count);
            //for (int ee = 0; ee < FilteredX.Count; ee++)
            //{ 
            //Console.WriteLine("GPSX - " + FilteredX[ee]);
            //Console.WriteLine("GPSY - " + FilteredY[ee]);
            // }
            //生成真数据（GPS）------------------------------------------------------------------------------------------------
            //for (int i = 1; i < indexBuff.Count; i++)
            //{
            //    theTrainBase.Add(
            //    theTrainFileMake.getSaveTrainFile(indexBuff[i - 1], indexBuff[i], theFilteredAZ, FilteredX, FilteredY, theInformationController.timeStep )
            //    );
            //}
            //if (theTrainBase != null && theTrainBase.Count >= 1)
            //{
            //    theFileSaver.saveInformation(theTrainBase, "TrainBase/GPSBase-" + DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss") + ".txt");
            //}

            //生成假数据（GPS）------------------------------------------------------------------------------------------------
            //theTrainBase.Clear();
            ////对时间戳（或者其他数据包B的数据）进行滤波来匹配数据
            //List<long> timeUse = theFilter.theFilerWork(theInformationController.timeStep, 0.4f, true, theInformationController.accelerometerZ.Count);
            //for (int i = 1; i < indexBuff.Count; i++)
            //{
            //    theTrainBase.Add(
            //    theTrainFileMake.getSaveTrainFileFake(indexBuff[i - 1], indexBuff[i], theFilteredAZ, FilteredX, FilteredY, timeUse )
            //    );
            //}
            //if (theTrainBase != null && theTrainBase.Count >= 1)
            //{
            //    theFileSaver.saveInformation(theTrainBase, "TrainBase/GPSFake-" + DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss") + ".txt");
            //}
            //生成通用的数据--------------------------------------------------------------------------------------------------
            theTrainBase = theTrainFileMake.getSaveTrainFile(indexBuff, theInformationController);
            theTrainBase.Clear();
            if (theTrainBase != null && theTrainBase.Count >= 1)
            {
                theFileSaver.saveInformation(theTrainBase, "TrainBase/TrainBase-" + DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss") + ".txt");
            }
            //生成决策树数据----------------------------------------------------------------------------------------------------
            theTrainBase.Clear();
            theTrainBase = theTrainFileMake.getSaveTrainFileForTreeDemo(indexBuff, theInformationController, theStepLengthController);
            if (theTrainBase != null && theTrainBase.Count >= 1)
            {
                theFileSaver.saveInformation(theTrainBase, "TrainBase/TrainBaseTree.txt");
            }
            //生成回归测试使用的文件--------------------------------------------------------------------------------------------
            theTrainBase.Clear();
            List<long> timeUse = theFilter.theFilerWork(theInformationController.timeStep, 0.4f, true, theInformationController.accelerometerZ.Count);
            for (int i = 1; i < indexBuff.Count; i++)
            {
                theTrainBase.Add(
                theTrainFileMake.getSaveTrainFileFake(indexBuff[i - 1], indexBuff[i], theFilteredAZ,timeUse)
                );
            }
            if (theTrainBase != null && theTrainBase.Count >= 1)
            {
                theFileSaver.saveInformation(theTrainBase, "TrainBase/TrainBaseFake.txt");
            }
        }

        //生成输出在tips里面的显示信息
        void makeLabelMehtod(int stepcounts2 = 0)
        {
            SystemSave.allStepCount = SystemSave.stepCount + indexBuff.Count;
            if (stepCheckMethod.SelectedIndex == 0)
            {
                theStepLabel.Content = "（当前分组）原始数据步数：" + PeackSearcher.TheStepCount + "    去除不可能项步数：" + thePeackFinder.peackBuff.Count;
                theStepLabel.Content += "\n历史存储步数：" + SystemSave.stepCount + "    总步数：" + SystemSave.allStepCount ;
                //先做thePositionController.getPositions(theStepAngeUse, theStepLengthUse);用来刷新内部缓存
            }
            else if (stepCheckMethod.SelectedIndex == 1)
            {
                theStepLabel.Content = "（当前分组）"+"  上界： "+ SystemSave.uperGateForShow.ToString("f2") +"  下界： "+ SystemSave.downGateForShow.ToString("f2")+ "  总步数： " + thePeackFinder.peackBuff.Count ;
                theStepLabel.Content += "\n历史存储步数：" + SystemSave.stepCount + "    总步数：" + SystemSave.allStepCount;
            }
            else if (stepCheckMethod.SelectedIndex == 2)
            {
                theStepLabel.Content = "当前阶段步数：" + stepcounts2 + "    总步数：" + (SystemSave.stepCount2 + stepcounts2);
            }
            else if (stepCheckMethod.SelectedIndex == 3)
            {
                theStepLabel.Content = "当前阶段步数：" + stepcounts2 + "    总步数：" + (SystemSave.stepCount2 + stepcounts2);
            }
            theStepLabel.Content += "\n绘制图像： " + SystemSave.pictureNumber;
            theStepLabel.Content += "    当前分组数据条目： " + theInformationController.accelerometerY.Count + "    总数据条目：" + SystemSave.getValuesCount(theInformationController.accelerometerY.Count);
            theStepLabel.Content += "\n-----------------------------------------------------------------------------";
            theStepLabel.Content += "\n使用轴向：" + stepCheckAxisUse.SelectionBoxItem;
            theStepLabel.Content += "\n思想： " + theStepAxis.getMoreInformation(stepCheckAxisUse.SelectedIndex);
            theStepLabel.Content += "\n\n" + stepCheckMethod.SelectionBoxItem;
            theStepLabel.Content += "\n思想： "+stepExtra.getMoreInformation(stepCheckMethod.SelectedIndex);
            theStepLabel.Content += "\n\n步长计算方法：" + StepLengthMethod.SelectionBoxItem;
            theStepLabel.Content += "\n思想： " + theStepLengthController.getMoreInformation(StepLengthMethod.SelectedIndex);
            theStepLabel.Content += "\n\n方向计算方法：" + HeadingMehtod.SelectionBoxItem;
            theStepLabel.Content += "\n思想： " + theAngelController .getMoreInformation(HeadingMehtod.SelectedIndex);
        }

        //获得走路的最新的slope数值使用
        //可以在后面用来判断行走姿态，算是为后面做的一个简单的准备工作，留下接口
        //实际上这个也分为两种
        //一种是每出现一个周期检查一下这个周期的slope的数值
        //一种是钉死窗口滑动，检查这个窗口的数值
        void stepModeCheck(List<int> indexBuff)
        {
            if (indexBuff.Count > 1)
            {
                List<double> X = theFilter.theFilerWork(theInformationController.accelerometerX);
                List<double> Y = theFilter.theFilerWork(theInformationController.accelerometerY);
                List<double> Z = theFilter.theFilerWork(theInformationController.accelerometerZ);
                double slopeWithPeack =  theStepModeCheckController.getModeCheckWithPeack
                    (
                      X, Y ,Z ,
                    indexBuff[indexBuff.Count - 2], indexBuff[indexBuff.Count-1]
                    );
                SystemSave.slopNow = slopeWithPeack;
                double slopeWithwindow = theStepModeCheckController.getModeCheckWithWindow(X, Y, Z);
                theStage = theStage.ChangeState(slopeWithwindow);
                stepSlopLabel.Content ="[" +theStage.getInformation() +"]" + "  Slop： " + slopeWithwindow.ToString("f2") + " / " + slopeWithPeack.ToString("f2");
            }
            else
            { 
               stepSlopLabel.Content  = "[" + theStage.getInformation() + "]" + "  Slop： 0/0";
            }
        }

        /*************************************************方法1（比较原始）*************************************************************/
        /*************************************************这个方法不再扩展也不会使用，放在这里是为了保留一个简单的架构*************************************************************/
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
                double theStepLength = theStepLengthController.getStepLength1();
                double theDegree = theAngelController.getAngelNow(theFilteredD);
                thePositionController.calculationPosition(theDegree, theStepLength);
                POSITION.Text += "\n角度： " + theDegree.ToString("f4") + " 步长： " + theStepLength.ToString("f4") + " 坐标： " + thePositionController.getPosition();
            }
        }

        /******************************按钮事件控制单元***********************************************/

        //保存数据控制的按钮
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            if (theFileSaver == null)
                return;
            string saveStringUse = "";
            for (int i = 0; i < SystemSave.savedPositions.Count; i++)
                saveStringUse += SystemSave.savedPositions[i].toString() + "\n";
            theFileSaver.saveInformation(saveStringUse);
        }

        //startServer按钮控制单元
        private void button2_Click(object sender, RoutedEventArgs e)
        {
           string information = makeStart();
            MessageBox.Show(information);
            //为了防止多次开启
            theStartButton.IsEnabled = false;
            theStopButton.IsEnabled = true;
        }

        //closeServer按钮控制单元
        private void button3_Click_1(object sender, RoutedEventArgs e)
        {
            string information = makeClose();
            MessageBox.Show(information);
            //为了防止多次开启
            theStartButton.IsEnabled = true;
            theStopButton.IsEnabled = false;
        }


        //开启封装方法
        public string  makeStart()
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
            stepExtra = new stepDetection();
            theStepModeCheckController = new stepModeCheck();
            theTrainFileMake = new TrainFileMaker();
            theStepAxis = new stepAxis();
            //相关工程更新
            makeFlashController();

            //正式开始
            string showInformation = theServerController.startTheServer();
            return showInformation;
        }
        //关闭分装方法
        public string makeClose()
        {
            string showInformation = theServerController.closeServer();
            return showInformation;
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

        //处理移动朝向方向的按钮逻辑
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


        //这个方法辅助用的字段两个：
        private double  X1Save = 0;
        private double  Y1Save = 0;
        private int savedIndex = 0;
        //实时的动态绘制路线图 (方法2,单纯地累加，误差一定会有，但是相对可控性提高)
        private void drawPositionLineOnTime(double X2 , double Y2,double heading)
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
            drawLine.Y1 = theCanvas.Height / 2 - Y1Save * 5;//怕跑出范围，所以就缩小了一些
            drawLine.X2 = theCanvas.Width / 2 + X2 * 5;//怕跑出范围，所以就缩小了一些
            drawLine.Y2 = theCanvas.Height / 2 - Y2* 5;//怕跑出范围，所以就缩小了一些
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
            theCanvas.Children.Remove(HeadingImage);
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
                drawLine.Y1 = theCanvas.Height / 2 - SystemSave.savedPositions[u].Y * 5;//怕跑出范围，所以就缩小了一些
                drawLine.X2 = theCanvas.Width / 2 + SystemSave.savedPositions[u+1].X * 5;//怕跑出范围，所以就缩小了一些
                drawLine.Y2 = theCanvas.Height / 2 - SystemSave.savedPositions[u+1].Y * 5;//怕跑出范围，所以就缩小了一些
                drawLine.Stroke = new SolidColorBrush(SystemSave.theOldColor);
                drawLine.StrokeThickness = 2;
                theCanvas.Children.Add(drawLine);
            }

            for (int u = 0; u < thePositionController.theTransformPosition.Count-1; u++)
            {
                Line drawLine = new Line();
                drawLine.X1 = theCanvas.Width / 2 + thePositionController.theTransformPosition[u].X *5;//怕跑出范围，所以就缩小了一些
                drawLine.Y1 = theCanvas.Height / 2 - thePositionController.theTransformPosition[u].Y * 5;//怕跑出范围，所以就缩小了一些
                drawLine.X2 = theCanvas.Width / 2 + thePositionController.theTransformPosition[u+1].X * 5;//怕跑出范围，所以就缩小了一些
                drawLine.Y2 = theCanvas.Height / 2 - thePositionController.theTransformPosition[u+1].Y * 5;//怕跑出范围，所以就缩小了一些
                drawLine.Stroke = new SolidColorBrush(SystemSave.theNewColor);
                drawLine.StrokeThickness = 2;
                theCanvas.Children.Add(drawLine);
            }

 
            flashHeadingPicture(thePositionController.theTransformPosition);
        }


        //修正显示当前方向的箭头的坐标和朝向
        private void flashHeadingPicture(List<transForm> trransForm)
        {
            if (trransForm.Count == 0)
                return;
            int indexForLast = thePositionController.theTransformPosition.Count - 1;
            //进行移动
            double imagePositionX =  theCanvas.Width / 2 + thePositionController.theTransformPosition[indexForLast].X * 5;//怕跑出范围，所以就缩小了一些
            double imagePositionY = theCanvas.Height / 2 - thePositionController.theTransformPosition[indexForLast].Y * 5;//怕跑出范围，所以就缩小了一些
            imagePositionX -= HeadingImage.Width / 2;
            imagePositionY -= HeadingImage.Height / 2;
            Canvas.SetLeft(HeadingImage, imagePositionX);
            Canvas.SetTop(HeadingImage, imagePositionY);
            theCanvas.Children.Add(HeadingImage);

            //设定旋转角
            double headingAngel = thePositionController.theTransformPosition[indexForLast].heading;
            double width = HeadingImage.ActualWidth;
            double height = HeadingImage.ActualHeight;
            HeadingImage.RenderTransform = new RotateTransform(headingAngel);
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


        private void button5_Click(object sender, RoutedEventArgs e)
        {
            button5.Background = new SolidColorBrush(SetColor());
        }
        
        private void button8_Click(object sender, RoutedEventArgs e)
        {
            //为了防止出现同步设定的问题，这里使用了简单的单例模式的思想
            if (SystemSave. theSettingWindow == null)
            {
                SystemSave.theSettingWindow = new socketServer.Settings();
                SystemSave.theSettingWindow.startSet(this);
                SystemSave.theSettingWindow.Show();
            }

        }

        //查询状态的方法
        //检查server是不是已经开启了
        public bool isServerStarted()
        {
            if (this.theServerController == null)
                return false;
            if (this.theServerController.Opened == false)
                return false ;
            return true;
        }
        //////////////////////////下面是一些示例用的方法///////////////////////////////////////////////////////////

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

        private void button9_Click(object sender, RoutedEventArgs e)
        {
            new insectUnityPlay().Show();
        }

        private void button2_Click_1(object sender, RoutedEventArgs e)
        {
            string theFileName = "route" + DateTime.Now.ToString("yyyy - MM - dd - hh - mm - ss") + ".png";
            new pictureMaker(). saveCanvasPicture(theCanvas , @"routeMap/"+theFileName );
            MessageBox.Show("路线图已经保存在routeMap文件夹中\n文件名："+theFileName);
        }

        //获得信息处理单元
        public double get3DHeadingPffset()
        {
            //如果没有数据就得不到最新的偏移量
            if (theInformationController == null)
                return -9999;
            double count = theInformationController.compassDegree.Count;
            if (count <= 0)
                return -9999;

            int indexForStep = indexBuff.Count - 1;
            int index = indexBuff[indexForStep];
            List<double> theFCompass = theFilter.theFilerWork(theInformationController.compassDegree);
            List<double> theFAHRS = theFilter.theFilerWork(theInformationController.AHRSZFromClient);
            if(HeadingMehtod.SelectedIndex == 0 || HeadingMehtod.SelectedIndex == 1)
                return 0 - theFCompass[index];
            else if(HeadingMehtod.SelectedIndex == 2)
                return 0 - theFAHRS[index];

            return 0;
        }

        private void StepLengthMethod_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (StepLengthMethod.SelectedIndex == 7 && SystemSave.StepLengthTree== null)
            {
                string informationS = "使用决策树进行模式判断需要首先建立一棵决策树。\n";
                informationS += "为了减少计算量本程序决策树的设定只有一次。\n";
                informationS += "建立决策树的过程如下：\nsettings ——> StepLength ——> Build Decision Tree\n";
                informationS += "如果没有创建决策树，步长估计方法为立即数";
                MessageBox.Show(informationS);
            }
        }

        private void theAppendix_Click(object sender, RoutedEventArgs e)
        {
            if (SystemSave.theAppendixWindow == null)
            {
                SystemSave.theAppendixWindow = new Windows.Appendix();
                SystemSave.theAppendixWindow.Show();
            }
        }
    }
}

        
        