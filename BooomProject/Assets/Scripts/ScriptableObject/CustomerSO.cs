using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// 顾客信息数据容器
/// </summary>
[CreateAssetMenu(fileName = "Customer", menuName = "Order System/Customer Info")]
public class CustomerSO : ScriptableObject
{
    [Tooltip("顾客头像图片")]
    public Sprite customerProfile;

    [Tooltip("顾客姓名")]
    public string customerName;
}
