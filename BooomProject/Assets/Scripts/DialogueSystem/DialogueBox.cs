using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueBox : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Image m_background;
    private FadeEffect m_box_fade_effect;

    [SerializeField] private AdvancedTMProUGUI m_text_content;

    [SerializeField] private GameObject m_character_panel;
    private AdvancedTMProUGUI m_speaker_name_text;
    private Image m_speaker_avatar;

    [SerializeField] private GameObject m_next_cursor;
    private FadeEffect m_next_cursor_fade_effect;
    private Animator m_next_cursor_animator;

    [Header("Configurations")]
    [SerializeField] private List<Sprite> m_background_images;

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
        GetComponent<CanvasGroup>().alpha = 0.89f;
        gameObject.SetActive(false);

        m_box_fade_effect = GetComponent<FadeEffect>();

        m_speaker_name_text = m_character_panel.GetComponentInChildren<AdvancedTMProUGUI>();
        m_speaker_avatar = m_character_panel.GetComponentInChildren<Image>();

        m_next_cursor_fade_effect = m_next_cursor.GetComponent<FadeEffect>();
        m_next_cursor_animator = m_next_cursor.GetComponent<Animator>();

        m_text_content.m_finish_action = CurrentTextFinish;
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
            if (GameManager.Instance.DeliverySceneInputHandler.UIInputActions.Cancel.triggered)
            {
                m_is_interactable = false;
                m_next_cursor_animator.SetTrigger(GameProperties.m_next_cursor_click_animation_hash);
                m_next_cursor_fade_effect.Fade(0.0f, GameplaySettings.m_next_cursor_fade_duration, null);

                m_next_text_status_action(true);
            }
            else if (GameManager.Instance.DeliverySceneInputHandler.UIInputActions.Submit.triggered)
            {
                m_is_interactable = false;
                m_next_cursor_animator.SetTrigger(GameProperties.m_next_cursor_click_animation_hash);
                m_next_cursor_fade_effect.Fade(0.0f, GameplaySettings.m_next_cursor_fade_duration, null);

                m_next_text_status_action(false);
            }
        }
        else
        {
            if (GameManager.Instance.DeliverySceneInputHandler.UIInputActions.Cancel.triggered)
            {
                if (m_text_can_skip)
                {
                    m_text_content.QuickShowRemainingText();
                }
            }
            else if (GameManager.Instance.DeliverySceneInputHandler.UIInputActions.Submit.triggered)
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

        AdvancedTMProUGUI.TextDisplayMethod next_text_method = dialogue.m_display_method;
        if (next_force_fadein && m_text_can_skip)   // can not force fading in a dialogue which can not be skipped
        {
            next_text_method = AdvancedTMProUGUI.TextDisplayMethod.FadingIn;
        }

        if (next_text_method == AdvancedTMProUGUI.TextDisplayMethod.Typing)
        {
            m_is_interactable = true;
        }
        this.gameObject.SetActive(true);
        if (!m_text_content.gameObject.activeInHierarchy) {
            m_text_content.gameObject.SetActive(true);
        }
        m_text_content.StartCoroutine(m_text_content.ShowText(dialogue.m_text, next_text_method));
    }

    public void Open(Action callback)
    {
        m_background.sprite = m_background_images[1];

        if (m_first_dialogue != null)
        {
            m_speaker_name_text.SetText(m_first_dialogue.m_speaker_name);
            m_speaker_avatar.sprite = m_first_dialogue.m_speaker_avatar;
        }

        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
            m_box_fade_effect.m_render_opacity = 0.0f;
            m_box_fade_effect.Fade(1.0f, GameplaySettings.m_dialogue_box_fadein_duration, null);
            m_speaker_name_text.GetComponent<FadeEffect>()?.Fade(1.0f, GameplaySettings.m_dialogue_box_fadein_duration, null);
            m_speaker_avatar.GetComponent<FadeEffect>()?.Fade(1.0f, GameplaySettings.m_dialogue_box_fadein_duration, callback);
        }
        else
        {
            callback?.Invoke();
        }

        m_text_content.Initialize();

        m_next_cursor_fade_effect.m_render_opacity = 0.0f;
    }
}
