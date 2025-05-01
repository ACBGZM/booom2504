using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueBox : MonoBehaviour
{
// ==========================================
    [Header("Components - Layout 1")]
    [SerializeField] private GameObject m_layout_1;

    [SerializeField] private AdvancedTMProUGUI m_text_content_l1;

    [SerializeField] private GameObject m_character_panel;
    private AdvancedTMProUGUI m_speaker_name_text;
    private Image m_speaker_avatar;

    [SerializeField] private GameObject m_next_cursor_l1;
    private FadeEffect m_next_cursor_fade_effect_l1;
    private Animator m_next_cursor_animator_l1;

// ==========================================
    [Header("Components - Layout 2")]
    [SerializeField] private GameObject m_layout_2;
    [SerializeField] private AdvancedTMProUGUI m_text_content_l2;
    [SerializeField] private GameObject m_next_cursor_l2;
    [SerializeField] private Image m_cg_image;
    private FadeEffect m_next_cursor_fade_effect_l2;
    private Animator m_next_cursor_animator_l2;

// ==========================================
    private AdvancedTMProUGUI m_text_content;
    private GameObject m_next_cursor;
    private FadeEffect m_next_cursor_fade_effect;
    private Animator m_next_cursor_animator;

    private FadeEffect m_box_fade_effect;

    private bool m_use_layout_1 = true;

    private bool m_is_interactable;
    private bool m_is_show_finished;
    private bool m_text_can_skip;
    private bool m_is_auto_play;

    private Action<bool> m_next_text_status_action;

    public void SetNextTextStatusAction(Action<bool> action) => m_next_text_status_action = action;

    private Dialogue m_first_dialogue;

    public void SetFirstDialogue(Dialogue first_dialogue) => m_first_dialogue = first_dialogue;

    public void Awake()
    {
        // hack: set enabled but alpha to 0 in editor to ensure awake() is executed in game
        GetComponent<CanvasGroup>().alpha = 1.0f;
        gameObject.SetActive(false);

        m_box_fade_effect = GetComponent<FadeEffect>();

        m_speaker_name_text = m_character_panel.GetComponentInChildren<AdvancedTMProUGUI>();
        m_speaker_avatar = m_character_panel.GetComponentInChildren<Image>();

        m_next_cursor_fade_effect_l1 = m_next_cursor_l1.GetComponent<FadeEffect>();
        m_next_cursor_animator_l1 = m_next_cursor_l1.GetComponent<Animator>();

        m_next_cursor_fade_effect_l2 = m_next_cursor_l2.GetComponent<FadeEffect>();
        m_next_cursor_animator_l2 = m_next_cursor_l2.GetComponent<Animator>();
    }

    public void Update()
    {
        if (m_is_interactable)
        {
            UpdateInput();
        }
    }

    public void UpdateInput()
    {
        if (m_is_show_finished)
        {
            if (EventHandlerManager.CallGetCommonInputHandler().UIInputActions.Cancel.triggered)
            {
                m_is_interactable = false;
                m_next_cursor_animator.SetTrigger(GameProperties.m_next_cursor_click_animation_hash);
                m_next_cursor_fade_effect.Fade(0.0f, GameplaySettings.m_next_cursor_fade_duration, null);

                m_next_text_status_action(true);
            }
            else if (EventHandlerManager.CallGetCommonInputHandler().UIInputActions.Submit.triggered)
            {
                m_is_interactable = false;
                m_next_cursor_animator.SetTrigger(GameProperties.m_next_cursor_click_animation_hash);
                m_next_cursor_fade_effect.Fade(0.0f, GameplaySettings.m_next_cursor_fade_duration, null);

                m_next_text_status_action(false);
            }
        }
        else
        {
            if (EventHandlerManager.CallGetCommonInputHandler().UIInputActions.Cancel.triggered)
            {
                if (m_text_can_skip)
                {
                    m_text_content.QuickShowRemainingText();
                }
            }
            else if (EventHandlerManager.CallGetCommonInputHandler().UIInputActions.Submit.triggered)
            {
                if (m_text_can_skip)
                {
                    m_text_content.QuickShowRemainingText();
                }
            }
        }
    }

    private void CurrentTextFinish()
    {
        if (m_is_auto_play)
        {
            m_is_interactable = false;
        }
        else
        {
            m_is_interactable = true;
            m_is_show_finished = true;
            m_next_cursor_fade_effect.Fade(1, 0.5f, null);
        }
    }

    public void Close(Action close_callback)
    {
        m_box_fade_effect.Fade(0.0f, GameplaySettings.m_dialogue_box_fadein_duration, () =>
        {
            gameObject.SetActive(false);
            close_callback?.Invoke();
        });
    }

    public IEnumerator ShowText(Dialogue dialogue, bool auto_next, bool next_force_fadein)
    {
        m_is_interactable = false;
        m_is_show_finished = false;

        if (!string.IsNullOrEmpty(m_text_content.text))
        {
            m_text_content.ClearCurrentText();
            yield return YieldHelper.WaitForSeconds(GameplaySettings.m_character_fade_out_duration, true);
        }

        m_text_can_skip = dialogue.m_can_skip;
        m_is_auto_play = auto_next;

        m_speaker_name_text.SetText(dialogue.m_speaker_name);
        m_speaker_avatar.sprite = dialogue.m_speaker_avatar;

        m_cg_image.sprite = dialogue.m_cg_sprite;

        AdvancedTMProUGUI.TextDisplayMethod next_text_method = dialogue.m_display_method;
        if (next_force_fadein && m_text_can_skip) // can not force fading in a dialogue which can not be skipped
        {
            next_text_method = AdvancedTMProUGUI.TextDisplayMethod.FadingIn;
        }

        if (next_text_method == AdvancedTMProUGUI.TextDisplayMethod.Typing)
        {
            m_is_interactable = true;
        }

        this.gameObject.SetActive(true);
        if (!m_text_content.gameObject.activeInHierarchy)
        {
            m_text_content.gameObject.SetActive(true);
        }

        m_text_content.StartCoroutine(m_text_content.ShowText(dialogue.m_text, next_text_method));
    }

    public void Open(Action callback)
    {
        if (m_first_dialogue != null)
        {
            m_use_layout_1 = m_first_dialogue.m_use_layout_1;
            if (m_first_dialogue.m_use_layout_1)
            {
                m_speaker_name_text.SetText(m_first_dialogue.m_speaker_name);
                m_speaker_avatar.sprite = m_first_dialogue.m_speaker_avatar;
            }
            else
            {
                m_cg_image.sprite = m_first_dialogue.m_cg_sprite;
            }
        }

        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);

            if (m_use_layout_1)
            {
                m_layout_1.SetActive(true);
                m_layout_2.SetActive(false);
                m_text_content = m_text_content_l1;
                m_next_cursor = m_next_cursor_l1;
                m_next_cursor_fade_effect = m_next_cursor_fade_effect_l1;
                m_next_cursor_animator = m_next_cursor_animator_l1;
            }
            else
            {
                m_layout_1.SetActive(false);
                m_layout_2.SetActive(true);
                m_text_content = m_text_content_l2;
                m_next_cursor = m_next_cursor_l2;
                m_next_cursor_fade_effect = m_next_cursor_fade_effect_l2;
                m_next_cursor_animator = m_next_cursor_animator_l2;
            }

            m_box_fade_effect.m_render_opacity = 0.0f;
            m_box_fade_effect.Fade(1.0f, GameplaySettings.m_dialogue_box_fadein_duration, null);

            if (m_use_layout_1)
            {
                m_speaker_name_text.GetComponent<FadeEffect>()
                    ?.Fade(1.0f, GameplaySettings.m_dialogue_box_fadein_duration, null);
                m_speaker_avatar.GetComponent<FadeEffect>()
                    ?.Fade(1.0f, GameplaySettings.m_dialogue_box_fadein_duration, callback);
            }
            else
            {
                m_cg_image.GetComponent<FadeEffect>()
                    ?.Fade(1.0f, GameplaySettings.m_dialogue_box_fadein_duration, callback);
            }
        }
        else
        {
            callback?.Invoke();
        }

        m_text_content.m_finish_action = CurrentTextFinish;

        m_text_content.Initialize();

        m_next_cursor_fade_effect.m_render_opacity = 0.0f;
    }
}
