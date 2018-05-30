using socketServer.Codes;
using socketServer.Codes.DecisionTree;
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
    /// Interaction logic for TreeViewWindow.xaml
    /// </summary>
    public partial class TreeViewWindow : Window
    {

        //这个窗口用来展示生成的决策树
        //简单的决策树可视化的问题
        public TreeViewWindow()
        {
            InitializeComponent();
        }

        theDecisionTree theTree;//决策树的引用
        int maxDepth = 910;//记录一下最深层数
        //唯一的公有方法 : 绘图
        public void drawDecisionTree(TypeCheckClass AIMCheckClass)
        {
            theDrawCanvas.Children.Clear();

            //------------------------------------分支--------------------------------------------------//
            switch (AIMCheckClass)
            {
                //分类步长
                case TypeCheckClass.StepLength: { theTree = SystemSave.StepLengthTree; } break;
                //分类Z轴移动状态
                case TypeCheckClass.ZMove: { theTree = SystemSave.StairTree; } break;
                //分类这一步是不是真的存在
                case TypeCheckClass.StepType: { theTree = SystemSave.StepModeTree; } break;
                default:
                    {
                        MessageBox.Show("查无此树");
                        Log.saveLog(LogType.error, "决策树未生成，因此绘制失败");
                        return;
                    }
            }
            //------------------------------------------------------------------------------------------//

            Console.WriteLine("开始绘制");
            theDecisionTreeNode root = theTree.theRoot;
            maxDepth = theTree.getDepth();
            YLength = theDrawCanvas.Height * 0.95 / maxDepth;
            //Console.WriteLine(theDrawCanvas.Width);
            //Console.WriteLine(theDrawCanvas.Height );
            drawEclipse(theDrawCanvas.Width / 2, 0);
            drawTree(root, theDrawCanvas.Width*0.02, theDrawCanvas.Width*0.98, theDrawCanvas.Width / 2,0);
        }


        //宽度优先遍历这棵树顺带画图
        //int startPosition, int endPosition表示绘制区间的开始和结尾
        //绘制区间是相对于空间canvas的
        List<int> nodeCountForEachDepth = new List<int>();
        double YLength  =2;
        private void drawTree(theDecisionTreeNode father , double startPosition , double endPosition,double fatherX , double fatherY)
        {
            double lengtForEach = (endPosition - startPosition) / father.childs.Count;
            double stepNow = 0;//每一个儿子节点的区间
            for (int i = 0; i < father.childs.Count; i++)
            {
                //描点画线
                double XforthisChild = startPosition + lengtForEach / 2 + lengtForEach*i;
                //越是深层给的纵向空间也就越多
                double YfotthisChild = father.childs[i].depth * YLength * getLengtWithScale(father.childs[i].depth);
                //Console.WriteLine(string.Format("X1= {0} , Y1 = {1} , X2 = {2} , Y2 = {3}" , fatherX, fatherY, XforthisChild, YfotthisChild));

                drawLine(fatherX, fatherY, XforthisChild, YfotthisChild);
                drawEclipse(XforthisChild , YfotthisChild);
                drawLabel(XforthisChild, YfotthisChild, father.childs[i].name);
                
                drawTree(father.childs[i], startPosition + stepNow , startPosition + stepNow + lengtForEach, XforthisChild, YfotthisChild);

               //为下一个节点做准备
               stepNow += lengtForEach;
            }
        }

        //绘制的时候并不是等高度的
        //每一层的高度都有所不同
        private double getLengtWithScale(int depth)
        {
            double k = (0.3) / (maxDepth);
            double b = 0.7;
            return (depth * k + b);
        }

        //显示label
        private void drawLabel(double X, double Y,string name)
        {
            Label aLabel = new Label();
            aLabel.Content = name;
            aLabel.FontSize = 10;
            //设定旋转角
            double headingAngel = 45;
            aLabel.RenderTransform = new RotateTransform(headingAngel);

            Canvas.SetLeft(aLabel, X-5 );
            Canvas.SetTop(aLabel, Y-5 );
            theDrawCanvas.Children.Add(aLabel);
        }

        //画线使用
        private void drawLine(double X1, double Y1 , double X2 , double Y2)
        {
            Line drawLine = new Line();
            drawLine.X1 = X1;
            drawLine.Y1 = Y1;
            drawLine.X2 = X2;
            drawLine.Y2 = Y2;

            drawLine.Stroke = new SolidColorBrush(SystemSave.theNewColor);
            drawLine.StrokeThickness = 0.5;
            theDrawCanvas.Children.Add(drawLine);
        }

        //画点使用
        void drawEclipse(double X , double  Y )
        {
            var ellipse = new Ellipse()
            {
                Width = 4,
                Height = 4,
                Fill = new SolidColorBrush(Colors.Red)
            };
            Canvas.SetLeft(ellipse, X - ellipse.Width / 2);
            Canvas.SetTop(ellipse, Y - ellipse.Height / 2);
            theDrawCanvas.Children.Add(ellipse);
        }
    }
}
