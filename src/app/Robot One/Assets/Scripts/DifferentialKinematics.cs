using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifferentialKinematics : MonoBehaviour {

    public bool LowLevelControl = false;
    public float LinearVelocity = 0.0f;
    public float AngularVelocity = 0.0f;

    public Wheel LeftWheel;
    public Wheel RightWheel;
    public Transform CenterOfMass;
    public Transform OdometryTransform;
    public LineRenderer PoseTrace;
    public LineRenderer OdometryTrace;
    private Rigidbody rigidbody, rigidbodyOdometry;
    private Vector3 currentPose, nextPose, odometryPose, currentGPS;
    private bool changePose, reset;
    public Vector3 Pose
    {
        get
        {
            return currentPose;
        }
    }

    public Vector3 Odometry
    {
        get
        {
            return odometryPose;
        }
    }
    public Vector3 GPS
    {
        get
        {
            return currentGPS;
        }
    }

    public Vector2 MotionStd = Vector2.zero;
    public Vector3 GPSStd = Vector3.zero;

    private Vector3 lastPosePoint = Vector3.zero;
    private Vector3 lastOdometryPoint = Vector3.zero;
    private bool trace;
    public bool Trace;

    private float LeftRadius, RightRadius;
    private float WheelsDistance;
    private float LeftVelocity, RightVelocity;
    private float DS, DTH, DSX, DSY, EDSX, EDSY, EDTH;

    public void SetPose(Vector3 pose)
    {
        nextPose = pose;
        currentPose = nextPose;
        odometryPose = nextPose;
        changePose = true;
        reset = false;
    }

    public void SetPose(float x, float y, float theta)
    {
        SetPose(new Vector3(x, y, theta));
    }

    public void Reset(Vector3 pose)
    {
        nextPose = pose;
        currentPose = nextPose;
        odometryPose = nextPose;
        changePose = true;
        reset = true;
    }

    public void Reset()
    {
        nextPose = new Vector3(0, 0, 0);
        currentPose = nextPose;
        odometryPose = nextPose;
        changePose = true;
        reset = true;
    }

    void Start ()
    {
        Physics.gravity = new Vector3(0, -98.0f, 0);
        rigidbody = GetComponent<Rigidbody>();
        rigidbodyOdometry = OdometryTransform.gameObject.GetComponent<Rigidbody>();
        rigidbody.centerOfMass = CenterOfMass.localPosition;
        rigidbodyOdometry.centerOfMass = CenterOfMass.localPosition;
        Vector3 leftWheelDims = LeftWheel.gameObject.GetComponent<MeshFilter>().mesh.bounds.size;
        LeftRadius = 0.5f * 0.5f * (LeftWheel.transform.localScale.y * leftWheelDims.y + LeftWheel.transform.localScale.z * leftWheelDims.z);
        Vector3 rightWheelDims = RightWheel.gameObject.GetComponent<MeshFilter>().mesh.bounds.size;
        RightRadius = 0.5f * 0.5f * (RightWheel.transform.localScale.y * rightWheelDims.y + RightWheel.transform.localScale.z * rightWheelDims.z);
        WheelsDistance = Vector3.Distance(LeftWheel.transform.position, RightWheel.transform.position);
    }
	
	void FixedUpdate ()
    {
        if(Trace != trace)
        {
            trace = Trace;
            PoseTrace.positionCount = 0;
            OdometryTrace.positionCount = 0;
            lastPosePoint = Pose;
            lastOdometryPoint = Odometry;
        }
        if(Input.GetKeyUp(KeyCode.R))
        {
            if(Input.GetKey(KeyCode.LeftShift))
                Reset();
            else
                Reset(Pose);
        }
        if(changePose)
        {
            rigidbody.velocity = new Vector3(0, 0, 0);
            rigidbody.angularVelocity = new Vector3(0, 0, 0);
            rigidbodyOdometry.velocity = new Vector3(0, 0, 0);
            rigidbodyOdometry.angularVelocity = new Vector3(0, 0, 0);
            if(!reset)
            {
                transform.position = new Vector3(-nextPose.y, transform.position.y, nextPose.x);
                OdometryTransform.position = new Vector3(-nextPose.y, transform.position.y, nextPose.x);
            }
            else
            {
                transform.position = new Vector3(-nextPose.y, 0.86f, nextPose.x);
                OdometryTransform.position = new Vector3(-nextPose.y, 0.86f, nextPose.x);
            }
            Vector3 e = transform.rotation.eulerAngles;
            if(!reset)
            {
                transform.localEulerAngles = new Vector3(e.x, -nextPose.z * Mathf.Rad2Deg, e.z);
                OdometryTransform.localEulerAngles = new Vector3(e.x, -nextPose.z * Mathf.Rad2Deg, e.z);
            }
            else
            {
                transform.localEulerAngles = new Vector3(0, -nextPose.z * Mathf.Rad2Deg, 0);
                OdometryTransform.localEulerAngles = new Vector3(0, -nextPose.z * Mathf.Rad2Deg, 0);
            }
            PoseTrace.positionCount = 0;
            OdometryTrace.positionCount = 0;
            lastPosePoint = Pose;
            lastOdometryPoint = Odometry;
            changePose = false;
            reset = false;
            return;
        }

        if (LowLevelControl)
        {
            LeftVelocity = LeftWheel.AngularSpeed * LeftRadius;
            RightVelocity = RightWheel.AngularSpeed * RightRadius;
            LinearVelocity = 0.5f * (LeftVelocity + RightVelocity);
            AngularVelocity = (RightVelocity - LeftVelocity) / (WheelsDistance);
        }
        else
        {
            LeftVelocity = LinearVelocity - ((WheelsDistance * 0.5f) * AngularVelocity);
            RightVelocity = LinearVelocity + ((WheelsDistance * 0.5f) * AngularVelocity);

            LeftWheel.AngularSpeed = LeftVelocity / LeftRadius;
            RightWheel.AngularSpeed = RightVelocity / RightRadius;
        }

        DS = Time.deltaTime * LinearVelocity;
        DTH = Time.deltaTime * AngularVelocity;
        DSX = DS * Mathf.Cos(DTH * 0.5f);
        EDSX = DSX;
        DSY = DS * Mathf.Sin(DTH * 0.5f);
        EDSY = DSY;
        EDTH = DTH;

        if(MotionStd.magnitude > 0)
        {
            if(Mathf.Abs(DS) > 0)
            {
                EDSX += (float)KinematicsUtils.randn(0, MotionStd.x);
                EDSY += (float)KinematicsUtils.randn(0, MotionStd.x);
            }

            if(Mathf.Abs(DTH) > 0)
            {
                EDTH += (float)KinematicsUtils.randn(0, MotionStd.y);
            }
        }
        
        transform.Translate(EDSY, 
                            0, 
                            EDSX, 
                            Space.Self);
        transform.Rotate(Vector3.down, 
                        EDTH * Mathf.Rad2Deg);

        if(MotionStd.magnitude > 0)
        {
            OdometryTransform.Translate(DSY, 
                                0, 
                                DSX, 
                                Space.Self);
            OdometryTransform.Rotate(Vector3.down, 
                            DTH * Mathf.Rad2Deg);
        }
        else
        {
            OdometryTransform.position = transform.position;
            OdometryTransform.rotation = transform.rotation;
        }

        currentPose.Set(transform.position.z, -transform.position.x, -KinematicsUtils.FixAngle(transform.rotation.eulerAngles.y * Mathf.Deg2Rad));
        odometryPose.Set(OdometryTransform.position.z, -OdometryTransform.position.x, -KinematicsUtils.FixAngle(OdometryTransform.rotation.eulerAngles.y * Mathf.Deg2Rad));
        currentGPS.Set(currentPose.x + (float)KinematicsUtils.randn(0, GPSStd.x), currentPose.y + (float)KinematicsUtils.randn(0, GPSStd.y), currentPose.z + (float)KinematicsUtils.randn(0, GPSStd.z));
        

        if(trace)
        {
            if((lastOdometryPoint-Odometry).magnitude > 0.2f)
            {
                OdometryTrace.positionCount = OdometryTrace.positionCount + 1;
                OdometryTrace.SetPosition(OdometryTrace.positionCount - 1, new Vector3(OdometryTransform.position.x, OdometryTransform.position.y - 0.8f, OdometryTransform.position.z));
                lastOdometryPoint = Odometry;
            }
            if((lastPosePoint-Pose).magnitude > 0.2f)
            {
                PoseTrace.positionCount = PoseTrace.positionCount + 1;
                PoseTrace.SetPosition(PoseTrace.positionCount - 1, new Vector3(transform.position.x, transform.position.y - 0.8f, transform.position.z));
                lastPosePoint = Pose;
            }
        }
    }
}
