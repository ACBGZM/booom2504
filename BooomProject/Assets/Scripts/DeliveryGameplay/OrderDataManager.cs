using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using System.Collections;

public class OrderDataManager : MonoBehaviour {
    public event Action OnAvailableOrdersChanged;
    public event Action OnAcceptedOrdersChanged;
    public event Action<string> OnOrderComplete;

    [SerializeField] private List<OrderSO> _allOrders;
    private List<RuntimeOrderSO> _availableOrders = new List<RuntimeOrderSO>();
    private List<RuntimeOrderSO> _acceptedOrders = new List<RuntimeOrderSO>();

    private Dictionary<string, int> _orderSeriesProgress = new Dictionary<string, int>();
    // 缓存特殊订单 uid, 普通订单允许重复生成
    private HashSet<string> _existingSpecialOrderUIDs = new HashSet<string>();

    //已接订单与目的节点编号映射表 TODO: 待持久化
    private Dictionary<RuntimeOrderSO, int> _acceptedOrdersNode = new Dictionary<RuntimeOrderSO, int>();

    // 当前送达正在处理的订单
    private RuntimeOrderSO currentHandleOrder;
    public int CurrentSpecialOrderCount => _availableOrders.Count(o => o.sourceOrder.isSpecialOrder);
    public int CurrentCommonOrderCount => _availableOrders.Count(o => !o.sourceOrder.isSpecialOrder);

    // 用于记录每个特殊订单系列当天是否已经生成过
    private Dictionary<string, int> _lastSpecialOrderGenDay = new Dictionary<string, int>();

    // 当前天数
    private int _currentDay = 0;

    // 差评订单数
    private int badOrderCount;
    public int BadOrderCount => badOrderCount;
    public List<RuntimeOrderSO> GetAvailableOrders() => _availableOrders;
    public List<RuntimeOrderSO> GetAcceptedOrders() => _acceptedOrders;

    private void Start() {
        InitializeSeriesProgress();
        LoadOrderProgress();
        // 更新当前天数及重置每日特殊订单生成标记
        CommonGameplayManager.GetInstance().TimeManager.OnDayPassed.AddListener(OnDayPassedHandler);

        GenerateInitialOrders();
        CommonGameplayManager.GetInstance().TimeManager.OnMinutePassed.AddListener(UpdateOrderTimes);
        OnOrderComplete += HandleOrderComplete;
    }

    private void OnEnable() {
        EventHandlerManager.updateArriveDistAndTime += OnUpdateArriveDistAndTime;
        EventHandlerManager.checkNodeOrder += OnCheckNodeOrder;
        EventHandlerManager.getCurrentOrder += OnGetCurrentOrder;
        EventHandlerManager.updateOrderStateToTransit += OnUpdateOrderStateToTransit;
        EventHandlerManager.upGoodOrderCount += OnUpGoodOrderCount;
        EventHandlerManager.upBadOrderCount += OnUpBadOrderCount;
        EventHandlerManager.handleNodeOrder += OnHandleNodeOrder;
    }

    private void OnDisable() {
        EventHandlerManager.updateArriveDistAndTime -= OnUpdateArriveDistAndTime;
        EventHandlerManager.checkNodeOrder -= OnCheckNodeOrder;
        EventHandlerManager.getCurrentOrder -= OnGetCurrentOrder;
        EventHandlerManager.updateOrderStateToTransit -= OnUpdateOrderStateToTransit;
        EventHandlerManager.upGoodOrderCount -= OnUpGoodOrderCount;
        EventHandlerManager.upBadOrderCount -= OnUpBadOrderCount;
        EventHandlerManager.handleNodeOrder -= OnHandleNodeOrder;
    }

    private void LoadOrderProgress() {
        foreach (var orderEntry in CommonGameplayManager.GetInstance().PlayerDataManager.orderSaves) {
            if (orderEntry.Value > 0) {
                ParseOrderUID(orderEntry.Key, out string prefix, out int number);
                UpdateSeriesProgress(prefix, number);
            }
        }
    }

    private void InitializeSeriesProgress() {
        var allSeries = _allOrders
            .Where(o => o.isSpecialOrder)
            .Select(o => GetOrderPrefix(o.orderUID))
            .Distinct();

        foreach (var series in allSeries) {
            if (!_orderSeriesProgress.ContainsKey(series)) {
                _orderSeriesProgress.Add(series, 0);
            }
        }
    }

