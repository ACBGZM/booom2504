using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// 维护聊天界面数据
public class ChatWindowManager : Singleton<ChatWindowManager> {
    [SerializeField] private PlayerDataManager _playerDataManager;
    [SerializeField] private Transform _chatUITransform;
    [SerializeField] private OrderUIManager _orderUIManager;
    [SerializeField] private Transform _tabArea;
    [SerializeField] private TextMeshProUGUI _chatTitleText;
    [SerializeField] private Transform _replyButtonContainer;
    [SerializeField] private Button _defaultReplyButton;
    [SerializeField] private Transform _itemsParent;
    [SerializeField] private ScrollRect _scrollRect;
    [SerializeField] private Button _returnButton;

    // 当前聊天记录
    private List<ChatFragment> _history = new List<ChatFragment>();
    private RuntimeOrderSO _currentOrder;
    private GameObject _leftItem;
    private GameObject _rightItem;
    private Sprite _customerProfile;
    private Sprite _myProfile;

    private void Start() {
        _chatUITransform.gameObject.SetActive(false);
        _orderUIManager.OnChatWindowOpen += OrderDataManager_OnChatWindowOpen;
        _returnButton.onClick.AddListener(Close);
    }

    // 添加聊天标题属性
    protected override void init() {
        _leftItem = Resources.Load<GameObject>(GameplaySettings.left_item_prefab_path);
        _rightItem = Resources.Load<GameObject>(GameplaySettings.right_item_prefab_path);
        _myProfile = Resources.Load<Sprite>(GameplaySettings.player_profile_path);
    }

    // 调用聊天窗口打开的事件
    public void OrderDataManager_OnChatWindowOpen(RuntimeOrderSO order) {
        _currentOrder = order;
        _tabArea.gameObject.SetActive(false);
        // 设置初始回复
        if (_currentOrder.sourceOrder.quickResponses != null && _currentOrder.sourceOrder.quickResponses.Count > 0) {
            _replyButtonContainer.gameObject.SetActive(true);
        } else {
            _replyButtonContainer.gameObject.SetActive(false);
        }
        _chatTitleText.text = $"（店家、{order.sourceOrder.customerSO.customerName}、我)";
        _history = order.sourceOrder.chatHistory;
        _customerProfile = order.sourceOrder.customerSO.customerProfile;
        UpdateContent();
        SetupReplyButton();
    }

    public void CreateChat() {
        _chatUITransform.gameObject.SetActive(true);
        foreach (ChatFragment fragment in _history) {
            GameObject obj;
            if (fragment.left) obj = Instantiate(_leftItem, _itemsParent);
            else obj = Instantiate(_rightItem, _itemsParent);
            obj.GetComponent<ChatItemUI>().Init(fragment);
        }
    }

    /// <summary>
    /// 更新回复按钮文本和点击事件的监听
    /// </summary>
    public void SetupReplyButton() {
        if (_replyButtonContainer == null || _defaultReplyButton == null) return;
        if (_currentOrder == null ||
            _currentOrder.sourceOrder == null ||
            _currentOrder.sourceOrder.quickResponses == null) {
            _replyButtonContainer.gameObject.SetActive(false);
            return;
        }

        List<QuickResponse> currentReply = _currentOrder.sourceOrder.quickResponses;
        _replyButtonContainer.gameObject.SetActive(currentReply.Count > 0);

        Button[] existingButtons = _replyButtonContainer.GetComponentsInChildren<Button>(true);

        // 激活/创建足够数量的按钮
        for (int i = 0; i < currentReply.Count; i++) {
            Button button;
            if (i < existingButtons.Length) {
                button = existingButtons[i];
                button.gameObject.SetActive(true);
            } else {
                button = Instantiate(_defaultReplyButton, _replyButtonContainer);
                //button.gameObject.AddComponent<ReplyButtonAnim>(); // 动画组件
            }
            SetupSingleButton(button, currentReply[i], i);
        }

        // 隐藏多余按钮
        for (int i = currentReply.Count; i < existingButtons.Length; i++) {
            existingButtons[i].gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 设置单个按钮的属性和事件
    /// </summary>
    private void SetupSingleButton(Button button, QuickResponse response, int index) {
        // 设置按钮文本
        TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
        if (buttonText != null) {
            buttonText.text = response.buttonText;
        } else {
            Debug.LogWarning("按钮缺少TextMeshPro组件");
        }
        // 移除旧监听
        button.onClick.RemoveAllListeners();
        int localIndex = index;
        QuickResponse localResponse = response;
        // 添加新监听
        button.onClick.AddListener(() => HandleReplyClick(localResponse, localIndex, response.isGoodReputation));
    }

    /// <summary>
    /// 处理回复按钮点击事件
    /// </summary>
    private void HandleReplyClick(QuickResponse response, int index, bool isGood) {
        if (_currentOrder == null || response == null) return;
        // 生成聊天记录
        string currentTime = CommonGameplayManager.GetInstance().TimeManager.currentTime.GetHourAndMinute();
        ChatFragment newChat = new ChatFragment(
            currentTime,
            _myProfile,
            response.responseText,
            false
        );
        // 更新数据
        _history.Add(newChat);
        if (response.cannotReatedly) {
            _currentOrder.sourceOrder.quickResponses.Clear();
            StartCoroutine(RefreshButtonsAfterFrame()); // 延迟刷新
        }
        // 更新UI
        UpdateContent();
        _scrollRect.normalizedPosition = Vector2.zero;
        // --------------------------------TODO 用户差评------------------------------
        if (!string.IsNullOrEmpty(response.customerResponseText)) {
            StartCoroutine(ShowCustomerReply(response.customerResponseText, 1.5f));
        }
    }

    // 客户延迟回复
    private IEnumerator ShowCustomerReply(string customerText, float delay) {
        yield return new WaitForSeconds(delay);
        Sprite customerProfile = _currentOrder.sourceOrder.customerSO.customerProfile;
        // 创建客户回复片段
        ChatFragment customerChat = new ChatFragment(
            CommonGameplayManager.GetInstance().TimeManager.currentTime.GetHourAndMinute(),
            customerProfile,
            customerText,
            true // 左侧显示
        );
        _history.Add(customerChat);
        UpdateContent();
        _scrollRect.normalizedPosition = Vector2.zero;
    }

    /// <summary>
    /// 延迟一帧刷新
    /// </summary>
    private IEnumerator RefreshButtonsAfterFrame() {
        yield return null;
        SetupReplyButton();
    }

    /// <summary>
    /// 刷新聊天内容，先销毁当前所有显示的聊天项，然后重新生成。
    /// </summary>
    public void UpdateContent() {
        // 清空对话内容
        for (int i = 0; i < _itemsParent.childCount; i++) {
            Destroy(_itemsParent.GetChild(i).gameObject);
        }
        StartCoroutine(ShowCustomerReply(1.5f));
        CreateChat();
    }
    private IEnumerator ShowCustomerReply(float delay) { yield return new WaitForSeconds(delay); }

    public void Close() {
        _tabArea.gameObject.SetActive(true);
    }
}
