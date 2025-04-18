using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ChatWindowManager : Singleton<ChatWindowManager>
{
    public GameObject leftItem;
    public GameObject rightItem;
    public List<ChatFragment> history = new List<ChatFragment>();
    public string customerName;
    public Sprite shopIcon;
    public Sprite customerIcon;
    public Sprite ownerIcon;
    public Transform viewPort;
    protected override void init()
    {
        leftItem = Resources.Load<GameObject>(GameplaySettings.left_item_prefab_path);
        rightItem = Resources.Load<GameObject>(GameplaySettings.right_item_prefab_path);
    }
    public void CreateChat()
    {
        // 创建历史对话
        foreach (ChatFragment fragment in history)
        {
            GameObject obj;
            if (fragment.left) obj = Instantiate(leftItem, viewPort);
            else obj = Instantiate(rightItem, viewPort);
            obj.GetComponent<ChatItemUI>().init(fragment);
        }
    }

    public void UpdateContent()
    {
        // 清空对话
        for(int i = 0; i < viewPort.childCount; i++)
        {
            Destroy(viewPort.GetChild(i).gameObject);
        }
        CreateChat();
    }
    // 发送信息
    public void OnSubmit(string content)
    {
        DateTime NowTime = DateTime.Now.ToLocalTime();
        // 将时间格式化
        string currentTime = NowTime.ToString("yyyy-MM-dd HH:mm");
        GameObject obj = Instantiate(rightItem, viewPort);
        print(content);
        ChatFragment newChat = new ChatFragment(currentTime, ownerIcon, content, false);
        obj.GetComponent<ChatItemUI>().init(newChat);
        // 添加到历史
        history.Add(newChat);
    }

    // 联系顾客按钮监听事件
    //public void OnButtonClick()
    //{
    //    // 传入history
    //    // 传入信息
    //    UpdateContent();
    //    // show
    //}
     
}
