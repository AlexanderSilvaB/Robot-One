#include <robotOne/robotOne.h>
#include <iostream>

using namespace std;

int main(int argc, char *argv[])
{
    // Connects to Robot One
    int handler = connectRobotOne("127.0.0.1");
    if(!handler)
    {
        cout << "Could not connect to Robot One" << endl;
        return -1;
    }
    
    // Show the robot trace
    setTrace(true);

    // Sets the odometry errors to zero
    Value2 odometryStd;
    odometryStd.values[0] = 0;
    odometryStd.values[1] = 0;
    setOdometryStd(&odometryStd);

    
    // Calculates the angular velocity to make a circular path
    float Vlinear = 10.0;
    float Radius = 10.0;
    float Vangular = Vlinear / Radius;

    // Sets the velocities
    Value2 velocity;
    velocity.values[0] = Vlinear;
    velocity.values[1] = Vangular;
    setVelocity(&velocity);

    // Run for some time
    Value3 pose;
    for(int i = 0; i < 200; i++)
    {
        // Waits a new frame of the simulator
        waitRobotOne();

        // Gets the current pose
        getPose(&pose);
        cout << pose.values[0] << ", " << pose.values[1] << ", " << pose.values[2] << endl;
    }

    // Disable trace
    setTrace(false);
    // Disconnects from the simulator
    disconnectRobotOne();
    return 0;
}