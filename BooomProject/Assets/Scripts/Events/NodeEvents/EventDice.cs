using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EventDice", menuName = "Event/NodeActions/EventDice")]
public class EventDice : EventNodeBase
{
    public List<Dialogue> dialogues = new List<Dialogue>();
    public List<Dialogue> branch1;
    public List<Dialogue> branch2;
    public NodeActionType type;
    public override void Execute()
    {
        base.Execute();
        DiceUIManager.Instance.ShowMe();
        ShowShake();
    }


    public void ShowDialogueText()
    {
        DialogueUIManager.GetInstance().StartCoroutine(StepThroughDialogueDataList());
    }
    public void ShowShake()
    {
        DiceUIManager.Instance.StartCoroutine(DiceShake());
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
        DiceUIManager.Instance.HideMe();

        dialogues.Clear();
        m_state = EventNodeState.Finished;
        m_on_finished?.Invoke(true);
    }
    public IEnumerator DiceShake()
    {
     
        // 骰子触发 需手动点击roll
       
        // 状态改为触发剧情
        GameManager.Instance.GameplayState = GameManager.DeliveryGameplayState.InCutscene;

        yield return new WaitUntil(() => DiceUIManager.Instance.val != 0);
        
        // 骰子结果动画
        yield return new WaitForSeconds(2);
        if (DiceUIManager.Instance.val != 0)
        {

            int val = DiceUIManager.Instance.val;
            Dialogue dialogue = new Dialogue();
            List<Dialogue> choice;
            bool award = false;
            if (val > 10)
            {
                award = true;
                choice = branch1;
            }
            else
            {
                choice = branch2;
            }
            EventHandlerManager.CallUpdateBuff(type, award);
            foreach(var temp in choice)
            {
                Dialogue d = new Dialogue();
                d.m_text = temp.m_text;
                d.m_speaker_avatar = temp.m_speaker_avatar;
                d.m_speaker_name = temp.m_speaker_name;
                d.m_display_method = temp.m_display_method;
                d.m_can_skip = temp.m_can_skip;
                dialogues.Add(d);
            }
            
        }
        DialogueUIManager.OpenDialogueBox(ShowDialogueText, dialogues[0]);
        
        
       
        
    }
}
