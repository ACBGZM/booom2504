using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class PhoneChatUI : MonoBehaviour {
    [SerializeField] private Transform buttomButton;
    [SerializeField] private TextMeshProUGUI chatTitle;
    [SerializeField] private Image phoneBackGroundImage;// phoneBackGround 组件
    [SerializeField] private Sprite chatSprtie; // 聊天界面UI图
    [SerializeField] private Sprite orderSprite; // 订单界面UI图
    [SerializeField] private GameObject tabArea; // tab组件
    [SerializeField] private GameObject myOrderList; // 我的订单组件
    [SerializeField] private GameObject chatButton; // 聊天界面的按钮


    public event EventHandler OnChatWindowShow;
    public Button backBtn;
    public Text title;
    public TMP_InputField inputField;
    public Transform itemsParent;
    public ScrollRect scrollRect;
    private void Awake() {
        backBtn.onClick.AddListener(() => {
            HideMe();
        });
        inputField.onSubmit.AddListener((message) => {
            OnSubmit(message);
            inputField.text = string.Empty;
        });
    }
    public void CreateChat() {
        // 创建历史对话
        foreach (ChatFragment fragment in ChatWindowManager.Instance.history) {
            GameObject obj;
            if (fragment.left) obj = Instantiate(ChatWindowManager.Instance.leftItem, itemsParent);
            else obj = Instantiate(ChatWindowManager.Instance.rightItem, itemsParent);
            obj.GetComponent<ChatItemUI>().init(fragment);
            chatTitle.text = obj.GetComponent<ChatItemUI>().chatTitle;
        }
    }

    public void UpdateContent() {
        // 清空对话
        for (int i = 0; i < itemsParent.childCount; i++) {
            Destroy(itemsParent.GetChild(i).gameObject);
        }
        CreateChat();
    }
    // 发送信息
    public void OnSubmit(string content) {
        DateTime NowTime = DateTime.Now.ToLocalTime();
        // 将时间格式化
        string currentTime = NowTime.ToString("HH:mm");
        GameObject obj = Instantiate(ChatWindowManager.Instance.rightItem, itemsParent);
        print(content);
        ChatFragment newChat = new ChatFragment(currentTime, ChatWindowManager.Instance.ownerIcon, content, false, "Test");
        obj.GetComponent<ChatItemUI>().init(newChat);
      
        // 添加到历史
        ChatWindowManager.Instance.history.Add(newChat);
        scrollRect.verticalNormalizedPosition = 0f;
    }

    // 联系顾客按钮监听事件
    // 测试方法
    public void OnButtonClick() {
        // 传入history
        // 传入信息
        ShowMe();
        UpdateContent();

    }

    public void ShowMe() {
        // 新增
        phoneBackGroundImage.sprite = chatSprtie;
        tabArea.gameObject.SetActive(false);
        myOrderList.gameObject.SetActive(false);
        chatButton.SetActive(true);

        gameObject.SetActive(true);
        UpdateContent();
    }

    public void HideMe() {
        // 新增
        phoneBackGroundImage.sprite = orderSprite;
        chatButton.SetActive(false);
        tabArea.gameObject.SetActive(true);
        myOrderList.gameObject.SetActive(true);

        gameObject.SetActive(false);
    }
}
