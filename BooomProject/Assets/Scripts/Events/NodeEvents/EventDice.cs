using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EventDice", menuName = "Event/NodeActions/EventDice")]
public class EventDice : EventNodeBase
{
  
    [SerializeField] private EventSequenceExecutor successExecutor;
    [SerializeField] private EventSequenceExecutor failureExecutor;
    public NodeActionType type;

    public override void Execute()
    {
        base.Execute();
        DiceUIManager.Instance.ShowMe();
        ShowShake();
    }

    //public void ShowDialogueText()
    //{
    //    DialogueUIManager.GetInstance().StartCoroutine(StepThroughDialogueDataList());
    //}

    public void ShowShake()
    {
        DiceUIManager.Instance.StartCoroutine(DiceShake());
    }

    //public IEnumerator StepThroughDialogueDataList()
    //{
    //    for (int i = 0; i < dialogues.Count; ++i)
    //    {
    //        DialogueUIManager.SetCanShowNextDialogue(false);

    //        Dialogue dialogue = dialogues[i];

    //        yield return DialogueUIManager.ShowDialogue(dialogue);

    //        yield return new WaitUntil(() => DialogueUIManager.GetCanShowNextDialogue());
    //    }
    //    DiceUIManager.Instance.HideMe();

    //    dialogues.Clear();
    //    m_state = EventNodeState.Finished;
    //    m_on_finished?.Invoke(true);
    //}

    public IEnumerator DiceShake()
    {
        // 骰子触发 需手动点击roll

        // 状态改为触发剧情
        CommonGameplayManager.GetInstance().PlayerState = EPlayerState.InCutscene;

        yield return new WaitUntil(() => DiceUIManager.Instance.val != 0);

        // 骰子结果动画
        yield return new WaitForSeconds(1.5f);
        if (DiceUIManager.Instance.val != 0)
        {
            int val = DiceUIManager.Instance.val;
            // Dialogue dialogue = new Dialogue();
            // List<Dialogue> choice;

            if (val > 10 && successExecutor != null)
            {
                EventHandlerManager.CallUpdateBuff(type, true);
                successExecutor.Initialize(Finished);
                successExecutor.Execute();
            }
            else if (val <= 10 && failureExecutor != null)
            {
                EventHandlerManager.CallUpdateBuff(type, false);
                failureExecutor.Initialize(Finished);
                failureExecutor.Execute();
            }
            else
            {
                Finished(true);
            }
                //foreach (var temp in choice)
                //{
                //    Dialogue d = new Dialogue();
                //    d.m_text = temp.m_text;
                //    d.m_speaker_avatar = temp.m_speaker_avatar;
                //    d.m_speaker_name = temp.m_speaker_name;
                //    d.m_display_method = temp.m_display_method;
                //    d.m_can_skip = temp.m_can_skip;
                //    dialogues.Add(d);
                //}
        }
      //  DialogueUIManager.OpenDialogueBox(ShowDialogueText, dialogues[0]);
    }
    private void Finished(bool success)
    {

        m_state = EventNodeState.Finished;
        m_on_finished?.Invoke(true);
    }
}
