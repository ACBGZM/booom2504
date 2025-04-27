using System.Collections.Generic;
using UnityEngine;

// 维护聊天界面数据
public class ChatWindowManager : Singleton<ChatWindowManager>
{
    private OrderDataManager _orderDataManager;
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
        _orderDataManager = CommonGameplayManager.GetInstance().OrderDataManager;
        _orderDataManager.OnChatWindowOpen += OrderDataManagerOnChatWindowOpen;
    }

    private void OrderDataManagerOnChatWindowOpen(RuntimeOrderSO order)
    {
        history = order.sourceOrder.chatHistory;
        customerName = order.sourceOrder.customerSO.customerName;
        customerIcon = order.sourceOrder.customerSO.customerProfile;
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
