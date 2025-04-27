using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 订单信息数据容器
/// </summary>
[CreateAssetMenu(fileName = "Order", menuName = "Order System/Order Info")]
public class OrderSO : ScriptableObject {
    [Header("基础信息")]
    [Tooltip("关联的顾客信息")]
    public CustomerSO customerSO;
    [Tooltip("订单标题")]
    public string orderTitle;
    [Tooltip("订单限制时间")]
    public int initialLimitTime;

    [Header("奖励与交互")]
    [Tooltip("本单奖励")]
    public int baseReward;
    [Tooltip("订单特殊事件")]
    public EventSequenceExecutor orderEvent;
    [Tooltip("气泡交流信息")]
    public string bubble;

    [Header("聊天设置")]
    [Tooltip("订单聊天历史记录")]
    public List<ChatFragment> chatHistory;
    [Tooltip("按钮的回复信息")]
    public List<QuickResponse> quickResponses = new List<QuickResponse>();

    [Header("位置信息")]
    [Tooltip("目的地地址")]
    public string destinationAddress;
    [Tooltip("目的地节点")]
    public int destinationNodeId;
}

[System.Serializable]
public class QuickResponse {
    [Tooltip("按钮显示文本")]
    public string buttonText;
    [Tooltip("实际发送文本")]
    public string responseText;
    [Tooltip("是否可以重复发送")]
    public bool cannotReatedly = false;
}
