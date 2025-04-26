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
        CommonGameplayManager.GetInstance().PlayerState = EPlayerState.PlayerIdle;
        m_on_finished?.Invoke(true);
    }
}
