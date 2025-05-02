using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "EventAttribution", menuName = "Event/NodeActions/EventAttribution")]
public class EventAttribution : EventNodeBase
{
    public PlayerAttribution type;
    public float val;
    public override void Execute()
    {
        base.Execute();
        EventHandlerManager.CallUpdateAttribution(type, val);
        m_state = EventNodeState.Finished;
        m_on_finished?.Invoke(true);
    }
}
