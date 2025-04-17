using UnityEngine;

// 订单顾客信息
[CreateAssetMenu(fileName = "Customer", menuName = "Order System/Customer Info")]
public class CustomerSO : ScriptableObject {
    public Sprite customerProfile;
    public string customerName;
}