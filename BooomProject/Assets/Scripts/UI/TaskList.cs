using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// ���񶩵��ű�
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
    /// ���Ķ�����Ϣ
    /// </summary>
    /// <param name="titleText">����ʱ���ı�</param>
    /// <param name="profileImage">�˿�ͷ��</param>
    /// <param name="customerNameText">�˿������ı�</param>
    /// <param name="distanceText">�����ı���Ϣ</param>
    /// <param name="addressText">�˿͵�ַ�ı���Ϣ</param>
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
    /// ������ɶ���
    /// </summary>
    public void RangeProductOrder()
    {
        for (int i = 0; i < 5;i++)
        {
            GameObject orderTemplatePrefab = Instantiate(this.orderTemplatePrefab, contentPanel);
        }
    }
}
    