using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FadeEffect))]
public class ResponsePanel : MonoBehaviour
{
    [SerializeField] private RectTransform m_panel_transform;
    [SerializeField] private DialogueUIManager m_dialogue_panel;
    private GameObject m_response_button_prefab;
    private List<AdvancedButton> m_buttons;
    private FadeEffect m_fade_effect;

    private void Start()
    {
        m_buttons = new List<AdvancedButton>();
        m_response_button_prefab = Resources.Load<GameObject>(GameplaySettings.m_response_button_prefab_path);
        m_fade_effect = GetComponent<FadeEffect>();
    }

    public void AddResponseButton(Response response, int index, Action<int> confirm_callback)
    {
        GameObject button_go = Instantiate(m_response_button_prefab);

        AdvancedButton button = button_go.GetComponent<AdvancedButton>();

        button.gameObject.name = "ResponseButton" + index;
        button.Initialize(response.m_text, index);

        button.transform.SetParent(transform);
        button.transform.localScale = Vector3.one;

        button.onClick.AddListener(DisableAllButtons);

        // prevent from creating buttons first and then deleting them during the response looping,
        // first close the panel (delete the button) and then execute the next node.
        button.m_confirmed_action = (_) => { Close(); };
        button.m_confirmed_action += confirm_callback;

        m_buttons.Add(button);
    }

    private void DisableAllButtons()
    {
        foreach (AdvancedButton button in m_buttons)
        {
            button.enabled = false;
        }
    }

    public void Open(int default_select_index)
    {
        gameObject.SetActive(true);

        m_fade_effect.m_render_opacity = 0.0f;
        m_fade_effect.Fade(1.0f, GameplaySettings.m_response_fade_in_duration, () =>
        {
            if(default_select_index >= 0 && default_select_index < m_buttons.Count)
            {
                m_buttons[default_select_index].Select();
            }
            /*
            else
            {
                m_buttons[0].Select();
            }
            */
        });
    }

    public void Close()
    {
        DialogueUIManager.SetCurrentSelectable(null);

        // close immediately to prevent coroutine timing errors during response looping
        m_fade_effect.Fade(0.0f, 0.0f, () =>
        {
            foreach (AdvancedButton button in m_buttons)
            {
                Destroy(button.gameObject);
            }
            m_buttons.Clear();
        });

        DialogueUIManager.HideSelectCursor();
    }
}

