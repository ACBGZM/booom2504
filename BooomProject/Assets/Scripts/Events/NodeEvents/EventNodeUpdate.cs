
using UnityEngine;

[CreateAssetMenu(fileName = "EventNodeUpdate", menuName = "Event/NodeActions/EventNodeUpdate")]
// ����ڵ��¼�
public class EventNodeUpdate : EventNodeBase
{
   
    public override void Execute()
    {
        base.Execute();
        Debug.Log("��ǰλ��" + EventHandlerManager.CallGetCurrentNode());
        // ����ʱ�������
        EventHandlerManager.CallUpdateArriveDistAndTime(EventHandlerManager.CallGetCurrentNode(), PlayerManager.Instance.speed);
        
    }
}
