using DG.Tweening;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OrderUIManager : MonoBehaviour {
    public event Action<RuntimeOrderSO> OnChatWindowOpen;
    [SerializeField] private Transform _orderTemplatePrefab;
    [SerializeField] private Transform _myOrderTemplatePrefab;
    [SerializeField] private Transform _availableOrderContainer;
    [SerializeField] private Transform _acceptedOrderContainer;
    private OrderDataManager _orderDataManagerInstance;
    private TextMeshProUGUI _orderState;
    private OrderUIPool _orderPool = new OrderUIPool();

    private void Start() {
        _orderDataManagerInstance = CommonGameplayManager.GetInstance().OrderDataManager;
        // 订阅数据层事件
        if (_orderDataManagerInstance != null) {
            _orderDataManagerInstance.OnAvailableOrdersChanged += RefreshAvailableOrdersUI;
            _orderDataManagerInstance.OnAcceptedOrdersChanged += RefreshAcceptedOrdersUI;
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
        List<RuntimeOrderSO> availableOrders = _orderDataManagerInstance.GetAvailableOrders();
        foreach (var order in availableOrders) {
            Transform orderItem = _orderPool.Get(_orderTemplatePrefab, _availableOrderContainer);
            SetupAvailableOrderUI(orderItem, order);
        }
    }

    // 刷新已接订单UI
    private void RefreshAcceptedOrdersUI() {
        ClearContainer(_acceptedOrderContainer);
        List<RuntimeOrderSO> acceptedOrders = _orderDataManagerInstance.GetAcceptedOrders();
        foreach (var order in acceptedOrders) {
            Transform orderItem = _orderPool.Get(_myOrderTemplatePrefab, _acceptedOrderContainer);
            SetupAcceptedOrderUI(orderItem, order);
        }
    }

    // 设置可用订单UI项
    private void SetupAvailableOrderUI(Transform item, RuntimeOrderSO order) {
        OrderUIItem ui = item.GetComponent<OrderUIItem>();
        if (ui == null) return;
        if (ui.profileImage != null) ui.profileImage.sprite = order.sourceOrder.customerSO.customerProfile;
        if (ui.limitTimeText != null) ui.limitTimeText.text = $"需在 <size=+5><color=#5bb0ff>{order.sourceOrder.initialLimitTime}</color></size> 分钟内送达";
        if (ui.customerNameText != null) ui.customerNameText.text = order.sourceOrder.customerSO.customerName;
        if (ui.distanceText != null) ui.distanceText.text = order.currentDistance;

        if (ui.customerAddressText != null) ui.customerAddressText.text
            = CommonGameplayManager.GetInstance().NodeGraphManager.GetNodeByIDRuntime(order.sourceOrder.customerSO.destNodeId)._address;
        if (ui.rewardContainer != null && ui.rewardIconPrefab != null) {
            foreach (Transform child in ui.rewardContainer) {
                if (child == ui.rewardIconPrefab) continue;
                Destroy(child.gameObject);
            }

            for (int i = 0; i < order.sourceOrder.baseReward - 1; i++) {
                Instantiate(ui.rewardIconPrefab, ui.rewardContainer);
            }

            //if (order.sourceOrder.baseReward >= 3 && order.sourceOrder.baseReward < 5) {
            //    Image orderImage = ui.rewardIconPrefab.GetComponent<Image>();
            //    orderImage.sprite = LoadFile.LoadImage(Application.dataPath + @"/Art/UI/Phone/lvstz.png", 101, 35);
            //    orderImage.rectTransform.sizeDelta = new Vector2(101, 50);
            //} else if (order.sourceOrder.baseReward >= 5) {
            //    Image orderImage = ui.rewardIconPrefab.GetComponent<Image>();
            //    orderImage.sprite = LoadFile.LoadImage(Application.dataPath + @"/Art/UI/Phone/hstz.png", 202, 35);
            //    orderImage.rectTransform.sizeDelta = new Vector2(202, 50);
            //}
        }

        Button btn = ui.mainButton;
        if (btn != null) {
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => {
                Debug.Log($"已接单: {order.sourceOrder.orderTitle}");
                _orderDataManagerInstance.AcceptOrder(order);
            });
            btn.interactable = CommonGameplayManager.GetInstance().OrderDataManager.CanAcceptMoreOrders();
        }
    }

    // 设置已接订单UI项
    private void SetupAcceptedOrderUI(Transform item, RuntimeOrderSO order) {
        OrderUIItem ui = item.GetComponent<OrderUIItem>();
        OrderSO sourceOrder = order.sourceOrder;
        if (ui.remainingTimeText != null) ui.remainingTimeText.text = order.isTimeout ? "已超时" : $"剩余 <size=+5><color=#5bb0ff>{order.remainingMinutes.ToString()}</color></size> 分钟";
        if (ui.profileImage != null) ui.profileImage.sprite = order.sourceOrder.customerSO.customerProfile;
        if (ui.customerLandMarkText != null) ui.customerLandMarkText.text = order.sourceOrder.destinationAddress;
        if (ui.bubbleText != null) ui.bubbleText.text = order.sourceOrder.bubble;

        if (ui.customerAddressText != null)
            ui.customerAddressText.text
                = CommonGameplayManager.GetInstance().NodeGraphManager.GetNodeByIDRuntime(order.sourceOrder.customerSO.destNodeId)._addressDetail;
        if (ui.rewardContainer != null && ui.rewardIconPrefab != null) {
            foreach (Transform child in ui.rewardContainer) {
                if (child == ui.rewardIconPrefab) continue;
                Destroy(child.gameObject);
            }

            for (int i = 0; i < order.sourceOrder.baseReward - 1; i++) {
                Instantiate(ui.rewardIconPrefab, ui.rewardContainer);
            }

            //if (order.sourceOrder.baseReward >= 3 && order.sourceOrder.baseReward < 5) {
            //    Image orderImage = ui.rewardIconPrefab.GetComponent<Image>();
            //    orderImage.sprite = LoadFile.LoadImage(Application.dataPath + @"/Art/UI/Phone/lvstz.png", 101, 35);
            //    orderImage.rectTransform.sizeDelta = new Vector2(101, 50);
            //} else if (order.sourceOrder.baseReward >= 5) {
            //    Image orderImage = ui.rewardIconPrefab.GetComponent<Image>();
            //    orderImage.sprite = LoadFile.LoadImage(Application.dataPath + @"/Art/UI/Phone/hstz.png", 202, 35);
            //    orderImage.rectTransform.sizeDelta = new Vector2(202, 50);
            //}
        }

        if (order.currentState == OrderState.InTransit) {
            _orderState = ui.orderState;
            _orderState.text = "已取餐";
        } else if (order.currentState == OrderState.Accepted) {
            _orderState = ui.orderState;
            _orderState.text = "未取餐";
        }

        Button btn = ui.mainButton;
        if (btn != null) {
            // 判断是否在大本营节点接单，若是，则改为已取餐
            if (CommonGameplayManager.GetInstance().NodeGraphManager.CurrentNode.NodeID == CommonGameplayManager.GetInstance().NodeGraphManager.BaseNodeID) {
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => OnChatButtonClicked(order));
            }
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

    private void OnDisable() {
        if (_orderDataManagerInstance != null) {
            _orderDataManagerInstance.OnAvailableOrdersChanged -= RefreshAvailableOrdersUI;
            _orderDataManagerInstance.OnAcceptedOrdersChanged -= RefreshAcceptedOrdersUI;
        }
        _orderPool.ClearPool();
    }
}
