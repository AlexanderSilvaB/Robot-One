﻿using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Lidar : MonoBehaviour
{
    public float RayLength = 8.0f;
    public int NumReadings = 11;
    public float AngleRange = 180;
    public float readingTime = 0.03f;
    public float Std = 0.0f;

    private float[] Readings;
    private float angleIncrement;
    private float startAngle, endAngle;
    private float lastTime = 0;
    private float currentAngle;
    private RaycastHit raycast;
    int currentIndex;
    private readonly System.Object m_lock = new System.Object();

    void Start ()
    {
        angleIncrement = AngleRange / NumReadings;
        startAngle = -(NumReadings / 2) * angleIncrement;
        endAngle = -startAngle;
        currentAngle = startAngle;
        Readings = new float[NumReadings];
        currentIndex = 0;
    }
	
	void Update ()
    {
        Monitor.Enter(m_lock);

        currentAngle = startAngle;
        for(currentIndex = 0; currentIndex < NumReadings; currentIndex++)
        {
            currentAngle += angleIncrement;
            transform.localEulerAngles = new Vector3(currentAngle, 0, 0);
            Debug.DrawRay(transform.position, transform.forward * RayLength, Color.red);
            if (Physics.Raycast(transform.position, transform.forward, out raycast, RayLength))
            {
                Readings[currentIndex] = raycast.distance;
            }
            else
            {
                Readings[currentIndex] = RayLength;
            }
        }
        lastTime = Time.time;
        Monitor.Exit(m_lock);
	}

    public float[] GetReadings()
    {
        float[] readings;
        Monitor.Enter(m_lock);
        readings = Readings;
        Monitor.Exit(m_lock);
        return readings;
    }
}
