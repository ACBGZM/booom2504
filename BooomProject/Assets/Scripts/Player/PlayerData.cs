using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 序列化玩家所有数据
/// </summary>
[System.Serializable]
public class PlayerData
{
    [Tooltip("速度")]
    public float speed;

    [Tooltip("声誉")]
    public int reputation;

    [Tooltip("好评率")]
    public float rating;

    [Tooltip("奖章数量")]
    public int medals;

    [Tooltip("订单与完成")]
    public List<SerializableKeyValuePair> orderSaves = new List<SerializableKeyValuePair>();
}
