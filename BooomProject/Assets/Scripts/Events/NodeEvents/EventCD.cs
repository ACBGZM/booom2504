using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "EventCD", menuName = "Event/NodeActions/EventCD")]
public class EventCD : EventNodeBase
{
    // 触发概率阈值
    public float probabilityThreshold = 0.5f;
    public bool needRandom = false;
    public int eventCD = 360;
    public override void Execute()
    {
        base.Execute();

        if(needRandom)
        {
            float probability = Random.value;
            if (probability > probabilityThreshold)
            {
                m_state = EventNodeState.Finished;
                m_on_finished?.Invoke(false);
            }
            else
            {
                ExecuteCD();
            }
        }
        else
        {
            ExecuteCD();
        }



    }
    public void ExecuteCD()
    {
        bool cd = true;
        int currentNodeId = CommonGameplayManager.GetInstance().NodeGraphManager.CurrentNode.NodeID;
        int lastTriggerTime = CommonGameplayManager.GetInstance().NodeGraphManager.GetNodeTriggerTime(currentNodeId);
        // 以分钟为单位
        int currentTime = CommonGameplayManager.GetInstance().TimeManager.currentTime.day * 24 * 60
                          + CommonGameplayManager.GetInstance().TimeManager.currentTime.hour * 60
                          + CommonGameplayManager.GetInstance().TimeManager.currentTime.minute;
        if (lastTriggerTime == 0 || currentTime - lastTriggerTime > eventCD)
        {
            lastTriggerTime = currentTime;
            CommonGameplayManager.GetInstance().NodeGraphManager.SetNodeTriggerTime(currentNodeId, currentTime);
            cd = false;
        }

        m_state = EventNodeState.Finished;
        if (cd)
        {
            Debug.Log($"事件冷却中，冷却时间为{GameplaySettings.TriggerCD - (currentTime - lastTriggerTime)}分钟");
            m_on_finished?.Invoke(false);
        }
        else
        {
            m_on_finished?.Invoke(true);
        }
    }
}
