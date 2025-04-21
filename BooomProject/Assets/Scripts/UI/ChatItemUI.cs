using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatItemUI : MonoBehaviour
{
    public TextMeshProUGUI timeText;
    public Image profileImage;
    public TextMeshProUGUI contentText;
    public bool isLeft;
    public string chatTitle;

    public void init(ChatFragment chat)
    {
        profileImage.sprite = chat.icon;
        contentText.text = chat.content;
        timeText.text = chat.time;
        isLeft = chat.left;
        chatTitle = chat.chatTitle;
    }
}
