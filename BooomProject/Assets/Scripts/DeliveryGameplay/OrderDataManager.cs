using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OrderDataManager : MonoBehaviour {
    public event Action OnAvailableOrdersChanged;
    public event Action OnAcceptedOrdersChanged;

    [SerializeField] private List<OrderSO> _allOrders;
    private List<RuntimeOrderSO> _availableOrders = new List<RuntimeOrderSO>();
    private List<RuntimeOrderSO> _acceptedOrders = new List<RuntimeOrderSO>();
    //已接订单与目的节点编号映射表 TODO: 待持久化
    private Dictionary<RuntimeOrderSO, int> _acceptedOrdersNode = new Dictionary<RuntimeOrderSO, int>();
    // 当前送达正在处理的订单
    private RuntimeOrderSO currentHandleOrder;

    [NonSerialized] public int generatedSpecialOrdersCount = 0;
    [NonSerialized] public int generatedCommonOrdersCount = 0;
    public List<RuntimeOrderSO> GetAvailableOrders() => _availableOrders;
    public List<RuntimeOrderSO> GetAcceptedOrders() => _acceptedOrders;

    private void Start() {
        // 按报酬排序
        _availableOrders.Clear();
        var sortedOrder = _allOrders.OrderBy(order => order.baseReward).ToList();
        for (int i = 0; i < _allOrders.Count; i++) {
            if (_allOrders[i].isSpecialOrder && generatedSpecialOrdersCount <= GameplaySettings.m_max_generate_special_orders) {
                _availableOrders.Add(new RuntimeOrderSO(_allOrders[i]));
                generatedSpecialOrdersCount++;
            } else if (!_allOrders[i].isSpecialOrder && generatedCommonOrdersCount <= GameplaySettings.m_max_generate_common_orders) {
                _availableOrders.Add(new RuntimeOrderSO(_allOrders[i]));
                generatedCommonOrdersCount++;
            }
        }
        TimeManager.Instance.OnMinutePassed.AddListener(UpdateOrderTimes);
    }

    private void OnEnable() {
        EventHandlerManager.updateArriveDistAndTime += OnUpdateArriveDistAndTime;
        EventHandlerManager.checkNodeOrder += OnCheckNodeOrder;
        EventHandlerManager.getCurrentOrder += OnGetCurrentOrder;
        EventHandlerManager.updateOrderStateToTransit += OnUpdateOrderStateToTransit;
    }

    private void OnDisable() {
        EventHandlerManager.updateArriveDistAndTime -= OnUpdateArriveDistAndTime;
        EventHandlerManager.checkNodeOrder -= OnCheckNodeOrder;
        EventHandlerManager.getCurrentOrder -= OnGetCurrentOrder;
        EventHandlerManager.updateOrderStateToTransit -= OnUpdateOrderStateToTransit;
    }

    public bool CanAcceptMoreOrders() {
        return _acceptedOrders.Count < GameplaySettings.m_max_accepted_orders;
    }

    public void AcceptOrder(RuntimeOrderSO runtimeOrder) {
        if (!CanAcceptMoreOrders() || !_availableOrders.Contains(runtimeOrder)) {
            Debug.LogWarning($"无法接单：{runtimeOrder.sourceOrder.orderTitle} , 已达到上限");
            return;
        }

        _availableOrders.Remove(runtimeOrder);

        // 初始化运行时数据
        runtimeOrder.acceptedTime = new GameTime {
            day = TimeManager.Instance.currentTime.day,
            hour = TimeManager.Instance.currentTime.hour,
            minute = TimeManager.Instance.currentTime.minute
        };
        runtimeOrder.remainingMinutes = runtimeOrder.sourceOrder.initialLimitTime;
        runtimeOrder.isTimeout = false;
        runtimeOrder.currentState = OrderState.Accepted;
        // 判断是否在大本营节点接单，若是，则改为已取餐
        if(CommonGameplayManager.GetInstance().NodeGraphManager.CurrentNode.NodeID == CommonGameplayManager.GetInstance().NodeGraphManager.BaseNodeID)
        {
            print($"{runtimeOrder.sourceOrder.orderTitle}已取货！");
            runtimeOrder.currentState = OrderState.InTransit;
        }
        _acceptedOrders.Add(runtimeOrder);

        int nodeIdx = runtimeOrder.sourceOrder.destinationNodeId;
        _acceptedOrdersNode.Add(runtimeOrder, nodeIdx);
        CommonGameplayManager.GetInstance().NodeGraphManager.ShowTargetNode(nodeIdx, true);

        Debug.Log($"接单: {runtimeOrder.sourceOrder.orderTitle}");

        // 通知UI
        OnAvailableOrdersChanged?.Invoke();
        OnAcceptedOrdersChanged?.Invoke();
    }

    public IEnumerator CompleteOrders(List<RuntimeOrderSO> ordersToComplete) {
        bool changed = false;
        foreach (RuntimeOrderSO order in ordersToComplete) {
            if (_acceptedOrders.Remove(order)) {
           //     Debug.Log($"订单完成: {order.sourceOrder.orderTitle}");
                changed = true;
                int nodeIdx = order.sourceOrder.customerSO.destNodeId;
                _acceptedOrdersNode.Remove(order);
                CommonGameplayManager.GetInstance().NodeGraphManager.ShowTargetNode(nodeIdx, false);
                if(order.sourceOrder.orderEvent != null)
                {
                    currentHandleOrder = order;
                    yield return StartCoroutine(ExecuteOrderEvents(order));
                    currentHandleOrder = null;
                }
            }
            if (order.sourceOrder.isSpecialOrder) {
                generatedSpecialOrdersCount--;
            } else {
                generatedCommonOrdersCount--;
            }
        }

        if (changed) {
            OnAcceptedOrdersChanged?.Invoke();
        }
    }

    private void UpdateOrderTimes(GameTime currentTime) {
        foreach (var runtimeOrder in _acceptedOrders) {
            if (runtimeOrder.isTimeout) continue;

            int elapsed = CalculateElapsedMinutes(runtimeOrder.acceptedTime, currentTime);
            runtimeOrder.remainingMinutes = runtimeOrder.sourceOrder.initialLimitTime - elapsed;

            // 检查超时
            if (runtimeOrder.remainingMinutes <= 0) {
                runtimeOrder.isTimeout = true;
                runtimeOrder.currentState = OrderState.Expired;
                // 处理超时订单
                Debug.Log($"订单超时：{runtimeOrder.sourceOrder.orderTitle}");
                // 扣声誉等
            }
        }
        OnAcceptedOrdersChanged?.Invoke(); // 通知 UI 刷新
    }

    private int CalculateElapsedMinutes(GameTime start, GameTime end) {
        int startMinutes = start.day * 1440 + start.hour * 60 + start.minute;
        int endMinutes = end.day * 1440 + end.hour * 60 + end.minute;
        int difference = Mathf.Max(0, endMinutes - startMinutes);
        return Mathf.Max(0, endMinutes - startMinutes);
    }

    // 更新订单预计到达时间与距离(外卖员当前所在节点位置更新调用)
    private void OnUpdateArriveDistAndTime(int currentNode, int speed) {
        //int targetNodeIdx;
        float dist;
        foreach (RuntimeOrderSO order in _availableOrders) {
            if (CommonGameplayManager.GetInstance().NodeGraphManager.GetNodeByIDRuntime(order.sourceOrder.customerSO.destNodeId) != null) {
                dist = CommonGameplayManager.GetInstance().NodeGraphManager.GetDistance(currentNode, order.sourceOrder.customerSO.destNodeId);
                order.currentDistance = $"{dist:F1}km";
                order.currentDeliveryTime = dist / speed;
            } else {
                order.currentDistance = "未设置目的地节点，请检查映射表";
                order.currentDeliveryTime = -1;
            }
        }
        foreach (RuntimeOrderSO order in _acceptedOrders) {
            if (CommonGameplayManager.GetInstance().NodeGraphManager.GetNodeByIDRuntime(order.sourceOrder.customerSO.destNodeId) != null) {
                dist = CommonGameplayManager.GetInstance().NodeGraphManager.GetDistance(currentNode, order.sourceOrder.customerSO.destNodeId);
                order.currentDistance = $"{dist:F1}km";
                order.currentDeliveryTime = dist / speed;
            } else {
                order.currentDistance = "未设置目的地节点，请检查映射表";
                order.currentDeliveryTime = -1;
            }
        }
    }

    // 判断是否有当前节点的订单
    private bool OnCheckNodeOrder(int nodeIdx) {
        if (_acceptedOrdersNode.ContainsValue(nodeIdx)) {
            // 查找与当前节点有关的订单
            var orders = _acceptedOrdersNode.Where(item => item.Value.Equals(nodeIdx)).Select(item => item.Key).ToList();
            // 剔除未取货的订单
            for (int i = orders.Count - 1; i >= 0; i--)
            {
                if (orders[i].currentState != OrderState.InTransit)
                {
                    orders.RemoveAt(i);
                }
            }
            StartCoroutine(CompleteOrders(orders));
            return true;
        }
        return false;
    }

    private IEnumerator ExecuteOrderEvents(RuntimeOrderSO order) {
        bool finished = false;
        order.sourceOrder.orderEvent.Initialize((b) => finished = b);
        order.sourceOrder.orderEvent.Execute();

        yield return new WaitUntil(() => finished == true);
    }

    private RuntimeOrderSO OnGetCurrentOrder()
    {
        return currentHandleOrder;
    }

    private void OnUpdateOrderStateToTransit()
    {
        foreach(var order in _acceptedOrders)
        {
            print($"{order.sourceOrder.orderTitle}已取货！");
            order.currentState = OrderState.InTransit;
        }

        foreach (var order in _acceptedOrders)
        {
            print($"{order.sourceOrder.orderTitle}状态：{order.currentState.ToString()}！");
            
        }

    }
}
