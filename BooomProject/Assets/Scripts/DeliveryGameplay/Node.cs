using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Node : MonoBehaviour, IClickable
{
    // runtime adjacent nodes reference

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
        GameManager.GetInstance().NodeGraphManager.CheckAndMoveTo(this);
    }

    private Vector3 _originalScale;
    public void ShowCanMove(bool canMove)
    {
        if (canMove)
        {
            transform.DOScale(_originalScale * 1.2f, 0.5f)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine);
        }
        else
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
}