    /// <summary>
    /// 更新当前只缓存特殊订单的 UID, 普通订单不受限制, 可以多次生成和接取。
    /// </summary>
    private void UpdateExistingOrdersCache() {
        _existingSpecialOrderUIDs.Clear();
        foreach (var o in _availableOrders) {
            if (o.sourceOrder.isSpecialOrder) {
                _existingSpecialOrderUIDs.Add(o.sourceOrder.orderUID);
            }
        }
        foreach (var o in _acceptedOrders) {
            if (o.sourceOrder.isSpecialOrder) {
                _existingSpecialOrderUIDs.Add(o.sourceOrder.orderUID);
            }
        }
    }

    /// <summary>
    /// 每天触发的事件, 更新当前天数并重置每日特殊订单的生成状态
    /// </summary>
    private void OnDayPassedHandler(GameTime gameTime) {
        _currentDay = gameTime.day;
        _lastSpecialOrderGenDay.Clear();
    }

    private void GenerateInitialOrders() {
        _availableOrders.Clear();
        UpdateExistingOrdersCache();

        // 特殊订单采用顺序且每天只生成一次
        var pendingSpecialOrders = _allOrders
            .Where(o => o.isSpecialOrder)
            .OrderBy(o => GetOrderPrefix(o.orderUID))
            .ThenBy(o => GetOrderNumber(o.orderUID))
            .Where(o => CanGenerateSpecialOrder(o.orderUID))
            .Take(GameplaySettings.m_max_generate_special_orders)
            .ToList();
        pendingSpecialOrders.ForEach(AddToAvailableOrders);

        // 普通订单随机挑选
        var availableCommonOrders = _allOrders
            .Where(o => !o.isSpecialOrder)
            .OrderBy(_ => Guid.NewGuid())
            .Take(GameplaySettings.m_max_generate_common_orders)
            .ToList();
        availableCommonOrders.ForEach(AddToAvailableOrders);

        OnAvailableOrdersChanged?.Invoke();
    }

    /// <summary>
    /// 检查特殊订单生成条件：
    /// 1. 必须是该系列的下一个订单（顺序生成）  
    /// 2. 未完成过
    /// 3. 当前未存在于已接或可接列表中  
    /// 4. 当天未生成过该系列的特殊订单
    /// </summary>
    private bool CanGenerateSpecialOrder(string uid) {
        ParseOrderUID(uid, out string prefix, out int number);
        int currentProgress = _orderSeriesProgress.TryGetValue(prefix, out var progress) ? progress : 0;

        // 每个系列每天只允许生成一次特殊订单
        if (_lastSpecialOrderGenDay.TryGetValue(prefix, out int lastGenDay) && lastGenDay == _currentDay)
            return false;

        return number == currentProgress + 1            // 顺序生成：必须为下一个订单号
            && !IsOrderCompleted(uid)                   // 未完成过
            && !_existingSpecialOrderUIDs.Contains(uid) // 保证当前未存在
            && !HasLaterCompletedOrder(prefix, number);
    }

    private bool HasLaterCompletedOrder(string prefix, int currentNumber) {
        return _allOrders.Any(o =>
            o.isSpecialOrder &&
            GetOrderPrefix(o.orderUID) == prefix &&
            GetOrderNumber(o.orderUID) > currentNumber &&
            IsOrderCompleted(o.orderUID));
    }

    private bool IsOrderCompleted(string orderUID) {
        return CommonGameplayManager.GetInstance().PlayerDataManager.orderSaves.ContainsKey(orderUID) &&
               CommonGameplayManager.GetInstance().PlayerDataManager.orderSaves[orderUID] > 0;
    }

    /// <summary>
    /// 添加订单到可接列表, 同时特殊订单生成时更新每日生成标记
    /// </summary>
    private void AddToAvailableOrders(OrderSO order) {
        var runtimeOrder = new RuntimeOrderSO(order);
        _availableOrders.Add(runtimeOrder);
        if (order.isSpecialOrder) {
            ParseOrderUID(order.orderUID, out string prefix, out int number);
            _lastSpecialOrderGenDay[prefix] = _currentDay;
        }
    }

