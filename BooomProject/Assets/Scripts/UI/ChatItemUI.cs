using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatItemUI : MonoBehaviour
{

    [SerializeField] private Image _bubbleBackground;
    [SerializeField] private Color _customerBubbleColor;
    [SerializeField] private TextMeshProUGUI _timeText;
    [SerializeField] private TextMeshProUGUI _contentText;
    [SerializeField] private Image _profileImage;
    public void Init(ChatFragment chat) {
        // 设置头像
        _profileImage = chat.profile;
        // 设置气泡颜色
        _bubbleBackground.color = _customerBubbleColor;
        // 设置内容
        _contentText.text = chat.content;
        _timeText.text = chat.time;
    }
}
