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
        if(order != null && !order.isTimeout && goodEvaluationSequence != null)
        {
            goodEvaluationSequence.Initialize(Finished);
            goodEvaluationSequence.Execute();
            //  $"{order.sourceOrder.orderTitle} 已超时送达，获得顾客差评！";
            // TODO：获得差评
        }
        else if(order != null &&order.isTimeout && badEvaluationSequence != null)
        {
            badEvaluationSequence.Initialize(Finished);
            badEvaluationSequence.Execute();
            //  $"{order.sourceOrder.orderTitle} 已按时送达，获得顾客好评！";
            // TODO：获得好评
        }
        else
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
