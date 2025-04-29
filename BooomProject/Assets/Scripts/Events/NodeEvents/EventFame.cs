//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//[CreateAssetMenu(fileName = "EventFame", menuName = "Event/NodeActions/EventFame")]
//public class EventFame : EventNodeBase
//{
    
//    [SerializeField] private EventSequenceExecutor successExecutor;
//    [SerializeField] private EventSequenceExecutor failureExecutor;
//    public NodeActionType type;
//    [SerializeField] private int fameThreshold;

//    public override void Execute()
//    {
//        base.Execute();
//        CheckFame();
//    }
//    public void CheckFame()
//    {
//        CommonGameplayManager.GetInstance().PlayerState = EPlayerState.InCutscene;

//        int Fame = CommonGameplayManager.GetInstance().PlayerDataManager.Reputation.Value;
//        Debug.Log(Fame);

//        if (Fame > fameThreshold && successExecutor != null)
//        {
//            successExecutor.Initialize(Finished);
//            successExecutor.Execute();
//            EventHandlerManager.CallUpdateBuff(type, true);

//        }
//        else if (Fame <= fameThreshold && failureExecutor != null)
//        {
            
//            failureExecutor.Initialize(Finished);
//            failureExecutor.Execute();
//            EventHandlerManager.CallUpdateBuff(type, false);
            
//        }
//        else
//        {
//            Finished(true);
//        }
//    }
//    private void Finished(bool success)
//    {
//        DiceUIManager.Instance.HideMe();
//        m_state = EventNodeState.Finished;
//        m_on_finished?.Invoke(true);
//    }
//}

