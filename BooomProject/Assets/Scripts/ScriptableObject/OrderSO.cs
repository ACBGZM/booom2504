using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 订单信息数据容器
/// </summary>
[CreateAssetMenu(fileName = "Order", menuName = "Order System/Order Info")]
public class OrderSO : ScriptableObject
{
    [Tooltip("关联的顾客信息")]
    public CustomerSO customerSO;

    [Tooltip("订单标题")]
    public string orderTitle;

    [Tooltip("订单限制时间")]
    public int orderLimitTime;

    [Tooltip("接单时间")] //（运行时计算）
    [System.NonSerialized] public GameTime acceptedTime;

    [Tooltip("剩余时间")]
    [System.NonSerialized] public float remainingMinutes;

    [Tooltip("是否超时")]
    [System.NonSerialized] public bool isTimeout;

    [Tooltip("订单地址")]
    public string orderAddress;

    // [Tooltip("订单与大本营的距离")]
    [Tooltip("订单与骑手当前位置的距离")]
    public string orderDistance;

    [Tooltip("订单等级")]
    public int range;

    [Tooltip("本单奖励")]
    public int reward;

    [Tooltip("其他信息")]
    public string bubble;

    [Tooltip("预计送达时间")]
    public float time;

    [Tooltip("订单聊天历史记录")]
    public List<ChatFragment> chatHistory;
}
