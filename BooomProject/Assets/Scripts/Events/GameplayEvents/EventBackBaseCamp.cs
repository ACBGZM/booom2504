using UnityEngine;

[CreateAssetMenu(fileName = "EventBackBaseCamp", menuName = "Event/GP/EventBackBaseCamp")]
public class EventBackBaseCamp : EventNodeBase
{
    public override void Execute()
    {
        base.Execute();

        DeliveryGameplayManager.Instance.SceneManager.LoadAsyncWithFading("02BaseCamp");

        m_on_finished?.Invoke(true);
    }
}
