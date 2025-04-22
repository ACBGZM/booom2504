using UnityEngine;

[CreateAssetMenu(fileName = "EventCloseDialogueBox", menuName = "Event/Close Dialogue Box")]
public class EventCloseDialogueBox : EventNodeBase
{
    public override void Execute()
    {
        base.Execute();
        DialogueUIManager.CloseDialogueBox(OnDialogueBoxClosed);
        m_state = EventNodeState.Finished;
    }

    public void OnDialogueBoxClosed()
    {
        GameManager.Instance.GameplayState = GameManager.DeliveryGameplayState.PlayerIdle;
        m_on_finished?.Invoke(true);
    }
}
