using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OrderManager : Singleton<OrderManager>
{
    public event Action<OrderSO> OnChatWindowOpen;

    [SerializeField] private List<OrderSO> _allOrders;
    [SerializeField] private Transform _orderTemplatePrefab;
    [SerializeField] private Transform _myOrderTemplatePrefab;
    [SerializeField] private Transform _availableOrderContainer;
    [SerializeField] private Transform _acceptedOrderContainer;

    private List<OrderSO> _availableOrders; // 可用订单
    private List<OrderSO> _acceptedOrders;  // 已接订单列表
    //已接订单与目的节点编号映射表 TODO: 待持久化
    private Dictionary<OrderSO, int> acceptedOrdersNode;

    protected override void init()
    {
        _availableOrders = new List<OrderSO>();
        _acceptedOrders = new List<OrderSO>();
        acceptedOrdersNode = new Dictionary<OrderSO, int>();
    }

    private void Start() {
        TimeManager.Instance.OnMinutePassed.AddListener(UpdateAllOrdersTime);
    }

    private void OnEnable()
    {
        EventHandlerManager.updateArriveDistAndTime += OnUpdateArriveDistAndTime;
        EventHandlerManager.checkNodeOrder += OnCheckNodeOrder;
    }

    private void OnDisable()
    {
        EventHandlerManager.updateArriveDistAndTime -= OnUpdateArriveDistAndTime;
        EventHandlerManager.checkNodeOrder -= OnCheckNodeOrder;
    }

    private void OnChatWithCustormer(OrderSO order)
    {
        OnChatWindowOpen?.Invoke(order);
    }

    public List<OrderSO> GetAcceptedOrderSO()
    {
        return _acceptedOrders;
    }

    private void OnAcceptOrder(OrderSO order, Transform orderItem)
    {
        if (_acceptedOrders.Count >= GameplaySettings.m_max_accepted_orders)
        {
            return;
        }
        _availableOrders.Remove(order);
        Destroy(orderItem.gameObject);
        //// 初始化订单历史记录
        //order.chatHistory = new List<ChatFragment>();
        //// 获取目的地节点
        //// SO文件待填
        //int nodeIdx;
        //if (!MapDataManager.Instance.nodeAddress.TryGetValue(order.customerSO.customerAddress, out nodeIdx))
        //{
        //    nodeIdx = -1;
        //}
        //GameManager.Instance.NodeGraphManager.ShowTargetNode(nodeIdx, true);

        // 添加order进节点映射表
        //acceptedOrdersNode.Add(order, nodeIdx);
    }

    public void CompleteOrders(List<OrderSO> orders)
    {
        foreach (OrderSO order in orders)
        {
            print($"送达订单{order.orderAddress}");
            _acceptedOrders.Remove(order);

            acceptedOrdersNode.Remove(order);
            // 结束展示地图目标节点
            int nodeIdx;
            if (!MapDataManager.Instance.nodeAddress.TryGetValue(order.customerSO.customerAddress, out nodeIdx))
            {
                nodeIdx = -1;
            }
            GameManager.Instance.NodeGraphManager.ShowTargetNode(nodeIdx, false);
        }

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

    // 计算分钟差
    private int CalculateElapsedMinutes(GameTime start, GameTime end) {
        // 转换为总分钟数
        int startMinutes = start.day * 1440 + start.hour * 60 + start.minute;
        int endMinutes = end.day * 1440 + end.hour * 60 + end.minute;

        return endMinutes - startMinutes;
    }

    // 更新订单预计到达时间与距离(外卖员当前所在节点位置更新调用)
    private void OnUpdateArriveDistAndTime(int currentNode, int speed)
    {
        int targetNodeIdx;
        float dist;
        foreach (OrderSO order in _availableOrders)
        {
            if (MapDataManager.Instance.nodeAddress.TryGetValue(order.orderAddress, out targetNodeIdx))
            {
                dist = GameManager.Instance.NodeGraphManager.GetDistance(currentNode, targetNodeIdx);
                order.orderDistance = $"{dist:F1}km";
                order.time = dist / speed;
            } else {
                order.orderDistance = "未设置目的地节点，请检查映射表";
                order.time = -1;
            }
        }
        foreach (OrderSO order in _acceptedOrders)
        {
            if (MapDataManager.Instance.nodeAddress.TryGetValue(order.orderAddress, out targetNodeIdx))
            {
                dist = GameManager.Instance.NodeGraphManager.GetDistance(currentNode, targetNodeIdx);
                order.orderDistance = $"{dist:F1}km";
                order.time = dist / speed;
            } else {
                order.orderDistance = "未设置目的地节点，请检查映射表";
                order.time = -1;
            }
        }
    }

    // 判断是否有当前节点的订单
    private bool OnCheckNodeOrder(int nodeIdx)
    {
        if (acceptedOrdersNode.ContainsValue(nodeIdx))
        {
            // 查找与当前节点有关的订单
            var orders = acceptedOrdersNode.Where(item => item.Value.Equals(nodeIdx)).Select(item => item.Key);
            CompleteOrders(orders.ToList());
            return true;
        }
        return false;
    }

}
