using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public static class EventHandlerManager
{
    #region 小节点事件
    public static event Action<int> rollFinish;
    public static event Action<NodeActionType, bool> updateBuff;
    public static event Action<bool> trafficBuff;
    public static event Action<bool> heavyRainBuff;
    #endregion
    #region 手机UI
    public static event Action chatWindowShow;

    #endregion

    #region 地图
    public static event Action<int, int> updateArriveDistAndTime;
    public static event Func<int, int, float> getDistance;
    public static event Func<int> getCurrentNode;
    public static event Action<int> showTargetNode;
    #endregion

    public static void CallRollFinish(int val)
    {
        rollFinish?.Invoke(val);
    }
    public static void CallTrafficBuff(bool award)
    {
        trafficBuff?.Invoke(award);
    }
    public static void CallHeavyRainBuff(bool award)
    {
        heavyRainBuff?.Invoke(award);
    }
    public static void CallUpdateBuff(NodeActionType type, bool award)
    {
        switch(type)
        {
            case NodeActionType.trafficLight:
                CallTrafficBuff(award);
                break;
            case NodeActionType.HeavyRain:
                CallHeavyRainBuff(award);
                break;

        }
    }

    public static void CallUpdateArriveDistAndTime(int currentNode,int speed)
    {
        updateArriveDistAndTime?.Invoke(currentNode, speed);
    }

    public static float CallGetDistance(int currentNode,int targetNode)
    {
        return getDistance?.Invoke(currentNode, targetNode) ?? -1;
    }
    public static int CallGetCurrentNode()
    {
        return getCurrentNode?.Invoke() ?? -1;
    }
    public static void CallChatWindowShow()
    {
        chatWindowShow?.Invoke();
    }
    public static void CallShowTargetNode(int nodeIdx)
    {
        showTargetNode?.Invoke(nodeIdx);
    }
}
