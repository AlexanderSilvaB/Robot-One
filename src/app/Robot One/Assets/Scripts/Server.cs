using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class Server : MonoBehaviour
{
    public DifferentialKinematics kinematics;
    public Camera RobotCamera;
    public Lidar RobotLidar;
    public ManualController manualController;
    public CameraSelector WorldCameraSelector;

    private readonly System.Object m_lock = new System.Object();
    private Thread thread;
    private bool running;
    private const int Port = 5046;
    private bool frameUpdated = false;
    private float frameTime = 0;
    private float lastFrameTime = 0;

    private bool captureFromCamera = false;
    private Rect cameraImageRect;
    private Texture2D cameraImage = null;
    private byte[] cameraImageBytes = null;

    private TcpListener tcpServer;
    private byte[] receiveBytes;

    void Start()
    {
        if (kinematics == null)
            kinematics = GetComponent<DifferentialKinematics>();
        if (WorldCameraSelector == null)
            WorldCameraSelector = GetComponent<CameraSelector>();

        if (RobotCamera == null)
            RobotCamera = Camera.main;
        if (RobotLidar == null)
            RobotLidar = GetComponent<Lidar>();

        receiveBytes = new byte[921600];

        GetCameraImage();

        IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
        tcpServer = new TcpListener(ipAddress, Port);
        tcpServer.Server.SendTimeout = 1000;
        tcpServer.Server.ReceiveTimeout = 1000;
        tcpServer.Start();
        running = true;
        thread = new Thread(new ThreadStart(ServerThread));
        thread.Start();
    }

    private void OnDestroy()
    {
        running = false;
        tcpServer.Stop();
    }

    void FixedUpdate()
    {
        Monitor.Enter(m_lock);
        bool pulse = false;
        if (!frameUpdated)
        {
            frameUpdated = true;
            frameTime = Time.time - lastFrameTime;
            lastFrameTime = Time.time;
            pulse |= true;
        }
        if (captureFromCamera)
        {
            GetCameraImage();
            captureFromCamera = false;
            pulse |= true;
        }
        if (pulse)
            Monitor.PulseAll(m_lock);
        Monitor.Exit(m_lock);
    }

    private void GetCameraImage()
    {
        if (cameraImage == null)
        {
            cameraImageRect = new Rect(0, 0, RobotCamera.targetTexture.width, RobotCamera.targetTexture.height);
            cameraImage = new Texture2D(RobotCamera.targetTexture.width, RobotCamera.targetTexture.height, TextureFormat.RGB24, false);
        }
        RenderTexture.active = RobotCamera.targetTexture;
        cameraImage.ReadPixels(cameraImageRect, 0, 0);
        RenderTexture.active = null;
        cameraImageBytes = cameraImage.GetRawTextureData();
    }

    private void ServerThread()
    {
        while (running)
        {
            try
            {
                TcpClient tcpClient = tcpServer.AcceptTcpClient();
                manualController.Enabled = false;
                tcpClient.Client.ReceiveTimeout = 1000;
                tcpClient.Client.SendTimeout = 1000;
                int zeroCtn = 0;
                while (running)
                {
                    try
                    {
                        int sz = tcpClient.Client.Receive(receiveBytes);
                        if (sz == 0)
                        {
                            zeroCtn++;
                            if(zeroCtn == 1000)
                            {
                                break;
                            }
                            continue;
                        }
                        if (receiveBytes[0] == (byte)2 || receiveBytes[0] == (byte)3)
                            continue;
                        int len = BitConverter.ToInt32(receiveBytes, 1);
                        string name = Encoding.UTF8.GetString(receiveBytes, 5, len);
                        float value = BitConverter.ToSingle(receiveBytes, 5 + len);
                        int szHeader = 5 + len + 4;
                        Debug.Log((receiveBytes[0] == (byte)0 ? "Get: " : "Set: ") + name);
                        if (receiveBytes[0] == (byte)1)
                            Debug.Log("Value: " + value);

                        float ret = 0;
                        byte[] retData = null;

                        if (receiveBytes[0] == (byte)0)
                        {
                            if (name == "Time.Wait")
                            {
                                Monitor.Enter(m_lock);
                                frameUpdated = false;
                                while (!frameUpdated)
                                    Monitor.Wait(m_lock);
                                ret = frameTime;
                                Monitor.Exit(m_lock);
                            }
                            else if (name == "Camera.Capture")
                            {
                                Monitor.Enter(m_lock);
                                captureFromCamera = true;
                                while (captureFromCamera)
                                    Monitor.Wait(m_lock);
                                retData = cameraImageBytes;
                                ret = retData.Length;
                                Monitor.Exit(m_lock);
                            }
                            else if (name == "Lidar.Read")
                            {
                                float[] readings = RobotLidar.GetReadings();
                                retData = new byte[readings.Length * 4];
                                Buffer.BlockCopy(readings, 0, retData, 0, retData.Length);
                                ret = readings.Length;
                            }
                            else if (name == "Lidar.Std")
                                ret = RobotLidar.Std;
                            else if (name == "World.Camera.Mode")
                                ret = (int)WorldCameraSelector.SelectedCamera;
                            else if (name == "World.Camera.Zoom")
                                ret = WorldCameraSelector.Distance;
                            else if (name == "GPS.X")
                                ret = kinematics.GPS.x;
                            else if (name == "GPS.Y")
                                ret = kinematics.GPS.y;
                            else if (name == "GPS.Theta")
                                ret = kinematics.GPS.z;
                            else if (name == "GPS")
                            {
                                retData = new byte[3 * 4];
                                byte[] bytes = BitConverter.GetBytes(kinematics.GPS.x);
                                Array.Copy(bytes, 0, retData, 0, 4);
                                bytes = BitConverter.GetBytes(kinematics.GPS.y);
                                Array.Copy(bytes, 0, retData, 4, 4);
                                bytes = BitConverter.GetBytes(kinematics.GPS.z);
                                Array.Copy(bytes, 0, retData, 8, 4);
                                ret = 3;
                            }
                            else if (name == "GPS.Std.X")
                                ret = kinematics.GPSStd.x;
                            else if (name == "GPS.Std.Y")
                                ret = kinematics.GPSStd.y;
                            else if (name == "GPS.Std.Theta")
                                ret = kinematics.GPSStd.z;
                            else if (name == "GPS.Std")
                            {
                                retData = new byte[3 * 4];
                                byte[] bytes = BitConverter.GetBytes(kinematics.GPSStd.x);
                                Array.Copy(bytes, 0, retData, 0, 4);
                                bytes = BitConverter.GetBytes(kinematics.GPSStd.y);
                                Array.Copy(bytes, 0, retData, 4, 4);
                                bytes = BitConverter.GetBytes(kinematics.GPSStd.z);
                                Array.Copy(bytes, 0, retData, 8, 4);
                                ret = 3;
                            }
                            else if (name == "Pose.X")
                                ret = kinematics.Pose.x;
                            else if (name == "Pose.Y")
                                ret = kinematics.Pose.y;
                            else if (name == "Pose.Theta")
                                ret = kinematics.Pose.z;
                            else if (name == "Pose")
                            {
                                retData = new byte[3 * 4];
                                byte[] bytes = BitConverter.GetBytes(kinematics.Pose.x);
                                Array.Copy(bytes, 0, retData, 0, 4);
                                bytes = BitConverter.GetBytes(kinematics.Pose.y);
                                Array.Copy(bytes, 0, retData, 4, 4);
                                bytes = BitConverter.GetBytes(kinematics.Pose.z);
                                Array.Copy(bytes, 0, retData, 8, 4);
                                ret = 3;
                            }
                            else if (name == "Velocity.Linear")
                                ret = kinematics.LinearVelocity;
                            else if (name == "Velocity.Angular")
                                ret = kinematics.AngularVelocity;
                            else if (name == "Velocity")
                            {
                                retData = new byte[2 * 4];
                                byte[] bytes = BitConverter.GetBytes(kinematics.LinearVelocity);
                                Array.Copy(bytes, 0, retData, 0, 4);
                                bytes = BitConverter.GetBytes(kinematics.AngularVelocity);
                                Array.Copy(bytes, 0, retData, 4, 4);
                                ret = 2;
                            }
                            else if (name == "Controller.LowLevel")
                                ret = kinematics.LowLevelControl ? 1.0f : 0.0f;
                            else if (name == "Velocity.Angular.Left")
                                ret = kinematics.LeftWheel.AngularSpeed;
                            else if (name == "Velocity.Angular.Right")
                                ret = kinematics.RightWheel.AngularSpeed;
                            else if (name == "Velocity.Wheels")
                            {
                                retData = new byte[2 * 4];
                                byte[] bytes = BitConverter.GetBytes(kinematics.LeftWheel.AngularSpeed);
                                Array.Copy(bytes, 0, retData, 0, 4);
                                bytes = BitConverter.GetBytes(kinematics.RightWheel.AngularSpeed);
                                Array.Copy(bytes, 0, retData, 4, 4);
                                ret = 2;
                            }
                            else if (name == "Odometry.X")
                                ret = kinematics.Odometry.x;
                            else if (name == "Odometry.Y")
                                ret = kinematics.Odometry.y;
                            else if (name == "Odometry.Theta")
                                ret = kinematics.Odometry.z;
                            else if (name == "Odometry")
                            {
                                retData = new byte[3 * 4];
                                byte[] bytes = BitConverter.GetBytes(kinematics.Odometry.x);
                                Array.Copy(bytes, 0, retData, 0, 4);
                                bytes = BitConverter.GetBytes(kinematics.Odometry.y);
                                Array.Copy(bytes, 0, retData, 4, 4);
                                bytes = BitConverter.GetBytes(kinematics.Odometry.z);
                                Array.Copy(bytes, 0, retData, 8, 4);
                                ret = 3;
                            }
                            else if (name == "Odometry.Std.Linear")
                                ret = kinematics.MotionStd.x;
                            else if (name == "Odometry.Std.Angular")
                                ret = kinematics.MotionStd.y;
                            else if (name == "Odometry.Std")
                            {
                                retData = new byte[2 * 4];
                                byte[] bytes = BitConverter.GetBytes(kinematics.MotionStd.x);
                                Array.Copy(bytes, 0, retData, 0, 4);
                                bytes = BitConverter.GetBytes(kinematics.MotionStd.y);
                                Array.Copy(bytes, 0, retData, 4, 4);
                                ret = 2;
                            }
                            else if (name == "Trace")
                                ret = kinematics.Trace ? 1.0f : 0.0f;
                        }
                        else
                        {
                            if (name == "Pose.X")
                            {
                                kinematics.SetPose(value, kinematics.Pose.y, kinematics.Pose.z);
                                ret = kinematics.Pose.x;
                            }
                            else if (name == "Pose.Y")
                            {
                                kinematics.SetPose(kinematics.Pose.x, value, kinematics.Pose.z);
                                ret = kinematics.Pose.y;
                            }
                            else if (name == "Pose.Theta")
                            {
                                kinematics.SetPose(kinematics.Pose.x, kinematics.Pose.y, value);
                                ret = kinematics.Pose.z;
                            }
                            else if (name == "Velocity.Linear")
                            {
                                kinematics.LinearVelocity = value;
                                ret = kinematics.LinearVelocity;
                            }
                            else if (name == "Velocity.Angular")
                            {
                                kinematics.AngularVelocity = value;
                                ret = kinematics.AngularVelocity;
                            }
                            else if (name == "Controller.LowLevel")
                            {
                                kinematics.LowLevelControl = value > 0;
                                ret = kinematics.LowLevelControl ? 1.0f : 0.0f;
                            }
                            else if (name == "Velocity.Angular.Left")
                            {
                                kinematics.LeftWheel.AngularSpeed = value;
                                ret = kinematics.LeftWheel.AngularSpeed;
                            }
                            else if (name == "Velocity.Angular.Right")
                            {
                                kinematics.RightWheel.AngularSpeed = value;
                                ret = kinematics.RightWheel.AngularSpeed;
                            }
                            else if (name == "Odometry.Std.Linear")
                            {
                                kinematics.MotionStd.x = value;
                                ret = kinematics.MotionStd.x;
                            }
                            else if (name == "Odometry.Std.Angular")
                            {
                                kinematics.MotionStd.y = value;
                                ret = kinematics.MotionStd.y;
                            }
                            else if (name == "Trace")
                            {
                                kinematics.Trace = value > 0;
                                ret = kinematics.Trace ? 1.0f : 0.0f;
                            }
                            else if (name == "Lidar.Std")
                            {
                                RobotLidar.Std = value;
                                ret = RobotLidar.Std;
                            }
                            else if (name == "GPS.Std.X")
                            {
                                kinematics.GPSStd.x = value;
                                ret = kinematics.GPSStd.x;
                            }
                            else if (name == "GPS.Std.Y")
                            {
                                kinematics.GPSStd.y = value;
                                ret = kinematics.GPSStd.y;
                            }
                            else if (name == "GPS.Std.Theta")
                            {
                                kinematics.GPSStd.z = value;
                                ret = kinematics.GPSStd.z;
                            }
                            else if (name == "World.Camera.Mode")
                            {
                                int v = (int)value;
                                if(v >= 0 && v < 3)
                                    WorldCameraSelector.ChooseCamera((CameraSelector.Cameras)v);
                                ret = (int)WorldCameraSelector.SelectedCamera;
                            }
                            else if (name == "World.Camera.Zoom")
                            {
                                WorldCameraSelector.DistanceChanged(value);
                                ret = WorldCameraSelector.Distance;
                            }
                            else if (name == "GPS.Std.Theta")
                            {
                                kinematics.GPSStd.z = value;
                                ret = kinematics.GPSStd.z;
                            }
                        }

                        byte[] data;
                        if (retData == null)
                        {
                            data = new byte[6];
                            data[0] = (byte)2;
                        }
                        else
                        {
                            data = new byte[6 + 4 + retData.Length];
                            data[0] = (byte)3;
                        }
                        data[1] = (byte)1;
                        byte[] retBytes = BitConverter.GetBytes(ret);
                        Array.Copy(retBytes, 0, data, 2, 4);
                        if (retData != null)
                        {
                            retBytes = BitConverter.GetBytes(retData.Length);
                            Array.Copy(retBytes, 0, data, 6, 4);
                            Array.Copy(retData, 0, data, 10, retData.Length);
                        }
                        tcpClient.Client.Send(data);
                    }
                    catch (Exception ex)
                    {
                        Debug.Log(ex.Message);
                    }
                }
                tcpClient.Close();
                manualController.Enabled = true;
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
            }
        }
        tcpServer.Stop();
    }
}
