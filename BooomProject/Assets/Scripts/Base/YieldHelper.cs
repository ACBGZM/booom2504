
using System;
using System.Collections;
using UnityEngine;

public static class YieldHelper
{
    public static IEnumerator WaitForSeconds(float total_wait_time, bool real_time = false)
    {
        float time = 0.0f;
        while (time < total_wait_time)
        {
            time += (real_time ? Time.unscaledDeltaTime : Time.deltaTime);
            yield return null;
        }
    }

    public static IEnumerator WaitUntil(Func<bool> func)
    {
        yield return func();
    }
}
