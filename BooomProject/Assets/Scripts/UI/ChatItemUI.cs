using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatItemUI : MonoBehaviour
{
    public TextMeshProUGUI timeText;
    public Image icon;
    public TextMeshProUGUI contentText;
    public bool isLeft;
    
    public void init(ChatFragment chat)
    {
        this.icon.sprite = chat.icon;
        contentText.text = chat.content;
        timeText.text = chat.time;
        this.isLeft = chat.left;   
    }
}
