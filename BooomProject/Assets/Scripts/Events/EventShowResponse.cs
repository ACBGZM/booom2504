using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "EventShowResponse", menuName = "Event/Show Response")]
public class EventShowResponse : EventNodeBase {
    // 玩家选择响应后触发相应的子执行器
    public List<Response> m_responses;
    public int m_default_select_index;
    public bool m_must_loop_all;

    private List<bool> m_rensponse_looped = new List<bool>();

    public bool MuseLoopAll() => m_must_loop_all;

    public override void Initialize(Action<bool> on_finished) {
        // 调用基类初始化（一般会设置 m_on_finished ）
        base.Initialize(on_finished);

        //m_rensponse_looped = new List<bool>();

        foreach (Response response in m_responses) {
            // EventSequenceExecutor executor = response.m_executor;
            // if (executor != null)
            // {
            //     executor.Initialize(m_on_finished);
            // }

            m_rensponse_looped.Add(false);
        }
    }

    public override void Execute() {
        base.Execute();

        // 创建响应按钮，让玩家进行选择；默认选中 m_default_select_index
        DialogueUIManager.CreateResponseButtons(m_responses, OnResponseConfirmed, m_default_select_index);
    }

    private void OnResponseConfirmed(int index) {
        if (index < m_responses.Count && m_responses[index] != null) {
            m_rensponse_looped[index] = true;
        }

        if (index < m_responses.Count &&
            m_responses[index] != null &&
            m_responses[index].m_executor != null) {
            // 初始化并执行响应的子执行器
            m_responses[index].m_executor.Initialize((bool result) => {
                // 当子执行器结束后，结束当前节点
                m_on_finished?.Invoke(result);
            });
            m_responses[index].m_executor.Execute();
        } else {
            // 如果没有子执行器，则直接结束当前节点
            m_on_finished?.Invoke(true);
        }
    }

    public bool IsAllLooped() {
        return m_rensponse_looped.All(x => x);
    }
}
