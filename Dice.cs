using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Dice
{
    private static System.Random rnd = new System.Random();

    public static bool RollChance(int percent)
    {
        if (percent <= 0)
        {
            return false;
        }
        if (percent >= 100)
        {
            return true;
        }
        return rnd.Next(100) < percent;
    }
}
