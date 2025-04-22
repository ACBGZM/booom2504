using UnityEngine;

[CreateAssetMenu(fileName = "EventCloseDialogueBox", menuName = "Event/Close Dialogue Box")]
public class EventCloseDialogueBox : EventNodeBase
{
    public override void Execute()
    {
        base.Execute();
        DialogueUIManager.CloseDialogueBox(OnDialogueBoxClosed);
        m_state = EventNodeState.Finished;
        GameManager.Instance.GameplayState = GameManager.DeliveryGameplayState.PlayerIdle;
    }

    public void OnDialogueBoxClosed()
    {
        m_on_finished?.Invoke(true);
    }
}
