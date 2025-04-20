using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OrderManager : MonoBehaviour {
    [SerializeField] private List<OrderSO> _orders;
    [SerializeField] private TaskList _taskList;

    private float _distance;// 订单距离，需单独计算

    // 生成订单
    public void OrderBuilder(OrderSO orderSO) {
        _taskList.ProductOrder(orderSO.orderTitle, orderSO.customerSO.customerName, _distance.ToString(), 
            orderSO.customerSO.customerAddress, orderSO.range, orderSO.customerSO.customerProfile);
    }
}