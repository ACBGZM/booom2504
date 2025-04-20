using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "EventShowResponse", menuName = "Event/Show Response")]
public class EventShowResponse : EventNodeBase
{
    // When a player selects a response, the corresponding executor will be triggered.
    // Once the executor is complete, the entile node will be concluded.
    public List<Response> m_responses;
    public int m_default_select_index;
    public bool m_must_loop_all;

    private List<bool> m_rensponse_looped;

    public bool MuseLoopAll() => m_must_loop_all;

    public override void Initialize(Action<bool> on_finished)
    {
        base.Initialize(on_finished);

        m_rensponse_looped = new List<bool>();

        foreach (Response response in m_responses)
        {
            EventSequenceExecutor executor = response.m_executor;
            if (executor != null)
            {
                executor.Initialize(m_on_finished);   // conclusion of any executor signifies the end of the entire node
            }

            m_rensponse_looped.Add(false);
        }
    }

    public override void Execute()
    {
        base.Execute();

        DialogueUIManager.CreateResponseButtons(m_responses, OnResponseConfirmed, m_default_select_index);
    }

    private void OnResponseConfirmed(int index)
    {
        if (index < m_responses.Count && m_responses[index] != null)
        {
            m_rensponse_looped[index] = true;
        }

        if (index < m_responses.Count && m_responses[index] != null && m_responses[index].m_executor != null)
        {
            m_responses[index].m_executor.Execute();
        }
        else
        {
            m_on_finished(true);
        }
    }

    public bool IsAllLooped()
    {
        return m_rensponse_looped.All(x => x);
    }
}
