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
            mobilerobotics = cdll.LoadLibrary("mobilerobotics.dll")
        else:
            mobilerobotics = cdll.LoadLibrary("mobilerobotics.so")
    except:
        try:
            if os.name == 'nt':
                mobilerobotics = cdll.LoadLibrary("libmobilerobotics.dll")
            else:
                mobilerobotics = cdll.LoadLibrary("libmobilerobotics.so")
        except:
            print("mobilerobotics not found")
            return None
    mobilerobotics.Connect.restype = c_int
    mobilerobotics.Disconnect.restype = c_int
    mobilerobotics.Version.restype = c_float
    mobilerobotics.Wait.restype = c_float
    mobilerobotics.Get.restype = c_float
    mobilerobotics.Set.restype = c_float
    mobilerobotics.SetData.restype = c_float
    mobilerobotics.SetDataFloat.restype = c_float
    status = mobilerobotics.Connect(c_char_p(address.encode('utf-8')))
    return mobilerobotics

def disconnect(mobilerobotics):
    return mobilerobotics.Disconnect()

def version(mobilerobotics):
    return mobilerobotics.Version()

def wait(mobilerobotics):
    return mobilerobotics.Wait()

def readLidar(mobilerobotics):
    sz = int(get(mobilerobotics, "Lidar.Read"))
    mobilerobotics.GetDataFloat.restype = POINTER(c_float * sz)
    readings = mobilerobotics.GetDataFloat().contents
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

def captureCamera(mobilerobotics):
    sz = int(get(mobilerobotics, "Camera.Capture"))
    mobilerobotics.GetData.restype = POINTER(c_ubyte * sz)
    data = mobilerobotics.GetData()
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

def get(mobilerobotics, name):
    return mobilerobotics.Get(c_char_p(name.encode('utf-8')))

def set(mobilerobotics, name, value):
    return mobilerobotics.Set(c_char_p(name.encode('utf-8')), c_float(value))

def pose(mobilerobotics, p = None):
    if p == None:
        x = get(mobilerobotics, "Pose.X")
        y = get(mobilerobotics, "Pose.Y")
        theta = get(mobilerobotics, "Pose.Theta")
    else:
        x = set(mobilerobotics, "Pose.X", p[0])
        y = set(mobilerobotics, "Pose.Y", p[1])
        theta = set(mobilerobotics, "Pose.Theta", p[2])
    return (x, y, theta)

def odometry(mobilerobotics):
    x = get(mobilerobotics, "Odometry.X")
    y = get(mobilerobotics, "Odometry.Y")
    theta = get(mobilerobotics, "Odometry.Theta")
    return (x, y, theta)

def odometryStd(mobilerobotics, std = None):
    if std == None:
        l = get(mobilerobotics, "Odometry.Std.Linear")
        a = get(mobilerobotics, "Odometry.Std.Angular")
    else:
        l = set(mobilerobotics, "Odometry.Std.Linear", std[0])
        a = set(mobilerobotics, "Odometry.Std.Angular", std[1])
    return (l, a)

def velocity(mobilerobotics, v = None):
    if v == None:
        l = get(mobilerobotics, "Velocity.Linear")
        a = get(mobilerobotics, "Velocity.Angular")
    else:
        l = set(mobilerobotics, "Velocity.Linear", v[0])
        a = set(mobilerobotics, "Velocity.Angular", v[1])
    return (l, a)

def lowLevelControl(mobilerobotics, enabled = None):
    if enabled == None:
        e = get(mobilerobotics, "Controller.LowLevel")
    else:
        v = 0.0
        if enabled:
            v = 1.0
        e = set(mobilerobotics, "Controller.LowLevel", v)
    if e > 0:
        return True
    return False

def trace(mobilerobotics, enabled = None):
    if enabled == None:
        e = get(mobilerobotics, "Trace")
    else:
        v = 0.0
        if enabled:
            v = 1.0
        e = set(mobilerobotics, "Trace", v)
    if e > 0:
        return True
    return False

def wheels(mobilerobotics, v = None):
    if v == None:
        l = get(mobilerobotics, "Velocity.Angular.Left")
        r = get(mobilerobotics, "Velocity.Angular.Right")
    else:
        l = set(mobilerobotics, "Velocity.Angular.Left", v[0])
        r = set(mobilerobotics, "Velocity.Angular.Right", v[1])
    return (l, r)

if __name__ == "__main__":
    mb = connect("127.0.0.1")
    v = version(mb)
    print('Version: %f' % v)
    print(pose(mb))
    disconnect(mb)
