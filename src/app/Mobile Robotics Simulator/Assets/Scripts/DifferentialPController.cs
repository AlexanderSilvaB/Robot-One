using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifferentialPController : MonoBehaviour
{
    public bool FollowTarget = false;
    public Transform Target;
    public Vector3 Position = Vector3.zero;
    public float TargetAngle = 0.0f;

    public DifferentialKinematics kinematics;

    public float Kp = 0.1818181818181818f;
    public float Ka = 0.4191980558930741f;
    public float Kb = -0.1818181818181818f;

    public float MaxDistance = 0.05f;
    public float MaxVelocity = 10.0f;

    private float dx, dy, dth, p;
    private float gamma, alpha, beta;

    void Start ()
    {
        kinematics.LinearVelocity = 0.0f;
        kinematics.AngularVelocity = 0.0f;
    }
	
	void FixedUpdate ()
    {
        Vector3 pose = kinematics.Pose;
        if (FollowTarget)
        {
            Position = Target.position;
            TargetAngle = KinematicsUtils.FixAngle(Target.rotation.eulerAngles.y * Mathf.Deg2Rad);
            dx = Position.z - pose.x;
            dy = -Position.x - pose.y;
        }
        else
        {
            dx = Position.x - pose.x;
            dy = Position.y - pose.y;
        }
        p = Mathf.Sqrt(dx * dx + dy * dy);
        if (p > MaxDistance)
        {
            dth = KinematicsUtils.FixAngle(TargetAngle - pose.z);
            gamma = Mathf.Atan2(dy, dx);
            alpha = KinematicsUtils.FixAngle(gamma - pose.z);
            beta = KinematicsUtils.FixAngle(TargetAngle - gamma);
            kinematics.LinearVelocity = Mathf.Min(Kp * p, MaxVelocity);
            kinematics.AngularVelocity = Ka * alpha + Kb * beta;
        }
        else
        {
            kinematics.LinearVelocity = 0.0f;
            kinematics.AngularVelocity = 0.0f;
        }
    }
}
