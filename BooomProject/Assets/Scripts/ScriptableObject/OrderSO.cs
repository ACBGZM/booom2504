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
    [Tooltip("������ַ")]
    public string orderAddress;
    // [Tooltip("�������Ӫ�ľ���")]
    [Tooltip("���������ֵ�ǰλ�õľ���")]
    public string orderDistance;
    [Tooltip("�����ȼ�")]
    public int range;
    [Tooltip("��������")]
    public int reward;
    [Tooltip("������Ϣ")]
    public string bubble;
    [Tooltip("Ԥ���ʹ�ʱ��")]
    public float time;
}