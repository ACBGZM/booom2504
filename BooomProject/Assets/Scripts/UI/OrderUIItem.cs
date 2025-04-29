using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OrderUIItem : MonoBehaviour {
    [Header("通用")]
    public Image profileImage;
    public Button mainButton; // 接单或聊天
    public Transform rewardContainer;
    public Transform rewardIconPrefab;
    public TextMeshProUGUI customerAddressText;
    public TextMeshProUGUI orderState;
    [Header("可用订单")]
    public TextMeshProUGUI limitTimeText;
    public TextMeshProUGUI customerNameText;
    public TextMeshProUGUI distanceText;
    [Header("已接取订单")]
    public TextMeshProUGUI remainingTimeText;
    public TextMeshProUGUI customerLandMarkText;
    public TextMeshProUGUI bubbleText;
}
