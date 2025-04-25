
using UnityEngine;

[CreateAssetMenu(fileName = "EventNodeUpdate", menuName = "Event/NodeActions/EventNodeUpdate")]
// 到达节点事件
public class EventNodeUpdate : EventNodeBase
{
   
    public override void Execute()
    {
        base.Execute();
        Debug.Log("当前位置" + GameManager.Instance.NodeGraphManager.CurrentNode.NodeID);
        // 订单剩余时间与距离
        //  EventHandlerManager.CallUpdateArriveDistAndTime(EventHandlerManager.CallGetCurrentNode(), PlayerManager.Instance.speed);
        m_state = EventNodeState.Finished;
        m_on_finished?.Invoke(true);
    }
}
