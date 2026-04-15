using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyButtons;

public class SegmentInterval
{
    public int startIndex;
    public int endIndex;
}

public class EnemyPath : MonoBehaviour
{
    public Portal parentPortal;
    [SerializeField]
    private int segmentsCount = 30;
    [SerializeField]
    private List<Transform> controlPoints;
    [SerializeField]
    private LineRenderer pathRenderer;

    private List<float> cumulativeLengths = new List<float> { };
    private float totalLength;

    public Color defaultColor;
    public Color attackAreaColor;

    public List<Vector3> spline;
    private void Awake()
    {
        if (controlPoints == null || controlPoints.Count == 0) return;

        Vector3[] wayPoints = new Vector3[controlPoints.Count];
        for (int i = 0; i < controlPoints.Count; i++)
            wayPoints[i] = controlPoints[i].position;

        spline = SplineGenerator.GenerateCatmullRomSpline(wayPoints, pointsPerSegment: segmentsCount);
        pathRenderer.positionCount = spline.Count;
        pathRenderer.SetPositions(spline.ToArray());
        CalculateCumulativeLengths(spline);
        SetDefaultPathColor();
    }

    [Button]
    public void ShowPath()
    {
        if (controlPoints == null || controlPoints.Count == 0) return;

        Vector3[] wayPoints = new Vector3[controlPoints.Count];
        for (int i = 0; i < controlPoints.Count; i++)
            wayPoints[i] = controlPoints[i].position;

        spline = SplineGenerator.GenerateCatmullRomSpline(wayPoints, pointsPerSegment: segmentsCount);
        pathRenderer.positionCount = spline.Count;
        pathRenderer.SetPositions(spline.ToArray());
        CalculateCumulativeLengths(spline);
        SetDefaultPathColor();
    }

    private void CalculateCumulativeLengths(List<Vector3> spline)
    {
        cumulativeLengths.Add(0f);
        for (int i = 1; i < spline.Count; i++)
        {
            float segmentLength = Vector3.Distance(spline[i - 1], spline[i]);
            totalLength += segmentLength;
            cumulativeLengths.Add(totalLength);
        }

    }

    public Vector3 GetOffset(int radius = 30)
    {
        return parentPortal.GetOffset(radius);
    }

    public void ShowAttackAreaOnPath(Vector3 center, Vector3 normal, float radius)
    {
        List<SegmentInterval> segments = FindHemisphereIntervals(center, normal, radius);
        if (segments.Count > 0)
            Debug.Log(segments[0].startIndex + "    " + segments[0].endIndex);
        Gradient tempGradient = new Gradient();
        tempGradient.mode = GradientMode.Fixed;
        GradientColorKey[] tempColorKeys = new GradientColorKey[segments.Count * 2 + 1];
        for (int i = 0, k = 0; i < segments.Count; i++, k+=2)
        {
            //Debug.Log(pathRenderer.positionCount + "   " + cumulativeLengths[segments[i].startIndex] / totalLength + "   " + cumulativeLengths[segments[i].endIndex] / totalLength);
            if (segments[i].startIndex == segments[i].endIndex)
            {
                if (segments[i].startIndex == pathRenderer.positionCount - 1)
                {
                    segments[i].startIndex -= 1;
                }
                else
                {
                    segments[i].endIndex += 1;
                }
            }
            tempColorKeys[k] = new GradientColorKey(defaultColor, cumulativeLengths[segments[i].startIndex] / totalLength);
            tempColorKeys[k + 1] = new GradientColorKey(attackAreaColor, cumulativeLengths[segments[i].endIndex] / totalLength);
        }
        tempColorKeys[segments.Count * 2] = new GradientColorKey(defaultColor, 1);
        tempGradient.colorKeys = tempColorKeys;
        pathRenderer.colorGradient = tempGradient;
    }

    public void SetDefaultPathColor()
    {
        Gradient tempGradient = new Gradient();
        GradientColorKey[] tempColorKeys = new GradientColorKey[2];
        tempColorKeys[0] = new GradientColorKey(defaultColor, 0);
        tempColorKeys[1] = new GradientColorKey(defaultColor, 1);
        tempGradient.colorKeys = tempColorKeys;
        pathRenderer.colorGradient = tempGradient;
    }

    bool IsPointInHemisphere(Vector3 point, Vector3 center, Vector3 normal, float radius)
    {
        //Debug.DrawLine(center, point, Color.red, 5f);
        Debug.DrawRay(center, normal, Color.blue * 1000, 10f);
        Vector3 dir = point - center;
        if (dir.sqrMagnitude > radius * radius)
        {
            return false;
        }

        return Vector3.Dot(dir, normal) >= 0f;
    }

    public List<SegmentInterval> FindHemisphereIntervals(Vector3 center, Vector3 normal, float radius)
    {
        var intervals = new List<SegmentInterval>();

        bool inside = false;
        int startIdx = -1;

        for (int i = 0; i < pathRenderer.positionCount; i++)
        {
            bool currentInside = IsPointInHemisphere(pathRenderer.GetPosition(i), center, normal, radius);

            if (currentInside && !inside)
            {
                startIdx = i;
                inside = true;
            }
            else if (!currentInside && inside)
            {
                intervals.Add(new SegmentInterval { startIndex = startIdx, endIndex = i - 1 });
                inside = false;
            }
        }

        if (inside)
        {
            intervals.Add(new SegmentInterval { startIndex = startIdx, endIndex = pathRenderer.positionCount - 1 });
        }

        return intervals;
    }
}
