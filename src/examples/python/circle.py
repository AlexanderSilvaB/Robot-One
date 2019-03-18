import robotOne as ro

handler = ro.connect("127.0.0.1")
ro.trace(handler, True)
ro.odometryStd(handler, [0, 0])


Vlinear = 10.0
Radius = 10.0
Vangular = Vlinear / Radius

try:
    for i in range(200):
        ro.velocity(handler, (Vlinear, Vangular))
        ro.wait(handler)
        R = ro.pose(handler)
        print(R)

    ro.velocity(handler, (0, 0))
except Exception, e:
    print(e)

ro.trace(handler, False)
ro.disconnect(handler)
