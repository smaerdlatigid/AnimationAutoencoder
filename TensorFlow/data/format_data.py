import numpy as np
import argparse

if __name__ == '__main__':
    parser = argparse.ArgumentParser()
    help_ = "Load data from Unity"
    parser.add_argument("-i","--input",help=help_)
    help_ = "Output file name"
    parser.add_argument("-o","--output",help=help_)
    args = parser.parse_args()

    f = open(args.input, "r")
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

    np.savetxt(args.output,data,fmt="%.3f")