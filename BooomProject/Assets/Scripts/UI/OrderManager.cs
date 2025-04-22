using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OrderManager : MonoBehaviour {
    [SerializeField] private List<OrderSO> _allOrders;
    [SerializeField] private Transform _orderTemplatePrefab;
    [SerializeField] private Transform _availableOrderContainer;
    [SerializeField] private Transform _acceptedOrderContainer;

    private List<OrderSO> _availableOrders; // 可用订单
    private List<OrderSO> _acceptedOrders;// 已接订单列表

    private void Awake() {
        _availableOrders=new List<OrderSO>();
        _acceptedOrders=new List<OrderSO>();
    }

    private void Start() => SortOrders();

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
    public Transform GenerateAcceptOrder() {
        //_----------------------------_
        return null;
    }

    // 接单按钮点击事件
    private void OnAcceptOrder(OrderSO order, Transform orderItem) {
        if (_acceptedOrders.Count >= GameplaySettings.m_max_accepted_orders) {
            Debug.Log("最大接单数量");
            return;
        }
        _availableOrders.Remove(order);
        Destroy(orderItem.gameObject);
        // 添加到已接列表
        _acceptedOrders.Add(order);
        GenerateAcceptOrder();
    }

    private void OnCompleteOrder(OrderSO order, Transform orderItem) {
        // 从已接列表移除
        _acceptedOrders.Remove(order);
        Destroy(orderItem.gameObject);

        // 将订单重新加入可用列表
        // _availableOrders.Add(order);
    }
}