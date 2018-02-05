using Accord.MachineLearning.VectorMachines.Learning;
using Accord.Math.Optimization.Losses;
using Accord.Statistics.Kernels;
using Accord.Statistics.Models.Regression.Linear;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace socketServer.Codes
{
    //专门操作Accord.net机器学习库的类
    //这个类主管线性回归
    class AccordNotNetUse
    {

        //回归得到的参数
        //通过trainBase来回归的到
        public double WeightA;
        public double WeightB;
        public double WeightC;
        private bool isMade = false;

        //配合一般公式的做法
        //用这个API做的回归
        //如果没有建立或者没有文件，就直接用瞎编的公式处理
        //这里这个瞎编的公式当然同样收到systemSave的统一处理
        public double linearStepLength(double VK , double  FK)
        {
            if (!isMade)
            {
                return SystemSave.CommonFormulaWeights [0][0]* VK
                       + SystemSave.CommonFormulaWeights[0][1] * FK + SystemSave.CommonFormulaWeights[0][2];
            }
            return WeightA * VK + WeightB * FK + WeightC;
        }

        public void BuildWeights()
        {
            // We will use Ordinary Least Squares to create a
            // linear regression model with an intercept term

            Console.WriteLine("building weights");
            var ols = new OrdinaryLeastSquares()
            {
                UseIntercept = true
            };
            string information = FileSaver.readFromTrainBase();
            string[] informationSplit = information.Split('\n');
            int trueLength = 0;//数据有时候并没有如此理想，因此还是分两次处理
            //否则有可能会有空项，继而产生空引用的错误
            //也是一个贪心的思想在啊
            for (int i = 0; i < informationSplit.Length; i++)
            {
                string[] informaitonUse = informationSplit[i].Split(',');
                    if (informaitonUse.Length < 14)
                    break;

                trueLength++;
            }
            double[][] inputsFromFile = new double[trueLength][];
            double[] outputsFromFile = new double[trueLength];
            for (int i = 0; i < trueLength; i++)
            {
                string[] informaitonUse = informationSplit[i].Split(',');
                if (informaitonUse.Length < 14)
                    break;

                inputsFromFile[i] = new double[] { Convert.ToDouble(informaitonUse[12]), Convert.ToDouble(informaitonUse[13]) };
                outputsFromFile[i] = Convert.ToDouble(informaitonUse[14]);
            }
            //Console.WriteLine("inputsFromFile ->"+ inputsFromFile.Length);
            //Console.WriteLine("outputsFromFile ->" + outputsFromFile.Length);
            //for (int i = 0; i < inputsFromFile.Length; i++)
            //{
            //    for (int j = 0; j < inputsFromFile[i].Length; j++)
            //        Console.Write(inputsFromFile[i][j] + "  ");
            //    Console.WriteLine("\n");
            //}    

            MultipleLinearRegression regression = ols.Learn(inputsFromFile, outputsFromFile);
            WeightA = regression.Coefficients[0]; // a = 0
            WeightB = regression.Coefficients[1]; // b = 0
            WeightC = regression.Intercept; // c = 1
            Console.WriteLine("WeightA = "+ WeightA + "  WeightB = "+ WeightB + "  WeightC = "+ WeightC);
   
        }


        public double[] getBufferWeightsWithType(int aimType = 0 , int allCount = 1)
        {
            Console.WriteLine("building weights for buffer for the type "+ aimType);
            var ols = new OrdinaryLeastSquares()
            {
                UseIntercept = true
            };
            string information = FileSaver.readFromTrainBase();
            string[] informationSplit = information.Split('\n');
            //否则有可能会有空项，继而产生空引用的错误
            //也是一个贪心的思想在啊
            List<int> indexUse = new List<int>();//记录可以使用的index
            for (int i = 0; i < informationSplit.Length; i++)
            {
                string[] informaitonUse = informationSplit[i].Split(',');
                if (informaitonUse.Length < 14)
                    break;

                int thistype = SystemSave.getTypeIndex(Convert.ToDouble(informaitonUse[14]), allCount);
                //Console.WriteLine("this type = "+ thistype);
                if (thistype == aimType)
                    indexUse.Add(i);
            }

            Console.WriteLine("fixed count = "+ indexUse.Count);
            double[][] inputsFromFile = new double[indexUse.Count][];
            double[] outputsFromFile = new double[indexUse.Count];
            for (int i = 0; i < indexUse.Count; i++)
            {
                string[] informaitonUse = informationSplit[i].Split(',');
                // if (informaitonUse.Length < 14)
                //     break;//这句话实际上不会被调用的

                //Console.WriteLine("informaitonUse[12] = " + informaitonUse[12]);
                //Console.WriteLine("informaitonUse[13] = " + informaitonUse[13]);
                inputsFromFile[i] = new double[] { Convert.ToDouble(informaitonUse[12]), Convert.ToDouble(informaitonUse[13]) };
                outputsFromFile[i] = Convert.ToDouble(informaitonUse[14]);
            }
            if (indexUse.Count > 0)
            {
                MultipleLinearRegression regression = ols.Learn(inputsFromFile, outputsFromFile);
                WeightA = regression.Coefficients[0];
                WeightB = regression.Coefficients[1];
                WeightC = regression.Intercept;
            }
            else
            {
                WeightA = 0;
                WeightB = 0;
                WeightC = 0;
            }
            //Console.WriteLine("WeightA = " + WeightA + "  WeightB = " + WeightB + "  WeightC = " + WeightC);
            return new double[] { WeightA  , WeightB , WeightC };
        }


//官方示例svm =========================================================================================================
        public void deom()
        {
            double[][] inputs =
          {
                new[] { 3.0, 1.0 },
                new[] { 7.0, 1.0 },
                new[] { 3.0, 1.0 },
                new[] { 3.0, 2.0 },
                new[] { 6.0, 1.0 },
            };

            // The task is to output a weighted sum of those numbers 
            // plus an independent constant term: 7.4x + 1.1y + 42
            double[] outputs =
            {
                7.4*3.0 + 1.1*1.0 + 42.0,
                7.4*7.0 + 1.1*1.0 + 42.0,
                7.4*3.0 + 1.1*1.0 + 42.0,
                7.4*3.0 + 1.1*2.0 + 42.0,
                7.4*6.0 + 1.1*1.0 + 42.0,
            };
            // Create a LibSVM-based support vector regression algorithm
            var teacher = new FanChenLinSupportVectorRegression<Gaussian>()
            {
                Tolerance = 1e-5,
                // UseKernelEstimation = true, 
                // UseComplexityHeuristic = true
                Complexity = 10000,
                Kernel = new Gaussian(0.1)
            };

            // Use the algorithm to learn the machine
            var svm = teacher.Learn(inputs, outputs);

            // Get machine's predictions for inputs
            double[] prediction = svm.Score(inputs);

            // Compute the error in the prediction (should be 0.0)
            double error = new SquareLoss(outputs).Loss(prediction);
            // MessageBox.Show("error " + error);
            string show = "";
            for (int i = 0; i < prediction.Length; i++)
                show += prediction[i] + "\n";
            MessageBox.Show(show);

        }

//官方示例多元性性回归 --------------------------------------------------------------------------------------------------------------------
        public void Linear()
        {
            // We will use Ordinary Least Squares to create a
            // linear regression model with an intercept term
            var ols = new OrdinaryLeastSquares()
            {
                UseIntercept = true
            };
            // Now suppose you have some points
            double[][] inputs =
            {
                new double[] { 1, 2 },
                new double[] { 3, 4 },
                new double[] { 5, 6 },
                new double[] { 7, 8 },
            };
            // located in the same Z (z = 1)
            double[] outputs = { 3, 7, 11, 15 };
            // Use Ordinary Least Squares to estimate a regression model
            MultipleLinearRegression regression = ols.Learn(inputs, outputs);
            // As result, we will be given the following:
            double a = regression.Coefficients[0]; // a = 0
            double b = regression.Coefficients[1]; // b = 0
            double c = regression.Intercept; // c = 1

            // This is the plane described by the equation
            // ax + by + c = z => 0x + 0y + 1 = z => 1 = z.

            // We can compute the predicted points using
            double[] predicted = regression.Transform(inputs);

            // And the squared error loss using 
            double error = new SquareLoss(outputs).Loss(predicted);
            MessageBox.Show("a = "+a +"  b = "+b +"  c = "+c);
        }

  
    }
}
