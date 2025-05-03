using UnityEngine;

[CreateAssetMenu(fileName = "EventRest", menuName = "Event/GP/EventRest")]
public class EventRest : EventNodeBase
{
    public override void Execute()
    {
        base.Execute();

        EventHandlerManager.EndDay();

        m_on_finished?.Invoke(true);
    }
}
