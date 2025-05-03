using UnityEngine;

[CreateAssetMenu(fileName = "GameEndingEventsReputation", menuName = "Event/GP/Game Ending - Reputation")]
public class GameEndingEventsReputation : EventNodeBase
{
    public EventShowDialogue Reputation3;
    public EventShowDialogue Reputation6;
    public EventShowDialogue Reputation10;

    public override void Execute()
    {
        base.Execute();

        int reputation = CommonGameplayManager.GetInstance().PlayerDataManager.Reputation.Value;
        if (reputation <= 3)
        {
            Reputation3.Initialize(OnFinish);
            Reputation3.Execute();
        }
        else if(reputation <= 6)
        {
            Reputation6.Initialize(OnFinish);
            Reputation6.Execute();
        }
        else /* if(reputation <= 10)*/
        {
            Reputation10.Initialize(OnFinish);
            Reputation10.Execute();
        }
    }

    public void OnFinish(bool success)
    {
        m_state = EventNodeState.Finished;
        m_on_finished?.Invoke(true);
    }
}
