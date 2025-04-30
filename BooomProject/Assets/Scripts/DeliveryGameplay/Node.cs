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

    // TODO：节点作为目的地，高亮方式
    public void TargetNodeHighLight(bool finished)
    {
        test = finished;
    }
}
