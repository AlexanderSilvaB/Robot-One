from migo import *
import math as m
from random import uniform

handler = connect("127.0.0.1")
trace(handler, True)
odometryStd(handler, [0.01, 0])

Krho = 1.0
Kp = 1.0
Ki = 0.01
Kd = 0.01

MaxV = 100.0
MaxD = 1.0

pose(handler, [0,0,0])

try:
    GL = [[0, 0, 0], [0, 20, 0], [20, 20, 0], [20, 0, 0], [0, 0, 0]]
    i = 0
    while True:
        G = GL[i]
        alpha_i = 0
        alpha_1 = 0
        print(G)
        while True:
            R = pose(handler)
            dx = G[0] - R[0]
            dy = G[1] - R[1]
            p = m.sqrt(dx*dx + dy*dy)
            #print(p)
            if p > MaxD:
                gamma = m.atan2(dy, dx)
                alpha = fixAngle(gamma - R[2])
                v = min(Krho * p, MaxV)
                w = Kp * alpha + Ki * alpha_i + Kd * (alpha - alpha_1)
                alpha_1 = alpha
                alpha_i = alpha_i + alpha
                velocity(handler, (v, w))
            else:
                break
            wait(handler)
        i = i + 1
        if i == len(GL):
            break

    velocity(handler, (0, 0))

    R = pose(handler)
    print(G)
    print(R)
except Exception, e:
    print(e)

#trace(handler, False)
disconnect(handler)
