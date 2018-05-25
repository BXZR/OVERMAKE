import tensorflow as tf
from tensorflow.examples.tutorials.mnist import input_data
import  numpy as np

# set random seed for comparing the two result calculations
tf.set_random_seed(1)

# this is data
#mnist = input_data.read_data_sets('MNIST_data', one_hot=True)

# hyperparameters
lr = 0.001
training_iters = 1000
batch_size = 64

n_inputs = 13   # MNIST data input (img shape: 28*28)
n_steps = 13    # time steps
n_hidden_units = 128   # neurons in hidden layer
n_classes = 4   # MNIST classes (0-9 digits)

# tf Graph input
x = tf.placeholder(tf.float32, [None, n_steps, n_inputs])
y = tf.placeholder(tf.float32, [None, n_classes])


# Define weights
weights = {
    # (15, 128)
    'in': tf.Variable(tf.random_normal([n_inputs, n_hidden_units])),
    # (128, 10)
    'out': tf.Variable(tf.random_normal([n_hidden_units, n_classes]))
}
biases = {
    # (128, )
    'in': tf.Variable(tf.constant(0.1, shape=[n_hidden_units])),
    # (10, )
    'out': tf.Variable(tf.constant(0.1, shape=[n_classes ]))
}


def RNN(X, weights, biases):
    # hidden layer for input to cell
    ########################################

    # transpose the inputs shape from
    # X ==> (128 batch * 28 steps, 28 inputs)
    X = tf.reshape(X, [-1, n_inputs])

    # into hidden
    # X_in = (128 batch * 28 steps, 128 hidden)
    X_in = tf.matmul(X, weights['in']) + biases['in']
    # X_in ==> (128 batch, 28 steps, 128 hidden)
    X_in = tf.reshape(X_in, [-1, n_steps, n_hidden_units])

    # cell
    ##########################################

    # basic LSTM Cell.
    # if int((tf.__version__).split('.')[1]) < 12 and int((tf.__version__).split('.')[0]) < 1:
    #     lstm_cell = tf.nn.rnn_cell.BasicLSTMCell(n_hidden_units, forget_bias=1.0, state_is_tuple=True)
    # else:
    #     print("22222")
    #     lstm_cell = tf.contrib.rnn.BasicLSTMCell(n_hidden_units)

    lstm_cell = tf.nn.rnn_cell.BasicLSTMCell(n_hidden_units, forget_bias=1.0, state_is_tuple=True)

    # lstm cell is divided into two parts (c_state, h_state)
    init_state = lstm_cell.zero_state(batch_size, dtype=tf.float32)

    outputs, final_state = tf.nn.dynamic_rnn(lstm_cell, X_in, initial_state=init_state, time_major=False)

    #unpack to list [(batch, outputs)..] * steps
    if int((tf.__version__).split('.')[1]) < 12 and int((tf.__version__).split('.')[0]) < 1:
        outputs = tf.unpack(tf.transpose(outputs, [1, 0, 2]))    # states is the last outputs
    else:

        outputs = tf.unstack(tf.transpose(outputs, [1,0,2]))


    results = tf.matmul(outputs[-1], weights['out']) + biases['out']    # shape = (128, 10)

    return results


pred = RNN(x, weights, biases)
cost = tf.reduce_mean(tf.nn.softmax_cross_entropy_with_logits(logits=pred, labels=y))
train_op = tf.train.AdamOptimizer(lr).minimize(cost)
#预测出来的步态类型
getType = tf.argmax(pred, 1)
correct_pred = tf.equal(tf.argmax(pred, 1), tf.argmax(y, 1))
accuracy = tf.reduce_mean(tf.cast(correct_pred, tf.float32))

with tf.Session() as sess:
    # tf.initialize_all_variables() no long valid from
    # 2017-03-02 if using tensorflow >= 0.12
    if int((tf.__version__).split('.')[1]) < 12 and int((tf.__version__).split('.')[0]) < 1:
        init = tf.initialize_all_variables()
    else:
        init = tf.global_variables_initializer()
    sess.run(init)

    print("数据读取=======================================================================")
    file = open("TrainBase.txt")
    linesForAll  = file.readlines()
    file.close()
    print("总行数：" , len(linesForAll))

    XforAll = []
    YforAll = []
    
    for eachline in linesForAll:
        line = eachline.split("," )
        XValue = []
        if len(line) >= n_inputs:
            index= 0
            while index < n_inputs :
                XValue.append( float(str(line[index])) )
                index += 1
            XforAll.append(XValue)
            YforAll.append( float(str(line[index])))
        else:
            print("not a line")

    file.close()
    
    print("训练LSTM=======================================================================")
    step = 0
    while (step+1) * batch_size < len(YforAll):
        #batch_xs, batch_ys = mnist.train.next_batch(batch_size)
        #batch_xs = batch_xs.reshape([batch_size, n_steps, n_inputs])
        
        batch_xs = [[0 for i in range(n_inputs)]  for i in range(n_steps)]
        batch_ys = [0 for i in range(n_steps)]
        lines = 0

        while lines < n_steps:
            index= 0
            while index < n_inputs :
                batch_xs[lines][index] = XforAll[step * batch_size + lines][index]
                index += 1
            
            batch_ys[lines] = YforAll[step * batch_size + lines]
            #print(batch_xs[lines])
            #print(batch_ys[lines])
            lines += 1
            
  
        batch_xs = np.resize(batch_xs,(batch_size,n_inputs ,n_steps ))
        batch_ys = np.resize(batch_xs,(batch_size,n_classes ))
        
        feed = { x: batch_xs,y: batch_ys}
        sess.run([train_op], feed_dict= feed)
        if step % 5 == 0:
            print(sess.run(accuracy , feed_dict = feed))
            #print(sess.run(getType, feed_dict = feed))
        step += 1

    print("测试LSTM=======================================================================")
    
    batch_xs = [[0 for i in range(n_inputs)]  for i in range(n_steps)]
    batch_ys = [0 for i in range(n_steps)]
    lines = 0
    while lines < n_steps:
        index= 0
        while index < n_inputs :
            batch_xs[lines][index] = XforAll[lines][index]
            index += 1
            
        batch_ys[lines] = YforAll[lines]
        lines += 1
        
    batch_xs = np.resize(batch_xs,(batch_size,n_inputs ,n_steps ))
    batch_ys = np.resize(batch_xs,(batch_size,n_classes ))

    feed = { x: batch_xs,y: batch_ys}
    print(sess.run(getType, feed_dict = feed)[0])
