using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OrderUIManager : Singleton<OrderUIManager> {
    public event Action<OrderSO> OnChatWindowOpen;
    [SerializeField] private Transform _orderTemplatePrefab;
    [SerializeField] private Transform _myOrderTemplatePrefab;
    [SerializeField] private Transform _availableOrderContainer;
    [SerializeField] private Transform _acceptedOrderContainer;

    private OrderUIPool _orderPool = new OrderUIPool();

    private void Start() {
        // 订阅数据层事件
        if (CommonGameplayManager.GetInstance().OrderDataManager != null) {
            CommonGameplayManager.GetInstance().OrderDataManager.OnAvailableOrdersChanged += RefreshAvailableOrdersUI;
            CommonGameplayManager.GetInstance().OrderDataManager.OnAcceptedOrdersChanged += RefreshAcceptedOrdersUI;
            // 初始加载
            RefreshAvailableOrdersUI();
            RefreshAcceptedOrdersUI();
        } else {
            Debug.LogError("未找到 OrderDataManager 实例");
        }
    }
    // 刷新可用订单UI
    private void RefreshAvailableOrdersUI() {
        ClearContainer(_availableOrderContainer);
        List<OrderSO> availableOrders = CommonGameplayManager.GetInstance().OrderDataManager.GetAvailableOrders();
        foreach (var order in availableOrders) {
            Transform orderItem = _orderPool.Get(_orderTemplatePrefab, _availableOrderContainer);
            SetupAvailableOrderUI(orderItem, order);
        }
        Debug.Log($"已刷新 {availableOrders.Count} 个可用订单。");
    }

    // 刷新已接订单UI
    private void RefreshAcceptedOrdersUI() {
        ClearContainer(_acceptedOrderContainer);
        List<OrderSO> acceptedOrders = CommonGameplayManager.GetInstance().OrderDataManager.GetAcceptedOrders();
        foreach (var order in acceptedOrders) {
            Transform orderItem = _orderPool.Get(_myOrderTemplatePrefab, _acceptedOrderContainer);
            SetupAcceptedOrderUI(orderItem, order);
        }
        Debug.Log($"已刷新 {acceptedOrders.Count} 个已接订单。");
    }

    // 设置可用订单UI项
    private void SetupAvailableOrderUI(Transform item, OrderSO order) {
        OrderUIItem ui = item.GetComponent<OrderUIItem>();
        if (ui == null) return;
        if (ui.profileImage != null) ui.profileImage.sprite = order.customerSO.customerProfile;

        if (ui.limitTimeText != null) ui.limitTimeText.text = $"需在 {order.orderLimitTime} 分钟内送达";
        if (ui.customerNameText != null) ui.customerNameText.text = order.customerSO.customerName;
        if (ui.distanceText != null) ui.distanceText.text = $"{order.orderDistance:F1}km";
        if (ui.customerAddressText != null) ui.customerAddressText.text
            = CommonGameplayManager.GetInstance().NodeGraphManager.GetNodeByIDRuntime(order.customerSO.destNodeId)._address;
        // if (ui.rangeText != null) ui.rangeText.text = $"Range: {order.range}";

        Button btn = ui.mainButton;
        if (btn != null) {
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => {
                Debug.Log($"已接单: {order.orderTitle}");
                CommonGameplayManager.GetInstance().OrderDataManager.AcceptOrder(order);
            });
            btn.interactable = CommonGameplayManager.GetInstance().OrderDataManager.CanAcceptMoreOrders();
        }
    }

    // 设置已接订单UI项
    private void SetupAcceptedOrderUI(Transform item, OrderSO order) {
        OrderUIItem ui = item.GetComponent<OrderUIItem>();
        if (ui == null) return;
        if (ui.profileImage != null) ui.profileImage.sprite = order.customerSO.customerProfile;

        if (ui.remainingTimeText != null) {
            ui.remainingTimeText.text = order.isTimeout ? "已超时" : $"剩余 {order.remainingMinutes} 分钟";
            //ui.remainingTimeText.color = order.isTimeout ? Color.red : (order.remainingMinutes < 30 ? Color.yellow : Color.green);
        }
        if (ui.orderTitleText != null) ui.orderTitleText.text = order.orderTitle;
        if (ui.orderAddressText != null) ui.orderAddressText.text = order.orderAddress;
        if (ui.customerAddressNameText != null)
            ui.customerAddressNameText.text
                = CommonGameplayManager.GetInstance().NodeGraphManager.GetNodeByIDRuntime( order.customerSO.destNodeId)._address;
        if (ui.customerAddressText != null)
            ui.customerAddressText.text
                = CommonGameplayManager.GetInstance().NodeGraphManager.GetNodeByIDRuntime( order.customerSO.destNodeId)._addressDetail;
        if (ui.bubbleText != null) ui.bubbleText.text = order.bubble;

        if (ui.rewardContainer != null && ui.rewardIconPrefab != null) {
            foreach (Transform child in ui.rewardContainer) {
                Destroy(child.gameObject);
            }
            for (int i = 0; i < order.reward; i++) {
                Instantiate(ui.rewardIconPrefab, ui.rewardContainer);
            }
        }

        Button btn = ui.mainButton;
        if (btn != null) {
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => OnChatButtonClicked(order));
            btn.interactable = true;
        }
    }

    private void OnChatButtonClicked(OrderSO order) {
        Debug.Log($"点击聊天: {order.orderTitle}");
        OnChatWindowOpen?.Invoke(order);
    }

    private void ClearContainer(Transform container) {
        for (int i = container.childCount - 1; i >= 0; i--) {
            _orderPool.Return(container.GetChild(i));
        }
    }

    protected override void OnDestroy() {
        if (CommonGameplayManager.GetInstance().OrderDataManager != null) {
            CommonGameplayManager.GetInstance().OrderDataManager.OnAvailableOrdersChanged -= RefreshAvailableOrdersUI;
            CommonGameplayManager.GetInstance().OrderDataManager.OnAcceptedOrdersChanged -= RefreshAcceptedOrdersUI;
        }
        _orderPool.ClearPool();
    }
}
