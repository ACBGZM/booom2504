using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AdvancedButton_MainMenu : Button
{
    private Image[] m_select_border;
    private Animator m_animator;

    public Action<int> m_confirmed_action;
    private int m_response_index;

    private TextMeshProUGUI _text;

    protected override void Awake()
    {
        base.Awake();

        m_select_border = transform.GetComponentsInChildren<Image>()[1..];
        foreach (Image img in m_select_border)
        {
            img.enabled = false;
        }

        m_animator = GetComponent<Animator>();

        onClick.AddListener(OnButtonClicked);

        _text = GetComponentInChildren<TextMeshProUGUI>();
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        foreach (Image img in m_select_border)
        {
            img.enabled = true;
        }

        _text.color = Color.black;
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        foreach (Image img in m_select_border)
        {
            img.enabled = false;
        }

        _text.color = Color.white;
    }

    public void OnButtonClicked()
    {
        m_animator.SetTrigger(GameProperties.m_click_button_animation_hash);
    }

    // animation event
    public void Confirm()
    {
        m_confirmed_action(m_response_index);
    }
}
