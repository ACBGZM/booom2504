using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "AttributionCheckEvent", menuName = "Event/Customer/AttributionCheckEvent")]

public class AttributionCheckEvent : EventNodeBase
{
    // 
    [SerializeField] private EventSequenceExecutor successExecutor;
    [SerializeField] private EventSequenceExecutor failureExecutor;
    // 阈值
    [SerializeField] private int threshold;
    // 判定条件属性
    [SerializeField] private PlayerAttribution playerAttribution;
    private float invalid = -1000;
    
   

    public override void Execute()
    {
        base.Execute();
        float val = playerAttribution switch
        {
            PlayerAttribution.Speed => CommonGameplayManager.GetInstance().PlayerDataManager.Speed.Value - threshold,
            PlayerAttribution.Reputation => CommonGameplayManager.GetInstance().PlayerDataManager.Reputation.Value - threshold,
            _ => invalid

        };
        Debug.Log($"当前声誉值：{CommonGameplayManager.GetInstance().PlayerDataManager.Reputation.Value}");
        Debug.Log($"当前速度值：{CommonGameplayManager.GetInstance().PlayerDataManager.Speed.Value}");

        // 成功事件
        if (val >= 0 && successExecutor != null)
        {
            successExecutor.Initialize(Finished);
            successExecutor.Execute();
        }
        // 失败事件
        else if (val < 0 && val != invalid && failureExecutor != null)
        {
            failureExecutor.Initialize(Finished);
            failureExecutor.Execute();
        }
        else
        {
            Finished(true);
        }
        
        
    }
    private void Finished(bool success)
    {
        
        m_state = EventNodeState.Finished;
        m_on_finished?.Invoke(true);
    }
   
}
