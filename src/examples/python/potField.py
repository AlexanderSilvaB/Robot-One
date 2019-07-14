import robotOne as ro
import math as m
import numpy as np


P0 = [-19, -19, 0]
PG = [19, 19, 0]
e = 2
E0 = 10
Katt = 0.2
Krep = 1.0
Kw = 0.4
Vmax = 5.0

Obs = [[-5, 5], [-15, 15], [-15, -15], [5, 5]]
for i in range(-18, 19):
    Obs.append([i, 20])
    Obs.append([i, -20])
    Obs.append([20, i])
    Obs.append([-20, i])

def modulo(P):
    return m.sqrt((P[0][0] ** 2) + (P[1][0] ** 2))

def distance(P1, P2):
    return m.sqrt(((P1[0] - P2[0]) ** 2) + ((P1[1] - P2[1]) ** 2))

h = ro.connect("127.0.0.1")
ro.trace(h, True)

try:

    ro.pose(h, P0)
    ro.wait(h)

    PR = ro.pose(h)
    p = distance(PR, PG)

    while p > e:
        PR = ro.pose(h)
        p = distance(PR, PG)
        Fatt = Katt * np.array([[PG[0] - PR[0]], [PG[1] - PR[1]]])
        Frep = np.array([[0], [0]])
        
        for i in range(len(Obs)):
            O = Obs[i]
            do = distance(PR, O)
            if do <= E0:
                Frep = Frep + (1/(do**3))*(1/E0 - 1/do)*np.array([[O[0] - PR[0]], [O[1] - PR[1]]])

        Frep = Krep * Frep
        Ftot = Fatt + Frep
        
        print(Fatt)
        print(Frep)
        v = min(modulo(Ftot), Vmax)
        w = Kw*(m.atan2(Ftot[1][0], Ftot[0][0])-PR[2])
        ro.velocity(h, [v, w])
        ro.wait(h)
        print([v, w])
except Exception(e):
    print(str(e))
    pass

ro.velocity(h, [0, 0])
ro.wait(h)

ro.disconnect(h)