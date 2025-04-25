using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class OrderManager : MonoBehaviour {
    public event Action<OrderSO> OnChatWindowOpen;
    [SerializeField] private List<OrderSO> _allOrders;
    [SerializeField] private Transform _orderTemplatePrefab;
    [SerializeField] private Transform _myOrderTemplatePrefab;
    [SerializeField] private Transform _availableOrderContainer;
    [SerializeField] private Transform _acceptedOrderContainer;

    private List<OrderSO> _availableOrders; // 可用订单
    private List<OrderSO> _acceptedOrders;  // 已接订单列表
    
    private void Awake() {
        _availableOrders = new List<OrderSO>();
        _acceptedOrders = new List<OrderSO>();
    }

    private void Start() => SortOrders();

    private void OnEnable()
    {
        EventHandlerManager.updateArriveDistAndTime += OnUpdateArriveDistAndTime;
    }
    private void OnDisable()
    {
        EventHandlerManager.updateArriveDistAndTime -= OnUpdateArriveDistAndTime;
    }
    // 按距离排序
    public void SortOrders() {
        _availableOrders.Clear();
        var sortedOrders = _allOrders.OrderBy(order => order.orderDistance).ToList();
        GenerateOrder(sortedOrders);
    }

    // 订单任务生成方法
    private void GenerateOrder(List<OrderSO> availableOrder) {

        
        
        foreach (var order in availableOrder) {
            var orderItem = GenerateAvailableOrder(
                orderNameText: order.orderTitle,
                customerNameText: order.customerSO.customerName,
                distanceText: $"{order.orderDistance:F1}km",
                addressText: order.customerSO.customerAddress,
                range: order.range,
                profileImage: order.customerSO.customerProfile
            // TODO 添加显示时间文本
            );
            Button btn = orderItem.GetComponentInChildren<Button>();
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => OnAcceptOrder(order, orderItem));
        }
    }

    /// <summary>
    /// 生成订单信息
    /// </summary>
    /// <param name="orderNameText">订单时间</param>
    /// <param name="customerNameText">顾客名称</param>
    /// <param name="distanceText">距离</param>
    /// <param name="addressText">顾客地址</param>
    /// <param name="profileImage">顾客头像</param>
    public Transform GenerateAvailableOrder(string orderNameText, string customerNameText, string distanceText, string addressText, int range, Sprite profileImage) {
        
        

        Transform order = Instantiate(_orderTemplatePrefab, _availableOrderContainer);
        TextMeshProUGUI m_nameText = order.transform.Find("OrderName/NameText").gameObject.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI m_customerNameTex = order.transform.Find("OrderInformation/CustomerNameText").gameObject.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI m_distanceText = order.transform.Find("OrderInformation/DistanceText").gameObject.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI m_addressText = order.transform.Find("OrderInformation/AddressText").gameObject.GetComponent<TextMeshProUGUI>();
        Image m_profileImage = order.transform.Find("ProfileImage").gameObject.GetComponent<Image>();

        m_nameText.text = orderNameText;
        m_profileImage.sprite = profileImage;
        m_customerNameTex.text = customerNameText;      


        m_distanceText.text = distanceText;
        m_addressText.text = addressText;
        return order;
    }

    public void GenerateAcceptOrder() {
        for (int i = 0; i < _acceptedOrderContainer.childCount; i++) {
            Destroy(_acceptedOrderContainer.GetChild(i).gameObject);
        }

        foreach (var orderSO in _acceptedOrders) {
            Transform order = Instantiate(_myOrderTemplatePrefab, _acceptedOrderContainer);
            //TextMeshProUGUI m_nameText = order.transform.Find("OrderTitle/Text").gameObject.GetComponent<TextMeshProUGUI>();
            Image m_profileImage = order.transform.Find("ProfileImage").gameObject.GetComponent<Image>();
            Image m_rewardImage = order.transform.Find("Reward/Image").gameObject.GetComponent<Image>();
            GameObject m_reward = order.transform.Find("Reward").gameObject;

            TextMeshProUGUI m_orderTitleText = order.transform.Find("OrderInformation/OrderTitle").gameObject.GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI m_orderAddressText = order.transform.Find("OrderInformation/OrderAddress").gameObject.GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI m_customerAddressNameText = order.transform.Find("OrderInformation/CustomerAddressName").gameObject.GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI m_customerAddressText = order.transform.Find("OrderInformation/CustomerAddress").gameObject.GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI m_BubbleText = order.transform.Find("Bubble/Text").gameObject.GetComponent<TextMeshProUGUI>();

            m_profileImage.sprite = orderSO.customerSO.customerProfile;
            m_orderTitleText.text = orderSO.orderTitle;
            m_orderAddressText.text = orderSO.orderAddress;
            m_customerAddressNameText.text = orderSO.customerSO.customerAddressName;
            m_customerAddressText.text = orderSO.customerSO.customerAddress;
            m_BubbleText.text = orderSO.bubble;
            for (int i = 0; i < orderSO.reward; i++) {
                Instantiate(m_rewardImage, m_reward.transform);
            }

            Button btn = order.GetComponentInChildren<Button>();
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => OnChatWithCustormer(orderSO));
        }
    }

    private void OnChatWithCustormer(OrderSO order) {
        OnChatWindowOpen?.Invoke(order);
    }

    public List<OrderSO> GetAcceptedOrderSO() {
        return _acceptedOrders;
    }

    // 接单按钮点击事件
    private void OnAcceptOrder(OrderSO order, Transform orderItem) {
        if (_acceptedOrders.Count >= GameplaySettings.m_max_accepted_orders) {
            Debug.Log("最大接单数量");
            return;
        }
        _availableOrders.Remove(order);
        Destroy(orderItem.gameObject);
        // 接取订单后 初始化其历史聊天
        order.chatHistory = new List<ChatFragment>();
        // 展示地图目标节点
        int nodeIdx = MapDataManager.Instance.nodeAddress[order.customerSO.customerAddress];
        GameManager.Instance.NodeGraphManager.ShowTargetNode(nodeIdx,true);
        // 添加到已接列表
        _acceptedOrders.Add(order);
        GenerateAcceptOrder();
    }

    private void OnCompleteOrder(OrderSO order, Transform orderItem) {
        // 从已接列表移除
        _acceptedOrders.Remove(order);
        Destroy(orderItem.gameObject);
        // 结束展示地图目标节点
        int nodeIdx = MapDataManager.Instance.nodeAddress[order.customerSO.customerAddress];
        GameManager.Instance.NodeGraphManager.ShowTargetNode(nodeIdx, false);
        // 将订单重新加入可用列表
        _availableOrders.Add(order);
    }
    // 更新订单预计到达时间与距离(外卖员当前所在节点位置更新调用)
    private void OnUpdateArriveDistAndTime(int currentNode,int speed)
    {
        foreach (OrderSO order in _availableOrders)
        {
            float dist = GameManager.Instance.NodeGraphManager.GetDistance(currentNode, MapDataManager.Instance.nodeAddress[order.orderAddress]);
            order.orderDistance = $"{dist:F1}km";
            order.time = dist / speed;
        }
        foreach (OrderSO order in _acceptedOrders)
        {
            float dist = GameManager.Instance.NodeGraphManager.GetDistance(currentNode, MapDataManager.Instance.nodeAddress[order.orderAddress]);
            order.time = dist / speed;
            order.orderDistance = $"{dist:F1}km";
        }
    }
}
