using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(fileName = "DefaultOrderEvent", menuName = "Event/Customer/DefaultOrderEvent")]
// 普通订单默认事件
public class DefaultOrderEvent : EventNodeBase
{
    [SerializeField] private EventSequenceExecutor goodEvaluationSequence;
    [SerializeField] private EventSequenceExecutor badEvaluationSequence;
    public override void Execute()
    {
        base.Execute();
        RuntimeOrderSO order = EventHandlerManager.CallGetCurrentOrder();

        Debug.Log($"剩余时间：{order.remainingMinutes}    是否超时：{order.isTimeout}");
        bool excuteGood = false;
        bool excuteBad = false;
        if (order != null && goodEvaluationSequence != null)
        {
            // 聊天触发好评 或者 聊天未触发且订单未超时， 结算订单好评
            if (order.orderEvaluation == Evaluation.Good ||
                order.orderEvaluation == Evaluation.None && !order.isTimeout)
            {
                goodEvaluationSequence.Initialize(Finished);
                goodEvaluationSequence.Execute();
                EventHandlerManager.CallUpGoodOrderCount();
                excuteGood = true;
            }

        }
        if(order != null && badEvaluationSequence != null)
        {
            // 聊天触发差评 或者 聊天未触发且订单超时， 结算订单差评
            if (order.orderEvaluation == Evaluation.Bad ||
               order.orderEvaluation == Evaluation.None && order.isTimeout)
            {
                badEvaluationSequence.Initialize(Finished);
                badEvaluationSequence.Execute();
                EventHandlerManager.CallUpBadOrderCount();
                excuteBad = true;
            }
        }
        // 两个都没执行
        if(!excuteGood && !excuteBad)
        {
            m_state = EventNodeState.Finished;
            m_on_finished?.Invoke(true);
        }
        
    }
 
    private void Finished(bool success)
    {

        m_state = EventNodeState.Finished;
        m_on_finished?.Invoke(true);
    }

}
