from mobilerobotics import *
import math as m
from random import uniform
import cv2

handler = connect("127.0.0.1")
trace(handler, True)
odometryStd(handler, [0.01, 0.01])

Kp = 0.1818181818181818
Ka = 0.4191980558930741
Kb = -0.1818181818181818

MaxV = 100.0
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
            R = odometry(handler)
            dx = G[0] - R[0]
            dy = G[1] - R[1]
            p = m.sqrt(dx*dx + dy*dy)
            #print(p)
            if p > MaxD:
                gamma = m.atan2(dy, dx)
                alpha = fixAngle(gamma - R[2])
                beta = fixAngle(G[2] - gamma)
                v = min(Kp * p, MaxV)
                w = Ka * alpha + Kb * beta
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

trace(handler, False)
disconnect(handler)
