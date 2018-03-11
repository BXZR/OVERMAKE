using socketServer.Codes;
using socketServer.Codes.DecisionTree;
using socketServer.Codes.stages;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
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
        stepModeCheck theStepModeCheckController;//推断行走状态：站立，行走，奔跑用 的控制单元
        TrainFileMaker theTrainFileMake;//制作数据集的控制单元
        stepAxis theStepAxis;//用来判定使用哪一个轴向的封装
        FSMBasic theStage = new StageStance();//当前状态的推断，使用的是有限状态机
        ZAxisMoveController theZMoveController;//Z轴向移动的控制单元
        CarCanculater theCarController;//控制车速的控制单元

        //公有的存储空间
        List<double> theStepAngeUse = new List<double>();
        List<double> theStepLengthUse = new List<double>();
        List<double> theFilteredD = new List<double>();
        List<double> theStairMode = new List<double>();//Z轴向移动

        int stepcounts = 0;//当前阶段的步数
        List<int> indexBuff = new List<int>();//确认一步的下标存储
        List<double> theFilteredAZ = new List<double>();//当前使用的轴

        //需要单个窗口记录的信息这些信息将会被发送到对应的客户端
        public double allStepCount = 0;
        public double allStepCountSave = 0;
        public double stepLengthNow = 0;
        public double stepAngleNow = 0;
        public double slopNow = 0;
        public double heightNow = 0;
        public double stepCount2 = 0;



        public MainWindow()
        {
            InitializeComponent();
        }

        //整体工作模式
        private void makeFlashController()
        {
            Log.saveLog(LogType.information, "开始进行刷新程序");
            //方法切换在这里判断执行
            if (theWorkType == workType.timerFlash)//有一点强硬的阶段
            {
                tm.Tick += new EventHandler(flashQuitck);
                tm.Interval = TimeSpan.FromSeconds(0.6);
            }
            else if (theWorkType == workType.withSavedData)//个人更加推荐这种方法
            {
                tm.Tick += new EventHandler(withSavedData);
                tm.Interval = TimeSpan.FromSeconds(SystemSave.systemFlashTimer);
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
            //如果扩展成多人同时操作（工作量超大）
            //可以考虑将informationController做成List然后所有内容循环处理

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
            string informationForPosition = thePositionController.getPositions(theStepAngeUse, theStepLengthUse, theStairMode);
            showPositiopnInformations(informationForPosition);
            //更新slope的数值
            stepModeCheck(indexBuff);
            //显示分辨率
            showResolutionn();
            //制作输出显示的内容
            //做这些输出显示内容是整合过的主题功能，与上面的slop和分辨率不太一样
            makeLabelMehtod(stepcounts);
            //绘制路线图
            drawPicturesShow();

            //如果数据足够多，就需要保存成一张图像,同时做刷新处理和一些额外的补充计算
            if (theInformationController.accelerometerY.Count > SystemSave.buffCount)
            makeFlash();
        }

        //额外补充计算////////////////////////////////////////////////////////////////////////////////////

        //POSITION显示的文本的制作过程
        //但是注意，获取焦点显示最后一行的方法会与选择方法的combox有选择上的冲突
        private void showPositiopnInformations(string informaitonIn)
        {
            POSITION.Text = informaitonIn;
            //光标定位到文本最后，如果得到用户选定，就会自动定位到最后一行了
            POSITION.Select(POSITION.Text.Length, 0);
           // POSITION.Focus();//获取焦点(这个与其他控件有冲突，不如直接让用户自己选定)
        }

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
            Log.saveLog(LogType.information, "缓冲区满，进行数据保存");
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
            thePictureMaker.createPictureFromData(theInformationController , SystemSave.DataPicturePath);
            // thePictureMaker.createPictureFromDataComplex(theInformationController);//暂时先不必要用特别复杂的图像生成方法，会卡
            theInformationController.flashInformation();
            SystemSave.stepCount += thePeackFinder.peackBuff.Count;
            this.allStepCountSave +=  thePeackFinder.peackBuff.Count;
            //方法2的刷新和存储
            SystemSave.stepCount2 += stepExtra.peackBuff.Count;
            stepCount2 += stepExtra.peackBuff.Count;
            stepExtra.makeFlash();
            SystemSave.makeFlash();
            theAngelController.makeMethod34Save();//对于这个连续的服务器端的AHRS需要这样做才能保持连续性能
            if (SystemSave.SystemModeInd == 2)
                theCarController.makeFlashForCar();
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
        List<double> stepCheckAxis( bool useFilter = true)
        {
            List<double> theFilteredAZ = new List<double>();
            switch (stepCheckAxisUse.SelectedIndex)//选择不同的轴向
            {
                case 0:
                    //基础方法:用Z轴加速度来做
                    theFilteredAZ = theStepAxis.AZ(theInformationController , theFilter, useFilter);
                    break;
                case 1:
                    //实验用方法：X轴向
                    theFilteredAZ = theStepAxis.AX(theInformationController, theFilter, useFilter);
                    break;
                case 2:
                    //实验用方法：X轴向
                    theFilteredAZ = theStepAxis.AY(theInformationController, theFilter, useFilter);
                    break;
                case 3:
                    //基础方法:用三个轴的加速度平方和开根号得到
                    theFilteredAZ = theStepAxis.ABXYZ(theInformationController, theFilter, useFilter);
                    break;
                case 4:
                    //基础方法:用三个轴的加速度最大方差得到
                    theFilteredAZ = theStepAxis.XYZMaxVariance(theInformationController, theFilter, useFilter);
                    break;
            }
            return theFilteredAZ;
        }

        //判断走了一步的方法///////////////////////////////////////////////////////////
        void stepDectionUse()
        {
            //0-1两种模式是针对行人的
            if (SystemSave.SystemModeInd <= 1)
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
                    indexBuff = stepExtra.stepDectionExtration4(theFilteredAZ, thePeackFinder);
                }
                else if (stepCheckMethod.SelectedIndex == 2)
                {
                    indexBuff = stepExtra.stepDectionExtration3(theFilteredAZ, thePeackFinder);
                }
                else if (stepCheckMethod.SelectedIndex == 3)
                {
                    //方法3：重复性判断方法，相对比较严格（感觉不是人能用的）
                    indexBuff = stepExtra.stepDetectionExtra1(theFilteredAZ);
                }
                else if (stepCheckMethod.SelectedIndex == 4)
                {
                    //方法4零点交叉
                    indexBuff = stepExtra.stepDetectionExtra2(theFilteredAZ);
                }

            //    //带约束的行人模式之下需要额外的计算来更加严格地剔除错误的步子
            if (SystemSave.SystemModeInd == 1)
                indexBuff = stepExtra.FixedStepCalculate(theInformationController, theFilter, indexBuff);
        }
            else if (SystemSave.SystemModeInd == 2)
            {
                //在这里的滤波仅仅是一个与之前程序配合的做法，但是也是主要的BUG所在
                indexBuff = theCarController.stepDectionExtrationForCar(theFilteredAZ);
            }
}

        //判断走楼梯的模式的方法
        //开销很大...
        void StairCheck(List<int> indexBuff)
        {
            //根本就不计算Z轴位移，其实也是另一种开关
            if (ZAxisSelect.SelectedIndex == 0)
            {
                theStairMode = new List<double>();
                for (int i = 0; i < indexBuff.Count; i++)
                {
                    theStairMode.Add(0);
                }
            }
            //决策树的方法
            if (ZAxisSelect.SelectedIndex == 1)
            {
                theStairMode = new List<double>();
                if (SystemSave.StairTree == null)
                {
                    theStairMode = theZMoveController.noneMethod(indexBuff);
                }
                else
                {
                    //这些数据在一些复杂的方法中会用到，因此计算出来备用
                    List<double> ax = theFilter.theFilerWork(theInformationController.accelerometerX);
                    List<double> ay = theFilter.theFilerWork(theInformationController.accelerometerY);
                    List<double> az = theFilter.theFilerWork(theInformationController.accelerometerZ);
                    List<double> gx = theFilter.theFilerWork(theInformationController.gyroX);
                    List<double> gy = theFilter.theFilerWork(theInformationController.gyroY);
                    List<double> gz = theFilter.theFilerWork(theInformationController.gyroZ);

                    theStairMode =  theZMoveController.DecisitionTreeMethod(indexBuff,ax,ay,az,gx,gy,gz);
                }
            }
            //人工神经网络方法
            if (ZAxisSelect.SelectedIndex == 2)
            {
                if (SystemSave.AccordANNforSLForZAxis == null)
                {
                    theStairMode = theZMoveController.noneMethod(indexBuff);
                }
                else 
                {
                    //这些数据在一些复杂的方法中会用到，因此计算出来备用
                    List<double> ax = theFilter.theFilerWork(theInformationController.accelerometerX);
                    List<double> ay = theFilter.theFilerWork(theInformationController.accelerometerY);
                    List<double> az = theFilter.theFilerWork(theInformationController.accelerometerZ);
                    List<double> gx = theFilter.theFilerWork(theInformationController.gyroX);
                    List<double> gy = theFilter.theFilerWork(theInformationController.gyroY);
                    List<double> gz = theFilter.theFilerWork(theInformationController.gyroZ);

                    theStairMode = theZMoveController.ANNZMove(indexBuff, ax, ay, az, gx, gy, gz);
                }
            }
            //记录最新的移动步长
            if (theStairMode.Count > 0)
            {
                this.heightNow = theStairMode[theStairMode.Count - 1];
                SystemSave.heightNow = theStairMode[theStairMode.Count - 1];
            }
        }


        //获取步长的方法//////////////////////////////////////////////////////////////////////////
        //这个方法在多个整体方法中是共用的
        //实际上获得步长的方法就只在这里进行计算，因为小方法很多，的也是在这里进行分类的
        void stepLengthGet(List<int> indexBuff, List<double> AZUse)
        {
            if (SystemSave.SystemModeInd <= 1)
            {
                //这些数据在一些复杂的方法中会用到，因此计算出来备用
                List<double> ax = theFilter.theFilerWork(theInformationController.accelerometerX);
                List<double> ay = theFilter.theFilerWork(theInformationController.accelerometerY);
                List<double> az = theFilter.theFilerWork(theInformationController.accelerometerZ);
                List<double> gx = theFilter.theFilerWork(theInformationController.gyroX);
                List<double> gy = theFilter.theFilerWork(theInformationController.gyroY);
                List<double> gz = theFilter.theFilerWork(theInformationController.gyroZ);

                //方法0，最土鳖的方法直接就是立即数
                if (StepLengthMethod.SelectedIndex == 0)
                {
                    for (int i = 0; i < indexBuff.Count; i++)
                    {
                        theStepLengthUse.Add(theStepLengthController.getStepLength1());
                    }
                }
                //方法1
                else if (StepLengthMethod.SelectedIndex == 1)
                {
                    for (int i = 0; i < indexBuff.Count; i++)
                    {
                        if (i >= 1)
                        {
                            theStepLengthUse.Add(theStepLengthController.getStepLength1(theStepAngeUse[i - 1], theStepAngeUse[i]));//这个写法后期需要大量的扩展，或者说这才是这个程序的核心所在
                        }
                        else
                            theStepLengthUse.Add(theStepLengthController.getStepLength1());
                    }
                }
                //方法2，一般公式
                else if (StepLengthMethod.SelectedIndex == 2)
                {
                    List<long> timeUse = theFilter.theFilerWork(theInformationController.timeStep);
                    for (int i = 0; i < indexBuff.Count; i++)
                    {
                        if (i >= 1)
                        {
                            // for (int v = 0; v < theInformationController.timeStep.Count; v++)
                            //    Console.WriteLine(theInformationController.timeStep[v]);
                            double stepLength = theStepLengthController.getStepLength2(indexBuff[i - 1], indexBuff[i], AZUse, timeUse);
                            theStepLengthUse.Add(stepLength);//这个写法后期需要大量的扩展，或者说这才是这个程序的核心所在
                        }
                        else
                            theStepLengthUse.Add(theStepLengthController.getStepLength1());
                    }
                }
                //方法3，一般公式 + 决策树
                else if (StepLengthMethod.SelectedIndex == 3)
                {
                    List<long> timeUse = theFilter.theFilerWork(theInformationController.timeStep);
                    for (int i = 0; i < indexBuff.Count; i++)
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
                                int mode = SystemSave.StepLengthTree.searchModeWithTree(ax[indexBuff[i]], ay[indexBuff[i]], az[indexBuff[i]], gx[indexBuff[i]], gy[indexBuff[i]], gz[indexBuff[i]]);
                                // Console.WriteLine("modeNow = " + mode);
                                double stepLength = theStepLengthController.getStepLength2WithMode(indexBuff[i - 1], indexBuff[i], AZUse, timeUse, mode);
                                theStepLengthUse.Add(stepLength);//这个写法后期需要大量的扩展，或者说这才是这个程序的核心所在
                            }
                        }
                        else
                        {
                            theStepLengthUse.Add(theStepLengthController.getStepLength1());
                        }
                    }
                }
                //方法4，一般公式 + 线性回归
                else if (StepLengthMethod.SelectedIndex == 4)
                {
                    List<long> timeUse = theFilter.theFilerWork(theInformationController.timeStep);
                    for (int i = 0; i < indexBuff.Count; i++)
                    {
                        if (i >= 1)
                        {
                            double stepLength = theStepLengthController.getStepLengthWithKLinear(indexBuff[i - 1], indexBuff[i], AZUse, timeUse);
                            theStepLengthUse.Add(stepLength);//这个写法后期需要大量的扩展，或者说这才是这个程序的核心所在
                        }
                        else
                            theStepLengthUse.Add(theStepLengthController.getStepLength1());
                    }
                }

                //方法5，一般公式 + ANN
                else if (StepLengthMethod.SelectedIndex == 5)
                {
                    List<long> timeUse = theFilter.theFilerWork(theInformationController.timeStep);
                    for (int i = 0; i < indexBuff.Count; i++)
                    {
                        if (i >= 1)
                        {
                            if (SystemSave.AccordANNforSL == null)
                            {
                                theStepLengthUse.Add(theStepLengthController.getStepLength1());

                            }
                            else
                            {
                                double stepLength = theStepLengthController.getStepLengthWithANN(indexBuff[i - 1], indexBuff[i], AZUse, timeUse);
                                theStepLengthUse.Add(stepLength);//这个写法后期需要大量的扩展，或者说这才是这个程序的核心所在
                            }
                        }
                        else
                            theStepLengthUse.Add(theStepLengthController.getStepLength1());
                    }
                }

                //方法5，身高相关
                else if (StepLengthMethod.SelectedIndex == 6)
                {
                    for (int i = 0; i < indexBuff.Count; i++)
                    {
                        theStepLengthUse.Add(theStepLengthController.getStepLength3());
                    }
                }
                //方法6，身高相关2，这个看上去更专业一点
                else if (StepLengthMethod.SelectedIndex == 7)
                {
                    List<long> timeUse2 = theFilter.theFilerWork(theInformationController.timeStep);
                    for (int i = 0; i < indexBuff.Count; i++)
                    {
                        if (i >= 1)
                        {
                            theStepLengthUse.Add(theStepLengthController.getStepLength11(indexBuff[i - 1], indexBuff[i], timeUse2));
                        }
                        else
                            theStepLengthUse.Add(theStepLengthController.getStepLength1());
                    }
                }
                //方法6，加速度开四次根号 Square  acceleration formula  （Weinberg approach）
                else if (StepLengthMethod.SelectedIndex == 8)
                {
                    for (int i = 0; i < indexBuff.Count; i++)
                    {
                        if (i >= 1)
                            theStepLengthUse.Add(theStepLengthController.getStepLength4(indexBuff[i - 1], indexBuff[i], AZUse));
                        else
                            theStepLengthUse.Add(theStepLengthController.getStepLength1());
                    }
                }
                //方法7 说是可以克服每一个行人的不同特征，其实就是加速度平均上的计算 （Scarlet approach）
                else if (StepLengthMethod.SelectedIndex == 9)
                {
                    for (int i = 0; i < indexBuff.Count; i++)
                    {
                        if (i >= 1)
                            theStepLengthUse.Add(theStepLengthController.getStepLength5(indexBuff[i - 1], indexBuff[i], AZUse));
                        else
                            theStepLengthUse.Add(theStepLengthController.getStepLength1());
                    }
                }
                //方法8，加速度平均开三次根号的做法
                else if (StepLengthMethod.SelectedIndex == 10)
                {
                    for (int i = 0; i < indexBuff.Count; i++)
                    {
                        if (i >= 1)
                            theStepLengthUse.Add(theStepLengthController.getStepLength6(indexBuff[i - 1], indexBuff[i], AZUse));
                        else
                            theStepLengthUse.Add(theStepLengthController.getStepLength1());
                    }
                }

                //方法9，使用关于腿长的倒置钟摆的方法（很好玩的方法）
                else if (StepLengthMethod.SelectedIndex == 11)
                {
                    List<long> timeUse2 = theFilter.theFilerWork(theInformationController.timeStep);
                    for (int i = 0; i < indexBuff.Count; i++)
                    {
                        if (i >= 1)
                        {
                            theStepLengthUse.Add(theStepLengthController.getStepLength8(indexBuff[i - 1], indexBuff[i], AZUse, timeUse2));
                        }
                        else
                            theStepLengthUse.Add(theStepLengthController.getStepLength1());
                    }
                }
                //方法10，单纯的某方向的二次积分的方法（更加适合于车）
                else if (StepLengthMethod.SelectedIndex == 12)
                {
                    
                    List<long> timeUse2 = theFilter.theFilerWork(theInformationController.timeStep);
                    for (int i = 0; i < indexBuff.Count; i++)
                    {
                        if (i >= 1)
                        {
                            //对于行人来说每一段都应该清零一下 重置当前记录下来的速度，在行人阶段其实就是清0,所以传入true
                            theStepLengthUse.Add(theStepLengthController.getStepLength9(indexBuff[i - 1], indexBuff[i], AZUse, timeUse2));
                        }
                        else
                            theStepLengthUse.Add(theStepLengthController.getStepLength1());
                    }
                }
            }
 //-------------------------------------------------------------------人车处理分界线------------------------------------------------------------------------//
            //针对车的第一种方法就是加速度积分
            else if (SystemSave.SystemModeInd == 2)
            {
                List<double> AxisForCarNoFilter= stepCheckAxis(false);//不滤波的各种轴向切换
                List<long> timeNoFilter = theInformationController.timeStep;

                List<double> AxisForCar = theFilter.theFilerWork(AxisForCarNoFilter) ; 
                List<long> TimeForCar = theFilter.theFilerWork(timeNoFilter);
                theCarController.flashSpeedForIntegral(false);//重置当前记录下来的速度
                //theStepLengthController.VNowForCarSave是从上一个阶段继承过来的数值
                for (int i = 0; i < indexBuff.Count; i++)
                {
                    if (i >= 1)
                        //theStepLengthUse.Add(theCarController.getCarSpeed(indexBuff[i - 1], indexBuff[i], AxisForCarNoFilter, timeNoFilter));
                        theStepLengthUse.Add(theCarController.getCarSpeed(indexBuff[i - 1], indexBuff[i], AxisForCar , TimeForCar));
                    else
                        theStepLengthUse.Add(0);
                }
            }
            //记录最新的移动步长
            if (theStepLengthUse.Count > 0)
            {
                this.stepLengthNow = theStepLengthUse[theStepLengthUse.Count - 1];
                SystemSave.stepLengthNow = theStepLengthUse[theStepLengthUse.Count - 1];
            }
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
                for (int i = 0; i < indexBuff.Count; i++)
                {
                    int indexUse = indexBuff[i];
                    double degree = theAngelController.getAngelNowWithMatrixRotate(
                        AX[indexUse], AY[indexUse], AZ[indexUse], MX[indexUse], MY[indexUse], MZ[indexUse]
                        );
                    degree = headingOffsetExtraCanculate(degree, false);
                    theStepAngeUse.Add(degree);
                }
            }
            else if (HeadingMehtod.SelectedIndex == 3)
            {
                List<double> AHRSZ = theFilter.theFilerWork(theInformationController.AHRSZFromClient, 0.1f);
                for (int i = 0; i < indexBuff.Count; i++)
                {
                    double degree = AHRSZ[indexBuff[i]];
                    degree = headingOffsetExtraCanculate(degree);
                    theStepAngeUse.Add(degree);
                }
            }
            else if (HeadingMehtod.SelectedIndex == 4)
            {
                //AHRS 方法server
                double ax = 0, ay = 0, az = 0, gx = 0, gy = 0, gz = 0, mx = 0, my = 0, mz = 0;
                // Console.WriteLine(AX.Count +"-------------");
                theAngelController.makeMehtod34Clear();
                List<double> headings = new List<double>();
                for (int i = 0; i < theInformationController.accelerometerX.Count; i++)
                {
                    double degree = 0;
                    ax = theInformationController.accelerometerX[i];
                    ay = theInformationController.accelerometerY[i];
                    az = theInformationController.accelerometerZ[i];
                    gx = theInformationController.gyroX[i];
                    gy = theInformationController.gyroY[i];
                    gz = theInformationController.gyroZ[i];
                    mx = theInformationController.magnetometerX[i];
                    my = theInformationController.magnetometerY[i];
                    mz = theInformationController.magnetometerZ[i];
                    degree = theAngelController.AHRSupdate(gx, gy, gz, ax, ay, az, mx, my, mz);
                    degree = headingOffsetExtraCanculate(degree, false);
                    headings.Add(degree);
                }
                headings = theFilter.theFilerWork(headings);
                for (int i = 0; i < indexBuff.Count; i++)
                {
                    theStepAngeUse.Add(headings[indexBuff[i]]);
                }
            }
            else if (HeadingMehtod.SelectedIndex == 5)
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

            else if (HeadingMehtod.SelectedIndex == 6)
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

            else if (HeadingMehtod.SelectedIndex == 7)
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
            else if (HeadingMehtod.SelectedIndex == 8)
            {
                //IMU 方法server
                double ax = 0, ay = 0, az = 0, gx = 0, gy = 0, gz = 0;
                theAngelController.makeMehtod34Clear();
                List<double> headings = new List<double>();
                for (int i = 0; i < theInformationController.accelerometerX.Count; i++)
                {
                    double degree = 0;
                    ax = theInformationController.accelerometerX[i];
                    ay = theInformationController.accelerometerY[i];
                    az = theInformationController.accelerometerZ[i];
                    gx = theInformationController.gyroX[i];
                    gy = theInformationController.gyroY[i];
                    gz = theInformationController.gyroZ[i];
                    degree = theAngelController.IMUupdate(gx, gy, gz, ax, ay, az);
                    degree = headingOffsetExtraCanculate(degree, false);
                    headings.Add(degree);
                }
                headings = theFilter.theFilerWork(headings);
                for (int i = 0; i < indexBuff.Count; i++)
                {
                    theStepAngeUse.Add(headings[indexBuff[i]]);
                }
            }

            //记录最新的移动方向
            if (theStepAngeUse.Count > 0)
            {
                this.stepAngleNow = theStepAngeUse[theStepAngeUse.Count - 1];
                SystemSave.stepAngleNow = theStepAngeUse[theStepAngeUse.Count - 1];
            }

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
            //生成文件，这个文件用做训练集，决策树或者其他操作，通用的--------------------------------------------------------------------------------------------------
            theTrainBase.Clear();
            theTrainBase = theTrainFileMake.getSaveTrainFile(indexBuff, theFilteredAZ,  theInformationController,theStepLengthController);
            if (theTrainBase != null && theTrainBase.Count >= 1)
            {
                Log.saveLog(LogType.information, "保存训练用的数据");
                theFileSaver.saveInformation(theTrainBase, SystemSave.TrainBasedFilePath);
            }
        }

        //生成输出在tips里面的显示信息
        void makeLabelMehtod(int stepcounts2 = 0)
        {
            //如果不是车的模式，也就是两种行人模式，那么就正常显示各种东西就可以了
            if (SystemSave.SystemModeInd < 2)
            {
                allStepCount = allStepCountSave + indexBuff.Count;
                SystemSave.allStepCount = SystemSave.stepCount + indexBuff.Count;
                if (stepCheckMethod.SelectedIndex == 0)
                {
                    theStepLabel.Content = "（当前分组）原始数据步数：" + PeackSearcher.TheStepCount + "    去除不可能项步数：" + thePeackFinder.peackBuff.Count;
                    theStepLabel.Content += "\n历史存储步数：" + SystemSave.stepCount + "/" + allStepCountSave + "    总步数：" + SystemSave.allStepCount + "/" + allStepCount;
                    //先做thePositionController.getPositions(theStepAngeUse, theStepLengthUse);用来刷新内部缓存
                }
                else if (stepCheckMethod.SelectedIndex == 1)
                {
                    theStepLabel.Content = "（当前分组）" + "  上界： " + SystemSave.uperGateForShow.ToString("f2") + "  下界： " + SystemSave.downGateForShow.ToString("f2") + "  总步数： " + thePeackFinder.peackBuff.Count;
                    theStepLabel.Content += "\n历史存储步数：" + SystemSave.stepCount + "/" + allStepCountSave + "    总步数：" + SystemSave.allStepCount + "/" + allStepCount;
                }
                else if (stepCheckMethod.SelectedIndex == 2)
                {
                    theStepLabel.Content = "当前阶段步数：" + stepcounts2 + "    总步数：" + (SystemSave.stepCount2 + stepcounts2) + "/" + (stepcounts2 + stepCount2);
                }
                else if (stepCheckMethod.SelectedIndex == 3)
                {
                    theStepLabel.Content = "当前阶段步数：" + stepcounts2 + "    总步数：" + (SystemSave.stepCount2 + stepcounts2) + "/" + (stepcounts2 + stepCount2);
                }
                theStepLabel.Content += "\n绘制图像： " + SystemSave.pictureNumber;
                theStepLabel.Content += "    当前分组数据条目： " + theInformationController.accelerometerY.Count + "    总数据条目：" + SystemSave.getValuesCount(theInformationController.accelerometerY.Count);
                theStepLabel.Content += "\n-----------------------------------------------------------------------------";
                theStepLabel.Content += "\n使用轴向：" + stepCheckAxisUse.SelectionBoxItem;
                theStepLabel.Content += "\n思想： " + theStepAxis.getMoreInformation(stepCheckAxisUse.SelectedIndex);
                theStepLabel.Content += "\n\n判步方法：" + stepCheckMethod.SelectionBoxItem;
                theStepLabel.Content += "\n思想： " + stepExtra.getMoreInformation(stepCheckMethod.SelectedIndex);
                theStepLabel.Content += "\n\n步长计算方法：" + StepLengthMethod.SelectionBoxItem;
                theStepLabel.Content += "\n思想： " + theStepLengthController.getMoreInformation(StepLengthMethod.SelectedIndex);
                theStepLabel.Content += "\n\n方向计算方法：" + HeadingMehtod.SelectionBoxItem;
                theStepLabel.Content += "\n思想： " + theAngelController.getMoreInformation(HeadingMehtod.SelectedIndex);
                theStepLabel.Content += "\n\n上下位移计算方法：" + ZAxisSelect.SelectionBoxItem;
                theStepLabel.Content += "\n思想： " + theZMoveController.getMoreInformation(ZAxisSelect.SelectedIndex);
            }
            //如果是车的模式只需要显示车相关的内容就可以了吧
            else if (SystemSave.SystemModeInd == 2)
            {
                allStepCount = allStepCountSave + indexBuff.Count;
                SystemSave.allStepCount = SystemSave.stepCount + indexBuff.Count;
                theStepLabel.Content = "当前阶段位移计算次数：" + stepcounts2 + "    总位移计算次数：" + (SystemSave.stepCount2 + stepcounts2) + "/" + (stepcounts2 + stepCount2);
                theStepLabel.Content += "\n绘制图像： " + SystemSave.pictureNumber;
                theStepLabel.Content += "    当前分组数据条目： " + theInformationController.accelerometerY.Count + "    总数据条目：" + SystemSave.getValuesCount(theInformationController.accelerometerY.Count);
                theStepLabel.Content += "\n-----------------------------------------------------------------------------";
                theStepLabel.Content += "\n使用轴向：" + stepCheckAxisUse.SelectionBoxItem;
                theStepLabel.Content += "\n思想： " + theStepAxis.getMoreInformation(stepCheckAxisUse.SelectedIndex);
                theStepLabel.Content += "\n\n方向计算方法：" + HeadingMehtod.SelectionBoxItem;
                theStepLabel.Content += "\n思想： " + theAngelController.getMoreInformation(HeadingMehtod.SelectedIndex);
                theStepLabel.Content += "\n\n单位位移计算方法：";
                theStepLabel.Content += "\n" + theCarController.getTipInformation() ;
            }
        }

        //显示分辨率
        void showResolutionn()
        {
            //一些额外的显示内容
            resolutionLabel.Content = "1m = " + SystemSave.routeLineScale.ToString("f2") +"pixel";
            if (SystemSave.routeLineScale > 1)
                resolutionLabel.Content += "s";
        }

        //获得走路的最新的slope数值使用
        //可以在后面用来判断行走姿态，算是为后面做的一个简单的准备工作，留下接口
        //实际上这个也分为两种
        //一种是每出现一个周期检查一下这个周期的slope的数值
        //一种是钉死窗口滑动，检查这个窗口的数值
        void stepModeCheck(List<int> indexBuff)
        {
            if (SystemSave.SystemModeInd <= 1)
            {
                if (indexBuff.Count > 1)
                {
                    List<double> X = theFilter.theFilerWork(theInformationController.accelerometerX);
                    List<double> Y = theFilter.theFilerWork(theInformationController.accelerometerY);
                    List<double> Z = theFilter.theFilerWork(theInformationController.accelerometerZ);
                    double slopeWithPeack = theStepModeCheckController.getModeCheckWithPeack
                        (
                          X, Y, Z,
                        indexBuff[indexBuff.Count - 2], indexBuff[indexBuff.Count - 1]
                        );
                    this.slopNow = slopeWithPeack;
                    SystemSave.slopNow = slopeWithPeack;
                    double slopeWithwindow = theStepModeCheckController.getModeCheckWithWindow(X, Y, Z);
                    theStage = theStage.ChangeState(slopeWithwindow, indexBuff, theFilteredAZ);
                    stepSlopLabel.Content = "[" + theStage.getInformation() + "]" + "  Slop： " + slopeWithwindow.ToString("f2") + " / " + slopeWithPeack.ToString("f2");
                }
                else
                {
                    stepSlopLabel.Content = "[" + theStage.getInformation() + "]" + "  Slop： 0/0";
                }
            }
            else if (SystemSave.SystemModeInd == 2)
            {
                stepSlopLabel.Content = "";
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

        //封装的更高一级的开始
        public void pressStartButton()
        {
            try
            {
                string information = makeStart();
                MessageBox.Show(information);
                //为了防止多次开启
                theStartButton.IsEnabled = false;
                theStopButton.IsEnabled = true;
                button4.IsEnabled = true;
                button6.IsEnabled = true;
            }
            catch
            {
                Log.saveLog(LogType.error, "服务端开启失败");
                MessageBox.Show("服务端开启失败\n原因可能是IP端口号设定不对\n可以在Setting ——> System Config中进行设定");
            }
        }

        //startServer按钮控制单元
        private void button2_Click(object sender, RoutedEventArgs e)
        {
            pressStartButton();
        }

        //closeServer按钮控制单元
        private void button3_Click_1(object sender, RoutedEventArgs e)
        {
            string information = makeClose();
            MessageBox.Show(information);
            //为了防止多次开启
            theStartButton.IsEnabled = true;
            theStopButton.IsEnabled = false;
            button4.IsEnabled = false;
            button6.IsEnabled = false;
        }


        //开启封装方法
        //可以指定informationController，这样可以在整体的server中获得分项信息处理之后的信息内容 
        public string  makeStart(information theInformationIn = null , bool isSingle = true)
        {
            //全世界的初始化都在这里完成
            if (isSingle == false)
            {
                theStartButton.IsEnabled = false;
                theStopButton.IsEnabled = false;
            }

            if (theInformationIn == null)
                theInformationController = new information();
            else
                theInformationController = theInformationIn;

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
            theZMoveController = new ZAxisMoveController();
            theCarController = new CarCanculater();
            //制作提示信息
            makeToolTips();
            //相关工程更新
            makeFlashController();

            //正式开始
            //单人可以出这个效果，多人使用共用server不必再次绑定
            if (isSingle)
            {
                SystemSave.theMainWindowForSingle = this;
                string showInformation = theServerController.startTheServer();
                Log.saveLog(LogType.information, showInformation);
                return showInformation;
            }
            return "";
        }

        //tool tips的制作
        private void makeToolTips()
        {
            string[] information = theStepAxis.getMoreInformation();
            for (int i = 0; i < stepCheckAxisUse.Items.Count; i++)
                (stepCheckAxisUse.Items[i] as ComboBoxItem).ToolTip = information[i];

            information = stepExtra.getMoreInformation();
            for (int i = 0; i < stepCheckMethod.Items.Count; i++)
                (stepCheckMethod.Items[i] as ComboBoxItem).ToolTip = information[i];

            information = theStepLengthController.getMoreInformation();
            for (int i = 0; i < StepLengthMethod.Items.Count; i++)
                (StepLengthMethod.Items[i] as ComboBoxItem).ToolTip = information[i];

            information = theAngelController.getMoreInformation();
            for (int i = 0; i < HeadingMehtod.Items.Count; i++)
                (HeadingMehtod.Items[i] as ComboBoxItem).ToolTip = information[i];

            information = theZMoveController.getMoreInformation();
            for (int i = 0; i < ZAxisSelect.Items.Count; i++)
                (ZAxisSelect.Items[i] as ComboBoxItem).ToolTip = information[i];
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
            //单人模式之下关闭窗口就是彻底结束
            if (theServerController != null && SystemSave.SystemServerMode == 1)
            {
                theServerController.closeServer();
            }
            //窗口关闭的时候做一次Log的保存
            Log.writeLogToFile();
        }


        //非自动测试触发的按钮
        private void button4_Click(object sender, RoutedEventArgs e)
        {
            //展示“滤波”之后的加速度的曲线
            List<double> theFiltered = theFilteredAZ;
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
            int stepCount = thePeackFinder.countStepWithStatic(theFiltered);

            ChartWindow theChartWindow = new ChartWindow();

            theChartWindow.CreateChartSpline(UseDataType.accelerometerY, theFiltered, stepCheckAxisUse.SelectionBoxItem.ToString());
            theChartWindow.Show();
            //MessageBox.Show("一共" + stepCount + "步");
        }

        //处理移动朝向方向的按钮逻辑
        private void button6_Click(object sender, RoutedEventArgs e)
        {
            //为了保证数据干净，要做一次滤波
            // List<double> theFiltered =  theFilter.theFilerWork(theInformationController.compassDegree);
            List<double> theFiltered = theStepAngeUse;
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
            drawLine.X1 = theCanvas.Width / 2 + X1Save * SystemSave.routeLineScale;//怕跑出范围，所以就缩小了一些
            drawLine.Y1 = theCanvas.Height / 2 - Y1Save * SystemSave.routeLineScale;//怕跑出范围，所以就缩小了一些
            drawLine.X2 = theCanvas.Width / 2 + X2 * SystemSave.routeLineScale;//怕跑出范围，所以就缩小了一些
            drawLine.Y2 = theCanvas.Height / 2 - Y2* SystemSave.routeLineScale;//怕跑出范围，所以就缩小了一些
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
                drawLine.X1 = theCanvas.Width / 2 + SystemSave.savedPositions[u].X * SystemSave.routeLineScale;//怕跑出范围，所以就缩小了一些
                drawLine.Y1 = theCanvas.Height / 2 - SystemSave.savedPositions[u].Y * SystemSave.routeLineScale;//怕跑出范围，所以就缩小了一些
                drawLine.X2 = theCanvas.Width / 2 + SystemSave.savedPositions[u+1].X * SystemSave.routeLineScale;//怕跑出范围，所以就缩小了一些
                drawLine.Y2 = theCanvas.Height / 2 - SystemSave.savedPositions[u+1].Y * SystemSave.routeLineScale;//怕跑出范围，所以就缩小了一些
                drawLine.Stroke = new SolidColorBrush(SystemSave.theOldColor);
                drawLine.StrokeThickness = 2;
                theCanvas.Children.Add(drawLine);
            }

            for (int u = 0; u < thePositionController.theTransformPosition.Count-1; u++)
            {
                Line drawLine = new Line();
                drawLine.X1 = theCanvas.Width / 2 + thePositionController.theTransformPosition[u].X * SystemSave.routeLineScale;//怕跑出范围，所以就缩小了一些
                drawLine.Y1 = theCanvas.Height / 2 - thePositionController.theTransformPosition[u].Y * SystemSave.routeLineScale;//怕跑出范围，所以就缩小了一些
                drawLine.X2 = theCanvas.Width / 2 + thePositionController.theTransformPosition[u+1].X * SystemSave.routeLineScale;//怕跑出范围，所以就缩小了一些
                drawLine.Y2 = theCanvas.Height / 2 - thePositionController.theTransformPosition[u+1].Y * SystemSave.routeLineScale;//怕跑出范围，所以就缩小了一些
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
            try
            {
                int indexForLast = thePositionController.theTransformPosition.Count - 1;
                //进行移动
                double imagePositionX = theCanvas.Width / 2 + thePositionController.theTransformPosition[indexForLast].X * SystemSave.routeLineScale;//怕跑出范围，所以就缩小了一些
                double imagePositionY = theCanvas.Height / 2 - thePositionController.theTransformPosition[indexForLast].Y * SystemSave.routeLineScale;//怕跑出范围，所以就缩小了一些
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
            catch
            {
                Log.saveLog(LogType.error, "方向键头出现控制冲突，一次操作被隔绝");
                Console.WriteLine("方向键头出现控制冲突，一次操作被隔绝");
                theCanvas.Children.Remove(HeadingImage);
                int indexForLast = thePositionController.theTransformPosition.Count - 1;
                //进行移动
                double imagePositionX = theCanvas.Width / 2 + thePositionController.theTransformPosition[indexForLast].X * SystemSave.routeLineScale;//怕跑出范围，所以就缩小了一些
                double imagePositionY = theCanvas.Height / 2 - thePositionController.theTransformPosition[indexForLast].Y * SystemSave.routeLineScale;//怕跑出范围，所以就缩小了一些
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
        }
        //设置路径颜色的方法
        public Color SetColor()
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
            new pictureMaker(). saveCanvasPicture(theCanvas , SystemSave.RoutePictureSavePath+theFileName );
            MessageBox.Show("路线图已经保存在routeMap文件夹中\n文件名："+theFileName);
            Log.saveLog(LogType.information, "保存一张路径的截图");
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
            if (StepLengthMethod.SelectedIndex == 3 && SystemSave.StepLengthTree== null)
            {
                string informationS = "使用决策树进行模式判断需要首先建立一棵决策树。\n";
                informationS += "为了减少计算量本程序决策树的设定只有一次。\n";
                informationS += "建立决策树的过程如下：\nSettings ——> StepLengthMore ——> Flash Or Build Decision Tree For Step Length\n";
                informationS += "如果没有创建决策树，步长估计方法为立即数方法";
                informationS += "如果修改设定，请重建决策树刷新设定";
                MessageBox.Show(informationS);
            }
            if (StepLengthMethod.SelectedIndex == 5 && SystemSave.AccordANNforSL == null)
            {
                string informationS = "使用人工神经网络进行模式判断需要首先建立人工神经网络。\n";
                informationS += "为了减少计算量本程序决策树的设定只有一次。\n";
                informationS += "建立人工神经网络的过程如下：\nSettings ——> StepLengthMore ——> Flash Or Build ANN For SL\n";
                informationS += "如果没有创建人工神经网络，步长估计方法为立即数方法";
                informationS += "如果修改设定，请重建人工神经网络刷新设定";
                MessageBox.Show(informationS);
            }
        }


        private void ZAxisSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(ZAxisSelect.SelectedIndex == 1 && SystemSave.StairTree == null)
            {
                string informationS = "选用了决策树的方法进行分类估计\n但是现在需要的决策树尚未建立\n";
                informationS += "\n建立决策树的过程如下：\nsettings ——> ZAxisMove ——> Flash Or Build Decision Tree For ZAxis\n";
                informationS += "\n如果没有建立决策树，此方法不计算Z轴移动";
                MessageBox.Show(informationS);
            }
            if (ZAxisSelect.SelectedIndex == 2 && SystemSave.AccordANNforSLForZAxis == null)
            {
                string informationS = "选用了人工神经网络的方法进行分类估计\n但是现在需要的人工神经网络尚未建立\n";
                informationS += "\n建立人工神经网络的过程如下：\nsettings ——> ZAxisMove ——> Flash Or Build ANN For ZAxis\n";
                informationS += "\n如果没有建立人工神经网络，此方法不计算Z轴移动";
                MessageBox.Show(informationS);
            }
        }

        private void HeadingMehtod_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }


        void ScaleCanvasBackPicture()
        {

        }

        //清空一下绘画的内容，重新定位到中心点，但是这个中心点的坐标已经修改为位移之后的坐标
        //一个很有意思的扩展项
        private void clearDraw_Click(object sender, RoutedEventArgs e)
        {
            operateFlashPosition();
        }

        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SystemSave.SystemModeInd = comboBox.SelectedIndex;

            //对于车子来说，不用判步也不用轴（轴固定），也没有步长和Z轴向的移动（当然也是可以扩展的）
            if (SystemSave.SystemModeInd == 2)
            {
                //stepCheckAxisUse.IsEnabled = false;
                stepCheckMethod.IsEnabled = false;
                StepLengthMethod.IsEnabled = false;
                ZAxisSelect.IsEnabled = false;
            }
            else
            {
               // stepCheckAxisUse.IsEnabled = true;
                stepCheckMethod.IsEnabled = true;
                StepLengthMethod.IsEnabled = true;
                ZAxisSelect.IsEnabled = true;
            }
        }

        //-------------------------------这个部分包含的操作可能也会被客户端通过网络方式调用（自己扯的RPC机制）-------------------------------------------//
        //所有这一类的方法都以“operateXXXXXX”作为命名方式

        //绘制重定位，当前位置变为地图中心，消除掉轨迹内容，从当前位置开始重新定位
        public void operateFlashPosition()
        {
            theInformationController.flashInformation();
            if (thePositionController.theTransformPosition.Count > 0)
            {
                transForm temp = thePositionController.theTransformPosition[thePositionController.theTransformPosition.Count - 1];
                SystemSave.startPositionX = temp.X;
                SystemSave.startPositionY = temp.Y;
                SystemSave.startPositionZ = temp.Z;
            }
            SystemSave.savedPositions.Clear();
            theCarController.flashZero();
        }
    }
}

        
        