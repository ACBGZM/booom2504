using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "EventSound", menuName = "Event/NodeActions/EventSound")]

public class EventSound : EventNodeBase
{
    public bool successful = false;
    public override void Execute()
    {

        base.Execute();
        if(successful)
        {
            SoundsManager.Instance.audioSource.PlayOneShot(SoundsManager.Instance.good);
        }
        else
        {
            SoundsManager.Instance.audioSource.PlayOneShot(SoundsManager.Instance.bad);
        }
        m_state = EventNodeState.Finished;
        m_on_finished?.Invoke(true);
    }
}
