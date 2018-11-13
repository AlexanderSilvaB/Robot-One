using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheel : MonoBehaviour {

    public float AngularSpeed = 0.0f;

	void Start ()
    {
        transform.Rotate(Vector3.right, Mathf.Rad2Deg * AngularSpeed);
	}
	
	void FixedUpdate ()
    {
        transform.Rotate(Vector3.right, Mathf.Rad2Deg * AngularSpeed * Time.deltaTime);
    }
}
