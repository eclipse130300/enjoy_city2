using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PercentageUtils
{
    public static int GetPercentageInt(int percent, int valueFrom)
    {
        float onePercent = Mathf.Round(valueFrom) / 100;
        return Mathf.RoundToInt(onePercent * percent);
    }
}
