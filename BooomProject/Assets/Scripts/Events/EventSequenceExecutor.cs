using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EventSequence", menuName = "Event/Event Sequence")]
public class EventSequenceExecutor : ScriptableObject
{
    public Action<bool> m_on_finished;

    private int m_index;

    public List<EventNodeBase> m_event_nodes;

    public void Initialize(Action<bool> on_executor_finished)
    {
        m_index = 0;

        foreach (EventNodeBase node in m_event_nodes)
        {
            node.Initialize(OnNodeFinished);
        }

        m_on_finished = on_executor_finished;
    }

    private void OnNodeFinished(bool success)
    {
        if (success)
        {
            ExecuteNextNode();
        }
        else
        {
            m_on_finished?.Invoke(false);
        }
    }

    private void ExecuteNextNode()
    {
        if (m_index < m_event_nodes.Count)
        {
            // todo: should move into specific business event
            if ((m_event_nodes[m_index] is EventShowResponse)
                && (m_event_nodes[m_index] as EventShowResponse).MuseLoopAll()
                && (m_event_nodes[m_index] as EventShowResponse).IsAllLooped())
            {
                ++m_index;
            }

            m_event_nodes[m_index++].Execute();

            // todo: should move into specific business event
            if ((m_event_nodes[m_index - 1] is EventShowResponse)
                && (m_event_nodes[m_index - 1] as EventShowResponse).MuseLoopAll()
                && !(m_event_nodes[m_index - 1] as EventShowResponse).IsAllLooped())
            {
                --m_index;
            }
        }
        else
        {
            m_on_finished?.Invoke(true);
        }
    }

    public void Execute()
    {
        m_index = 0;
        ExecuteNextNode();
    }
}
