using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatItemUI : MonoBehaviour
{
    public Text time;
    public Image icon;
    public Text content;
    public bool left;
    
    public void init(ChatFragment chat)
    {
        this.icon.sprite = chat.icon;
        this.content.text = chat.content;
        this.time.text = chat.time;
        this.left = chat.left;   
    }
}
