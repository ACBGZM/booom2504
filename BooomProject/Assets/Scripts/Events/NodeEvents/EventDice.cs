using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EventDice", menuName = "Event/NodeActions/EventDice")]
public class EventDice : EventNodeBase
{
  
    [SerializeField] private EventSequenceExecutor successExecutor;
    [SerializeField] private EventSequenceExecutor failureExecutor;
    public NodeActionType type;
    [SerializeField] private int TrafficThreshold;

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

            if (val > TrafficThreshold && successExecutor != null)
            {
                successExecutor.Initialize(Finished);
                successExecutor.Execute();
                EventHandlerManager.CallUpdateBuff(type, true);
                
            }
            else if (val <= TrafficThreshold && failureExecutor != null)
            {
                failureExecutor.Initialize(Finished);
                failureExecutor.Execute();
                EventHandlerManager.CallUpdateBuff(type, false); 
            }
            else
            {
                Finished(true);
            }
        }

    }
    private void Finished(bool success)
    {
        DiceUIManager.Instance.HideMe();
        m_state = EventNodeState.Finished;
        m_on_finished?.Invoke(true);
    }
}
