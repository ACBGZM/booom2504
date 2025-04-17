using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OrderManager : MonoBehaviour {
    [SerializeField] private Transform _orderContainer;
    [SerializeField] private Transform _orderTemplete;

    private void Awake() {
        //_orderTemplete.gameObject.SetActive(false);
    }

    // 设置订单文本
    public void SetOrderSO(OrderSO orderSO) {
        Transform orderTransform = Instantiate(_orderTemplete, _orderContainer);
        orderTransform.gameObject.SetActive(true);

        // 订单文本信息以及顾客头像
        TextMeshProUGUI titleText = orderTransform.Find("TitleText").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI distanceText = orderTransform.Find("DistanceText").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI customerNameText = orderTransform.Find("CustomerNameText").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI Address = orderTransform.Find("AddressText").GetComponent<TextMeshProUGUI>();
        Image profileImage = orderTransform.Find("ProfileImage").GetComponent<Image>();

        titleText.text = orderSO.orderTitle;
        distanceText.text = orderSO.orderDistance.ToString();
        customerNameText.text = orderSO.customerSO.customerName;
        profileImage.sprite = orderSO.customerSO.customerProfile;

        Button orderButton = orderTransform.GetComponentInChildren<Button>();
        if (orderButton != null) {
            orderButton.onClick.AddListener(() => OnOrderButtonClicked(orderSO));
        }
    }

    private void OnOrderButtonClicked(OrderSO clickedOrder) {
        Debug.Log($"Clicked Order: {clickedOrder.orderTitle}");
    }
}