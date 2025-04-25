
using UnityEngine;

[CreateAssetMenu(fileName = "EventNodeUpdate", menuName = "Event/NodeActions/EventNodeUpdate")]
// ����ڵ��¼�
public class EventNodeUpdate : EventNodeBase
{
   
    public override void Execute()
    {
        base.Execute();
        Debug.Log("��ǰλ��" + GameManager.Instance.NodeGraphManager.CurrentNode.NodeID);
        // ����ʣ��ʱ�������
        //  EventHandlerManager.CallUpdateArriveDistAndTime(EventHandlerManager.CallGetCurrentNode(), PlayerManager.Instance.speed);
        m_state = EventNodeState.Finished;
        m_on_finished?.Invoke(true);
    }
}
