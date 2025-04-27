using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(fileName = "DefaultOrderEvent", menuName = "Event/Customer/DefaultOrderEvent")]
// 普通订单默认事件
public class DefaultOrderEvent : EventNodeBase
{
    public List<Dialogue> m_dialogues;
    public override void Execute()
    {
        base.Execute();
        RuntimeOrderSO order = EventHandlerManager.CallGetCurrentOrder();
        if (order != null)
        {
            Dialogue d = new Dialogue();
            d.m_speaker_avatar = null;
            d.m_speaker_name = "？？？";
            d.m_can_skip = true;
            if(order.isTimeout)
            {
                d.m_text = $"{order.sourceOrder.orderTitle} 已超时送达，获得顾客差评！";
                // TODO：获得差评
            }
            else
            {
                d.m_text = $"{order.sourceOrder.orderTitle} 已按时送达，获得顾客好评！";
                // TODO：获得好评
            }
            m_dialogues.Add(d);
            DialogueUIManager.OpenDialogueBox(ShowDialogueText, m_dialogues.First());
            CommonGameplayManager.GetInstance().PlayerState = EPlayerState.InCutscene;
        }
        else
        {
            m_state = EventNodeState.Finished;
            m_on_finished?.Invoke(true);
        }
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
        m_dialogues.Clear();    
        m_state = EventNodeState.Finished;
        m_on_finished?.Invoke(true);
    }
}
