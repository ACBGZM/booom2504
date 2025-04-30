using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OrderDataManager : MonoBehaviour {
    public event Action OnAvailableOrdersChanged;
    public event Action OnAcceptedOrdersChanged;
    public event Action<string> OnOrderComplete;

    [SerializeField] private List<OrderSO> _allOrders;
    private List<RuntimeOrderSO> _availableOrders = new List<RuntimeOrderSO>();
    private List<RuntimeOrderSO> _acceptedOrders = new List<RuntimeOrderSO>();

    private Dictionary<string, int> _orderSeriesProgress = new Dictionary<string, int>();
    //已接订单与目的节点编号映射表 TODO: 待持久化
    private Dictionary<RuntimeOrderSO, int> _acceptedOrdersNode = new Dictionary<RuntimeOrderSO, int>();
    // 已完成订单总数
    private int finishedOrderCount;
    // 好评订单数
    private int goodOrderCount;
    // 差评订单数
    private int badOrderCount;
    // 当前送达正在处理的订单
    private RuntimeOrderSO currentHandleOrder;

    [NonSerialized] public int generatedSpecialOrdersCount = 0;
    [NonSerialized] public int generatedCommonOrdersCount = 0;
    public List<RuntimeOrderSO> GetAvailableOrders() => _availableOrders;
    public List<RuntimeOrderSO> GetAcceptedOrders() => _acceptedOrders;
    public int FinishedOrderCount => finishedOrderCount;
    public int GoodOrderCount => goodOrderCount;
    public int BadOrderCount => badOrderCount;

    private void Start() {
        LoadOrderProgress();
        GenerateInitialOrders();
        CommonGameplayManager.GetInstance().TimeManager.OnMinutePassed.AddListener(UpdateOrderTimes);
    }

    private void OnEnable() {
        EventHandlerManager.updateArriveDistAndTime += OnUpdateArriveDistAndTime;
        EventHandlerManager.checkNodeOrder += OnCheckNodeOrder;
        EventHandlerManager.getCurrentOrder += OnGetCurrentOrder;
        EventHandlerManager.updateOrderStateToTransit += OnUpdateOrderStateToTransit;
        EventHandlerManager.upGoodOrderCount += OnUpGoodOrderCount;
        EventHandlerManager.upBadOrderCount += OnUpBadOrderCount;
    }

    private void OnDisable() {
        EventHandlerManager.updateArriveDistAndTime -= OnUpdateArriveDistAndTime;
        EventHandlerManager.checkNodeOrder -= OnCheckNodeOrder;
        EventHandlerManager.getCurrentOrder -= OnGetCurrentOrder;
        EventHandlerManager.updateOrderStateToTransit -= OnUpdateOrderStateToTransit;
        EventHandlerManager.upGoodOrderCount -= OnUpGoodOrderCount;
        EventHandlerManager.upBadOrderCount -= OnUpBadOrderCount;
    }

    private void LoadOrderProgress() {
        // 从存档加载已完成订单
        foreach (var order in CommonGameplayManager.GetInstance().PlayerDataManager.orderSaves) {
            if (order.Value) // 如果订单已完成
            {
                ParseOrderUID(order.Key, out string prefix, out int number);
                UpdateSeriesProgress(prefix, number);
            }
        }
    }

    private void GenerateInitialOrders() {
        _availableOrders.Clear();
        // 按UID排序所有订单
        var sortedOrders = _allOrders
            .Where(IsOrderAvailable)
            .OrderBy(order => GetOrderPrefix(order.orderUID))
            .ThenBy(order => GetOrderNumber(order.orderUID))
            .ToList();
        // 生成符合条件的订单
        foreach (var order in sortedOrders) {
            if (order.isSpecialOrder) {
                if (CanGenerateSpecialOrder(order.orderUID) && generatedSpecialOrdersCount <= GameplaySettings.m_max_generate_special_orders) {
                    AddToAvailableOrders(order);
                }
            } else {
                // 普通订单生成
                if (generatedCommonOrdersCount <= GameplaySettings.m_max_generate_common_orders) {
                    AddToAvailableOrders(order);
                    generatedCommonOrdersCount++;
                }
            }
        }
        OnAvailableOrdersChanged?.Invoke();
    }

    private bool CanGenerateSpecialOrder(string uid) {
        // 如果已经完成前序订单
        ParseOrderUID(uid, out string currentPrefix, out int currentNumber);
        // 获取该系列当前进度
        if (!_orderSeriesProgress.TryGetValue(currentPrefix, out int progress)) {
            progress = 0;
        }
        // 检查是否是当前系列的下一个订单
        return currentNumber == progress + 1;
    }

    private void AddToAvailableOrders(OrderSO order) {
        var runtimeOrder = new RuntimeOrderSO(order);
        _availableOrders.Add(runtimeOrder);

        if (order.isSpecialOrder) {
            generatedSpecialOrdersCount++;
        }
    }

    private void HandleOrderComplete(string orderUID) {
        if (CommonGameplayManager.GetInstance().PlayerDataManager.orderSaves.ContainsKey(orderUID)) {
            CommonGameplayManager.GetInstance().PlayerDataManager.orderSaves[orderUID] = true;
        } else {
            CommonGameplayManager.GetInstance().PlayerDataManager.orderSaves.Add(orderUID, true);
        }
        ParseOrderUID(orderUID, out string prefix, out int number);
        UpdateSeriesProgress(prefix, number);
        // 生成后续订单
        GenerateFollowOrders(prefix, number);
    }

    private void UpdateSeriesProgress(string prefix, int number) {
        if (_orderSeriesProgress.ContainsKey(prefix)) {
            if (number > _orderSeriesProgress[prefix]) {
                _orderSeriesProgress[prefix] = number;
            }
        } else {
            _orderSeriesProgress.Add(prefix, number);
        }
    }

    private void GenerateFollowOrders(string completedPrefix, int completedNumber) {
        // 获取下一个订单
        var nextOrder = _allOrders.FirstOrDefault(order => order.isSpecialOrder &&
                GetOrderPrefix(order.orderUID) == completedPrefix &&
                GetOrderNumber(order.orderUID) == completedNumber + 1);
        if (nextOrder != null && CanGenerateSpecialOrder(nextOrder.orderUID)) {
            AddToAvailableOrders(nextOrder);
            OnAvailableOrdersChanged?.Invoke();
        }
    }

    // 解析订单UID
    private void ParseOrderUID(string uid, out string prefix, out int number) {
        prefix = System.Text.RegularExpressions.Regex.Match(uid, "^[A-Za-z]+").Value;
        number = int.Parse(System.Text.RegularExpressions.Regex.Match(uid, "\\d+$").Value);
    }

    // 获取订单前缀
    private string GetOrderPrefix(string uid) =>
        System.Text.RegularExpressions.Regex.Match(uid, "^[A-Za-z]+").Value;

    // 获取订单数字
    private int GetOrderNumber(string uid) =>
        int.Parse(System.Text.RegularExpressions.Regex.Match(uid, "\\d+$").Value);

    // 检查订单是否已经完成
    private bool IsOrderAvailable(OrderSO order) {
        return !CommonGameplayManager.GetInstance().PlayerDataManager.orderSaves.ContainsKey(order.orderUID) ||
               !CommonGameplayManager.GetInstance().PlayerDataManager.orderSaves[order.orderUID];
    }

    public bool CanAcceptMoreOrders() {
        return _acceptedOrders.Count < GameplaySettings.m_max_accepted_orders;
    }

    public void AcceptOrder(RuntimeOrderSO runtimeOrder) {
        if (!CanAcceptMoreOrders() || !_availableOrders.Contains(runtimeOrder)) {
            Debug.LogWarning($"无法接单：{runtimeOrder.sourceOrder.orderUID} , 已达到上限");
            return;
        }

        _availableOrders.Remove(runtimeOrder);

        // 初始化运行时数据
        runtimeOrder.acceptedTime = new GameTime {
            day = CommonGameplayManager.GetInstance().TimeManager.currentTime.day,
            hour = CommonGameplayManager.GetInstance().TimeManager.currentTime.hour,
            minute = CommonGameplayManager.GetInstance().TimeManager.currentTime.minute
        };
        runtimeOrder.remainingMinutes = runtimeOrder.sourceOrder.initialLimitTime;
        runtimeOrder.isTimeout = false;
        runtimeOrder.currentState = OrderState.Accepted;
        // 判断是否在大本营节点接单，若是，则改为已取餐
        if(CommonGameplayManager.GetInstance().NodeGraphManager.IsOnBaseCampNode())
        {
            print($"{runtimeOrder.sourceOrder.orderUID}已取货！");
            runtimeOrder.currentState = OrderState.InTransit;
        }
        _acceptedOrders.Add(runtimeOrder);

        int nodeIdx = runtimeOrder.sourceOrder.destinationNodeId;
        _acceptedOrdersNode.Add(runtimeOrder, nodeIdx);
        CommonGameplayManager.GetInstance().NodeGraphManager.ShowTargetNode(nodeIdx, true);

        Debug.Log($"接单: {runtimeOrder.sourceOrder.orderUID}");

        // 通知UI
        OnAvailableOrdersChanged?.Invoke();
        OnAcceptedOrdersChanged?.Invoke();
    }

    public IEnumerator CompleteOrders(List<RuntimeOrderSO> ordersToComplete) {
        bool changed = false;
        foreach (RuntimeOrderSO order in ordersToComplete) {
            if (_acceptedOrders.Remove(order)) {
                // Debug.Log($"订单完成: {order.sourceOrder.orderTitle}");
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
            OnOrderComplete?.Invoke(order.sourceOrder.orderUID);
            finishedOrderCount++;
        }
        if (changed) {
            OnAcceptedOrdersChanged?.Invoke();
        }
        // 送完单更新好评率
        if(finishedOrderCount > 0)
        {
            CommonGameplayManager.GetInstance().PlayerDataManager.Rating.Value = goodOrderCount / finishedOrderCount;
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
                // -------------------- TODO 处理超时订单 -------------------------
                Debug.Log($"订单超时：{runtimeOrder.sourceOrder.orderUID}");
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
            print($"{order.sourceOrder.orderUID}已取货！");
            order.currentState = OrderState.InTransit;
        }
        OnAcceptedOrdersChanged?.Invoke();

        foreach (var order in _acceptedOrders)
        {
            print($"{order.sourceOrder.orderUID}状态：{order.currentState.ToString()}！");

        }
    }

    private void OnUpGoodOrderCount()
    {
        goodOrderCount++;
    }

    private void OnUpBadOrderCount()
    {
        badOrderCount++;
    }
}
