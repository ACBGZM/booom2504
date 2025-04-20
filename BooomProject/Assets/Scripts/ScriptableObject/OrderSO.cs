using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 订单信息数据容器
/// </summary>
[CreateAssetMenu(fileName = "Order", menuName = "Order System/Order Info")]
public class OrderSO : ScriptableObject {
    [Tooltip("关联的顾客信息")]
    public CustomerSO customerSO;
    [Tooltip("订单标题")]
    public string orderTitle;
    [Tooltip("订单与大本营的距离")]
    public string orderDistance;
    [Tooltip("订单等级")]
    public int range;
}