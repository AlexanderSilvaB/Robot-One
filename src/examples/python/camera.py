from migo import *
import cv2
import math as m

handler = connect()
trace(handler, True)

MaxV = 5.0
road_color_min = np.array([0, 0, 0],np.uint8)
road_color_max = np.array([255, 90, 180],np.uint8)

try:
    while True:
        sz, buff = captureCamera(handler)
        img = cameraToCV(buff)

        hsv = cv2.cvtColor(img, cv2.COLOR_BGR2HSV)
        mask = cv2.inRange(hsv, road_color_min, road_color_max)
        lineMask = mask[150:160,:]

        cnts = cv2.findContours(lineMask.copy(), cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE)
        cnts = cnts[1]
        w = 0
        if len(cnts) > 0:
            c = cnts[0]
            M = cv2.moments(c)
            if M["m00"] != 0:
                cX = int(M["m10"] / M["m00"])
                cY = int(M["m01"] / M["m00"])


                cv2.line(img, (cX, 155 + cY), (160, 240), (0,0,255), 3)
                #cv2.circle(img, (cX, 155 + cY), 7, (255, 255, 255), -1)
                w = 0.6*m.atan2(160-cX, 80)
                print(m.degrees(w))

        

        velocity(handler, [MaxV, w])

        cv2.imshow("image", img)
        cv2.imshow("mask", mask)
        key = cv2.waitKey(1)
        if key == 27:
            break
except Exception, e:
    print(e)

trace(handler, False)
disconnect(handler)
