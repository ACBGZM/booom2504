using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ������Ϣ��������
/// </summary>
[CreateAssetMenu(fileName = "Order", menuName = "Order System/Order Info")]
public class OrderSO : ScriptableObject {
    [Tooltip("�����Ĺ˿���Ϣ")]
    public CustomerSO customerSO;
    [Tooltip("��������")]
    public string orderTitle;
    [Tooltip("�������Ӫ�ľ���")]
    public string orderDistance;
    [Tooltip("�����ȼ�")]
    public int range;
}