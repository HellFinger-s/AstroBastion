using UnityEngine;

#if UNITY_EDITOR
using System.Reflection;
#endif


public class LineRendererTravelTimeCalculator : MonoBehaviour
{
    [Header("Path source")]
    public LineRenderer line;
    public bool useWorldSpaceOverride = false;
    public bool closedLoop = false;           

    [Header("Enemy (speed source)")]
    public BaseEnemy baseEnemy; 

    [Header("Target percent from start")]
    [Range(0f, 1f)] public float percent01 = 0.25f;

    [Header("Visualization")]
    public float gizmoRadius = 0.25f;

    private Vector3[] _pointsWorld;
    private float[] _cumulative; // cumulative length at vertex i
    private float _totalLength;
    private int _cachedHash;

    public float TotalLength
    {
        get
        {
            RebuildCacheIfNeeded();
            return _totalLength;
        }
    }

    public float DistanceAtPercent => TotalLength * Mathf.Clamp01(percent01);

    public float EnemySpeed => GetEnemySpeed();

    public float TimeSeconds
    {
        get
        {
            float v = EnemySpeed;
            if (v <= 0f) return float.PositiveInfinity;
            return DistanceAtPercent / v;
        }
    }

    public bool IsValid()
    {
        return line != null && line.positionCount >= 2;
    }

    public bool TryGetWorldPosition(out Vector3 worldPos)
    {
        worldPos = default;

        if (!IsValid())
            return false;

        RebuildCacheIfNeeded();

        if (_totalLength <= 1e-6f)
        {
            worldPos = _pointsWorld[0];
            return true;
        }

        float targetDist = Mathf.Clamp01(percent01) * _totalLength;

        int segIndex = FindSegmentByDistance(targetDist);
        float segStartDist = _cumulative[segIndex];
        float segEndDist = _cumulative[segIndex + 1];
        float segLen = Mathf.Max(1e-6f, segEndDist - segStartDist);

        float t = (targetDist - segStartDist) / segLen;

        Vector3 a = _pointsWorld[segIndex];
        Vector3 b = _pointsWorld[segIndex + 1];
        worldPos = Vector3.Lerp(a, b, t);
        return true;
    }

    public bool TryGetWorldTangent(out Vector3 tangent)
    {
        tangent = Vector3.forward;
        if (!IsValid()) return false;

        RebuildCacheIfNeeded();
        if (_pointsWorld == null || _pointsWorld.Length < 2) return false;

        float targetDist = Mathf.Clamp01(percent01) * Mathf.Max(_totalLength, 0f);
        int segIndex = FindSegmentByDistance(targetDist);

        Vector3 a = _pointsWorld[segIndex];
        Vector3 b = _pointsWorld[segIndex + 1];
        tangent = (b - a).normalized;
        return true;
    }

    private int FindSegmentByDistance(float dist)
    {
        int lo = 0;
        int hi = _cumulative.Length - 2;

        while (lo <= hi)
        {
            int mid = (lo + hi) >> 1;
            float a = _cumulative[mid];
            float b = _cumulative[mid + 1];

            if (dist < a) hi = mid - 1;
            else if (dist >= b) lo = mid + 1;
            else return mid;
        }

        return Mathf.Clamp(lo, 0, _cumulative.Length - 2);
    }

    private void RebuildCacheIfNeeded()
    {
        if (!IsValid())
        {
            _pointsWorld = null;
            _cumulative = null;
            _totalLength = 0f;
            _cachedHash = 0;
            return;
        }

        int hash = ComputeHash();
        if (hash == _cachedHash && _pointsWorld != null && _cumulative != null)
            return;

        _cachedHash = hash;

        int count = line.positionCount;
        bool lrWorld = line.useWorldSpace;
        bool treatAsWorld = useWorldSpaceOverride ? true : lrWorld;

        int outCount = closedLoop ? count + 1 : count;

        if (_pointsWorld == null || _pointsWorld.Length != outCount)
            _pointsWorld = new Vector3[outCount];

        for (int i = 0; i < count; i++)
        {
            Vector3 p = line.GetPosition(i);
            _pointsWorld[i] = treatAsWorld ? p : line.transform.TransformPoint(p);
        }
        if (closedLoop)
            _pointsWorld[outCount - 1] = _pointsWorld[0];

        if (_cumulative == null || _cumulative.Length != outCount)
            _cumulative = new float[outCount];

        _cumulative[0] = 0f;
        float sum = 0f;
        for (int i = 0; i < outCount - 1; i++)
        {
            sum += Vector3.Distance(_pointsWorld[i], _pointsWorld[i + 1]);
            _cumulative[i + 1] = sum;
        }
        _totalLength = sum;
    }

    private int ComputeHash()
    {
        unchecked
        {
            int h = 17;
            h = h * 31 + (line ? line.GetInstanceID() : 0);
            h = h * 31 + line.positionCount;
            h = h * 31 + (line.useWorldSpace ? 1 : 0);
            h = h * 31 + (useWorldSpaceOverride ? 1 : 0);
            h = h * 31 + (closedLoop ? 1 : 0);
            int count = line.positionCount;
            int step = Mathf.Max(1, count / 8);
            for (int i = 0; i < count; i += step)
            {
                Vector3 p = line.GetPosition(i);
                h = h * 31 + p.GetHashCode();
            }
            return h;
        }
    }

    private float GetEnemySpeed()
    {

        if (baseEnemy == null) return 0f;

#if UNITY_EDITOR
        var t = baseEnemy.GetType();
        try
        {
            return baseEnemy.GetBasicSpeed();

            var m = t.GetMethod("GetSpeed", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (m != null && m.ReturnType == typeof(float) && m.GetParameters().Length == 0)
                return (float)m.Invoke(baseEnemy, null);

            string[] propNames = { "Speed", "speed", "MoveSpeed", "moveSpeed", "CurrentSpeed", "currentSpeed" };
            foreach (var name in propNames)
            {
                var p = t.GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (p != null && p.PropertyType == typeof(float) && p.GetIndexParameters().Length == 0)
                    return (float)p.GetValue(baseEnemy);
            }

            string[] fieldNames = { "speed", "moveSpeed", "currentSpeed", "_speed", "_moveSpeed", "_currentSpeed" };
            foreach (var name in fieldNames)
            {
                var f = t.GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (f != null && f.FieldType == typeof(float))
                    return (float)f.GetValue(baseEnemy);
            }
        }
        catch { }
#endif
        return 0f;
    }
}