
using Accord.Neuro;
using Accord.Neuro.Learning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace socketServer.Codes.AcordUse
{
    //专门操作Accord.net机器学习库的类
    //这个类主管ANN
    class AccordANN 
    {
        private bool isBuilt = false;
        ActivationNetwork network;
        int[] outputsFromFile;//labels标签

        //上下楼梯计算用的ANN-----------------------------------------------------------------------------------
        public void BuildANNForStair()
        {
            if (isBuilt == false)
            {
                string information = FileSaver.readFromTrainBase();
                string[] informationSplit = information.Split('\n');
                int trueLength = 0;//数据有时候并没有如此理想，因此还是分两次处理
                                   //否则有可能会有空项，继而产生空引用的错误
                                   //也是一个贪心的思想在啊
                for (int i = 0; i < informationSplit.Length; i++)
                {
                    string[] informaitonUse = informationSplit[i].Split(',');
                    if (informaitonUse.Length < 17)
                        break;

                    trueLength++;
                }
                double[][] inputsFromFile = new double[trueLength][];
                outputsFromFile = new int[trueLength];
                for (int i = 0; i < trueLength; i++)
                {
                    string[] informaitonUse = informationSplit[i].Split(',');
                    if (informaitonUse.Length < 17)
                        break;

                    inputsFromFile[i] = new double[] 
                    {
                        Convert.ToDouble(informaitonUse[0]), Convert.ToDouble(informaitonUse[2]), Convert.ToDouble(informaitonUse[2]),
                         Convert.ToDouble(informaitonUse[3]), Convert.ToDouble(informaitonUse[4]), Convert.ToDouble(informaitonUse[5])
                    };
                    outputsFromFile[i] = SystemSave.getTypeIndexForStair(Convert.ToDouble(informaitonUse[16]));
                }

                int numberOfInputs = 6;
                int numberOfClasses = 3;//对于楼梯，只有三种情况，上楼，下剅以及平地走
                int hiddenNeurons = SystemSave.accordANNHiddenLayerCount;

                double[][] outputs = Accord.Statistics.Tools.Expand(outputsFromFile, numberOfClasses, -1, 1);
                // Next we can proceed to create our network
                var function = new BipolarSigmoidFunction(2);
                network = new ActivationNetwork(function,
                  numberOfInputs, hiddenNeurons, numberOfClasses);

                // Heuristically randomize the network
                new NguyenWidrow(network).Randomize();

                // Create the learning algorithm
                var teacher = new LevenbergMarquardtLearning(network);

                // Teach the network for 10 iterations:
                double error = Double.PositiveInfinity;
                for (int i = 0; i < SystemSave.accordANNTrainTime; i++)
                    error = teacher.RunEpoch(inputsFromFile, outputs);
                isBuilt = true;
            }
        }

        public int getModeWithANNForStair(double AX, double AY, double AZ, double GX, double GY, double GZ)
        {
            double[] input = new double[] { AX, AY,AZ,GX,GY,GZ };// 0
            int answer;
            double[] output = network.Compute(input);
            answer = getMaxIndex(output);
            //Console.WriteLine(answer + " is the mode");
            return answer;
        }




        //步长计算用的ANN-----------------------------------------------------------------------------------
        public void BuildANNForSL( )
        {
            if (isBuilt == false)
            {
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
                outputsFromFile = new int[trueLength];
                for (int i = 0; i < trueLength; i++)
                {
                    string[] informaitonUse = informationSplit[i].Split(',');
                    if (informaitonUse.Length < 14)
                        break;

                    inputsFromFile[i] = new double[] { Convert.ToDouble(informaitonUse[12]), Convert.ToDouble(informaitonUse[13]) };
                    outputsFromFile[i] = SystemSave.getTypeIndex(Convert.ToDouble(informaitonUse[14]));
                }

                int numberOfInputs = 2;
                int numberOfClasses = SystemSave.CommonFormulaWeights.Count;//这个同样也受systemSave公式族的制约
                int hiddenNeurons = SystemSave.accordANNHiddenLayerCount;

                double[][] outputs = Accord.Statistics.Tools.Expand(outputsFromFile, numberOfClasses, -1, 1);
                // Next we can proceed to create our network
                var function = new BipolarSigmoidFunction(2);
                network = new ActivationNetwork(function,
                  numberOfInputs, hiddenNeurons, numberOfClasses);

                // Heuristically randomize the network
                new NguyenWidrow(network).Randomize();

                // Create the learning algorithm
                var teacher = new LevenbergMarquardtLearning(network);

                // Teach the network for 10 iterations:
                double error = Double.PositiveInfinity;
                for (int i = 0; i < SystemSave.accordANNTrainTime; i++)
                    error = teacher.RunEpoch(inputsFromFile, outputs);
                isBuilt = true;
            }
        }

        public int getModeWithANNForSL(double VK ,double FK )
        {
            double[] input = new double[] { VK, FK };// 0
            int answer;
            double[] output = network.Compute(input);
            answer = getMaxIndex(output);
            //Console.WriteLine(answer + " is the mode");
            return answer;
        }

        int getMaxIndex(double [] output)
        {
            int indexUse = -1;
            double maxValue = -9999999;
            for (int i = 0; i < output.Length; i++)
            {
                if (output[i] > maxValue)
                {
                    maxValue = output[i];
                    indexUse = i;
                }
            }
            return indexUse;
        }

        //这是一个官方的示例-----------------------------
        public void checkClass()
        {
            // Here we will be creating a neural network to process 3-valued input
            // vectors and classify them into 4-possible classes. We will be using
            // a single hidden layer with 5 hidden neurons to accomplish this task.
            // 
            int numberOfInputs = 3;
            int numberOfClasses = 4;
            int hiddenNeurons = 5;

            // Those are the input vectors and their expected class labels
            // that we expect our network to learn.
            // 
            double[][] input =
            {
                new double[] { -1, -1, -1 }, // 0
                new double[] { -1,  1, -1 }, // 1
                new double[] {  1, -1, -1 }, // 1
                new double[] {  1,  1, -1 }, // 0
                new double[] { -1, -1,  1 }, // 2
                new double[] { -1,  1,  1 }, // 3
                new double[] {  1, -1,  1 }, // 3
                new double[] {  1,  1,  1 }  // 2
             };

                int[] labels =
                {
                    0,
                    1,
                    1,
                    0,
                    2,
                    3,
                    3,
                    2,
                };

            // In order to perform multi-class classification, we have to select a 
            // decision strategy in order to be able to interpret neural network 
            // outputs as labels. For this, we will be expanding our 4 possible class
            // labels into 4-dimensional output vectors where one single dimension 
            // corresponding to a label will contain the value +1 and -1 otherwise.

            double[][] outputs = Accord.Statistics.Tools.Expand(labels, numberOfClasses, -1, 1);

            // Next we can proceed to create our network
            var function = new BipolarSigmoidFunction(2);
            var network = new ActivationNetwork(function,
              numberOfInputs, hiddenNeurons, numberOfClasses);

            // Heuristically randomize the network
            new NguyenWidrow(network).Randomize();

            // Create the learning algorithm
            var teacher = new LevenbergMarquardtLearning(network);

            // Teach the network for 10 iterations:
            double error = Double.PositiveInfinity;
            for (int i = 0; i < 10; i++)
                error = teacher.RunEpoch(input, outputs);

            // At this point, the network should be able to 
            // perfectly classify the training input points.

           input =new double[][]
             {
                new double[] { -1, -1, -1 }// 0
             }; 

            for (int i = 0; i < input.Length; i++)
            {
                int answer;
                double[] output = network.Compute(input[i]);
                //double response = output.Max(out answer);
                answer = getMaxIndex(output);
                int expected = labels[i];
                Console.WriteLine(expected +" is expected");
                Console.WriteLine(answer + " is answer");
                // at this point, the variables 'answer' and
                // 'expected' should contain the same value.
            }
        }
       
    }
}
