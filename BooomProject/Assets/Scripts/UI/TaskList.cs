using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Image = UnityEngine.UI.Image;

/// <summary>
/// ���񶩵��ű�
/// </summary>
public class TaskList : MonoBehaviour
{
    [SerializeField] private List<GameObject> orderTemplatePrefab;
    [SerializeField] private Transform contentPanel;

    /// <summary>
    /// ���ɶ�����Ϣ
    /// </summary>
    /// <param name="titleText">����ʱ��</param>
    /// <param name="customerNameText">�˿�����</param>
    /// <param name="distanceText">����</param>
    /// <param name="addressText">�˿͵�ַ</param>
    /// <param name="range">0���ͼ����� 1���м����� 2���߼�����</param>
    /// <param name="profileImage">�˿�ͷ��</param>
    // /// <param name="path">�˿�ͷ���ļ�·�� ·����ʽ��Application.dataPath + @"/�ļ�·��"</param>
    public void ProductOrder(string titleText, string customerNameText, string distanceText, string addressText, int range, Sprite profileImage)
    {
        TakingOrder takeOrder = orderTemplatePrefab[range].transform.Find("OrderButton").gameObject.GetComponent<TakingOrder>();
        TextMeshProUGUI m_titleText = orderTemplatePrefab[range].transform.Find("OrderTitle/TitleText").gameObject.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI m_customerNameTex = orderTemplatePrefab[range].transform.Find("OrderIformation/CustomerNameText").gameObject.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI m_distanceText = orderTemplatePrefab[range].transform.Find("OrderInformation/DistanceText").gameObject.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI m_addressText = orderTemplatePrefab[range].transform.Find("OrderInformation/AddressText").gameObject.GetComponent<TextMeshProUGUI>();
        Image m_profileImage = orderTemplatePrefab[range].transform.Find("ProfileImage").gameObject.GetComponent<Image>();

        m_titleText.text = titleText;
        m_profileImage.sprite = profileImage;
        m_customerNameTex.text = customerNameText;
        m_distanceText.text = distanceText;
        m_addressText.text = addressText;
        takeOrder.range = range;
        GameObject order = Instantiate(this.orderTemplatePrefab[range], contentPanel);
    }
}
    