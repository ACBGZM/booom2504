using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// �ڵ��ַӳ����������
[CreateAssetMenu(fileName = "NodeAddress", menuName = "NodeAddress/NodeAddressSO")]
public class NodeAddressSO : ScriptableObject
{
    // �� ��ַ��  ֵ �ڵ���
    public SerializableDictionary<string, int> nodeAddress;
}
