import tensorflow  as tf
import  numpy as np


print("========================训练的过程================================")
def  add_layer(inputs , in_size,out_size,activation_function=None):
    A=tf.Variable(tf.zeros([in_size,out_size]) ,name = "AForX")
    B=tf.Variable(tf.zeros([1,out_size]) ,name = "AForB")
    SLC=tf.matmul(inputs,A)+ B
    if activation_function==None:
        outputs=SLC
    else:
        outputs=activation_function(SLC)
    return outputs

 

xs = tf.placeholder(tf.float32,[None,2])
sl=tf.placeholder(tf.float32,[None,1])

#定义隐含层,隐含层有10个神经元
l1=add_layer(xs,2,10,activation_function=tf.nn.relu)
#定义输出层,假设没有任何激活函数
prediction=add_layer(l1,10,1,activation_function=None)
loss=tf.reduce_mean(tf.reduce_sum(tf.square(sl-prediction),reduction_indices=[1]))
train_step=tf.train.GradientDescentOptimizer(0.1).minimize(loss)
init=tf.global_variables_initializer()
sess = tf.Session()
sess.run(init)

index = 0
file = open("TrainBase.txt")
for eachline in file:
    read = eachline .split("," )
    if len(read) <3 :
        break
    X = [[],[]]
    SLData = []
    X[0].append (float(str(read[12])))
    X[1].append (float(str(read[13])))
    SLData.append(float(str(read[14].split("\n")[0])))
    
    X = np.resize(X, (len(X[0]),2))
    SLData = np.resize(SLData , (len(SLData),1))
    
    feed = { xs: X , sl : SLData }
    sess.run(train_step , feed_dict = feed)
    
    if index% 4000 ==0:
        print("------------------------------")
        print(X , "---" , SLData)
        print(sess.run(prediction, feed_dict = feed))
        print(sess.run(loss, feed_dict = feed))
        print("------------------------------")

        #for var in tf.trainable_variables():
        #    print (var) 
        #    print (sess.run(var, feed_dict = feed))  
    index += 1

print("========================保存到文件中================================")
saver = tf.train.Saver()
saver.save(sess, './modelForBP.ckpt')
print("Model is saved.")

print("========================读取最新信息使用训练好的内容进行推测================================")
saver.restore(sess, './modelForBP.ckpt')
file = open("TrainBase.txt")
read = file.readline().split("," )
X = [[],[]]
SLData = []
X[0].append (float(str(read[12])))
X[1].append (float(str(read[13])))
SLData.append(float(str(read[14].split("\n")[0])))
        
X = np.resize(X, (len(X[0]),2))
SLData = np.resize(SLData , (len(SLData),1))
        
feed = { xs: X , sl : SLData }
sess.run(train_step , feed_dict = feed)
        
print("------------------------------")
print(X , "---" , SLData)
print(sess.run(prediction, feed_dict = feed))
print(sess.run(loss, feed_dict = feed))
print("------------------------------")
        





