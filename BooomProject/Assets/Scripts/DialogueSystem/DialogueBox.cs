using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DialogueBox : MonoBehaviour
{
    [SerializeField] private AdvancedTMProUGUI m_text_content;

    [SerializeField] private GameObject m_character_panel;
    private AdvancedTMProUGUI m_speaker_name_text;
    private Image m_speaker_avatar;

    [SerializeField] private GameObject m_cg_panel;
    [SerializeField] private Image m_cg_image;

    private FadeEffect m_box_fade_effect;

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
                m_next_text_status_action(true);
            }
            else if (EventHandlerManager.CallGetCommonInputHandler().UIInputActions.Submit.triggered)
            {
                m_is_interactable = false;
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

        if (m_speaker_avatar.sprite != null)
        {
            m_speaker_avatar.GetComponent<FadeEffect>()
                ?.Fade(1.0f, GameplaySettings.m_dialogue_box_fadein_duration, null);
        }
        else
        {
            m_speaker_avatar.GetComponent<CanvasGroup>().alpha = 0.0f;
        }

        if (m_cg_image.sprite != null)
        {
            m_cg_panel.GetComponent<FadeEffect>()
                ?.Fade(1.0f, GameplaySettings.m_dialogue_box_fadein_duration, null);
        }
        else
        {
            m_cg_panel.GetComponent<CanvasGroup>().alpha = 0.0f;
        }

        m_text_content.StartCoroutine(m_text_content.ShowText(dialogue.m_text, next_text_method));
    }

    public void Open(Action callback)
    {
        if (m_first_dialogue != null)
        {
            m_speaker_name_text.SetText(m_first_dialogue.m_speaker_name);
            m_speaker_avatar.sprite = m_first_dialogue.m_speaker_avatar;
            m_cg_image.sprite = m_first_dialogue.m_cg_sprite;
        }

        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);

            m_box_fade_effect.m_render_opacity = 0.0f;
            m_box_fade_effect.Fade(1.0f, GameplaySettings.m_dialogue_box_fadein_duration, null);

            if (m_speaker_avatar.sprite != null)
            {
                m_speaker_avatar.GetComponent<FadeEffect>()
                    ?.Fade(1.0f, GameplaySettings.m_dialogue_box_fadein_duration, null);
            }

            if (m_cg_image.sprite != null)
            {
                m_cg_panel.GetComponent<FadeEffect>()
                    ?.Fade(1.0f, GameplaySettings.m_dialogue_box_fadein_duration, null);
            }

            m_speaker_name_text.GetComponent<FadeEffect>()
                ?.Fade(1.0f, GameplaySettings.m_dialogue_box_fadein_duration, callback);

        }
        else
        {
            callback?.Invoke();
        }

        m_text_content.m_finish_action = CurrentTextFinish;
        m_text_content.Initialize();
    }
}
