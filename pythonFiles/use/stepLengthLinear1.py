import tensorflow as tf

x = tf.placeholder(tf.float32)
y = tf.placeholder(tf.float32)
A = tf.Variable(tf.zeros([1]))
B = tf.Variable(tf.zeros([1]))
C = tf.Variable(tf.zeros([1]))
sl = tf.placeholder(tf.float32)

SL = A * x + B * y + C

lost = tf.reduce_mean(tf.square (sl-SL))
optimizer = tf.train.GradientDescentOptimizer(0.01)
train_step = optimizer .minimize (lost)

sess = tf.Session()
init = tf.global_variables_initializer()
sess.run(init)

steps = 1000

index = 0
file = open("TrainBaseFake.txt")
for eachline in file:
    read = eachline .split("," )
    if len(read) <3 :
        break
    xs = read[0]
    ys = read[1]
    sls = read[2].split("\n")[0]
    feed = {x : float(str(xs)), y : float(str(ys)) , sl : float(str(sls))}
    sess.run(train_step , feed_dict = feed)
    index += 1
    if index % 50 ==1 :
        print("xs = ", xs)
        print("ys = ", ys)
        print("sls = ", sls)
        print("A = %f" % sess.run(A))
        print("B = %f" % sess.run(B))
        print("C = %f" % sess.run(C))
        print("lost = %f" % sess.run (lost , feed_dict = feed))
        print("--------------------------")
