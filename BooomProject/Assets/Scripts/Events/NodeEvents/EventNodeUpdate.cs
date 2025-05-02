using UnityEngine;

[CreateAssetMenu(fileName = "EventNodeUpdate", menuName = "Event/NodeActions/EventNodeUpdate")]
public class EventNodeUpdate : EventNodeBase
{
    public override void Execute()
    {
        base.Execute();
        int nodeId = CommonGameplayManager.GetInstance().NodeGraphManager.CurrentNode.NodeID;
        int lastId = CommonGameplayManager.GetInstance().NodeGraphManager.LastNode.NodeID;
        //Debug.Log("上一个位置" + lastId);
        //Debug.Log("当前位置" + nodeId);
        //float distance = CommonGameplayManager.GetInstance().NodeGraphManager.GetDistance(lastId, nodeId);
        //float costTime = distance / (CommonGameplayManager.GetInstance().PlayerDataManager.Speed.Value * 0.1f);
        //Debug.Log($"路程消耗{costTime} 分钟");
        //int minute = (int)costTime;
        //int second = (int)(costTime - minute) * 60;
       // CommonGameplayManager.GetInstance().TimeManager.currentTime.minute += minute;
       
        // 走到大本营节点，更新订单状态为已取货
        if (CommonGameplayManager.GetInstance().NodeGraphManager.IsOnBaseCampNode())
        {
            EventHandlerManager.CallUpdateOrderStateToTransit();
        }
        // 判断该节点是否为目的节点,并执行事件

        if (EventHandlerManager.CallCheckNodeOrder(nodeId))
        {

        }

        m_state = EventNodeState.Finished;
        m_on_finished?.Invoke(true);
    }
}
