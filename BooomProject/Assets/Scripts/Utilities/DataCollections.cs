using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ChatFragment 
{
    public string chatTitle;
    public string time;
    public Sprite icon;
    public string content;
    public bool left;

    public ChatFragment(string time, Sprite icon, string content, bool left, string chatTitle) 
    {
        this.time = time;
        this.icon = icon;
        this.content = content;
        this.left = left;
        this.chatTitle = chatTitle;
    }

}
[System.Serializable]
public class DiceSide
{
    // ������
    public Vector3 center;
    // �淨��
    public Vector3 normal;
    // ��ֵ
    public int val;
}

[System.Serializable]
public class SerializableDictionary<TKey, TValue>
{
    public List<TKey> keys = new List<TKey>();
    public List<TValue> values = new List<TValue>();

    // ���ֵ�ת��Ϊ�����б�
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

    // �������б�ת��Ϊ�ֵ�
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
