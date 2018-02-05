import tensorflow as tf
import  numpy as np

sl = tf.placeholder(tf.float32)
x = tf.placeholder(tf.float32,[1,2])
A = tf.Variable(tf.zeros([2,1]))
C = tf.Variable(tf.zeros([1,1]))

SL =tf.add( tf.matmul(x , A),C)

file = open("TrainBase.txt")
lines = len(file.readlines()) 
optimizer = tf.train.GradientDescentOptimizer(0.01)

lost = tf.reduce_sum(tf.pow(SL-sl, 2))/2

train_step = optimizer .minimize (lost)

sess = tf.Session()
init = tf.global_variables_initializer()
sess.run(init)

steps = 1000


index = 0
file = open("TrainBase.txt")
for eachline in file:
    read = eachline .split("," )
    if len(read) <3 :
        break
    
    X = [[],[]]
    X[0].append (float(str(read[12])))
    X[1].append (float(str(read[13])))
    X = np.resize(X,( 1 ,2))
    
    SLData = float(str(read[14].split("\n")[0]))
    
    feed = {x : X , sl :SLData}
    sess.run(train_step , feed_dict = feed)
    index += 1
    
    if index % 4000 ==0 :
        print("A = " , sess.run(A))
        print("C = %f" % sess.run(C))
        print("lost = %f" % sess.run (lost , feed_dict = feed))
        print("--------------------------")
