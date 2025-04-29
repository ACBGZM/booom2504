using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "EvaluationEvent", menuName = "Event/Customer/EvaluationEvent")]
public class EvaluationEvent : EventNodeBase
{
    // 是否为好评
    public bool good;
    public override void Execute()
    {
        base.Execute();
        if(good)
        {
            // TODO: 订单评价
            // 好评订单 + 1
            EventHandlerManager.CallUpGoodOrderCount();
        }
        else
        {
            // 差评订单 + 1
            EventHandlerManager.CallUpBadOrderCount();
        }
        m_state = EventNodeState.Finished;
        m_on_finished?.Invoke(true);
    }

}
