using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "SequenceExcuse", menuName = "Event/NodeActions/SequenceExcuse")]
public class SequenceExcuse : EventNodeBase
{
    [SerializeField] private EventSequenceExecutor eventSequence;
    public override void Execute()
    {
        base.Execute();
        int currentNodeId = CommonGameplayManager.GetInstance().NodeGraphManager.CurrentNode.NodeID;
        if(EventHandlerManager.CallCheckNodeOrder(currentNodeId).Count > 0)
        {
            if (eventSequence != null)
            {
                eventSequence.Initialize(Finished);
                eventSequence.Execute();
            }
            else
            {
                Finished(true);
            }
        }
        else
        {
            Finished(true);
        }
        

    }
    private void Finished(bool success)
    {
        
        m_state = EventNodeState.Finished;
        m_on_finished?.Invoke(true);
    }
}
