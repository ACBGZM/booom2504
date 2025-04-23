using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;



// ά�������������
public class ChatWindowManager : Singleton<ChatWindowManager>
{
    [SerializeField] private OrderManager _orderManager;
    public GameObject leftItem;
    public GameObject rightItem;
    // ��ǰ������ʷ
    public List<ChatFragment> history = new List<ChatFragment>();
    public string customerName;
    public Sprite shopIcon;
    public Sprite customerIcon;
    public Sprite ownerIcon;

    private void Start() {
        _orderManager.OnChatWindowOpen += OrderManager_OnChatWindowOpen;
    }

    private void OrderManager_OnChatWindowOpen(OrderSO order) {
        Debug.Log(order.customerSO.customerName);
    }

    protected override void init()
    {
        leftItem = Resources.Load<GameObject>(GameplaySettings.left_item_prefab_path);
        rightItem = Resources.Load<GameObject>(GameplaySettings.right_item_prefab_path);
    }

}
