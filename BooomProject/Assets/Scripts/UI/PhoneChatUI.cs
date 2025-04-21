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
        // ������ʷ�Ի�
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
        // ��նԻ�
        for (int i = 0; i < itemsParent.childCount; i++)
        {
            Destroy(itemsParent.GetChild(i).gameObject);
        }
        CreateChat();
    }
    // ������Ϣ
    public void OnSubmit(string content)
    {
        DateTime NowTime = DateTime.Now.ToLocalTime();
        // ��ʱ���ʽ��
        string currentTime = NowTime.ToString("HH:mm");
        GameObject obj = Instantiate(ChatWindowManager.Instance.rightItem, itemsParent);
        print(content);
        ChatFragment newChat = new ChatFragment(currentTime, ChatWindowManager.Instance.ownerIcon, content, false);
        obj.GetComponent<ChatItemUI>().init(newChat);
      
        // ��ӵ���ʷ
        ChatWindowManager.Instance.history.Add(newChat);
        scrollRect.verticalNormalizedPosition = 0f;
    }

   // ��ϵ�˿Ͱ�ť�����¼�
   // ���Է���
    public void OnButtonClick()
    {
        // ����history
        // ������Ϣ
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
