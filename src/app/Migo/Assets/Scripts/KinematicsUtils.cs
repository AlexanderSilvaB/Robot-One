using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KinematicsUtils
{
    private static System.Random random = new System.Random();
    public static float FixAngleD(float deg)
    {
        deg = deg % 360.0f;
        if (deg > 180.0f)
            deg -= 360.0f;
        return deg;
    }

    public static float FixAngle(float rad)
    {
        rad = rad % (Mathf.PI * 2.0f);
        if (rad > Mathf.PI)
            rad -= 2.0f * Mathf.PI;
        return rad;
    }

    public static double rand()
    {
        return random.NextDouble();
    }

    public static double randn(double mean, double stdDev)
    {
        double u1 = 1.0-random.NextDouble(); //uniform(0,1] random doubles
        double u2 = 1.0-random.NextDouble();
        double randStdNormal = System.Math.Sqrt(-2.0 * System.Math.Log(u1)) *
                    System.Math.Sin(2.0 * System.Math.PI * u2); //random normal(0,1)
        double randNormal = mean + stdDev * randStdNormal;
        return randNormal;
    }

    public static double randn()
    {
        return randn(0, 1);
    }
}
