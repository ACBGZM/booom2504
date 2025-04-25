using UnityEngine;

[CreateAssetMenu(fileName = "EventNodeUpdate", menuName = "Event/NodeActions/EventNodeUpdate")]
public class EventNodeUpdate : EventNodeBase
{
    public override void Execute()
    {
        base.Execute();
        int nodeIdx = GameManager.Instance.NodeGraphManager.CurrentNode.NodeID;
        Debug.Log("当前位置" + nodeIdx);
        // 判断该节点是否为目的节点
        if (EventHandlerManager.CallCheckNodeOrder(nodeIdx))
        {
            // TODO：判断具体某个订单，执行某个事件
        }
        // 订单剩余时间与距离
        //  EventHandlerManager.CallUpdateArriveDistAndTime(EventHandlerManager.CallGetCurrentNode(), PlayerManager.Instance.speed);
        m_state = EventNodeState.Finished;
        m_on_finished?.Invoke(true);
    }
}
