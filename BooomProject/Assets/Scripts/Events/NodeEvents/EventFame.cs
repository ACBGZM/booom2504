using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EventFame", menuName = "Event/NodeActions/EventFame")]
public class EventFame : EventNodeBase
{
    public List<Dialogue> dialogues = new List<Dialogue>();
    public List<Dialogue> branch1;
    public List<Dialogue> branch2;
    public NodeActionType type;
    public int fameThreshold;
    public override void Execute()
    {
        base.Execute();
        CheckFame();


    }
    public void ShowDialogueText()
    {
        DialogueUIManager.GetInstance().StartCoroutine(StepThroughDialogueDataList());
    }
  
    public IEnumerator StepThroughDialogueDataList()
    {
        for (int i = 0; i < dialogues.Count; ++i)
        {
            DialogueUIManager.SetCanShowNextDialogue(false);

            Dialogue dialogue = dialogues[i];

            yield return DialogueUIManager.ShowDialogue(dialogue);

            yield return new WaitUntil(() => DialogueUIManager.GetCanShowNextDialogue());
        }
 
        dialogues.Clear();
        m_state = EventNodeState.Finished;
        m_on_finished?.Invoke(true);
    }
    public void CheckFame()
    {
        GameManager.Instance.GameplayState = GameManager.DeliveryGameplayState.InCutscene;


        //TODO ÉùÓþ»ñÈ¡
        //²âÊÔÉùÓþ
        int Fame = Random.Range(0, 10);
        Debug.Log(Fame);
        List<Dialogue> choice;
        bool award = false;
        if (Fame > fameThreshold)
        {
            award = true;
            choice = branch1;
        }
        else
        {
            choice = branch2;
        }
        EventHandlerManager.CallUpdateBuff(type, award);
        foreach (var temp in choice)
        {
            Dialogue d = new Dialogue();
            d.m_text = temp.m_text;
            d.m_speaker_avatar = temp.m_speaker_avatar;
            d.m_speaker_name = temp.m_speaker_name;
            d.m_display_method = temp.m_display_method;
            d.m_can_skip = temp.m_can_skip;
            dialogues.Add(d);
        }
        DialogueUIManager.OpenDialogueBox(ShowDialogueText, dialogues[0]);
    }
}
