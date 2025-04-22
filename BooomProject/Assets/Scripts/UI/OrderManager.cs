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

    private List<OrderSO> _availableOrders; // ���ö���
    private List<OrderSO> _acceptedOrders;// �ѽӶ����б�

    private void Awake() {
        _availableOrders=new List<OrderSO>();
        _acceptedOrders=new List<OrderSO>();
    }

    private void Start() => SortOrders();

    // ����������
    public void SortOrders() {
        _availableOrders.Clear();
        var sortedOrders = _allOrders.OrderBy(order => order.orderDistance).ToList();
        GenerateOrder(sortedOrders);
    }

    // �����������ɷ���
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
    /// ���ɶ�����Ϣ
    /// </summary>
    /// <param name="orderNameText">����ʱ��</param>
    /// <param name="customerNameText">�˿�����</param>
    /// <param name="distanceText">����</param>
    /// <param name="addressText">�˿͵�ַ</param>
    /// <param name="profileImage">�˿�ͷ��</param>
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

    // �ӵ���ť����¼�
    private void OnAcceptOrder(OrderSO order, Transform orderItem) {
        if (_acceptedOrders.Count >= GameplaySettings.m_max_accepted_orders) {
            Debug.Log("���ӵ�����");
            return;
        }
        _availableOrders.Remove(order);
        Destroy(orderItem.gameObject);
        // ��ӵ��ѽ��б�
        _acceptedOrders.Add(order);
        GenerateAcceptOrder();
    }

    private void OnCompleteOrder(OrderSO order, Transform orderItem) {
        // ���ѽ��б��Ƴ�
        _acceptedOrders.Remove(order);
        Destroy(orderItem.gameObject);

        // ���������¼�������б�
        // _availableOrders.Add(order);
    }
}