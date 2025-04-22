using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "EventShowDialogue", menuName = "Event/Show Dialogue")]
public class EventShowDialogue : EventNodeBase
{
    public List<Dialogue> m_dialogues;

    public override void Execute()
    {
        base.Execute();
        DialogueUIManager.OpenDialogueBox(ShowDialogueText, m_dialogues.First());
        GameManager.Instance.GameplayState = GameManager.DeliveryGameplayState.InCutscene;
    }

    public void ShowDialogueText()
    {
        DialogueUIManager.GetInstance().StartCoroutine(StepThroughDialogueDataList());
    }

    public IEnumerator StepThroughDialogueDataList()
    {
        for (int i = 0; i < m_dialogues.Count; ++i)
        {
            DialogueUIManager.SetCanShowNextDialogue(false);

            Dialogue dialogue = m_dialogues[i];

            yield return DialogueUIManager.ShowDialogue(dialogue);

            yield return new WaitUntil(() => DialogueUIManager.GetCanShowNextDialogue());
        }

        m_state = EventNodeState.Finished;
        m_on_finished?.Invoke(true);
    }
}
