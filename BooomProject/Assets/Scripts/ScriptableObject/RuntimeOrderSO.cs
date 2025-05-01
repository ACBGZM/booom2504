using System.Collections.Generic;
using UnityEngine;

public class RuntimeOrderSO {
    // 基础数据引用
    public OrderSO sourceOrder { get; private set; }

    // 运行时状态
    public GameTime acceptedTime { get; set; }
    public OrderState currentState { get; set; }
    public float remainingMinutes { get; set; }
    public bool isTimeout { get; set; }
    public string currentDistance { get; set; }
    public float currentDeliveryTime { get; set; }
    public float praiseProbability { get; set; }
    public List<QuickResponse> runtimeQuickResponses; // 运行时使用的副本
    public List<ChatFragment> runtimeChatHistory;

    [Tooltip("订单评价")]
    public Evaluation orderEvaluation;
    public RuntimeOrderSO(OrderSO sourceOrder) {
        sourceOrder.bubble = sourceOrder.chatHistory.Count == 0 ? "未送达" : sourceOrder.chatHistory[0].content;
        this.sourceOrder = sourceOrder;
        currentState = OrderState.Available;
        runtimeQuickResponses = new List<QuickResponse>(sourceOrder.quickResponses);
        runtimeChatHistory = new List<ChatFragment>(sourceOrder.chatHistory);
    }
}

public enum OrderState {
    Available,
    Accepted,
    InTransit,
    Completed,
    Expired
}
