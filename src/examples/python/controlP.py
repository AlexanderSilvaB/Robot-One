import robotOne as ro
import math as m
from random import uniform

handler = ro.connect("127.0.0.1")
ro.trace(handler, True)
ro.odometryStd(handler, [0.01, 0.01])

Kp = 0.1818181818181818
Ka = 0.4191980558930741
Kb = -0.1818181818181818

MaxV = 5.0
MaxD = 2.0
#Square = 490
Square = 50

try:
    GL = [[0, 0, 0], [0, 20, 0], [20, 20, 0], [20, 0, 0], [0, 0, 0]]
    i = 0
    while True:
        G = GL[i]
        print(G)
        while True:
            R = ro.odometry(handler)
            dx = G[0] - R[0]
            dy = G[1] - R[1]
            p = m.sqrt(dx*dx + dy*dy)
            #print(p)
            if p > MaxD:
                gamma = m.atan2(dy, dx)
                alpha = ro.fixAngle(gamma - R[2])
                beta = ro.fixAngle(G[2] - gamma)
                v = min(Kp * p, MaxV)
                w = Ka * alpha + Kb * beta
                ro.velocity(handler, (v, w))
            else:
                break
            ro.wait(handler)
        i = i + 1
        if i == len(GL):
            break

    ro.velocity(handler, (0, 0))

    R = ro.pose(handler)
    print(G)
    print(R)
except Exception, e:
    print(e)

ro.trace(handler, False)
ro.disconnect(handler)
