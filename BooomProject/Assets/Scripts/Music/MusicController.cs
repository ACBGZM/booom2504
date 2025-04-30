using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 控制背景音乐脚本
/// </summary>
public class MusicController : MonoBehaviour
{
    private void Awake()
    {
        if (FindObjectsOfType<MusicController>().Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(this);
    }
}
