using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OrderManager : MonoBehaviour {
    [SerializeField] private List<OrderSO> _orders;
    [SerializeField] private TaskList _taskList;

    private float _distance;// �������룬�赥������

    // ���ɶ���
    public void OrderBuilder(OrderSO orderSO) {
        _taskList.ProductOrder(orderSO.orderTitle, orderSO.customerSO.customerName, _distance.ToString(), 
            orderSO.customerSO.customerAddress, orderSO.range, orderSO.customerSO.customerProfile);
    }
}