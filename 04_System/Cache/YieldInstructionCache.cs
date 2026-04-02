using System.Collections.Generic;
using UnityEngine;

internal static class YieldInstructionCache
{
    private class FloatComparer : IEqualityComparer<float>
    {
        public bool Equals(float x, float y)
        {
            return Mathf.Approximately(x, y);
        }

        public int GetHashCode(float obj)
        {
            return obj.GetHashCode();
        }
    }

    public static readonly WaitForFixedUpdate WaitForFixedUpdate = new WaitForFixedUpdate();

    private static readonly Dictionary<float, WaitForSeconds> TimeInterval =
        new Dictionary<float, WaitForSeconds>(new FloatComparer());

    public static WaitForSeconds WaitForSeconds(float seconds)
    {
        if (!TimeInterval.TryGetValue(seconds, out var waitForSeconds))
        {
            TimeInterval.Add(seconds, waitForSeconds = new WaitForSeconds(seconds));
        }

        return waitForSeconds;
    }

    private static readonly Dictionary<float, WaitForSecondsRealtime> WaitForSecondsRealtimeDictionary =
        new Dictionary<float, WaitForSecondsRealtime>(new FloatComparer());

    public static WaitForSecondsRealtime WaitForSecondsRealtime(float seconds)
    {
        if (!WaitForSecondsRealtimeDictionary.TryGetValue(seconds, out var waitForSecondsRealtime))
        {
            WaitForSecondsRealtimeDictionary.Add(seconds, waitForSecondsRealtime = new WaitForSecondsRealtime(seconds));
        }

        return waitForSecondsRealtime;
    }
}