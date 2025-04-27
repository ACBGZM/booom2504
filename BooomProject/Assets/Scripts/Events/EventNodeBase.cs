using System;
using UnityEngine;

public enum EventNodeState {
    Waiting = 0,
    Executing,
    Finished,
}

public class EventNodeBase : ScriptableObject {
    protected Action<bool> m_on_finished;
    [HideInInspector] public EventNodeState m_state;

    public virtual void Initialize(Action<bool> on_finished) {
        m_on_finished = on_finished;
        m_state = EventNodeState.Waiting;
    }

    public virtual void Execute() {
        if (m_state == EventNodeState.Executing) {
            Debug.LogWarning($"节点 {name} 执行。");
        } else if (m_state == EventNodeState.Finished) {
            return;
        }
        m_state = EventNodeState.Executing;
    }
}
