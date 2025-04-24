using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// 节点地址映射数据容器
[CreateAssetMenu(fileName = "NodeAddress", menuName = "NodeAddress/NodeAddressSO")]
public class NodeAddressSO : ScriptableObject
{
    // 键 地址名  值 节点编号
    public SerializableDictionary<string, int> nodeAddress;
}
