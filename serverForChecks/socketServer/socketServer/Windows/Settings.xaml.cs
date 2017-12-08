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



        public void startSet(MainWindow theIN)
        {
            theMainWindow = theIN;
            if (theMainWindow.isServerStarted() == false)
            {
                saveRestartButton.IsEnabled = false;//没有开启服务器就没必要重启
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
                setValues();
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
                string information = theMainWindow.makeClose();
                information += "\n-----------------------\n" + theMainWindow.makeStart();
                this.Close();
                MessageBox.Show(information);

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

            if (CanculateZMove.IsChecked == true)
                SystemSave.isStairsUp = true;
            else
                SystemSave.isStairsUp = false;

            SystemSave.DecisionTreeMethodID = TreeMethod.SelectedIndex;
            SystemSave.CanculateHeadingMode = HeadingCanculateMode.SelectedIndex;

            SystemSave.startPositionX = Convert.ToDouble(StartPositionX.Text);
            SystemSave.startPositionY = Convert.ToDouble(StartPositionY.Text);
            SystemSave.startPositionZ = Convert.ToDouble(StartPositionZ.Text);
            SystemSave.routeLineScale = Convert.ToDouble(routeLineScale.Text);
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

            if (SystemSave.isStairsUp == true)
                CanculateZMove.IsChecked = true;
            else
                CanculateZMove.IsChecked =  false;

            StartPositionX.Text = SystemSave.startPositionX.ToString();
            StartPositionY.Text = SystemSave.startPositionY.ToString();
            StartPositionZ.Text = SystemSave.startPositionZ.ToString();
            routeLineScale.Text = SystemSave.routeLineScale.ToString();

            //记录一次最初的数值
            getStartValue();
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

        private static bool ValueIsAllowZMove;
        private static bool ValueIsCuttingDectionTree;
        private static int  ValueHeadingCanculateMode;
        private static string  ValueSystemStartAngleForCHM1;

        private static string ValueStartPositionX;
        private static string ValueStartPositionY;
        private static string ValueStartPositionZ;

        private static string ValuerRouteLineScale;

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

              ValueIsAllowZMove = SystemSave.isStairsUp;
              ValueIsCuttingDectionTree = SystemSave.isCutForDecisionTree;

              ValueStairHeight = SystemSave.StairHeight.ToString();
              ValueHeadingCanculateMode = SystemSave.CanculateHeadingMode;
              ValueSystemStartAngleForCHM1 = SystemSave.startAngleForCHM1.ToString();

              ValueStartPositionX = SystemSave.startPositionX.ToString();
              ValueStartPositionY = SystemSave.startPositionY.ToString();
              ValueStartPositionZ = SystemSave.startPositionZ.ToString();
              
              ValuerRouteLineScale = SystemSave.routeLineScale.ToString();

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

            if (ValueIsAllowZMove)
                CanculateZMove.IsChecked = true;
            else
                CanculateZMove.IsChecked = false;

            TreeMethod.SelectedIndex =  ValueTreeMethod;
            HeadingCanculateMode.SelectedIndex = ValueHeadingCanculateMode;

            StartPositionX.Text = ValueStartPositionX;
            StartPositionY.Text = ValueStartPositionY;
            StartPositionZ.Text = ValueStartPositionZ;
            routeLineScale.Text = ValuerRouteLineScale;
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

        private void CanculateZMove_Checked(object sender, RoutedEventArgs e)
        {
            if (CanculateZMove.IsChecked == true && SystemSave.StairTree == null)
            {
                MessageBox.Show("当前检测上下楼梯的方向需要使用决策树\n当前这棵决策树还没有被建立");
            } 
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
            if (HeadingCanculateMode.SelectedIndex == 1)
            {
                string informationS = "有关3D判断当前移动方向的方法说明：\n";
                informationS += "这种方法需要事先有一定的基础数据来计算偏移量\n";
                informationS += "因此最开始的"+SystemSave.sampleTime+"步会因为采样而失效";
                informationS += "此外这种方法只会对前三种方向判定算法生效";
                TipsForHeading.Content += informationS;
            }
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            TreeViewWindow theWindow = new TreeViewWindow();
            theWindow.Show();
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
            theWindow.drawDecisionTree(0);
        }
    }
}
