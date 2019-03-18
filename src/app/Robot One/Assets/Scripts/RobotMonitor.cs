using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RobotMonitor : MonoBehaviour
{
    public DifferentialKinematics kinematics;
    public Text PositionText;

    void Start()
    {
        
    }

    
    void Update()
    {
        PositionText.text = "Position: " + Math.Round(kinematics.Pose.x, 4) + ", " + Math.Round(kinematics.Pose.y, 4) + ", " + Math.Round(kinematics.Pose.z, 4)+" rad";
    }
}
