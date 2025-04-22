using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
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
       
        dialogues.Clear();
        m_state = EventNodeState.Finished;
        m_on_finished?.Invoke(true);
    }
    public IEnumerator DiceShake()
    {
        Button startBtn = DiceUIManager.Instance.diceCanvas.GetComponentInChildren<Button>();
        TMP_Text result = DiceUIManager.Instance.diceCanvas.GetComponentInChildren<TMP_Text>();
        startBtn.onClick.Invoke();
        yield return new WaitUntil(() => !result.text.Equals(string.Empty));
        
        if (!result.text.Equals(string.Empty))
        {

            int val = int.Parse(result.text);
            Dialogue dialogue = new Dialogue();
            if (val > 10)
            {
                dialogue.m_text = string.Format("你掷出了'{0}'点！大成功！时间节省 1m！", val);
            }
            else
            {
                dialogue.m_text = string.Format("你掷出了'{0}'点！大失败！时间增加 1m！", val);
            }

            dialogues.Add(dialogue);
        }
        DialogueUIManager.OpenDialogueBox(ShowDialogueText, dialogues[0]);
        
        GameManager.Instance.GameplayState = GameManager.DeliveryGameplayState.InCutscene;
       
        DiceUIManager.Instance.HideMe();
    }
}
