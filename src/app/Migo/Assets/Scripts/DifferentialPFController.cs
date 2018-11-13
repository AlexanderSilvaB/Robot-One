using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifferentialPFController : MonoBehaviour
{
    public bool FollowTarget = false;
    public Transform Target;
    public Vector3 Position = Vector3.zero;

    public Vector2[] Obstacles;

    public DifferentialKinematics kinematics;

    public float Katt = 0.5f;
    public float Krep = 0.3f;
    public float Kw = 0.2f;

    public float MaxDistance = 0.05f;
    public float MaxVelocity = 10.0f;
    public float EventsHorizon = 0.1f;

    private float dx, dy, p;
    private Vector2 fatt, frep, ftot;


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
            fatt.Set(Katt * dx, Katt * dy);
            float frepX = 0, frepY = 0;
            if(Obstacles != null && Obstacles.Length > 0)
            {
                float e = 0;
                for (int i = 0; i < Obstacles.Length; i++)
                {
                    dx = Obstacles[i].x - pose.x;
                    dy = Obstacles[i].y - pose.y;
                    e = Mathf.Sqrt(dx*dx + dy*dy);
                    e = (1.0f / (e * e * e)) * ((1.0f / EventsHorizon) - (1.0f / e));
                    frepX += e * dx;
                    frepY += e * dy;
                }
            }
            frep.Set(Krep * frepX, Krep * frepY);
            ftot = fatt + ftot;
            kinematics.LinearVelocity = Mathf.Min(ftot.magnitude, MaxVelocity);
            kinematics.AngularVelocity = Kw * (Mathf.Atan2(-ftot.y, ftot.x) - pose.z);
        }
        else
        {
            kinematics.LinearVelocity = 0.0f;
            kinematics.AngularVelocity = 0.0f;
        }
    }
}
