using UnityEngine;
using System.Collections.Generic;

public static class SplineGenerator
{

    public static List<Vector3> GenerateCatmullRomSpline(Vector3[] controlPoints, int pointsPerSegment = 20)
    {
        if (controlPoints == null || controlPoints.Length < 2)
            return new List<Vector3>();

        List<Vector3> splinePoints = new List<Vector3>();

        if (controlPoints.Length == 2)
        {
            for (int i = 0; i <= pointsPerSegment; i++)
            {
                float t = (float)i / pointsPerSegment;
                splinePoints.Add(Vector3.Lerp(controlPoints[0], controlPoints[1], t));
            }
            return splinePoints;
        }

        for (int segment = 0; segment < controlPoints.Length - 1; segment++)
        {
            Vector3 p0 = (segment > 0) ? controlPoints[segment - 1] : controlPoints[segment] + (controlPoints[segment] - controlPoints[segment + 1]);
            Vector3 p1 = controlPoints[segment];
            Vector3 p2 = controlPoints[segment + 1];
            Vector3 p3 = (segment + 2 < controlPoints.Length) ? controlPoints[segment + 2] : controlPoints[segment + 1] + (controlPoints[segment + 1] - controlPoints[segment]);

            for (int i = 0; i < pointsPerSegment; i++)
            {
                float t = (float)i / pointsPerSegment;
                Vector3 point = CatmullRom(p0, p1, p2, p3, t);
                splinePoints.Add(point);
            }
        }

        splinePoints.Add(controlPoints[controlPoints.Length - 1]);

        return splinePoints;
    }

    private static Vector3 CatmullRom(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        float t2 = t * t;
        float t3 = t2 * t;

        return 0.5f * (
            (2f * p1) +
            (-p0 + p2) * t +
            (2f * p0 - 5f * p1 + 4f * p2 - p3) * t2 +
            (-p0 + 3f * p1 - 3f * p2 + p3) * t3
        );
    }
}