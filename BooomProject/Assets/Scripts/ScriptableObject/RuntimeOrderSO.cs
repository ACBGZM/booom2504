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

    public RuntimeOrderSO(OrderSO sourceOrder) {
        this.sourceOrder = sourceOrder;
        currentState = OrderState.Available;
    }
}

public enum OrderState {
    Available,
    Accepted,
    InTransit,
    Completed,
    Expired
}
