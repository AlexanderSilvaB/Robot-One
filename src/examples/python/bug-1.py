import robotOne as ro
import math as m

# Goal
G = [-10.0, 8.0]
# Derired error
E = 0.5
# Sets the max linear speed
V = 1.0

# Begin Robot-One communication
handler = ro.connect("127.0.0.1")
# Shows the trace of the robot
ro.trace(handler, True)
# Sets the odometry errors to zero
ro.odometryStd(handler, [0, 0])



try:
    while True:
        # Gets the current pose of the robot
        R = ro.pose(handler)
        
        # Calculates the target distance
        dx = G[0] - R[0]
        dy = G[1] - R[1]
        dist = m.sqrt(dx*dx + dy*dy)
        print('Dist: %f' % dist)
        if dist < E:
            break

        # Calculates the target linear speed
        v = min(V, 0.2 * dist)

        # Calculates the target angle
        W = m.atan2(dy, dx) - R[2]
        w = ro.fixAngle(W)
        
        # Reads the lidar
        ret = ro.readLidar(handler)
        lidar = ret[1]

        # Adjust the target angle according to the lidar readings
        for j in range(len(lidar)):
            lidar[j] = ((8 - lidar[j])/8.0)*(m.pi/len(lidar))
            if j < len(lidar)/2:
                lidar[j] *= -1
            w += lidar[j]

        print("Dist: %f m, Speed: [ %f m/s, %f deg/s ]" % (dist, v, m.degrees(w)))

        # Sets the robot velocities
        ro.velocity(handler, (v, w))
        # Waits the simmulator to finish requested actions
        ro.wait(handler)

    # Sets the robot speed to zero
    ro.velocity(handler, (0, 0))
except Exception, e:
    print(e)

# Hides the robot trace
ro.trace(handler, False)
# Disconnects from Robot-One
ro.disconnect(handler)