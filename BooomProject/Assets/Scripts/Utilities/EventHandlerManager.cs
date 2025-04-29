using System;

public static class EventHandlerManager
{
    #region 小节点事件
    public static event Action<int> rollFinish;
    public static event Action<PlayerAttribution, float> updateAttribution;
    //public static event Action<bool> trafficBuff;
    //public static event Action<bool> heavyRainBuff;
    #endregion

    #region 手机UI
    // public static event Action chatWindowShow;

    #endregion

    #region 地图
    public static event Action<int, int> updateArriveDistAndTime;

    #endregion

    #region 全局manager
    public static event Action playerStateChanged;
    public static event Func<CommonInput> getCommonInputHandler;
    #endregion

    #region order
    public static event Func<int, bool> checkNodeOrder;
    public static event Func<RuntimeOrderSO> getCurrentOrder;
    public static event Action updateOrderStateToTransit;
    #endregion

    public static void CallRollFinish(int val)
    {
        rollFinish?.Invoke(val);
    }

    //public static void CallTrafficBuff(bool award)
    //{
    //    trafficBuff?.Invoke(award);
    //}

    //public static void CallHeavyRainBuff(bool award)
    //{
    //    heavyRainBuff?.Invoke(award);
    //}

    public static void CallUpdateAttribution(PlayerAttribution type, float value)
    {
        //switch (type)
        //{
        //    case NodeActionType.trafficLight:
        //        CallTrafficBuff(award);
        //        break;

        //    case NodeActionType.HeavyRain:
        //        CallHeavyRainBuff(award);
        //        break;
        //}
        updateAttribution?.Invoke(type, value);
    }

    public static void CallUpdateArriveDistAndTime(int currentNode, int speed)
    {
        updateArriveDistAndTime?.Invoke(currentNode, speed);
    }

    //public static void CallChatWindowShow()
    //{
    //    chatWindowShow?.Invoke();
    //}

    public static bool CallCheckNodeOrder(int nodeIdx)
    {
        return checkNodeOrder?.Invoke(nodeIdx) ?? false;
    }

    public static void CallPlayerStateChanged()
    {
        playerStateChanged?.Invoke();
    }

    public static CommonInput CallGetCommonInputHandler()
    {
        return getCommonInputHandler?.Invoke();
    }

    public static RuntimeOrderSO CallGetCurrentOrder()
    {
        return getCurrentOrder?.Invoke() ?? null;
    }
    public static void CallUpdateOrderStateToTransit()
    {
        updateOrderStateToTransit?.Invoke();
    }
}
