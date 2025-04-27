using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ChatFragment
{
    public string chatTitle;
    public string time;
    public Image profile;
    public string content;
    public bool left;

    public ChatFragment(string time, Image profile, string content, bool left, string chatTitle)
    {
        this.time = time;
        this.profile = profile;
        this.content = content;
        this.left = left;
        this.chatTitle = chatTitle;
    }
}

[System.Serializable]
public class DiceSide
{
    // 面中心
    public Vector3 center;

    // 面法线
    public Vector3 normal;

    // 面值
    public int val;
}

[System.Serializable]
public class SerializableDictionary<TKey, TValue>
{
    public List<TKey> keys = new List<TKey>();
    public List<TValue> values = new List<TValue>();

    // 将字典转换为两个列表
    public void FromDictionary(Dictionary<TKey, TValue> dict)
    {
        keys.Clear();
        values.Clear();
        foreach (var pair in dict)
        {
            keys.Add(pair.Key);
            values.Add(pair.Value);
        }
    }

    // 将两个列表转换为字典
    public Dictionary<TKey, TValue> ToDictionary()
    {
        var dict = new Dictionary<TKey, TValue>();
        for (int i = 0; i < keys.Count; i++)
        {
            dict[keys[i]] = values[i];
        }
        return dict;
    }
}
