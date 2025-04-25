using System.Collections.Generic;
using UnityEngine;

// 维护聊天界面数据
public class ChatWindowManager : Singleton<ChatWindowManager>
{
    [SerializeField] private OrderManager _orderManager;
    public GameObject leftItem;
    public GameObject rightItem;
    // 当前对象历史
    public List<ChatFragment> history = new List<ChatFragment>();

    public string customerName;
    public Sprite shopIcon;
    public Sprite customerIcon;
    public Sprite ownerIcon;

    private void Start()
    {
        _orderManager.OnChatWindowOpen += OrderManager_OnChatWindowOpen;
    }

    private void OrderManager_OnChatWindowOpen(OrderSO order)
    {
        history = order.chatHistory;
        customerName = order.customerSO.customerName;
        customerIcon = order.customerSO.customerProfile;
        // TODO: 商家头像信息获取
        EventHandlerManager.CallChatWindowShow();
    }

    protected override void init()
    {
        leftItem = Resources.Load<GameObject>(GameplaySettings.left_item_prefab_path);
        rightItem = Resources.Load<GameObject>(GameplaySettings.right_item_prefab_path);
        ownerIcon = Resources.Load<Sprite>(GameplaySettings.owner_icon_path);
    }
}
