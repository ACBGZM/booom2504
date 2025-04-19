using Microsoft.Unity.VisualStudio.Editor;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using Image = UnityEngine.UI.Image;

/// <summary>
/// 任务订单脚本
/// </summary>
public class TaskList : MonoBehaviour
{
    [SerializeField] private List<GameObject> orderTemplatePrefab;
    [SerializeField] private Transform contentPanel;


    /// <summary>
    /// 生成订单信息
    /// </summary>
    /// <param name="titleText">订单时间</param>
    /// <param name="customerNameText">顾客名称</param>
    /// <param name="distanceText">距离</param>
    /// <param name="addressText">顾客地址</param>
    /// <param name="range">0、低级订单 1、中级订单 2、高级订单</param>
    /// <param name="profileImage">顾客头像</param>
    /// <param name="path">顾客头像文件路径 路径格式：Application.dataPath + @"/文件路径"</param>
    private void ProductOrder(string titleText, string customerNameText, string distanceText, string addressText, int range, Sprite profileImage, string path)
    {
        TakingOrder takeOrder = orderTemplatePrefab[range].transform.Find("OrderButton").gameObject.GetComponent<TakingOrder>();
        TextMeshProUGUI m_titleText = orderTemplatePrefab[range].transform.Find("orderTitle/TitleText").gameObject.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI m_customerNameTex = orderTemplatePrefab[range].transform.Find("OrderIformation/CustomerNameText").gameObject.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI m_distanceText = orderTemplatePrefab[range].transform.Find("OrderIformation/DistanceText").gameObject.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI m_addressText = orderTemplatePrefab[range].transform.Find("OrderIformation/AddressText").gameObject.GetComponent<TextMeshProUGUI>();
        Image m_profileImage = orderTemplatePrefab[range].transform.Find("ProfileImage").gameObject.GetComponent<Image>();

        m_titleText.text = titleText;
        m_profileImage.sprite = LoadFile.LoadImage(path);
        m_customerNameTex.text = customerNameText;
        m_distanceText.text = distanceText;
        m_addressText.text = addressText;
        takeOrder.range = range;
        GameObject order = Instantiate(this.orderTemplatePrefab[range], contentPanel);
    }
}
    