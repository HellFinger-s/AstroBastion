using System;
using UnityEngine;

public static class EnumUtils
{
    public static T GetNext<T>(T current) where T : Enum
    {
        var values = Enum.GetValues(typeof(T));
        int currentIndex = Array.IndexOf(values, current);
        int nextIndex = (currentIndex + 1) % values.Length;
        return (T)values.GetValue(nextIndex);
    }
}
