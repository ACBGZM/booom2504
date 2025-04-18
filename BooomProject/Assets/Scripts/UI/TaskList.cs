using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// 任务订单脚本
/// </summary>
public class TaskList : MonoBehaviour
{
    [SerializeField] private GameObject orderTemplatePrefab;
    [SerializeField] private Transform contentPanel;

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// 更改订单信息
    /// </summary>
    /// <param name="titleText">订单时间文本</param>
    /// <param name="profileImage">顾客头像</param>
    /// <param name="customerNameText">顾客名称文本</param>
    /// <param name="distanceText">距离文本信息</param>
    /// <param name="addressText">顾客地址文本信息</param>
    public void Change(string titleText, Sprite profileImage, string customerNameText, string distanceText, string addressText)
    {
        TextMeshProUGUI m_titleText = orderTemplatePrefab.transform.Find("orderTitle/TitleText").gameObject.GetComponent<TextMeshProUGUI>();
        Sprite m_profileImage = orderTemplatePrefab.transform.Find("OrderTemplate/ProfileImage").gameObject.GetComponent<Sprite>();
        TextMeshProUGUI m_customerNameTex = orderTemplatePrefab.transform.Find("OrderTemplate/OrderIformation/CustomerNameText").gameObject.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI m_distanceText = orderTemplatePrefab.transform.Find("OrderTemplate/OrderIformation/DistanceText").gameObject.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI m_addressText = orderTemplatePrefab.transform.Find("OrderTemplate/OrderIformation/AddressText").gameObject.GetComponent<TextMeshProUGUI>();

        m_titleText.text = titleText;
        m_profileImage = profileImage;
        m_customerNameTex.text = customerNameText;
        m_distanceText.text = distanceText;
        m_addressText.text = addressText;
    }


    /// <summary>
    /// 随机生成订单
    /// </summary>
    public void RangeProductOrder()
    {
        for (int i = 0; i < 5;i++)
        {
            GameObject orderTemplatePrefab = Instantiate(this.orderTemplatePrefab, contentPanel);
        }
    }
}
    