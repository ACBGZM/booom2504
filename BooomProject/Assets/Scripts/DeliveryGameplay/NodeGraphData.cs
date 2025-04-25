using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class EdgeData
{
    public int nodeA;
    public int nodeB;
    public int cost;
    public Vector2[] curvePoints;
}

[CreateAssetMenu(menuName = "NodeGraph/NodeGraph Data")]
public class NodeGraphData : ScriptableObject
{
    public List<EdgeData> _edges = new List<EdgeData>();

#if UNITY_EDITOR

    private void OnValidate()
    {
        EditorApplication.delayCall += TriggerRefresh;
    }

    private void TriggerRefresh()
    {
        if (this == null)
        {
            return;
        }

        var manager = FindObjectOfType<NodeGraphManager>(true);
        if (manager != null && manager.NodeGraphData == this)
        {
            manager.ReloadNodeGraph();
        }
    }

#endif
}
