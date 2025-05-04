using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AdvancedButton : Button
{
    private Image m_select_border;
    private Animator m_animator;

    private RectTransform m_rect_transform;

    public Action<int> m_confirmed_action;
    private int m_response_index;

    private AdvancedTMProUGUI _advancedText;

    protected override void Awake()
    {
        base.Awake();

        m_select_border = transform.Find("SelectedImage").GetComponent<Image>();
        m_select_border.enabled = false;

        m_animator = GetComponent<Animator>();

        m_rect_transform = GetComponent<RectTransform>();

        onClick.AddListener(OnButtonClicked);
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        Select();
    }

    public override void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);
        m_select_border.enabled = true;
        _advancedText.color = Color.black;

        DialogueUIManager.SetCurrentSelectable(this);
        DialogueUIManager.SetSelectedCursorPosition(transform.position + new Vector3(m_rect_transform.rect.width / 2.0f * 1.1f, m_rect_transform.rect.height / 2.0f, 0.0f));
    }

    public override void OnDeselect(BaseEventData eventData)
    {
        base.OnDeselect(eventData);
        m_select_border.enabled = false;
        _advancedText.color = Color.white;
    }

    public void OnButtonClicked()
    {
        m_animator.SetTrigger(GameProperties.m_click_button_animation_hash);
        DialogueUIManager.GetInstance().PlaySelectCursorAnimation();
    }

    public void Initialize(string text, int index)
    {
        m_response_index = index;

        _advancedText = GetComponentInChildren<AdvancedTMProUGUI>();
        // Debug.Log("Active? " + gameObject.activeInHierarchy);
        _advancedText.StartCoroutine(_advancedText.ShowText(text, AdvancedTMProUGUI.TextDisplayMethod.FadingIn));
    }

    // animation event
    public void Confirm()
    {
        m_confirmed_action(m_response_index);
    }
}
