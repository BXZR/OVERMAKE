from sklearn import svm
import numpy as np
from matplotlib import pyplot as plt 

file = open("TrainBaseFake.txt")
X = []
Y = []
SL = []
for eachline in file:
    read = eachline .split("," )
    if len(read) <3 :
        break

    X.append (float(str(read[0])))
    Y.append (float(str(read[1])))
    SL.append(float(str(read[2].split("\n")[0])))


X = np.arange(-5.,9.,0.1)
X=np.random.permutation(X)
X_=[[i] for i in X]
#print X
b=5.
y=0.5 * X ** 2.0 +3. * X + b + np.random.random(X.shape)* 10.
y_=[i for i in y]

rbf1=svm.SVR(kernel='rbf',C=1, )#degree=2,,gamma=, coef0=

rbf1.fit(X_,y_)

result1 = rbf1.predict(X_)

plt.hold(True)
plt.plot(X,y,'bo',fillstyle='none')
plt.plot(X,result1,'r.')

plt.show()
