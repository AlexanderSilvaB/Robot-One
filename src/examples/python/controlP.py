import robotOne as ro
import math as m

def distance(P1, P2):
    return m.sqrt(((P1[0] - P2[0]) ** 2) + ((P1[1] - P2[1]) ** 2))

h = ro.connect("127.0.0.1")
ro.trace(h, True)

V = 20.0

Kp = 0.1818181818181818
Ka = 0.4191980558930741
Kb = -0.1818181818181818

PS = [   [0, 0, 0], 
        [8.70, 0.15, -0.11],
        [32.70, -0.10, 0.05],
        [54.10, -1.78, -0.81],
        [53.69, -54.03, -1.74],
        [54.10, -1.78, 2.33],
        [32.70, -0.10, 3.09],
        [8.70, 0.15, 3.03],
        [0, 0, m.pi]]

for i in range(len(PS)):
    P = PS[i]
    R = ro.pose(h)
    p = distance(P, R)
    while p > 1:
        gamma = m.atan2(P[1] - R[1], P[0] - R[0])
        alpha = ro.fixAngle(gamma - R[2])
        beta = ro.fixAngle(P[2] - gamma)
        w = Ka * alpha + Kb * beta
        v = min(Kp * p, V)
        ro.velocity(h, [v, w])
        ro.wait(h)
        R = ro.pose(h)
        p = distance(P, R)
        print("Point: %d, Distance: %f, Angular: %f" % (i, p, w))

ro.velocity(h, [0, 0])
ro.wait(h)

ro.disconnect(h)