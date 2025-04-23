using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EventHandlerManager
{
    public static event Action<int> rollFinish;

    public static void CallRollFinish(int val)
    {
        rollFinish?.Invoke(val);
    }
 
}
