using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour, IClickable
{
    [Tooltip("地址")]
    public string _address;

    [Tooltip("详细地址")]
    public string _addressDetail;

    [SerializeField] private EventSequenceExecutor _executor;
    [SerializeField] private NodeActionType type;
    public bool test;

    public virtual void ExecuteEvents()
    {
        if (_executor != null)
        {
            _executor.Initialize(OnExecutorFinished);
            _executor.Execute();
        }
    }

    protected virtual void OnExecutorFinished(bool success)
    {
    }

    public struct Edge
    {
        public Edge(int cost, Vector3[] path)
        {
            _cost = cost;
            _path = path;
        }

        public int _cost;
        public Vector3[] _path;
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
        // Debug.Log($"node clicked ({_nodeID})");
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
        Vector3 nodeIDDisplayPosition = transform.position + new Vector3(0.0f, 1.0f, 0.0f) * 0.5f;
        UnityEditor.Handles.Label(transform.position + Vector3.up * 0.5f, $"ID:{NodeID}", nodeIDDisplayStyle);
    }

#endif

    // TODO：节点作为目的地，高亮方式
    public void TargetNodeHighLight(bool finished)
    {
        test = finished;
    }
}
