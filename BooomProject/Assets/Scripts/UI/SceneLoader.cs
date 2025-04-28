using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 场景切换动画脚本
/// </summary>
public class SceneLoader : MonoBehaviour
{
    // UICanvas 的事件System
    //[SerializeField] private GameObject eventSystem;

    // 场景切换动画机
    public Animator sceneAnimator;

    private void Start()
    {
        DontDestroyOnLoad(this);
        //DontDestroyOnLoad (eventSystem);
    }
}
