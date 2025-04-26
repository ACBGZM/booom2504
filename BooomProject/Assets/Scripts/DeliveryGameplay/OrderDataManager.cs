using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OrderDataManager : Singleton<OrderDataManager> 
{
    public event Action OnAvailableOrdersChanged;
    public event Action OnAcceptedOrdersChanged;

    [SerializeField] private List<OrderSO> _allOrders;
    private List<OrderSO> _availableOrders = new List<OrderSO>();
    private List<OrderSO> _acceptedOrders = new List<OrderSO>();
    // private Dictionary<OrderSO, int> acceptedOrdersNode = new Dictionary<OrderSO, int>();
    //已接订单与目的节点编号映射表 TODO: 待持久化
    private Dictionary<OrderSO, int> acceptedOrdersNode;

    public List<OrderSO> GetAvailableOrders() => new List<OrderSO>(_availableOrders); // 防止外部修改
    public List<OrderSO> GetAcceptedOrders() => new List<OrderSO>(_acceptedOrders);

    protected override void init() {
        _availableOrders = new List<OrderSO>();
        _acceptedOrders = new List<OrderSO>();
        acceptedOrdersNode = new Dictionary<OrderSO, int>();
    }

    private void Start() {
        SortOrders();
        TimeManager.Instance.OnMinutePassed.AddListener(UpdateOrderTimes);
    }

    // 按距离排序
    public void SortOrders() {
        _availableOrders.Clear();
        _availableOrders = _allOrders.OrderBy(order => order.orderDistance).ToList();
    }

    // 添加可用订单
    public void SetAvailableOrders(List<OrderSO> orders) {
        _availableOrders = new List<OrderSO>(orders);
        OnAvailableOrdersChanged?.Invoke();
    }

    public bool CanAcceptMoreOrders() {
        return _acceptedOrders.Count < GameplaySettings.m_max_accepted_orders;
    }

    public void AcceptOrder(OrderSO order) {
        if (!CanAcceptMoreOrders() || !_availableOrders.Contains(order)) {
            Debug.LogWarning($"无法接单：{order.orderTitle} , 已达到上限");
            return;
        }
        _availableOrders.Remove(order);

        // 初始化接受时间和剩余时间
        order.acceptedTime = new GameTime {
            day = TimeManager.Instance.currentTime.day,
            hour = TimeManager.Instance.currentTime.hour,
            minute = TimeManager.Instance.currentTime.minute
        };
        // 初始化剩余时间
        order.remainingMinutes = order.orderLimitTime;
        order.isTimeout = false;

        _acceptedOrders.Add(order);

        // int nodeIdx = GetNodeIndexForOrder(order);
        // acceptedOrdersNode.Add(order, nodeIdx);
        // GameManager.Instance.NodeGraphManager.ShowTargetNode(nodeIdx, true);

        Debug.Log($"接单: {order.orderTitle}");

        // 通知UI
        OnAvailableOrdersChanged?.Invoke();
        OnAcceptedOrdersChanged?.Invoke();
    }

    public void CompleteOrders(List<OrderSO> ordersToComplete) {
        bool changed = false;
        foreach (OrderSO order in ordersToComplete) {
            if (_acceptedOrders.Remove(order)) {
                Debug.Log($"订单完成: {order.orderTitle}");
                changed = true;
                // int nodeIdx = GetNodeIndexForOrder(order);
                // acceptedOrdersNode.Remove(order);
                // GameManager.Instance.NodeGraphManager.ShowTargetNode(nodeIdx, false);
            }
        }

        if (changed) {
            OnAcceptedOrdersChanged?.Invoke();
        }
    }

    public void UpdateOrderTimes(GameTime currentTime) {
        bool uiNeedsRefresh = false;
        List<OrderSO> newlyTimedOutOrders = new List<OrderSO>();

        foreach (var order in _acceptedOrders) {
            if (order.isTimeout) continue; // 跳过已超时订单
            int elapsed = CalculateElapsedMinutes(order.acceptedTime, currentTime);
            int newRemaining = order.orderLimitTime - elapsed;
            // 检查剩余时间
            if (newRemaining != order.remainingMinutes) {
                Debug.Log($"{order.orderTitle} 剩余时间变化");
                order.remainingMinutes = newRemaining;
                uiNeedsRefresh = true;
            }
            // 检查超时
            if (order.remainingMinutes <= 0 && !order.isTimeout) {
                Debug.Log($"{order.orderTitle} 检测到超时");
                order.remainingMinutes = 0;
                order.isTimeout = true;
                newlyTimedOutOrders.Add(order);
                uiNeedsRefresh = true;
                // Debug.Log($"订单超时: {order.orderTitle}"); // Already exists
            }
        }

        if (uiNeedsRefresh) {
            OnAcceptedOrdersChanged?.Invoke(); // 通知 UI 刷新
        }
    }

    private int CalculateElapsedMinutes(GameTime start, GameTime end) {
        int startMinutes = start.day * 1440 + start.hour * 60 + start.minute;
        int endMinutes = end.day * 1440 + end.hour * 60 + end.minute;
        int difference = Mathf.Max(0, endMinutes - startMinutes);
        return Mathf.Max(0, endMinutes - startMinutes);
    }

    private int GetNodeIndexForOrder(OrderSO order) {
        int nodeIdx;
        if (!MapDataManager.Instance.nodeAddress.TryGetValue(order.customerSO.customerAddress, out nodeIdx)) {
           nodeIdx = -1; // Or handle error appropriately
        }
        return nodeIdx;
    }
}
