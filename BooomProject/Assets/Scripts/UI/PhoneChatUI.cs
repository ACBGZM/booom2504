using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class PhoneChatUI : MonoBehaviour
{
    [SerializeField] private Transform buttomButton;
    //public event EventHandler OnChatWindowShow;
    public Button backBtn;
    public Text title;
    public TMP_InputField inputField;
    public Transform itemsParent;
    public ScrollRect scrollRect;
    private void Awake()
    {
        backBtn.onClick.AddListener(() =>
        {
            HideMe();
        });
        inputField.onSubmit.AddListener((message) =>
        {
            OnSubmit(message);
            inputField.text = string.Empty;
        });
    }
    public void CreateChat()
    {
        // 创建历史对话
        foreach (ChatFragment fragment in ChatWindowManager.Instance.history)
        {
            GameObject obj;
            if (fragment.left) obj = Instantiate(ChatWindowManager.Instance.leftItem, itemsParent);
            else obj = Instantiate(ChatWindowManager.Instance.rightItem, itemsParent);
            obj.GetComponent<ChatItemUI>().init(fragment);
           
        }
    }

    public void UpdateContent()
    {
        // 清空对话
        for (int i = 0; i < itemsParent.childCount; i++)
        {
            Destroy(itemsParent.GetChild(i).gameObject);
        }
        CreateChat();
    }
    // 发送信息
    public void OnSubmit(string content)
    {
        DateTime NowTime = DateTime.Now.ToLocalTime();
        // 将时间格式化
        string currentTime = NowTime.ToString("HH:mm");
        GameObject obj = Instantiate(ChatWindowManager.Instance.rightItem, itemsParent);
        print(content);
        ChatFragment newChat = new ChatFragment(currentTime, ChatWindowManager.Instance.ownerIcon, content, false);
        obj.GetComponent<ChatItemUI>().init(newChat);
      
        // 添加到历史
        ChatWindowManager.Instance.history.Add(newChat);
        scrollRect.verticalNormalizedPosition = 0f;
    }

   // 联系顾客按钮监听事件
   // 测试方法
    public void OnButtonClick()
    {
        // 传入history
        // 传入信息
        ShowMe();
        UpdateContent();
       
    }

    public void ShowMe()
    {
        gameObject.SetActive(true);
        UpdateContent();
    }

    public void HideMe()
    {
        gameObject.SetActive(false);
    }
}
