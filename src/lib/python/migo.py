from ctypes import*
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
    return rad

def connect(address = None):
    if address == None:
        address = "127.0.0.1"
    try:
        if os.name == 'nt':
            migo = cdll.LoadLibrary("migo.dll")
        else:
            migo = cdll.LoadLibrary("migo.so")
    except:
        try:
            if os.name == 'nt':
                migo = cdll.LoadLibrary("libmigo.dll")
            else:
                migo = cdll.LoadLibrary("libmigo.so")
        except:
            print("migo not found")
            return None
    migo.connectMigo.restype = c_int
    migo.disconnectMigo.restype = c_int
    migo.versionMigo.restype = c_float
    migo.waitMigo.restype = c_float
    migo.get.restype = c_float
    migo.set.restype = c_float
    migo.setData.restype = c_float
    migo.setDataFloat.restype = c_float
    status = migo.connectMigo(c_char_p(address.encode('utf-8')))
    return migo

def disconnect(migo):
    return migo.disconnectMigo()

def version(migo):
    return migo.versionMigo()

def wait(migo):
    return migo.waitMigo()

def readLidar(migo):
    sz = int(get(migo, "Lidar.Read"))
    migo.getDataFloat.restype = POINTER(c_float * sz)
    readings = migo.getDataFloat().contents
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

def captureCamera(migo):
    sz = int(get(migo, "Camera.Capture"))
    migo.getData.restype = POINTER(c_ubyte * sz)
    data = migo.getData()
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

def get(migo, name):
    return migo.get(c_char_p(name.encode('utf-8')))

def set(migo, name, value):
    return migo.set(c_char_p(name.encode('utf-8')), c_float(value))

def pose(migo, p = None):
    if p == None:
        x = get(migo, "Pose.X")
        y = get(migo, "Pose.Y")
        theta = get(migo, "Pose.Theta")
    else:
        x = set(migo, "Pose.X", p[0])
        y = set(migo, "Pose.Y", p[1])
        theta = set(migo, "Pose.Theta", p[2])
    return (x, y, theta)

def odometry(migo):
    x = get(migo, "Odometry.X")
    y = get(migo, "Odometry.Y")
    theta = get(migo, "Odometry.Theta")
    return (x, y, theta)

def odometryStd(migo, std = None):
    if std == None:
        l = get(migo, "Odometry.Std.Linear")
        a = get(migo, "Odometry.Std.Angular")
    else:
        l = set(migo, "Odometry.Std.Linear", std[0])
        a = set(migo, "Odometry.Std.Angular", std[1])
    return (l, a)

def velocity(migo, v = None):
    if v == None:
        l = get(migo, "Velocity.Linear")
        a = get(migo, "Velocity.Angular")
    else:
        l = set(migo, "Velocity.Linear", v[0])
        a = set(migo, "Velocity.Angular", v[1])
    return (l, a)

def lowLevelControl(migo, enabled = None):
    if enabled == None:
        e = get(migo, "Controller.LowLevel")
    else:
        v = 0.0
        if enabled:
            v = 1.0
        e = set(migo, "Controller.LowLevel", v)
    if e > 0:
        return True
    return False

def trace(migo, enabled = None):
    if enabled == None:
        e = get(migo, "Trace")
    else:
        v = 0.0
        if enabled:
            v = 1.0
        e = set(migo, "Trace", v)
    if e > 0:
        return True
    return False

def wheels(migo, v = None):
    if v == None:
        l = get(migo, "Velocity.Angular.Left")
        r = get(migo, "Velocity.Angular.Right")
    else:
        l = set(migo, "Velocity.Angular.Left", v[0])
        r = set(migo, "Velocity.Angular.Right", v[1])
    return (l, r)

if __name__ == "__main__":
    mb = connect("127.0.0.1")
    v = version(mb)
    print('Version: %f' % v)
    print(pose(mb))
    disconnect(mb)
