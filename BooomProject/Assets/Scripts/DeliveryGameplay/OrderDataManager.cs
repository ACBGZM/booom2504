using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OrderDataManager : MonoBehaviour
{
    public event Action OnAvailableOrdersChanged;
    public event Action OnAcceptedOrdersChanged;
    public event Action<OrderSO> OnChatWindowOpen;

    [SerializeField] private List<OrderSO> _allOrders;
    private List<OrderSO> _availableOrders = new List<OrderSO>();
    private List<OrderSO> _acceptedOrders = new List<OrderSO>();
    // private Dictionary<OrderSO, int> acceptedOrdersNode = new Dictionary<OrderSO, int>();
    //已接订单与目的节点编号映射表 TODO: 待持久化
    private Dictionary<OrderSO, int> acceptedOrdersNode;

    public List<OrderSO> GetAvailableOrders() => new List<OrderSO>(_availableOrders); // 防止外部修改
    public List<OrderSO> GetAcceptedOrders() => new List<OrderSO>(_acceptedOrders);

    protected void Awake() {
        _availableOrders = new List<OrderSO>();
        _acceptedOrders = new List<OrderSO>();
        acceptedOrdersNode = new Dictionary<OrderSO, int>();
    }

    private void Start() {
        SortOrders();
        TimeManager.Instance.OnMinutePassed.AddListener(UpdateOrderTimes);
    }

    private void OnEnable() {
        EventHandlerManager.updateArriveDistAndTime += OnUpdateArriveDistAndTime;
        EventHandlerManager.checkNodeOrder += OnCheckNodeOrder;
    }

    private void OnDisable() {
        EventHandlerManager.updateArriveDistAndTime -= OnUpdateArriveDistAndTime;
        EventHandlerManager.checkNodeOrder -= OnCheckNodeOrder;
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

        int nodeIdx = order.customerSO.destNodeId;
        acceptedOrdersNode.Add(order, nodeIdx);
        CommonGameplayManager.GetInstance().NodeGraphManager.ShowTargetNode(nodeIdx, true);

        Debug.Log($"接单: {order.orderTitle}");

        // 通知UI
        OnAvailableOrdersChanged?.Invoke();
        OnAcceptedOrdersChanged?.Invoke();
    }

    private void OnChatWithCustormer(OrderSO order) {
        OnChatWindowOpen?.Invoke(order);
    }

    public IEnumerator CompleteOrders(List<OrderSO> ordersToComplete) {
        bool changed = false;
        foreach (OrderSO order in ordersToComplete) {
            if (_acceptedOrders.Remove(order)) {
                Debug.Log($"订单完成: {order.orderTitle}");
                changed = true;
                int nodeIdx = order.customerSO.destNodeId;
                acceptedOrdersNode.Remove(order);
                CommonGameplayManager.GetInstance().NodeGraphManager.ShowTargetNode(nodeIdx, false);
                if(order.orderEvent != null)
                {
                    yield return StartCoroutine(ExecuteOrderEvents(order));

                }
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

    // 每分钟更新所有订单剩余时间
    private void UpdateAllOrdersTime(GameTime currentTime) {
        List<OrderSO> timeoutOrders = new List<OrderSO>();

        foreach (var order in _acceptedOrders) {
            int elapsed = CalculateElapsedMinutes(order.acceptedTime, currentTime);
            order.remainingMinutes = order.orderLimitTime - elapsed;

            if (order.remainingMinutes <= 0) {
                order.isTimeout = true;
                timeoutOrders.Add(order);
            }
        }
        // 处理超时订单
        foreach (var order in timeoutOrders) {
            Debug.Log($"订单超时：{order.orderTitle}");
            // 扣声誉等
        }
    }

    // 更新订单预计到达时间与距离(外卖员当前所在节点位置更新调用)
    private void OnUpdateArriveDistAndTime(int currentNode, int speed) {
        int targetNodeIdx;
        float dist;
        foreach (OrderSO order in _availableOrders) {
            if (CommonGameplayManager.GetInstance().NodeGraphManager.GetNodeByIDRuntime(order.customerSO.destNodeId) != null) {
                dist = CommonGameplayManager.GetInstance().NodeGraphManager.GetDistance(currentNode, order.customerSO.destNodeId);
                order.orderDistance = $"{dist:F1}km";
                order.time = dist / speed;
            } else {
                order.orderDistance = "未设置目的地节点，请检查映射表";
                order.time = -1;
            }
        }
        foreach (OrderSO order in _acceptedOrders) {
            if (CommonGameplayManager.GetInstance().NodeGraphManager.GetNodeByIDRuntime(order.customerSO.destNodeId) != null) {
                dist = CommonGameplayManager.GetInstance().NodeGraphManager.GetDistance(currentNode, order.customerSO.destNodeId);
                order.orderDistance = $"{dist:F1}km";
                order.time = dist / speed;
            } else {
                order.orderDistance = "未设置目的地节点，请检查映射表";
                order.time = -1;
            }
        }
    }

    // 判断是否有当前节点的订单
    public bool OnCheckNodeOrder(int nodeIdx) {
        if (acceptedOrdersNode.ContainsValue(nodeIdx)) {
            // 查找与当前节点有关的订单
            var orders = acceptedOrdersNode.Where(item => item.Value.Equals(nodeIdx)).Select(item => item.Key);
            StartCoroutine(CompleteOrders(orders.ToList()));
            return true;
        }
        return false;
    }

    private IEnumerator ExecuteOrderEvents(OrderSO order) {
        bool finished = false;
        order.orderEvent.Initialize((b) => finished = b);
        order.orderEvent.Execute();

        yield return new WaitUntil(() => finished == true);
    }
}
