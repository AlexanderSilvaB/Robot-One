using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Robot : MonoBehaviour
{
    public DifferentialKinematics kinematics = null;
    public Camera RobotCamera = null;
    public GameObject FirstPersonCamera = null;
    public GameObject ThirdPersonCamera = null;
    public GameObject TopCamera = null;
    public Lidar RobotLidar = null;
    public ManualController manualController = null;
    void Start()
    {
        WalkOnChild(transform);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void WalkOnChild(Transform transform)
    {
        foreach(Transform t in transform)
        {
            GameObject go = t.gameObject;

            kinematics = GetComponentOrDefault(go, kinematics);
            RobotLidar = GetComponentOrDefault(go, RobotLidar);
            manualController = GetComponentOrDefault(go, manualController);
            RobotCamera = GetComponentOrDefault(go, RobotCamera, "Camera", false);
            FirstPersonCamera = GetGameObjectOrDefault(go, FirstPersonCamera, "FirstPersonCamera");
            ThirdPersonCamera = GetGameObjectOrDefault(go, ThirdPersonCamera, "ThirdPersonCamera");
            TopCamera = GetGameObjectOrDefault(go, TopCamera, "TopCamera");
            
            WalkOnChild(t);
        }
    }

    private T GetComponentOrDefault<T>(GameObject go, T defaultValue, string name = null, bool checkDefault = true)
    {
        if(name != null)
        {
            if(go.name != name)
                return defaultValue;
        }
        if(checkDefault && defaultValue != null)
            return defaultValue;
        T t = go.GetComponent<T>();
        if(t != null)
            return t;
        return defaultValue;
    }

    private GameObject GetGameObjectOrDefault(GameObject go, GameObject defaultValue, string name)
    {
        if(defaultValue != null)
            return defaultValue;
        if(go.name != name)
            return defaultValue;
        return go;
    }
}
