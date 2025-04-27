using UnityEngine;

[CreateAssetMenu(fileName = "EventNodeUpdate", menuName = "Event/NodeActions/EventNodeUpdate")]
public class EventNodeUpdate : EventNodeBase
{
    public override void Execute()
    {
        base.Execute();
        int nodeIdx = CommonGameplayManager.GetInstance().NodeGraphManager.CurrentNode.NodeID;
        Debug.Log("当前位置" + nodeIdx);
        // 判断该节点是否为目的节点,并执行事件
      
        if (EventHandlerManager.CallCheckNodeOrder(nodeIdx))
        {
       
        }

        m_state = EventNodeState.Finished;
        m_on_finished?.Invoke(true);
    }
}