    private void HandleOrderComplete(string orderUID) {
        CommonGameplayManager.GetInstance().PlayerDataManager.AddFinishedOrderCount();
        if (CommonGameplayManager.GetInstance().PlayerDataManager.orderSaves.ContainsKey(orderUID)) {
            CommonGameplayManager.GetInstance().PlayerDataManager.orderSaves[orderUID]++;
        } else {
            CommonGameplayManager.GetInstance().PlayerDataManager.orderSaves.Add(orderUID, 1);
        }
        ParseOrderUID(orderUID, out string prefix, out int number);
        foreach (var order in _allOrders) {
            if (order.orderUID == orderUID && order.isSpecialOrder) {
                UpdateSeriesProgress(prefix, number);
            }
        }
        // 生成后续订单
        int maxTotal = GameplaySettings.m_max_generate_special_orders + GameplaySettings.m_max_generate_common_orders;
        int needed = maxTotal - _availableOrders.Count - _acceptedOrders.Count;
        Debug.Log($"需要{needed}个订单");
        if (needed > 0) {
            GenerateFollowOrders(prefix, number);
            FillRemainingSlots(needed);
        }
        OnAvailableOrdersChanged?.Invoke();
    }

    private void FillRemainingSlots(int needed) {
        UpdateExistingOrdersCache();

        // 补充特殊订单, 满足顺序及每日生成条件
        var pendingSpecialOrders = _allOrders
            .Where(o => o.isSpecialOrder && CanGenerateSpecialOrder(o.orderUID))
            .OrderBy(o => GetOrderNumber(o.orderUID))
            .Take(Mathf.Min(needed, GameplaySettings.m_max_generate_special_orders - CurrentSpecialOrderCount))
            .ToList();

        foreach (var order in pendingSpecialOrders) {
            AddToAvailableOrders(order);
            needed--;
        }

        // 补充普通订单, 不受已存在判断限制, 可重复接取
        var commonPool = _allOrders
            .Where(o => !o.isSpecialOrder)
            .ToList();

        while (needed > 0 && commonPool.Count > 0) {
            int randomIndex = UnityEngine.Random.Range(0, commonPool.Count);
            AddToAvailableOrders(commonPool[randomIndex]);
            needed--;
        }

        // 如果仍有空缺, 随机挑选普通订单
        while (needed > 0) {
            int randomIndex = UnityEngine.Random.Range(0, _allOrders.Count);
            var order = _allOrders[randomIndex];
            if (!order.isSpecialOrder) {
                AddToAvailableOrders(order);
                needed--;
            }
        }
    }

    private void UpdateSeriesProgress(string prefix, int number) {
        if (_orderSeriesProgress.TryGetValue(prefix, out int currentProgress)) {
            if (number > currentProgress) {
                _orderSeriesProgress[prefix] = number;
            }
        } else {
            Debug.LogWarning($"订单系列未预注册 {prefix}");
            _orderSeriesProgress.Add(prefix, number);
        }
    }

    /// <summary>
    /// 生成完成订单后同系列的下一个特殊订单
    /// </summary>
    private void GenerateFollowOrders(string completedPrefix, int completedNumber) {
        int currentSpecialCount = _availableOrders.Count(o => o.sourceOrder.isSpecialOrder);
        if (currentSpecialCount >= GameplaySettings.m_max_generate_special_orders) return;
        var nextOrder = _allOrders.FirstOrDefault(order => order.isSpecialOrder &&
                GetOrderPrefix(order.orderUID) == completedPrefix &&
                GetOrderNumber(order.orderUID) == completedNumber + 1);
        if (nextOrder != null && CanGenerateSpecialOrder(nextOrder.orderUID)) {
            AddToAvailableOrders(nextOrder);
        }
    }

    // 解析订单 UID 为前缀和数字部分
    private void ParseOrderUID(string uid, out string prefix, out int number) {
        prefix = System.Text.RegularExpressions.Regex.Match(uid, "^[A-Za-z]+").Value;
        number = int.Parse(System.Text.RegularExpressions.Regex.Match(uid, "\\d+$").Value);
    }

    // 获取订单的前缀部分
    private string GetOrderPrefix(string uid) =>
        System.Text.RegularExpressions.Regex.Match(uid, "^[A-Za-z]+").Value;

