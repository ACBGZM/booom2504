using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EventTrafficLight", menuName = "Event/NodeActions/EventTrafficLight")]
public class EventTrafficLight : EventNodeBase
{
    public List<Dialogue> dialogues = new List<Dialogue>();

    public override void Execute()
    {
        base.Execute();
        DiceUIManager.Instance.ShowMe();
        TrafficLightArise();
    }


    public void ShowDialogueText()
    {
        DialogueUIManager.GetInstance().StartCoroutine(StepThroughDialogueDataList());
    }
    public void TrafficLightArise()
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
     
        // ���Ӵ��� ���ֶ����roll
       
        // ״̬��Ϊ��������
        GameManager.Instance.GameplayState = GameManager.DeliveryGameplayState.InCutscene;

        yield return new WaitUntil(() => DiceUIManager.Instance.val != 0);
        
        // ���ӽ������
        yield return new WaitForSeconds(2);
        if (DiceUIManager.Instance.val != 0)
        {

            int val = DiceUIManager.Instance.val;
            Dialogue dialogue = new Dialogue();
            if (val > 10)
            {
                dialogue.m_text = string.Format("�������ƿ�������������˽ý���������û�������κζ���Ĺ�ע��ʱ���ʡ 1m��");
            }
            else
            {
                dialogue.m_text = string.Format("һ������������������͵�ɲ����������һ���亹��ʱ������ 3m��");
            }

            dialogues.Add(dialogue);
        }
        DialogueUIManager.OpenDialogueBox(ShowDialogueText, dialogues[0]);
        
        
       
        
    }
}
