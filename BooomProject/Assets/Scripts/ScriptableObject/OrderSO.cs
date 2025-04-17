using System.Collections.Generic;
using UnityEngine;

// �����ı���Ϣ
[CreateAssetMenu(fileName = "Order", menuName = "Order System/Order Info")]
public class OrderSO : ScriptableObject
{
    public CustomerSO customerSO;
    public string orderTitle;
    public string orderDistance;
    public string Address;
}
