using System.Collections.Generic;
using UnityEngine;
using JetBrains.Annotations;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class NodeGraphManager : MonoBehaviour
{
    [SerializeField] private NodeGraphData _nodeGraphData;
    public NodeGraphData NodeGraphData => _nodeGraphData;

    private Dictionary<int, Node> _allNodes = new Dictionary<int, Node>();
    // 任意两节点间最短消耗
    private float[,] dist;
    private int nodeCnt;
    private void Awake()
    {
        nodeCnt = _nodeGraphData._edges.Count;
        dist = new float[nodeCnt, nodeCnt];
        ResetDist();
        Floyed();
        Node[] nodes = FindObjectsOfType<Node>();
        foreach (Node node in nodes)
        {
            _allNodes.Add(node.NodeID, node);
        }
        
        foreach (EdgeData edgeData in _nodeGraphData._edges)
        {
            if (!_allNodes.ContainsKey(edgeData.nodeA) || !_allNodes.ContainsKey(edgeData.nodeB))
            {
                Debug.LogError($"Edge data contains invalid node IDs: {edgeData.nodeA}, {edgeData.nodeB}");
                continue;
            }

            Vector3 begin = _allNodes[edgeData.nodeA].transform.position;
            Vector3 end =  _allNodes[edgeData.nodeB].transform.position;

            List<Vector3> path = new List<Vector3> { begin };
            foreach (var point in edgeData.curvePoints)
            {
                path.Add(begin + (Vector3)point);
            }

            path.Add(end);

            List<Vector3> reversedPath = new List<Vector3>(path);
            reversedPath.Reverse();

            _allNodes[edgeData.nodeA].AdjacentNodes
                .Add(_allNodes[edgeData.nodeB], new Node.Edge(edgeData.cost, path.ToArray()));
            _allNodes[edgeData.nodeB].AdjacentNodes.Add(_allNodes[edgeData.nodeA],
                new Node.Edge(edgeData.cost, reversedPath.ToArray()));
        }
    }
    private void ResetDist()
    {
        for (int i = 0; i < nodeCnt; i++)
        {
            for (int j = 0; j < nodeCnt; j++)
            {
                dist[i,j] = -1;
            }
        }
        foreach (EdgeData edgeData in _nodeGraphData._edges)
        {
            dist[edgeData.nodeA, edgeData.nodeB] = edgeData.cost;
            dist[edgeData.nodeB, edgeData.nodeA] = edgeData.cost;
        }
    }
    private void Floyed()
    {
        for(int i = 0; i < nodeCnt; i ++ )
        {
            for(int k = 0; k < nodeCnt; k ++ )
            {
                if (dist[i, k] == -1) continue;
                for(int j = 0; j < nodeCnt; j ++ )
                {
                    if (dist[k,j] == -1) continue;
                    dist[i,j] = Mathf.Min(dist[i, j], dist[i,k] + dist[k,j]);
                }
            }
        }

        
    }
    public void Start()
    {
        ShowCanMoveNodes(CurrentNode, true);
    }

    public Node GetNodeByIDRuntime(int nodeID)
    {
        return _allNodes.GetValueOrDefault(nodeID);
    }

    [SerializeField] private int _currentNodeID = -1;
    public Node CurrentNode => GetNodeByIDRuntime(_currentNodeID);

    public void CheckAndMoveTo(Node targetNode)
    {
        if (CurrentNode.AdjacentNodes.ContainsKey(targetNode))
        {
            GameManager.Instance.DeliveryPlayer.Move(CurrentNode.AdjacentNodes[targetNode]._path
            , () => 
            {
                ShowCanMoveNodes(CurrentNode, false);
                _currentNodeID = targetNode.NodeID;
                ShowCanMoveNodes(targetNode, true);

                targetNode.ExecuteEvents();
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

        if (edge.curvePoints.Length == 0)
        {
            Handles.DrawLine(start, end);
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
        }

        Vector3 labelPos = Vector3.Lerp(start, end, 0.5f);
        Handles.Label(labelPos, $"Cost: {edge.cost}");
    }
#endif
}