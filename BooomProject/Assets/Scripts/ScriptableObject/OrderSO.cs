using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Order", menuName = "Order System/Order Info")]
public class OrderSO : ScriptableObject
{
    public CustomerSO customerSO;
    public string orderTitle;
    public float orderDistance;
}
