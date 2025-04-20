using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ChatFragment {
    public string chatTitle;
    public string time;
    public Sprite icon;
    public string content;
    public bool left;

    public ChatFragment(string time, Sprite icon, string content, bool left, string chatTitle) {
        this.time = time;
        this.icon = icon;
        this.content = content;
        this.left = left;
        this.chatTitle = chatTitle;
    }
}

