using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "EventDiceToCustomer", menuName = "Event/Customer/EventDiceToCustomer")]
public class EventDiceToCustomer : EventNodeBase
{

    [SerializeField] private EventSequenceExecutor successExecutor;
    [SerializeField] private EventSequenceExecutor failureExecutor;
    [SerializeField] private int threshold;
    // 2d20触发需要的速度
    [SerializeField] private float needSpeed;
    // 2d20触发需要的声誉
    [SerializeField] private int needReputation;
    // 2d20触发需要的好评率
    [SerializeField] private float needEvaluation;
    // 事件参考的人物属性
    [SerializeField] private PlayerAttribution reference;
    // 奖励的属性
    [SerializeField] private PlayerAttribution awardType;

    [SerializeField] private float awardValue;
    [SerializeField] private float punishmentSpeed;

    public override void Execute()
    {
        base.Execute();


        ShowShake();
    }

    public void ShowShake()
    {
        Debug.Log(CommonGameplayManager.GetInstance().PlayerDataManager.Speed.Value);

        bool twoDice = reference switch
        {
            PlayerAttribution.Speed => CommonGameplayManager.GetInstance().PlayerDataManager.Speed.Value >= needSpeed,
            PlayerAttribution.Reputation=> CommonGameplayManager.GetInstance().PlayerDataManager.Reputation.Value >= needReputation,
            PlayerAttribution.Evaluation => CommonGameplayManager.GetInstance().PlayerDataManager.Rating.Value >= needEvaluation,
            _ => false
        };
        Debug.Log(twoDice);
        CommonGameplayManager.GetInstance().StartCoroutine(DiceShake(twoDice));
    }


    public IEnumerator DiceShake(bool twoDice)
    {
        // 骰子触发 需手动点击roll

        // 状态改为触发剧情
        CommonGameplayManager.GetInstance().PlayerState = EPlayerState.InCutscene;
        // 骰子点数
        int val = 0;
        if (twoDice)
        {
            TwoDiceUIManager.Instance.ShowMe();
            yield return new WaitUntil(() => TwoDiceUIManager.Instance.result.text != string.Empty);
            val = TwoDiceUIManager.Instance.val;
        }
        else
        {
            DiceUIManager.Instance.ShowMe();
            yield return new WaitUntil(() => DiceUIManager.Instance.result.text != string.Empty);
            val = DiceUIManager.Instance.val;
        }
        Debug.Log(val);
        // 骰子结果动画
        yield return new WaitForSeconds(1.5f);
        //if (DiceUIManager.Instance.val != 0)
        //{


        if (val > threshold && successExecutor != null)
        {
            successExecutor.Initialize(Finished);
            successExecutor.Execute();
            EventHandlerManager.CallUpdateAttribution(awardType, awardValue);

        }
        else if (val <= threshold && failureExecutor != null)
        {
            failureExecutor.Initialize(Finished);
            failureExecutor.Execute();
            EventHandlerManager.CallUpdateAttribution(awardType, punishmentSpeed);
        }
        else
        {
            Finished(true);
        }
        //}

    }
    private void Finished(bool success)
    {
        DiceUIManager.Instance.HideMe();
        TwoDiceUIManager.Instance.HideMe();
        m_state = EventNodeState.Finished;
        m_on_finished?.Invoke(true);
    }
}
