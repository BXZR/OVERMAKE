using socketServer.Codes.DecisionTree;
using socketServer.Windows;
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


        //后直接在最开始的界面打开的时候，IP设置不可以重复进行，否则太乱了
        private void makeCancalSomeSetting()
        {
            if (theMainWindow == null)
            {
                ServerIPText.IsEnabled = false;
                LocalIPGetter.IsEnabled = false;
                ServerPortText.IsEnabled = false;
                theDrawColorButton.IsEnabled = false;
            }
        }
        public void startSet(MainWindow theIN = null)
        {
            theMainWindow = theIN;
            if (theMainWindow == null)
            {
                Tip.Content = "当前不可在这个面板设置服务器IP和端口。";
            }
            else if (theMainWindow.isServerStarted() == false)
            {
                saveRestartButton.IsEnabled = false;//没有开启服务器就没必要重启
               
            }
            else
            {
                Tip.Content = "检测到服务器已经运行，不推荐修改IP设置。";
                systemFlashTimerText.IsEnabled = false;//刷新速率在启动之后是不能修改的
            }
        }

        private void button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                setValues();
                SaveCommonFormulaWeightsFamily();
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
                setValues();
                SaveCommonFormulaWeightsFamily();
                if (theMainWindow != null)
                {
                    string information = theMainWindow.makeClose();
                    information += "\n-----------------------\n" + theMainWindow.makeStart();
                    MessageBox.Show(information);
                }
                this.Close();
                

            }
            catch (Exception eee)
            {
                MessageBox.Show("出错：" + eee.Message);
            }
        }


        private void setValues()
        {
            SystemSave.serverIP = ServerIPText.Text;
            SystemSave.serverPort = Convert.ToInt32(ServerPortText.Text);
            SystemSave.angleOffset = Convert.ToInt32(packageOffset.Text);
            SystemSave.zeroCrossOffset = Convert.ToDouble(ZeroLine.Text);
            SystemSave.Stature = Convert.ToDouble(Stature.Text);
            SystemSave.WeightForFemale = Convert.ToDouble(FemailWeight.Text);
            SystemSave.WeightForMale = Convert.ToDouble(MailWeight.Text) ;
            SystemSave.stepLengthWeightForAMinus = Convert.ToDouble(Aminus.Text);
            SystemSave.stepLengthWeightForScarlet = Convert.ToDouble(Scarlet.Text );
            SystemSave.stepLengthWeightForKim = Convert.ToDouble(Kim.Text);
            SystemSave.peackThresholdForStepDetection = Convert.ToInt32( PeackStepthreshold.Text );

            SystemSave.maxAForStart = Convert.ToDouble(MaxAForStart.Text);
            SystemSave.minAForStart = Convert.ToDouble(MinAForStart.Text);
            SystemSave.Dertshold = Convert.ToDouble(ChangeValue.Text);
            SystemSave.uperGateForStart = Convert.ToDouble(UpForStart.Text);
            SystemSave.downGateForStart = Convert.ToDouble(DownForStart.Text);
            SystemSave.TrainBasedFile = TrainBasePath.Text;
            SystemSave.StairHeight = Convert.ToDouble( StairHeight.Text );
            SystemSave.startAngleForCHM1 = Convert.ToDouble(HCM1Start.Text);
            if (isMaleCheckc.IsChecked == true)
                SystemSave.isMale = true;
            else
                SystemSave.isMale = false;
            if (Draw_route_with_buffer.IsChecked == true)
                SystemSave.drawWithBuffer = true;
            else
                SystemSave.drawWithBuffer = false;
            if (UseOffSet.IsChecked == true)
                SystemSave.UseHeadingOffset = true;
            else
                SystemSave.UseHeadingOffset = false;
            if (DynamicallyZeroLine.IsChecked == true)
                SystemSave.isDynamicallyZeroLineForStepDection = true;
            else
                SystemSave.isDynamicallyZeroLineForStepDection = false;

            if (isPruning.IsChecked == true)
                SystemSave.isCutForDecisionTree = true;
            else
                SystemSave.isCutForDecisionTree = false;

            SystemSave.FilterMode = FilterModeSelects.SelectedIndex;

            SystemSave.DecisionTreeMethodID = TreeMethod.SelectedIndex;
            SystemSave.CanculateHeadingMode = HeadingCanculateMode.SelectedIndex;

            SystemSave.startPositionX = Convert.ToDouble(StartPositionX.Text);
            SystemSave.startPositionY = Convert.ToDouble(StartPositionY.Text);
            SystemSave.startPositionZ = Convert.ToDouble(StartPositionZ.Text);
            SystemSave.routeLineScale = routeLineScaleSlider.Value; //Convert.ToDouble(routeLineScale.Text);

            SystemSave.SystemModeInd = SystemModeSelect.SelectedIndex;
            SystemSave.stepLengthForImmediate = Convert.ToDouble(immediateSL.Text);
            SystemSave.legLengthInHeight  = Convert.ToDouble(LegInStature.Text);
            SystemSave.changeGateForImmediate2 = Convert.ToDouble(headingChangeGate.Text);
            SystemSave.percentWhenMeetChangeGate = Convert.ToDouble(StepLengthRatio.Text);
            SystemSave.MSHeadingGate = Convert.ToDouble(MSHeadingGate.Text);
            SystemSave.systemFlashTimer = Convert.ToDouble(systemFlashTimerText.Text);
            SystemSave.accordANNHiddenLayerCount = Convert.ToInt32(ANNHiddenNeuronsText.Text);
            SystemSave.accordANNTrainTime = Convert.ToInt32(ANNTrainTimesText.Text);

            SystemSave.StaturaMethod2_A = Convert.ToDouble(saturate2AText.Text);
            SystemSave.StaturaMethod2_B = Convert.ToDouble(saturate2BText.Text);
        }

        private void saveRestart_Loaded(object sender, RoutedEventArgs e)
        {
            ServerIPText.Text = SystemSave.serverIP;
            ServerPortText.Text = SystemSave.serverPort.ToString();
            packageOffset.Text = SystemSave.angleOffset.ToString();
            ZeroLine.Text  = SystemSave.zeroCrossOffset.ToString();
            Stature.Text = SystemSave.Stature.ToString();
            FemailWeight.Text = SystemSave.WeightForFemale.ToString();
            MailWeight.Text = SystemSave.WeightForMale.ToString();
            Aminus.Text = SystemSave.stepLengthWeightForAMinus.ToString();
            Scarlet.Text = SystemSave.stepLengthWeightForScarlet.ToString();
            Kim.Text = SystemSave.stepLengthWeightForKim.ToString() ;
            PeackStepthreshold.Text = SystemSave.peackThresholdForStepDetection.ToString();

            MaxAForStart.Text = SystemSave.maxAForStart.ToString();
            MinAForStart.Text = SystemSave.minAForStart.ToString();
            ChangeValue.Text = SystemSave.Dertshold.ToString();
            UpForStart.Text = SystemSave.uperGateForStart.ToString();
            DownForStart.Text = SystemSave.downGateForStart.ToString();
            TrainBasePath.Text = SystemSave.TrainBasedFile;
            TreeMethod.SelectedIndex = SystemSave.DecisionTreeMethodID;
            HeadingCanculateMode.SelectedIndex = SystemSave.CanculateHeadingMode;
            StairHeight.Text = SystemSave.StairHeight.ToString();
            HCM1Start.Text = SystemSave.startAngleForCHM1.ToString();
            if (SystemSave.isMale)
                isMaleCheckc.IsChecked = true;
            else
                isMaleCheckc.IsChecked = false;

            if (SystemSave.drawWithBuffer)
                Draw_route_with_buffer.IsChecked = true;
            else
                Draw_route_with_buffer.IsChecked = false;

            if (SystemSave.UseHeadingOffset == true)
                UseOffSet.IsChecked = true;
            else
                UseOffSet.IsChecked = false;

            if (SystemSave.isDynamicallyZeroLineForStepDection )
                DynamicallyZeroLine.IsChecked = true;
            else
                DynamicallyZeroLine.IsChecked = false;

            if (SystemSave.isCutForDecisionTree == true)
                isPruning.IsChecked = true;
            else
                isPruning.IsChecked  = false;

            FilterModeSelects.SelectedIndex = SystemSave.FilterMode;

            StartPositionX.Text = SystemSave.startPositionX.ToString();
            StartPositionY.Text = SystemSave.startPositionY.ToString();
            StartPositionZ.Text = SystemSave.startPositionZ.ToString();
            routeLineScaleSlider.Value = SystemSave.routeLineScale;

            SystemModeSelect.SelectedIndex = SystemSave.SystemModeInd;

            //加载绘制颜色
            theDrawColorButton.Foreground = new SolidColorBrush(SystemSave.theOldColor);
            //记录一次最初的数值
            getStartValue();

            if (SystemSave.SystemServerMode == 2)//多人模式之下不允许restart，因为多客户端有可能造成混乱
                saveRestartButton.IsEnabled = false;
            immediateSL.Text = SystemSave.stepLengthForImmediate.ToString();
            LegInStature.Text = SystemSave.legLengthInHeight.ToString();

            headingChangeGate.Text = SystemSave.changeGateForImmediate2.ToString();
            StepLengthRatio.Text = SystemSave.percentWhenMeetChangeGate.ToString();
            MSHeadingGate.Text = SystemSave.MSHeadingGate.ToString();
            TipsMake();//制作tips
            CommonFamilyCountChoice.SelectedIndex = SystemSave.CommonFormulaWeights.Count -1;
            systemFlashTimerText.Text = SystemSave.systemFlashTimer.ToString();
            ANNHiddenNeuronsText.Text = SystemSave.accordANNHiddenLayerCount.ToString();
            ANNTrainTimesText.Text = SystemSave.accordANNTrainTime.ToString();

            saturate2AText.Text = SystemSave.StaturaMethod2_A.ToString();
            saturate2BText.Text = SystemSave.StaturaMethod2_B.ToString();

            makeCommonFormulaWeightsFamily();//初始的公式族的内容
            makeCancalSomeSetting();
        }

        //第一次打开窗口的时候记录默认数值
        private static bool hasBasicValue = false;
        private static string ValueServerIPText;
        private static string ValueServerPortText;
        private static string ValuepackageOffset;
        private static string ValueZeroLine;
        private static string ValueStature;
        private static string ValueFemailWeight;
        private static string ValueMailWeight;
        private static bool ValueIsMale;
        private static bool ValueDrawWithBuff;
        private static bool ValueUseHeadOffset;
        private static string ValueAminus;
        private static string ValueScarlet;
        private static string ValuesKim;
        private static string ValueStepPeackThreshold;

        private static string ValueMaxAForStart;
        private static string ValueMinAForStart;
        private static string ValueDertshold;
        private static string ValueUperGateForStart;
        private static string ValueDownGateForStart;
        private static bool ValueIsDynamicallyZeroLine;
        private static string ValueDecisionTreePath;
        private static int ValueTreeMethod;
        private static string ValueStairHeight;

        private static bool ValueIsCuttingDectionTree;
        private static int  ValueHeadingCanculateMode;
        private static string  ValueSystemStartAngleForCHM1;

        private static string ValueStartPositionX;
        private static string ValueStartPositionY;
        private static string ValueStartPositionZ;

        private static double ValuerRouteLineScale;
        private static int ValueSystemMode;

        private static int ValueUseFilters;
        private static string ValueImmeDiateSL;
        private static string ValueLegInStature;

        private static string ValueHeadingGate;
        private static string ValueRatioInSL2;
        private static string ValueMSHeadingGate;
        private static string ValueSystemFlashTimer;

        private static string ValueAccordANNHiddenLayerCount;
        private static string ValueAccordANNTrainTime;

        private static string ValueStature2A;
        private static string ValueStature2B;

        void getStartValue()
        {
            if (hasBasicValue == false)
            {
              ValueServerIPText = SystemSave.serverIP;
              ValueServerPortText = SystemSave.serverPort.ToString();
              ValuepackageOffset = SystemSave.angleOffset.ToString();
              ValueZeroLine = SystemSave.zeroCrossOffset.ToString();
              ValueStature = SystemSave.Stature.ToString();
              ValueFemailWeight = SystemSave.WeightForFemale.ToString();
              ValueMailWeight = SystemSave.WeightForMale.ToString();
              ValueIsMale = SystemSave.isMale;
              ValueDrawWithBuff = SystemSave.drawWithBuffer;
              ValueAminus = SystemSave.stepLengthWeightForAMinus.ToString();
              ValueScarlet = SystemSave.stepLengthWeightForScarlet.ToString();
              ValuesKim = SystemSave.stepLengthWeightForKim.ToString();
              ValueUseHeadOffset = SystemSave.UseHeadingOffset;
              ValueStepPeackThreshold = SystemSave.peackThresholdForStepDetection.ToString();

              ValueMaxAForStart = SystemSave.maxAForStart.ToString();
              ValueMinAForStart = SystemSave.minAForStart.ToString();
              ValueDertshold = SystemSave.Dertshold.ToString();
              ValueUperGateForStart = SystemSave.uperGateForStart.ToString();
              ValueDownGateForStart = SystemSave.downGateForStart.ToString();
              ValueIsDynamicallyZeroLine = SystemSave.isDynamicallyZeroLineForStepDection;
              ValueDecisionTreePath = SystemSave.TrainBasedFile;
              ValueTreeMethod = SystemSave.DecisionTreeMethodID;

              ValueIsCuttingDectionTree = SystemSave.isCutForDecisionTree;

              ValueStairHeight = SystemSave.StairHeight.ToString();
              ValueHeadingCanculateMode = SystemSave.CanculateHeadingMode;
              ValueSystemStartAngleForCHM1 = SystemSave.startAngleForCHM1.ToString();

              ValueStartPositionX = SystemSave.startPositionX.ToString();
              ValueStartPositionY = SystemSave.startPositionY.ToString();
              ValueStartPositionZ = SystemSave.startPositionZ.ToString();
              
              ValuerRouteLineScale = SystemSave.routeLineScale;
              ValueSystemMode = SystemSave.SystemModeInd;
              ValueUseFilters = SystemSave.FilterMode;
              ValueImmeDiateSL = SystemSave.stepLengthForImmediate.ToString();
              ValueLegInStature = SystemSave.legLengthInHeight.ToString();
              
              ValueHeadingGate = SystemSave.changeGateForImmediate2.ToString();
              ValueRatioInSL2 = SystemSave.percentWhenMeetChangeGate.ToString();
              ValueMSHeadingGate = SystemSave.MSHeadingGate.ToString();
              ValueSystemFlashTimer = SystemSave.systemFlashTimer.ToString();

              ValueAccordANNHiddenLayerCount = SystemSave.accordANNHiddenLayerCount.ToString();
              ValueAccordANNTrainTime = SystemSave.accordANNTrainTime.ToString();
              
              ValueStature2A = SystemSave.StaturaMethod2_A.ToString();
              ValueStature2B = SystemSave.StaturaMethod2_B.ToString();

                hasBasicValue = true;//最初数值只会被记录一次
           }
        }

        //获取最初的数值
        void setStartValue()
        {
            ServerIPText.Text = ValueServerIPText ;
            ServerPortText.Text = ValueServerPortText ;
            packageOffset.Text = ValuepackageOffset ;
            ZeroLine.Text = ValueZeroLine ;
            Stature.Text = ValueStature;
            FemailWeight.Text = ValueFemailWeight;
            MailWeight.Text = ValueMailWeight;
            Aminus.Text = ValueAminus;
            Scarlet.Text = ValueScarlet;
            Kim.Text = ValuesKim;
            PeackStepthreshold.Text = ValueStepPeackThreshold;

            MaxAForStart.Text = ValueMaxAForStart;
            MinAForStart.Text = ValueMinAForStart;
            ChangeValue.Text = ValueDertshold;
            UpForStart.Text = ValueUperGateForStart;
            DownForStart.Text = ValueDownGateForStart;
            TrainBasePath.Text = ValueDecisionTreePath;
            StairHeight.Text = ValueStairHeight;

            HCM1Start.Text = ValueSystemStartAngleForCHM1;
            if (ValueIsMale)
                isMaleCheckc.IsChecked = true;
            else
                isMaleCheckc.IsChecked = false;
            if (ValueDrawWithBuff)
                Draw_route_with_buffer.IsChecked = true;
            else
                Draw_route_with_buffer.IsChecked = false;
            if(ValueUseHeadOffset)
                UseOffSet.IsChecked = true;
            else
                UseOffSet.IsChecked = false;

            if (ValueIsDynamicallyZeroLine)
                DynamicallyZeroLine.IsChecked = true;
            else
                DynamicallyZeroLine.IsChecked = false;

            if (ValueIsCuttingDectionTree)
                isPruning.IsChecked = true;
            else
                isPruning.IsChecked = false;

            FilterModeSelects.SelectedIndex = ValueUseFilters;

            TreeMethod.SelectedIndex =  ValueTreeMethod;
            HeadingCanculateMode.SelectedIndex = ValueHeadingCanculateMode;

            StartPositionX.Text = ValueStartPositionX;
            StartPositionY.Text = ValueStartPositionY;
            StartPositionZ.Text = ValueStartPositionZ;
            routeLineScaleSlider.Value = ValuerRouteLineScale;

            SystemModeSelect.SelectedIndex = ValueSystemMode;
            immediateSL.Text = ValueImmeDiateSL;
            LegInStature.Text = ValueLegInStature;

            headingChangeGate.Text = ValueHeadingGate;
            StepLengthRatio.Text = ValueRatioInSL2;
            MSHeadingGate.Text =  ValueMSHeadingGate;
            systemFlashTimerText.Text = ValueMSHeadingGate;

            ANNHiddenNeuronsText.Text =  ValueAccordANNHiddenLayerCount;
            ANNTrainTimesText.Text =  ValueAccordANNTrainTime;


            saturate2AText.Text = ValueStature2A;
            saturate2BText.Text = ValueStature2B;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            setStartValue();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            //取消引用，可以开启下一个设定窗口
            SystemSave.theSettingWindow = null;
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            theDecisionTreeNode.nodeCountAll = 0;
            theDecisionTreeNode.maxDepth = 0;
            SystemSave.StepLengthTree = new Codes.DecisionTree.theDecisionTree();
            //SystemSave.StepLengthTree.BuildTheTree("TrainBase/TrainBaseTree.txt",0);
            SystemSave.StepLengthTree.BuildTheTree(SystemSave.TrainBasedFile, 0);
            string informationS = "根据文件"+ TrainBasePath.Text +"内容已经建立决策树\n";
            if (isPruning.IsChecked == true)
                informationS += "(使用了一些剪枝策略)\n";
            else
                informationS += "(未使用剪枝策略)\n";
            informationS += "决策树的节点个数： " + SystemSave.StepLengthTree.getNodeCount()+"\n";
            informationS += "决策树的深度： " + SystemSave.StepLengthTree.getDepth();
            MessageBox.Show(informationS);
            DrawTreeButton1.IsEnabled = true;
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            theDecisionTreeNode.nodeCountAll = 0;
            theDecisionTreeNode.maxDepth = 0;
            SystemSave.StairTree = new Codes.DecisionTree.theDecisionTree();
            //SystemSave.StairTree.BuildTheTree("TrainBase/TrainBaseTree.txt",1);
            SystemSave.StairTree.BuildTheTree(SystemSave.TrainBasedFile, 1);
            string informationS = "根据文件" + TrainBasePath.Text + "内容已经建立决策树\n";
            if (isPruning.IsChecked == true)
                informationS += "(使用了一些剪枝策略)\n";
            else
                informationS += "(未使用剪枝策略)\n";
            informationS += "决策树的节点个数： " + SystemSave.StairTree.getNodeCount() + "\n";
            informationS += "决策树的深度： " + SystemSave.StairTree.getDepth();
            MessageBox.Show(informationS);
            DrawTreeButton2.IsEnabled = true;
        }

        private void HeadingCanculateMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // TipsMake(true);
            if (HeadingCanculateMode.SelectedIndex == 1)
            {
                string informationS = "----------说明----------\n";
                informationS += "有关3D Free判断当前移动方向的方法说明：\n";
                informationS += "这种方法需要事先有一定的基础数据来计算偏移量\n";
                informationS += "因此最开始的" + SystemSave.sampleTime + "步会因为采样而失效\n";
                informationS += "此外,这种方法只会对前三种方向判定算法生效";
                MessageBox.Show(informationS);
            }
        }

        //因为初始化的时候也应该做这一步，但是因为控件的初始化的并不完全相同，所以需要使用loaded方法统一处理
        //为此需要封装成一个方法
        private void TipsMake(bool sample = false)
        {
            try
            {
                //if (HeadingCanculateMode.SelectedIndex == 1)
                //{
                //    string  informationS = "----------------------------------------说明-----------------------------------------\n";
                //    informationS += "有关3D Free判断当前移动方向的方法说明：\n";
                //    informationS += "这种方法需要事先有一定的基础数据来计算偏移量\n";
                //    informationS += "因此最开始的" + SystemSave.sampleTime + "步会因为采样而失效";
                //    informationS += "此外这种方法只会对前三种方向判定算法生效";
                //    TipsForHeading.Content = informationS;
                //}
                //else
                //{
                //    TipsForHeading.Content = "----------------------------------------说明------------------------------------------\n当前没有必须说明的项目";
                //}

                if (!sample)
                {
                    CommonFormulaFamilyLabel.Content =
                        "一般计算步长的计算公式为\n" +
                        "SL= α * VK + β * FK + γ \n" +
                        "右侧容器每一项为一组公式参数\n" +
                        "以此建立一般公式的公式族\n\n" +
                        "可直接使用族中首项公式计算步长\n" +
                        "决策树和神经网络方法目标分类数\n" +
                        "等于公式族中所有公式的数量\n" +
                        "根据分类结果用不同公式计算步长\n\n" +
                        "双击容器项调整α，β，γ 参数数值\n" +
                        "用下拉框有限制地选择公式族大小";
                }
            }
            catch
            {
                Console.WriteLine("初始化尚未完成");
            }


        }
        private void button3_Click(object sender, RoutedEventArgs e)
        {
            TreeViewWindow theWindow = new TreeViewWindow();
            theWindow.Show();
            theWindow.Title= "Step Length Tree";
            theWindow.drawDecisionTree(0);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (SystemSave.StepLengthTree == null)
                DrawTreeButton1.IsEnabled = false;
            if (SystemSave.StairTree == null)
                DrawTreeButton2.IsEnabled = false;
        }

        private void button3_Click_1(object sender, RoutedEventArgs e)
        {
            TreeViewWindow theWindow = new TreeViewWindow();
            theWindow.Show();
            theWindow.Title = "Z Move Tree";
            theWindow.drawDecisionTree(1);
        }

        private void button3_Click_2(object sender, RoutedEventArgs e)
        {
            if (theMainWindow != null)
            {
                theDrawColorButton.Foreground = new SolidColorBrush(theMainWindow.SetColor());
            }
            else
            {
                MessageBox.Show("当前这一项不可编辑。");
            }
        }

        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void button3_Click_3(object sender, RoutedEventArgs e)
        {
            ServerIPText.Text = SystemSave.getIPAddress();
        }

        private void button4_Click(object sender, RoutedEventArgs e)
        {
            //单纯的保存
            setValues();
            SaveCommonFormulaWeightsFamily();
            MessageBox.Show("数据保存成功");
        }

        private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void comboBox_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            CommonFamily.Items.Clear();
            int countforWeight = CommonFamilyCountChoice.SelectedIndex + 1;

            if (countforWeight >= SystemSave.CommonFormulaWeights.Count)
            {
                for (int i = 0; i < SystemSave.CommonFormulaWeights.Count; i++)
                {
                    ListBoxItem theItem = new ListBoxItem();
                    theItem.Content = string.Format("α = {0} , β = {1} , γ = {2}",
                    SystemSave.CommonFormulaWeights[i][0].ToString("f2"),
                    SystemSave.CommonFormulaWeights[i][1].ToString("f2"),
                    SystemSave.CommonFormulaWeights[i][2].ToString("f2"));

                    CommonFamily.Items.Add(theItem);
                }
                for (int i = 0; i < countforWeight - SystemSave.CommonFormulaWeights.Count; i++)
                {
                    ListBoxItem theItem = new ListBoxItem();
                    theItem.Content = "α = 1.00 , β = 2.00 , γ = 3.00";
                    CommonFamily.Items.Add(theItem);
                }
            }
            else
            {
                for (int i = 0; i < countforWeight; i++)
                {
                    ListBoxItem theItem = new ListBoxItem();
                    theItem.Content = string.Format("α = {0} , β = {1} , γ = {2}",
                    SystemSave.CommonFormulaWeights[i][0].ToString("f2"),
                    SystemSave.CommonFormulaWeights[i][1].ToString("f2"),
                    SystemSave.CommonFormulaWeights[i][2].ToString("f2"));

                    CommonFamily.Items.Add(theItem);
                }
            }


        }

        private void CommonFamily_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && e.RightButton == MouseButtonState.Released)
            {
                commonWeightSetting settingsForWeight =  new commonWeightSetting();

                settingsForWeight.Show();
                settingsForWeight.makeStart(CommonFamily.SelectedItem as ListBoxItem);
            }
        }

        //加载原有的一般公式参数族
        void makeCommonFormulaWeightsFamily()
        {
            CommonFamily.Items.Clear();
            for (int i = 0; i < SystemSave.CommonFormulaWeights.Count; i++)
            {
                ListBoxItem theItem = new ListBoxItem();
                //这个逗号分隔非常有用
                //因为保存数据的时候使用的是字符串处理的东西
                theItem.Content = string.Format("α = {0} , β = {1} , γ = {2}" ,
                SystemSave.CommonFormulaWeights[i][0].ToString("f2"), 
                SystemSave.CommonFormulaWeights[i][1].ToString("f2"), 
                SystemSave.CommonFormulaWeights[i][2].ToString("f2"));

                CommonFamily.Items.Add(theItem);
            }
        }

        //字符串处理保存整整个一般公式族的参数
        private void SaveCommonFormulaWeightsFamily()
        {
            SystemSave.CommonFormulaWeights = new List<double[]>();
            for (int i = 0; i < CommonFamily.Items.Count; i++)
            {
                string information = (string)((ListBoxItem)CommonFamily.Items[i]).Content;
                information = information.Replace(" ", "");
                string[] values = information.Split(',');
                double A = Convert.ToDouble(values[0].Split('=')[1]);
                double B = Convert.ToDouble(values[1].Split('=')[1]);
                double C = Convert.ToDouble(values[2].Split('=')[1]);
                double[] newWeights = new double[] { A, B, C };
                //Console.WriteLine("information = " + information);
                SystemSave.CommonFormulaWeights.Add(newWeights);
            }
        }

        private void comboBox_SelectionChanged_2(object sender, SelectionChangedEventArgs e)
        {

        }

        private void routeLineScaleSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            routeLineScaleSliderValueText.Content = routeLineScaleSlider.Value.ToString("f2");
        }
    }
}
