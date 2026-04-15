using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VectorUtils
{
    private static System.Random rnd = new System.Random();
    public static Vector3 GetRandomOne()
    {
        return new Vector3(rnd.Next(0, 100), rnd.Next(0, 100), rnd.Next(0, 100)) * 0.1f;
    }

    public static Vector3 RandomRotate(Vector3 vector, int angle)
    {
        return Quaternion.Euler(rnd.Next(0, angle), rnd.Next(0, angle), rnd.Next(0, angle)) * vector;
    }
}
