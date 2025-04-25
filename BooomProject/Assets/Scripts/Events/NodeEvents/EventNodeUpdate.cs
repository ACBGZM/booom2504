
using UnityEngine;

[CreateAssetMenu(fileName = "EventNodeUpdate", menuName = "Event/NodeActions/EventNodeUpdate")]
// 到达节点事件
public class EventNodeUpdate : EventNodeBase
{
    // 送达该节点次数
    int arriveCnt;
    public override void Execute()
    {
        base.Execute();
        int nodeIdx = GameManager.Instance.NodeGraphManager.CurrentNode.NodeID;
        Debug.Log("当前位置" + nodeIdx);
        // 判断该节点是否为目的节点
        if(EventHandlerManager.CallCheckNodeOrder(nodeIdx))
        {
            // 累加送达次数
            arriveCnt++;
            // TODO：根据送达次数 执行事件

            

        }
        // 订单剩余时间与距离
        //  EventHandlerManager.CallUpdateArriveDistAndTime(EventHandlerManager.CallGetCurrentNode(), PlayerManager.Instance.speed);
        m_state = EventNodeState.Finished;
        m_on_finished?.Invoke(true);
    }
}
