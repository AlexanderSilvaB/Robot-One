using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RobotMonitor : MonoBehaviour
{
    public DifferentialKinematics kinematics;
    public Server server;
    public ManualController manualController;
    public Text PositionText;

    void Start()
    {
        
    }

    
    void Update()
    {
        string text = "Client: " + (server.Connected ? ("Connected ("+server.ClientName+")") : "Disconnected");
        text += "\n";
        text += "Manual controller: " + (manualController.Enabled ? "Enabled" : "Disabled");
        text += "\n";
        text += "Position: " + Math.Round(kinematics.Pose.x, 4) + " m, " + Math.Round(kinematics.Pose.y, 4) + " m, " + Math.Round(kinematics.Pose.z, 4) + " rad";
        text += "\n";
        text += "Velocity: " + Math.Round(kinematics.LinearVelocity, 4) + " m/s, " + Math.Round(kinematics.AngularVelocity, 4) + " rad/s";
        text += "\n";
        PositionText.text = text;
    }
}
