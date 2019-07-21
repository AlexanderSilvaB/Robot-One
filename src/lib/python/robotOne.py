from ctypes import *
import math
import os

try:
    from PIL import Image
except:
    print("PIL not found")

try:
    import numpy as np
    import cv2
except:
    print("OpenCV not found")

def fixAngle(rad):
    rad = math.fmod(rad, math.pi * 2)
    if rad > math.pi:
        rad = rad - (2 * math.pi)
    elif rad < -math.pi:
        rad = rad + (2 * math.pi)
    return rad

def connect(address = None):
    if address == None:
        address = "127.0.0.1"
    try:
        if os.name == 'nt':
            rone = cdll.LoadLibrary("robotOne.dll")
        else:
            rone = cdll.LoadLibrary("robotOne.so")
    except:
        try:
            if os.name == 'nt':
                rone = cdll.LoadLibrary("librobotOne.dll")
            else:
                rone = cdll.LoadLibrary("librobotOne.so")
        except:
            print("robot one not found")
            return None
    rone.connectRobotOne.restype = c_int
    rone.disconnectRobotOne.restype = c_int
    rone.versionRobotOne.restype = c_float
    rone.waitRobotOne.restype = c_float
    rone.get.restype = c_float
    rone.set.restype = c_float
    rone.setData.restype = c_float
    rone.setDataFloat.restype = c_float
    status = rone.connectRobotOne(c_char_p(address.encode('utf-8')))
    return rone

def disconnect(rone):
    return rone.disconnectRobotOne()

def version(rone):
    return rone.versionRobotOne()

def wait(rone):
    return rone.waitRobotOne()

def readLidar(rone):
    sz = int(get(rone, "Lidar.Read"))
    rone.getDataFloat.restype = POINTER(c_float * sz)
    readings = rone.getDataFloat().contents
    data = [readings[i] for i in range(sz)]
    inc = math.pi / sz
    start = -(sz/2)*inc
    end = -start
    angles = [start+i*inc for i in range(0, sz)]
    inc = math.pi/2
    x, y = [],[]
    for i in range(0, sz):
        if data[i] > 0:
            x.append(-data[i]*math.cos(inc + angles[i]))
            y.append(data[i]*math.sin(inc + angles[i]))
    return angles, data, x, y

def captureCamera(rone):
    szf = get(rone, "Camera.Capture")
    if szf <> 230400.0:
        return 0, None 
    sz = int(szf)
    rone.getData.restype = POINTER(c_ubyte * sz)
    data = rone.getData()
    return sz, data

def cameraToPil(data):
    im = Image.frombuffer("RGB", (320, 240), data.contents, "raw", "RGB", 0, 1)
    im = im.transpose(Image.FLIP_TOP_BOTTOM)
    return im

def cameraToCV(data):
    arr = np.ctypeslib.as_array((c_ubyte * 230400).from_address(addressof(data.contents)))
    im = arr.reshape(240,320,3)
    im = cv2.flip( im, 0 )
    im = cv2.cvtColor(im, cv2.COLOR_RGB2BGR)
    return im

def get(rone, name):
    return rone.get(c_char_p(name.encode('utf-8')))

def set(rone, name, value):
    return rone.set(c_char_p(name.encode('utf-8')), c_float(value))

def pose(rone, p = None):
    if p == None:
        x = get(rone, "Pose.X")
        y = get(rone, "Pose.Y")
        theta = get(rone, "Pose.Theta")
    else:
        x = set(rone, "Pose.X", p[0])
        y = set(rone, "Pose.Y", p[1])
        theta = set(rone, "Pose.Theta", p[2])
    return (x, y, theta)

def odometry(rone):
    x = get(rone, "Odometry.X")
    y = get(rone, "Odometry.Y")
    theta = get(rone, "Odometry.Theta")
    return (x, y, theta)

def gps(rone):
    x = get(rone, "GPS.X")
    y = get(rone, "GPS.Y")
    theta = get(rone, "GPS.Theta")
    return (x, y, theta)


def odometryStd(rone, std = None):
    if std == None:
        l = get(rone, "Odometry.Std.Linear")
        a = get(rone, "Odometry.Std.Angular")
    else:
        l = set(rone, "Odometry.Std.Linear", std[0])
        a = set(rone, "Odometry.Std.Angular", std[1])
    return (l, a)

def gpsStd(rone, std = None):
    if std == None:
        x = get(rone, "GPS.Std.X")
        y = get(rone, "GPS.Std.Y")
        theta = get(rone, "GPS.Std.Theta")
    else:
        x = set(rone, "GPS.Std.X", std[0])
        y = set(rone, "GPS.Std.Y", std[1])
        theta = set(rone, "GPS.Std.Theta", std[2])
    return (x, y, theta)

def velocity(rone, v = None):
    if v == None:
        l = get(rone, "Velocity.Linear")
        a = get(rone, "Velocity.Angular")
    else:
        l = set(rone, "Velocity.Linear", v[0])
        a = set(rone, "Velocity.Angular", v[1])
    return (l, a)

def lowLevelControl(rone, enabled = None):
    if enabled == None:
        e = get(rone, "Controller.LowLevel")
    else:
        v = 0.0
        if enabled:
            v = 1.0
        e = set(rone, "Controller.LowLevel", v)
    if e > 0:
        return True
    return False

def trace(rone, enabled = None):
    if enabled == None:
        e = get(rone, "Trace")
    else:
        v = 0.0
        if enabled:
            v = 1.0
        e = set(rone, "Trace", v)
    if e > 0:
        return True
    return False

def wheels(rone, v = None):
    if v == None:
        l = get(rone, "Velocity.Angular.Left")
        r = get(rone, "Velocity.Angular.Right")
    else:
        l = set(rone, "Velocity.Angular.Left", v[0])
        r = set(rone, "Velocity.Angular.Right", v[1])
    return (l, r)

if __name__ == "__main__":
    mb = connect("127.0.0.1")
    v = version(mb)
    print('Version: %f' % v)
    print(pose(mb))
    disconnect(mb)