    // 获取订单数字
    private int GetOrderNumber(string uid) =>
        int.Parse(System.Text.RegularExpressions.Regex.Match(uid, "\\d+$").Value);

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
        // 判断是否在大本营节点接单, 若是, 则改为已取
        if (CommonGameplayManager.GetInstance().NodeGraphManager.IsOnBaseCampNode()) {
            print($"{runtimeOrder.sourceOrder.orderUID}已取货");
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
            //if (!order.sourceOrder.isSpecialOrder) {
            //    order.orderEvaluation = Evaluation.Good;
            //}
            string orderUID = order.sourceOrder.orderUID;
            _acceptedOrders.Remove(order);
            changed = true;
            int nodeIdx = order.sourceOrder.destinationNodeId;
            _acceptedOrdersNode.Remove(order);
            CommonGameplayManager.GetInstance().NodeGraphManager.ShowTargetNode(nodeIdx, false);
            if (order.sourceOrder.orderEvent != null) {
                currentHandleOrder = order;
                yield return StartCoroutine(ExecuteOrderEvents(order));
                currentHandleOrder = null;
            }
            OnOrderComplete?.Invoke(orderUID);
        }
        if (changed) {
            OnAcceptedOrdersChanged?.Invoke();
        }
        // 送完单更新好评率
        float rating = (float)CommonGameplayManager.GetInstance().PlayerDataManager.goodOrderCount / CommonGameplayManager.GetInstance().PlayerDataManager.finishedOrderCount;
        CommonGameplayManager.GetInstance().PlayerDataManager.Rating.Value = rating;
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
                // 超时订单
                if (runtimeOrder.remainingMinutes <= 0 && runtimeOrder.orderEvaluation == Evaluation.None) {
                    runtimeOrder.orderEvaluation = Evaluation.Bad;
                }
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
            if (CommonGameplayManager.GetInstance().NodeGraphManager.GetNodeByIDRuntime(order.sourceOrder.destinationNodeId) != null) {
                dist = CommonGameplayManager.GetInstance().NodeGraphManager.GetDistance(currentNode, order.sourceOrder.destinationNodeId);
                order.currentDistance = $"{dist:F1}km";
                order.currentDeliveryTime = dist / speed;
            } else {
                order.currentDistance = "未设置目的地节点, 请检查映射表";
                order.currentDeliveryTime = -1;
            }
        }
        foreach (RuntimeOrderSO order in _acceptedOrders) {
            if (CommonGameplayManager.GetInstance().NodeGraphManager.GetNodeByIDRuntime(order.sourceOrder.destinationNodeId) != null) {
                dist = CommonGameplayManager.GetInstance().NodeGraphManager.GetDistance(currentNode, order.sourceOrder.destinationNodeId);
                order.currentDistance = $"{dist:F1}km";
                order.currentDeliveryTime = dist / speed;
            } else {
                order.currentDistance = "未设置目的地节点, 请检查映射表";
                order.currentDeliveryTime = -1;
            }
        }
    }

    // 判断是否有当前节点的订单
    private List<RuntimeOrderSO> OnCheckNodeOrder(int nodeIdx) {
        if (_acceptedOrdersNode.ContainsValue(nodeIdx)) {
            // 查找与当前节点有关的订单
            var orders = _acceptedOrdersNode.Where(item => item.Value.Equals(nodeIdx)).Select(item => item.Key).ToList();
            // 剔除未取货的订单
            for (int i = orders.Count - 1; i >= 0; i--) {
                if (orders[i].currentState != OrderState.InTransit && orders[i].currentState != OrderState.Expired) {
                    orders.RemoveAt(i);
                }
            }
            //StartCoroutine(CompleteOrders(orders));
            return orders;
        }
        return new List<RuntimeOrderSO>();
    }
    private void OnHandleNodeOrder(int nodeIdx) {
        var orders = OnCheckNodeOrder(nodeIdx);
        if (orders.Count > 0) {
            StartCoroutine(CompleteOrders(orders));
        }
    }
    private IEnumerator ExecuteOrderEvents(RuntimeOrderSO order) {
        bool finished = false;
        order.sourceOrder.orderEvent.Initialize((b) => finished = b);
        order.sourceOrder.orderEvent.Execute();

        yield return new WaitUntil(() => finished == true);
    }

    private RuntimeOrderSO OnGetCurrentOrder() {
        return currentHandleOrder;
    }

    private void OnUpdateOrderStateToTransit() {
        foreach (var order in _acceptedOrders.Where(o => o.currentState == OrderState.Accepted)) {
            order.currentState = OrderState.InTransit;
        }
        OnAcceptedOrdersChanged?.Invoke();
    }

    private void OnUpGoodOrderCount() {
        CommonGameplayManager.GetInstance().PlayerDataManager.AddGoodOrderCount();
    }

    private void OnUpBadOrderCount() {
        badOrderCount++;
    }
}
