using UnityEngine;

/// <summary>
/// �˿���Ϣ��������
/// </summary>
[CreateAssetMenu(fileName = "Customer", menuName = "Order System/Customer Info")]
public class CustomerSO : ScriptableObject {
    [Tooltip("�˿�ͷ��ͼƬ")]
    public Sprite customerProfile;
    [Tooltip("�˿�����")]
    public string customerName;
    [Tooltip("�˿͵�ַ")]
    public string customerAddressName;
    [Tooltip("�˿���ϸ��ַ")]
    public string customerAddress;
}