using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

#endif

public class NodeGraphManager : MonoBehaviour
{
    [SerializeField] private NodeGraphData _nodeGraphData;
    public NodeGraphData NodeGraphData => _nodeGraphData;

    private Dictionary<int, Node> _allNodes = new Dictionary<int, Node>();
    // 节点ID 映射到 索引
    private Dictionary<int, int> nodeIdTOIndex = new Dictionary<int, int>();
    private float[,] dist;    // 任意两节点间最短消耗
    private int nodeCnt;
    // 结点上次触发事件事件
    private Dictionary<int, int> nodeTriggerTime = new Dictionary<int, int>();

    public bool IsOnBaseCampNode()
    {
        return _currentNodeID == 1;
    }

    public bool IsOnTargetNode()
    {
        return _currentNodeID is >= 2 and <= 12;
    }

    private static bool _isDataLoaded = false;

    private void Awake()
    {
        if (_isDataLoaded)
        {
            Destroy(gameObject);
            return;
        }

        nodeCnt = 0;

        Node[] nodes = FindObjectsOfType<Node>();
        foreach (Node node in nodes)
        {
            _allNodes.Add(node.NodeID, node);
            // 索引从0开始
            nodeIdTOIndex.Add(node.NodeID,nodeCnt);
            nodeCnt++;
        }

        foreach (EdgeData edgeData in _nodeGraphData._edges)
        {
            if (!_allNodes.ContainsKey(edgeData.nodeA) || !_allNodes.ContainsKey(edgeData.nodeB))
            {
                Debug.LogError($"Edge data contains invalid node IDs: {edgeData.nodeA}, {edgeData.nodeB}");
                continue;
            }

            Vector3 begin = _allNodes[edgeData.nodeA].transform.position;
            Vector3 end = _allNodes[edgeData.nodeB].transform.position;

            List<Vector3> path = new List<Vector3> { begin };
            foreach (var point in edgeData.curvePoints)
            {
                path.Add(begin + (Vector3)point);
            }

            path.Add(end);

            float distance = 0;
            for (int i = 0; i < path.Count - 1; ++i)
            {
                distance += Vector2.Distance(path[i], path[i + 1]);
            }

            List<Vector3> reversedPath = new List<Vector3>(path);
            reversedPath.Reverse();

            _allNodes[edgeData.nodeA].AdjacentNodes.Add(
                _allNodes[edgeData.nodeB],
                new Node.Edge(path.ToArray(), distance));
            _allNodes[edgeData.nodeB].AdjacentNodes.Add(
                _allNodes[edgeData.nodeA],
                new Node.Edge(reversedPath.ToArray(), distance));
        }

        dist = new float[nodeCnt, nodeCnt];
        ResetDist();
        Floyed();

        _isDataLoaded = true;
        _lastNodeID = CurrentNode.NodeID;
    }

    private void ResetDist()
    {
        for (int i = 0; i < nodeCnt; i++)
        {
            for (int j = 0; j < nodeCnt; j++)
            {
                // -1代表无边权
                dist[i, j] = -1;
            }
        }
        foreach (EdgeData edgeData in _nodeGraphData._edges)
        {
            Node a = GetNodeByIDRuntime(edgeData.nodeA);
            Node b = GetNodeByIDRuntime(edgeData.nodeB);
            float distance = a.AdjacentNodes[b]._distance;
            dist[nodeIdTOIndex[edgeData.nodeA], nodeIdTOIndex[edgeData.nodeB]] = distance;
            dist[nodeIdTOIndex[edgeData.nodeB], nodeIdTOIndex[edgeData.nodeA]] = distance;
        }
    }

    private void Floyed()
    {
        for (int i = 0; i < nodeCnt; i++)
        {
            for (int k = 0; k < nodeCnt; k++)
            {
                if (dist[i, k] == -1) continue;
                for (int j = 0; j < nodeCnt; j++)
                {
                    if (dist[k, j] == -1) continue;
                    dist[i, j] = Mathf.Min(dist[i, j], dist[i, k] + dist[k, j]);
                }
            }
        }
    }

    public void RefreshMovingHints()
    {
        ShowCanMoveNodes(CurrentNode, true);
        CurrentNode.CheckShowEnterButton();
    }

    public void ResetCurrentNode()
    {
        _currentNodeID = 1;
    }

    public Node GetNodeByIDRuntime(int nodeID)
    {
        return _allNodes.GetValueOrDefault(nodeID);
    }

