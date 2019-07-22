using System;
using System.Net.Sockets;
using System.Text;

namespace Scripts.GameLanguage
{
    public class API
    {
        TcpClient client;
        byte[] input;

        public void Connect()
        {
            client = new TcpClient("127.0.0.1", 5046);
            client.Client.ReceiveTimeout = 1000;
            client.Client.SendTimeout = 1000;

            input = new byte[921600];
        }

        public void Disconnect()
        {
            client.Close();
        }

        private float get(string key)
        {
            byte[] output = new byte[9 + key.Length];
            output[0] = 0; // get
            Array.Copy(BitConverter.GetBytes(key.Length), 0, output, 1, 4); // name len
            Array.Copy(Encoding.UTF8.GetBytes(key), 0, output, 5, key.Length); // name
            Array.Copy(BitConverter.GetBytes(0.0f), 0, output, 5 + key.Length, 4); // value
            client.Client.Send(output);
            
            int sz = client.Client.Receive(input);
            if (sz == 0 || input[0] != 2 || input[1] != 1)
                return -1;
            return BitConverter.ToSingle(input, 2);
        }

        private float set(string key, float value)
        {
            byte[] output = new byte[9 + key.Length];
            output[0] = 1; // set
            Array.Copy(BitConverter.GetBytes(key.Length), 0, output, 1, 4); // name len
            Array.Copy(Encoding.UTF8.GetBytes(key), 0, output, 5, key.Length); // name
            Array.Copy(BitConverter.GetBytes(value), 0, output, 5 + key.Length, 4); // value
            client.Client.Send(output);

            int sz = client.Client.Receive(input);
            if (sz == 0 || input[0] != 2 || input[1] != 1)
                return -1;
            return BitConverter.ToSingle(input, 2);
        }

        private float wait()
        {
            return get("Time.Wait");
        }

        private void getGPS(out float x, out float y, out float theta)
        {
            x = get("GPS.X");
            y = get("GPS.Y");
            theta = get("GPS.Theta");
        }

        private void getGPSStd(out float x, out float y, out float theta)
        {
            x = get("GPS.Std.X");
            y = get("GPS.Std.Y");
            theta = get("GPS.Std.Theta");
        }

        private void getPose(out float x, out float y, out float theta)
        {
            x = get("Pose.X");
            y = get("Pose.Y");
            theta = get("Pose.Theta");
        }

        private void getVelocity(out float linear, out float angular)
        {
            linear = get("Velocity.Linear");
            angular = get("Velocity.Angular");
        }

        private bool getLowLevel()
        {
            return get("Controller.LowLevel") > 0;
        }

        private void getWheels(out float left, out float right)
        {
            left = get("Velocity.Angular.Left");
            right = get("Velocity.Angular.Right");
        }

        private void getOdometry(out float x, out float y, out float theta)
        {
            x = get("Odometry.X");
            y = get("Odometry.Y");
            theta = get("Odometry.Theta");
        }

        private void getOdometryStd(out float linear, out float angular)
        {
            linear = get("Odometry.Std.Linear");
            angular = get("Odometry.Std.Angular");
        }

        private bool getTrace()
        {
            return get("Trace") > 0;
        }

        private void setGPS(float x, float y, float theta)
        {
            set("GPS.X", x);
            set("GPS.Y", y);
            set("GPS.Theta", theta);
        }

        private void setGPSStd(float x, float y, float theta)
        {
            set("GPS.Std.X", x);
            set("GPS.Std.Y", y);
            set("GPS.Std.Theta", theta);
        }

        private void setPose(float x, float y, float theta)
        {
            set("Pose.X", x);
            set("Pose.Y", y);
            set("Pose.Theta", theta);
        }

        private void setVelocity(float linear, float angular)
        {
            set("Velocity.Linear", linear);
            set("Velocity.Angular", angular);
        }

        private bool setLowLevel(bool enabled)
        {
            return set("Controller.LowLevel", enabled ? 1.0f : 0.0f) > 0;
        }

        private void setWheels(float left, float right)
        {
            set("Velocity.Angular.Left", left);
            set("Velocity.Angular.Right", right);
        }

        private void setOdometry(float x, float y, float theta)
        {
            set("Odometry.X", x);
            set("Odometry.Y", y);
            set("Odometry.Theta", theta);
        }

        private void setOdometryStd(float linear, float angular)
        {
            set("Odometry.Std.Linear", linear);
            set("Odometry.Std.Angular", angular);
        }

        private bool setTrace(bool enabled)
        {
            return set("Trace", enabled ? 1.0f : 0.0f) > 0;
        }

        public void Run()
        {

        }
    }
}
