using UnityEngine;

[CreateAssetMenu(fileName = "EventGoToDeilvery", menuName = "Event/GP/EventGoToDeilvery")]
public class EventGoToDeilvery : EventNodeBase
{
    public override void Execute()
    {
        base.Execute();

        BaseCampGameplayManager.Instance.SceneManager.LoadAsyncWithFading("03Delivery");

        m_on_finished?.Invoke(true);
    }
}
