using UnityEngine;

public class DifferentialPIDController : MonoBehaviour {

    public bool FollowTarget = false;
    public Transform Target;
    public Vector3 Position = Vector3.zero;
    public float TargetAngle = 0.0f;
    public DifferentialKinematics kinematics;

    public float Krho = 0.5f;
    public float Kp = 0.5f;
    public float Ki = 0.001f;
    public float Kd = 0.03f;

    public float MaxDistance = 0.05f;
    public float MaxVelocity = 10.0f;

    private float dx, dy, dth, p;
    private float gamma, alpha, i_alpha, alpha_1;

    void Start ()
    {
        kinematics.LinearVelocity = 0.0f;
        kinematics.AngularVelocity = 0.0f;
        alpha_1 = 0;
        i_alpha = 0;
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
        p = Mathf.Sqrt(dx*dx + dy*dy);
        if (p > MaxDistance)
        {
            dth = KinematicsUtils.FixAngle(TargetAngle - pose.z);
            gamma = Mathf.Atan2(dy, dx);
            alpha = KinematicsUtils.FixAngle(gamma - pose.z);
            kinematics.LinearVelocity = Mathf.Min(Krho * p, MaxVelocity);
            kinematics.AngularVelocity = Kp * alpha + Ki * i_alpha + Kd * (alpha - alpha_1);
            alpha_1 = alpha;
            i_alpha += alpha;
        }
        else
        {
            alpha_1 = 0;
            i_alpha = 0;
            kinematics.LinearVelocity = 0.0f;
            kinematics.AngularVelocity = 0.0f;
        }
    }
}