    [SerializeField] private int _currentNodeID = -1;
    [SerializeField] private int _lastNodeID = -1;
    [SerializeField] private int tryToNodeID = -1;
    public Node CurrentNode => GetNodeByIDRuntime(_currentNodeID);
    public Node LastNode => GetNodeByIDRuntime(_lastNodeID);
    public Node TryToNode => GetNodeByIDRuntime(tryToNodeID);
    public void CheckAndMoveTo(Node targetNode)
    {
        if (CurrentNode.AdjacentNodes.ContainsKey(targetNode))
        {
            ShowCanMoveNodes(CurrentNode, false);
            CurrentNode.OnLeave();
            targetNode.ShowIsMovingTo(true);
            tryToNodeID = targetNode.NodeID;
            DeliveryGameplayManager.Instance.DeliveryPlayer.Move(CurrentNode.AdjacentNodes[targetNode]._path
            , () =>
            {
                targetNode.ShowIsMovingTo(false);
                _lastNodeID = _currentNodeID;
                _currentNodeID = targetNode.NodeID;
                ShowCanMoveNodes(targetNode, true);
                SetBaseCampHintActive(!IsOnBaseCampNode());
                targetNode.OnReach();
            });
        }
    }

    public void ShowCanMoveNodes(Node fromNode, bool canMove)
    {
        foreach (var adjacentNode in fromNode.AdjacentNodes)
        {
            adjacentNode.Key.ShowCanMove(canMove);
        }
    }

#if UNITY_EDITOR
    private Dictionary<Node, List<EdgeData>> _adjacency = new Dictionary<Node, List<EdgeData>>();

    private void OnValidate()
    {
        if (_nodeGraphData != null)
        {
            EditorApplication.delayCall += ReloadNodeGraph;
        }
    }

    public void ReloadNodeGraph()
    {
        _allNodes.Clear();
        _adjacency.Clear();

        Node[] nodes = FindObjectsOfType<Node>();
        foreach (var node in nodes)
        {
            _allNodes[node.NodeID] = node;
            _adjacency[node] = new List<EdgeData>();
        }

        foreach (EdgeData edge in _nodeGraphData._edges)
        {
            if (_allNodes != null &&
                _allNodes.TryGetValue(edge.nodeA, out Node nodeA) &&
                _allNodes.TryGetValue(edge.nodeB, out Node nodeB))
            {
                _adjacency[nodeA].Add(edge);
                _adjacency[nodeB].Add(edge);
            }
        }

        SceneView.RepaintAll();
    }

    public Node GetNodeByIDEditor(int nodeID)
    {
        _allNodes.TryGetValue(nodeID, out Node node);
        return node;
    }

    private void OnDrawGizmos()
    {
        if (_nodeGraphData == null || _adjacency.Count == 0)
        {
            return;
        }

        foreach (var edge in _nodeGraphData._edges)
        {
            if (_allNodes.TryGetValue(edge.nodeA, out Node nodeA)
                && _allNodes.TryGetValue(edge.nodeB, out Node nodeB))
            {
                DrawEdge(nodeA.transform.position, nodeB.transform.position, edge);
            }
        }
    }

    private void DrawEdge(Vector3 start, Vector3 end, EdgeData edge)
    {
        Handles.color = Color.green;
        float distance = 0.0f;
        if (edge.curvePoints.Length == 0)
        {
            Handles.DrawLine(start, end);
            distance = Vector2.Distance(start, end);
        }
        else
        {
            List<Vector3> path = new List<Vector3> { start };
            foreach (var point in edge.curvePoints)
            {
                path.Add(start + (Vector3)point);
            }

            path.Add(end);

            Handles.DrawPolyLine(path.ToArray());

            for (int i = 0; i < path.Count - 1; ++i)
            {
                distance += Vector2.Distance(path[i], path[i + 1]);
            }
        }

        Vector3 labelPos = Vector3.Lerp(start, end, 0.5f);
        Handles.Label(labelPos, $"dis: {distance:F1}");
    }

#endif

    public float GetDistance(int currentNode, int targetNode)
    {
        return dist[nodeIdTOIndex[currentNode], nodeIdTOIndex[targetNode]];
    }

    public void ShowTargetNode(int nodeIdx, bool isShow)
    {
        Node targetNode = GetNodeByIDRuntime(nodeIdx);
        if (targetNode == null || targetNode == default)
        {
            Debug.LogError("Invalid target node id!");
            return;
        }

        targetNode.TargetNodeHighLight(isShow);
    }
    public int GetNodeTriggerTime(int nodeId)
    {
        int time;
        nodeTriggerTime.TryGetValue(nodeId,out time);
        return time;
    }
    public void SetNodeTriggerTime(int nodeId, int time)
    {
        if(nodeTriggerTime.ContainsKey(nodeId))
        {
            nodeTriggerTime[nodeId] = time;
        }
        else
        {
            nodeTriggerTime.Add(nodeId, time);
        }

    }

    [SerializeField] private GameObject _baseCampHint;
    public void SetBaseCampHintActive(bool isActive)
    {
        _baseCampHint.SetActive(!IsOnBaseCampNode() && isActive);
    }
}
