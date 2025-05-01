using System;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Node : MonoBehaviour, IClickable
{
    [Tooltip("地址")]
    public string _address;

    [Tooltip("详细地址")]
    public string _addressDetail;

    [SerializeField] private EventSequenceExecutor _executor;
    [SerializeField] private NodeActionType type;
    public bool test;

    public string TargetSceneName;
    #region 结点震荡
    [Header("震荡参数")]
    [Tooltip("单次震荡持续时间（秒）")]
    [SerializeField] private float shakeDuration = 1f;
    [Tooltip("震荡强度（最大旋转角度）")]
    [SerializeField] private float shakeStrength = 20f;
    [Tooltip("每秒震动次数")]
    [SerializeField] private int vibrato = 20;
    [Tooltip("随机方向偏移角度（0-180）")]
    [SerializeField][Range(0, 180)] private float randomness = 90f;

    private Tween currentShakeTween; // 当前震荡的 Tween 实例
    #endregion
    public virtual void OnReach()
    {
        if (_executor != null)
        {
            _executor.Initialize(OnExecutorFinished);
            _executor.Execute();
        }
    }

    public virtual void OnLeave()
    {
        DeliveryGameplayManager.Instance.ShowEnterButton(false, null, null);
    }

    protected virtual void OnExecutorFinished(bool success)
    {
        CheckShowEnterButton();
    }

    public void CheckShowEnterButton()
    {
        if (!String.IsNullOrEmpty(TargetSceneName))
        {
            DeliveryGameplayManager.Instance.ShowEnterButton(true, transform, () =>
            {
                DeliveryGameplayManager.Instance.SceneManager.LoadAsyncWithFading(TargetSceneName);
            });
        }
    }

    public struct Edge
    {
        public Edge(Vector3[] path, float distance)
        {
            _path = path;
            _distance = distance;
        }

        public Vector3[] _path;
        public float _distance;
    }

    // runtime adjacent nodes reference
    public Dictionary<Node, Edge> AdjacentNodes { get; set; } = new Dictionary<Node, Edge>();

    [SerializeField] private int _nodeID;
    public int NodeID => _nodeID;

    private void Awake()
    {
        _originalScale = transform.localScale;
    }

    public void OnClick()
    {
        CommonGameplayManager.GetInstance().NodeGraphManager.CheckAndMoveTo(this);
    }

    private Vector3 _originalScale;

    public void ShowCanMove(bool canMove)
    {
        if (canMove)
        {
            transform.DOScale(_originalScale * 1.2f, 0.5f)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine);
        } else
        {
            transform.DOKill();
            transform.localScale = _originalScale;
        }
    }

    public void ShowIsMovingTo(bool isMovingTo)
    {
        if (isMovingTo)
        {
            transform.DOScale(_originalScale * 1.5f, 0.3f)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine);
        } else
        {
            transform.DOKill();
            transform.localScale = _originalScale;
        }
    }

#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        GUIStyle nodeIDDisplayStyle = new();
        nodeIDDisplayStyle.normal.textColor = Color.cyan;
        UnityEditor.Handles.Label(
            transform.position + Vector3.up * 0.5f,
            $"#{NodeID} {_address}",
            nodeIDDisplayStyle);
    }

#endif
    #region shake
    /// <summary>
    /// 开始循环震荡效果
    /// </summary>
    public void StartShake()
    {
        // 停止当前可能存在的震荡
        StopShake(false);

        // 创建震荡 Tween 并设置循环
        currentShakeTween = transform.DOShakeRotation(
            duration: shakeDuration,
            strength: new Vector3(shakeStrength, shakeStrength, shakeStrength),
            vibrato: vibrato,
            randomness: randomness,
            fadeOut: false // 不自动减弱
        )
        .SetLoops(-1) // -1 表示无限循环
        .SetEase(Ease.Linear); // 线性缓动保持均匀震荡
    }

    /// <summary>
    /// 停止震荡效果
    /// </summary>
    /// <param name="resetRotation">是否重置为初始旋转角度</param>
    public void StopShake(bool resetRotation = true)
    {
        if (currentShakeTween != null)
        {
            currentShakeTween.Kill(resetRotation); // 杀死 Tween 并可选重置旋转
            currentShakeTween = null;
        }
        if (resetRotation)
        {
            transform.rotation = Quaternion.identity;
        }
    }
    #endregion
    // TODO：节点作为目的地，高亮方式
    public void TargetNodeHighLight(bool isShow)
    {
        if(isShow)
        {
            // 放大
            transform.localScale *= 1.5f;
            StartShake();
        }
        else
        {
            // 恢复
            transform.localScale /= 1.5f;
            StopShake();
        }
    }
}
