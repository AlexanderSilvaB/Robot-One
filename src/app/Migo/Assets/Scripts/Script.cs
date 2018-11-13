using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script : MonoBehaviour
{
    public DifferentialKinematics kinematics;
    public string Code = "";
    private Context context;
    private Interpreter interpreter;

	void Start ()
    {
        if (kinematics == null)
            kinematics = GetComponent<DifferentialKinematics>();
        context = new Context();
        interpreter = new Interpreter();

        Vector3 pose = kinematics.Pose;
        context.Variables["Robot.X"] = pose.x;
        context.Variables["Robot.Y"] = pose.y;
        context.Variables["Robot.Theta"] = pose.z;
        context.Variables["Velocity.Linear"] = kinematics.LinearVelocity;
        context.Variables["Velocity.Angular"] = kinematics.AngularVelocity;
    }
	
	void FixedUpdate ()
    {
        interpreter.Compile(Code);
        interpreter.Execute(context);

        kinematics.LinearVelocity = (float)context.Variables["Velocity.Linear"];
        kinematics.AngularVelocity = (float)context.Variables["Velocity.Angular"];
    }
}
