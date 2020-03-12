import numpy as np

f = open("animation_data.txt", "r")
lines = f.readlines()
f.close()

data = np.zeros((len(lines),147))

for i in range(len(lines)):
    v3 = lines[i].split(")(")
    v3[0] = v3[0][1:]
    v3[-1] = v3[-1][:-2]
    slist = ','.join(v3)
    data[i] = np.array(slist.split(',')).astype(float)

np.savetxt("test_data.txt",data)