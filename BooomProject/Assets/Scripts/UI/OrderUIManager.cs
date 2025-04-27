using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OrderUIManager : Singleton<OrderUIManager> {
    public event Action<RuntimeOrderSO> OnChatWindowOpen;
    [SerializeField] private Transform _orderTemplatePrefab;
    [SerializeField] private Transform _myOrderTemplatePrefab;
    [SerializeField] private Transform _availableOrderContainer;
    [SerializeField] private Transform _acceptedOrderContainer;
    //[SerializeField] private OrderDataManager _orderDataManager;

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

        Debug.Log(Application.dataPath + "/UI/Phone/hstz.png");
    }
    // 刷新可用订单UI
    private void RefreshAvailableOrdersUI() {
        ClearContainer(_availableOrderContainer);
        List<RuntimeOrderSO> availableOrders = CommonGameplayManager.GetInstance().OrderDataManager.GetAvailableOrders();
        foreach (var order in availableOrders) {
            Transform orderItem = _orderPool.Get(_orderTemplatePrefab, _availableOrderContainer);
            SetupAvailableOrderUI(orderItem, order);
        }
        // Debug.Log($"已刷新 {availableOrders.Count} 个可用订单。");
    }

    // 刷新已接订单UI
    private void RefreshAcceptedOrdersUI() {
        ClearContainer(_acceptedOrderContainer);
        List<RuntimeOrderSO> acceptedOrders = CommonGameplayManager.GetInstance().OrderDataManager.GetAcceptedOrders();
        foreach (var order in acceptedOrders) {
            Transform orderItem = _orderPool.Get(_myOrderTemplatePrefab, _acceptedOrderContainer);
            SetupAcceptedOrderUI(orderItem, order);
        }
        // Debug.Log($"已刷新 {acceptedOrders.Count} 个已接订单。");
    }

    // 设置可用订单UI项
    private void SetupAvailableOrderUI(Transform item, RuntimeOrderSO order) {
        OrderUIItem ui = item.GetComponent<OrderUIItem>();
        if (ui == null) return;
        if (ui.profileImage != null) ui.profileImage.sprite = order.sourceOrder.customerSO.customerProfile;

        if (ui.limitTimeText != null) ui.limitTimeText.text = $"需在 {order.sourceOrder.initialLimitTime} 分钟内送达";
        if (ui.customerNameText != null) ui.customerNameText.text = order.sourceOrder.customerSO.customerName;
        if (ui.distanceText != null) ui.distanceText.text = order.currentDistance;
        if (ui.customerAddressText != null) ui.customerAddressText.text
            = CommonGameplayManager.GetInstance().NodeGraphManager.GetNodeByIDRuntime(order.sourceOrder.customerSO.destNodeId)._address;
        if (ui.rewardContainer != null && ui.rewardIconPrefab != null) {
            foreach (Transform child in ui.rewardContainer) {
                if (child == ui.rewardIconPrefab) continue;
                Destroy(child.gameObject);
            }

            /*for (int i = 0; i < order.sourceOrder.baseReward - 1; i++) {
                Instantiate(ui.rewardIconPrefab, ui.rewardContainer);
            }*/

            if (order.sourceOrder.baseReward >= 3 && order.sourceOrder.baseReward < 5)
            {
                Image orderImage = ui.rewardIconPrefab.GetComponent<Image>();
                orderImage.sprite = LoadFile.LoadImage(Application.dataPath + @"/Art/UI/Phone/lvstz.png", 101, 50);
                orderImage.rectTransform.sizeDelta = new Vector2(101, 50);
            }
            else if (order.sourceOrder.baseReward >= 5)
            {
                Image orderImage = ui.rewardIconPrefab.GetComponent<Image>();
                orderImage.sprite = LoadFile.LoadImage(Application.dataPath + @"/Art/UI/Phone/hstz.png", 202, 50);
                orderImage.rectTransform.sizeDelta = new Vector2(202, 50);
            }
        }

        Button btn = ui.mainButton;
        if (btn != null) {
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => {
                Debug.Log($"已接单: {order.sourceOrder.orderTitle}");
                CommonGameplayManager.GetInstance().OrderDataManager.AcceptOrder(order);
            });
            btn.interactable = CommonGameplayManager.GetInstance().OrderDataManager.CanAcceptMoreOrders();
        }
    }

    // 设置已接订单UI项
    private void SetupAcceptedOrderUI(Transform item, RuntimeOrderSO order) {
        OrderUIItem ui = item.GetComponent<OrderUIItem>();
        OrderSO sourceOrder = order.sourceOrder;

        ui.remainingTimeText.text = order.isTimeout ? "已超时"
            : $"剩余 {order.remainingMinutes} 分钟";
        //ui.remainingTimeText.color = order.isTimeout ? Color.red : (order.remainingMinutes < 30 ? Color.yellow : Color.green);
        if (ui.orderTitleText != null) ui.orderTitleText.text = order.sourceOrder.orderTitle;
        if (ui.orderAddressText != null) ui.orderAddressText.text = order.sourceOrder.destinationAddress;
        if (ui.customerAddressNameText != null)
            ui.customerAddressNameText.text
                = CommonGameplayManager.GetInstance().NodeGraphManager.GetNodeByIDRuntime(order.sourceOrder.customerSO.destNodeId)._address;
        if (ui.profileImage != null) ui.profileImage.sprite = order.sourceOrder.customerSO.customerProfile;
        if (ui.customerAddressText != null)
            ui.customerAddressText.text
                = CommonGameplayManager.GetInstance().NodeGraphManager.GetNodeByIDRuntime(order.sourceOrder.customerSO.destNodeId)._addressDetail;
        if (ui.bubbleText != null) ui.bubbleText.text = order.sourceOrder.bubble;

        if (ui.rewardContainer != null && ui.rewardIconPrefab != null) {
            foreach (Transform child in ui.rewardContainer) {
                if (child == ui.rewardIconPrefab) continue;
                Destroy(child.gameObject);
            }

            /*for (int i = 0; i < order.sourceOrder.baseReward - 1; i++) {
                Instantiate(ui.rewardIconPrefab, ui.rewardContainer);
            }*/

            if (order.sourceOrder.baseReward >= 3 && order.sourceOrder.baseReward < 5)
            {
                Image orderImage = ui.rewardIconPrefab.GetComponent<Image>();
                orderImage.sprite = LoadFile.LoadImage(Application.dataPath + @"/Art/UI/Phone/lvstz.png", 101, 50);
                orderImage.rectTransform.sizeDelta = new Vector2(101, 50);
            }
            else if (order.sourceOrder.baseReward >= 5)
            {
                Image orderImage = ui.rewardIconPrefab.GetComponent<Image>();
                orderImage.sprite = LoadFile.LoadImage(Application.dataPath + @"/Art/UI/Phone/hstz.png", 202, 50);
                orderImage.rectTransform.sizeDelta = new Vector2(202, 50);
            }
        }

        Button btn = ui.mainButton;
        if (btn != null) {
            // Debug.Log($"添加聊天事件: {order.sourceOrder.orderTitle}");
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => OnChatButtonClicked(order));
            btn.interactable = true;
        }
    }

    private void OnChatButtonClicked(RuntimeOrderSO runtimeOrder) {
        Debug.Log($"点击聊天: {runtimeOrder.sourceOrder.orderTitle}");
        OnChatWindowOpen?.Invoke(runtimeOrder);
    }

    private void ClearContainer(Transform container) {
        for (int i = container.childCount - 1; i >= 0; i--) {
            _orderPool.Return(container.GetChild(i));
        }
    }

    protected override void OnDestroy() {
        CommonGameplayManager.GetInstance().OrderDataManager.OnAvailableOrdersChanged -= RefreshAvailableOrdersUI;
        CommonGameplayManager.GetInstance().OrderDataManager.OnAcceptedOrdersChanged -= RefreshAcceptedOrdersUI;
        _orderPool.ClearPool();
    }
}
