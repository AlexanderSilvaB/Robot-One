using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManualController : MonoBehaviour
{
    public bool Enabled = true;
    public float LinearVelocity = 10.0f;
    public float MaxLinearVelocity = 20.0f;
    public float AngularVelocity = 0.76f;
    public float MaxAngularVelocity = 1.57f;

    public DifferentialKinematics kinematics;

    void Start ()
    {
		
	}
	
	void Update ()
    {
        if (Enabled)
        {
            if (Input.GetKey(KeyCode.Z))
            {
                kinematics.LinearVelocity = Input.GetKey(KeyCode.LeftShift) ? MaxLinearVelocity : LinearVelocity;
            }
            else
            {
                kinematics.LinearVelocity = 0.0f;
            }
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                kinematics.AngularVelocity = Input.GetKey(KeyCode.LeftShift) ? MaxAngularVelocity : AngularVelocity;
            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                kinematics.AngularVelocity = Input.GetKey(KeyCode.LeftShift) ? -MaxAngularVelocity : -AngularVelocity;
            }
            else
            {
                kinematics.AngularVelocity = 0.0f;
            }
        }
    }
}
