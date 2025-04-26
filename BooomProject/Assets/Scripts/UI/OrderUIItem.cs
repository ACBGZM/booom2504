using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OrderUIItem : MonoBehaviour {
    [Header("通用")]
    public Image profileImage;
    public Button mainButton; // 接单或聊天
    [Header("可用订单")]
    public TextMeshProUGUI limitTimeText;
    public TextMeshProUGUI customerNameText;
    public TextMeshProUGUI distanceText;
    public TextMeshProUGUI orderAddressText;
    // public TextMeshProUGUI rangeText;
    [Header("已接取订单")]
    public TextMeshProUGUI remainingTimeText;
    public TextMeshProUGUI orderTitleText;
    public TextMeshProUGUI customerAddressNameText;
    public TextMeshProUGUI customerAddressText;
    public TextMeshProUGUI bubbleText;
    public Transform rewardContainer;
    public Image rewardIconPrefab;

    // public Button acceptButton;
    // public Button chatButton;
}
