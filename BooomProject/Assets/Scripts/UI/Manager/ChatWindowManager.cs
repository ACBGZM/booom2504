using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 维护聊天界面数据
public class ChatWindowManager : Singleton<ChatWindowManager>
{
    [SerializeField] private Transform _chatUITransform;
    [SerializeField] private OrderUIManager _orderUIManager;
    [SerializeField] private Transform _tabArea;
    [SerializeField] private TextMeshProUGUI _chatTitleText;
    [SerializeField] private GameObject _replyButton;
    [SerializeField] private Transform _itemsParent;
    [SerializeField] private ScrollRect _scrollRect;
    [SerializeField] private Button _returnButton;

    // 当前聊天记录
    private List<ChatFragment> _history = new List<ChatFragment>();
    private RuntimeOrderSO _currentOrder;
    private GameObject _leftItem;
    private GameObject _rightItem;
    //private Sprite _customerProfile;
    private Image _myProfile;

    // 当前回复下标
    private int _currentReplyIndex = -1;

    private void Start()
    {
        _chatUITransform.gameObject.SetActive(false);
        _orderUIManager.OnChatWindowOpen += OrderDataManager_OnChatWindowOpen;
        _returnButton.onClick.AddListener(Close);
    }

    // 添加聊天标题属性
    protected override void init()
    {
        _leftItem = Resources.Load<GameObject>(GameplaySettings.left_item_prefab_path);
        _rightItem = Resources.Load<GameObject>(GameplaySettings.right_item_prefab_path);
        _myProfile = Resources.Load<Image>(GameplaySettings.owner_icon_path);
    }

    // 调用聊天窗口打开的事件
    public void OrderDataManager_OnChatWindowOpen(RuntimeOrderSO order) {
        _currentOrder = order;
        _tabArea.gameObject.SetActive(false);
        // 初始快速回复设置
        if (_currentOrder.sourceOrder.quickResponses != null && _currentOrder.sourceOrder.quickResponses.Count > 0) {
            _currentReplyIndex = 0;
            _replyButton.SetActive(true);
        } else {
            _replyButton.SetActive(false);
        }
        _chatTitleText.text = $"（店家、{order.sourceOrder.customerSO.customerName}、我";
        _history = order.sourceOrder.chatHistory;
        UpdateContent();
        SetupReplyButton();
    }

    public void CreateChat() {
        _chatUITransform.gameObject.SetActive(true);
        foreach (ChatFragment fragment in _history) {
            GameObject obj;
            if (fragment.left)
                obj = Instantiate(_leftItem, _itemsParent);
            else
                obj = Instantiate(_rightItem, _itemsParent);
            obj.GetComponent<ChatItemUI>().Init(fragment);
        }
    }

    /// <summary>
    /// 更新回复按钮文本和点击事件的监听
    /// </summary>
    public void SetupReplyButton() {
        // 如果没有订单数据，或者快速回复列表为空，则关闭回复按钮
        if (_currentOrder == null || _currentOrder.sourceOrder.quickResponses.Count == 0) {
            _replyButton.SetActive(false);
            return;
        }
        // 显示当前使用的快速回复文本
        QuickResponse currentReply = _currentOrder.sourceOrder.quickResponses[_currentReplyIndex];
        _replyButton.GetComponentInChildren<TextMeshProUGUI>().text = currentReply.buttonText;

        Button replyBtn = _replyButton.GetComponent<Button>();
        replyBtn.onClick.RemoveAllListeners();
        replyBtn.onClick.AddListener(() => {
            GameTime now = TimeManager.Instance.currentTime;
            string currentTime = now.ToString().Split(' ')[3];
            // 生成一个新的聊天气泡项
            GameObject obj = Instantiate(_rightItem, _itemsParent);
            ChatFragment newChat = new ChatFragment(
                currentTime,
                _myProfile,
                currentReply.responseText,
                false,
                _currentOrder.sourceOrder.customerSO.customerName
            );
            // 添加到全局聊天历史中
            _history.Add(newChat);
            UpdateContent();
            // 滚动到最新聊天位置
            _scrollRect.verticalNormalizedPosition = 1f;
            Debug.Log("发送信息: " + currentReply.responseText);
            // 如果快速回复使用后被消耗
            if (currentReply.cannotReatedly) {
                _currentOrder.sourceOrder.quickResponses.RemoveAt(_currentReplyIndex);
                if (_currentOrder.sourceOrder.quickResponses.Count == 0) {
                    // 重置下标并禁用按钮
                    _currentReplyIndex = -1;
                    _replyButton.SetActive(false);
                } else {
                    if (_currentReplyIndex >= _currentOrder.sourceOrder.quickResponses.Count) {
                        _currentReplyIndex = 0;
                    }
                    // 更新按钮显示
                    SetupReplyButton();
                }
            }
        });
    }

    /// <summary>
    /// 刷新聊天内容，先销毁当前所有显示的聊天项，然后重新生成。
    /// </summary>
    public void UpdateContent() {
        // 清空对话内容
        for (int i = 0; i < _itemsParent.childCount; i++) {
            Destroy(_itemsParent.GetChild(i).gameObject);
        }
        CreateChat();
    }

    public void Close() {
        _tabArea.gameObject.SetActive(true);
    }
}
