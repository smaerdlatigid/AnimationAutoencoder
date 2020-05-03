import numpy as np

f = open("unity_data.txt", "r")
lines = f.readlines()
f.close()

njoints = len(lines[0].split(')('))
data = np.zeros((len(lines),njoints*4))

for i in range(len(lines)):
    v3 = lines[i].split(")(")
    v3[0] = v3[0][1:]
    v3[-1] = v3[-1][:-2]
    slist = ','.join(v3)
    data[i] = np.array(slist.split(',')).astype(float)

np.savetxt("train.txt",data,fmt="%.3f")