using UnityEngine;

[CreateAssetMenu(fileName = "EventReloadBaseCamp", menuName = "Event/GP/EventReloadBaseCamp")]
public class EventReloadBaseCamp : EventNodeBase
{
    public override void Execute()
    {
        base.Execute();

        BaseCampGameplayManager.Instance.SceneManager.LoadAsyncWithFading("02BaseCamp");

        m_on_finished?.Invoke(true);
    }
}
