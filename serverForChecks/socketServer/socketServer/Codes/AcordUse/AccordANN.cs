
using Accord.Neuro;
using Accord.Neuro.Learning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace socketServer.Codes.AcordUse
{
    class AccordANN 
    {
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

                int expected = labels[i];
                Console.WriteLine(expected +" is expected");
                // at this point, the variables 'answer' and
                // 'expected' should contain the same value.
            }
        }
       
    }
}
