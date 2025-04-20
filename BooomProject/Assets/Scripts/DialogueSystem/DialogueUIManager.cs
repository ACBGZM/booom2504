using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DialogueUIManager : MonoBehaviour
{
    [SerializeField] private DialogueBox m_dialogue_box;
    [SerializeField] private ResponsePanel m_response_panel;
    [SerializeField] private RectTransform m_select_cursor_transform;

    public static bool m_is_showing { get; private set; }

    private static bool m_auto; // todo

    public static bool m_can_show_next_text;
    public static bool m_next_text_force_fadein;
    public static bool GetCanShowNextDialogue() => m_can_show_next_text;
    public static void SetCanShowNextDialogue(bool can_show_next) => m_can_show_next_text = can_show_next;

    private static Selectable m_current_selectable;
    public static void SetCurrentSelectable(Selectable selectable) => m_current_selectable = selectable;

    private static DialogueUIManager s_instance;
    private DialogueUIManager() { }
    public static DialogueUIManager GetInstance() // => s_instance;
    {
        if (s_instance == null)
        {
            lock (typeof(DialogueUIManager))
            {
                if (s_instance == null)
                {
                    GameObject go = new GameObject("DialogueUIManager");
                    s_instance = go.AddComponent<DialogueUIManager>();
                    // DontDestroyOnLoad(go);
                }
            }
        }
        return s_instance;
    }

    private void Awake()
    {
        if (s_instance != null && s_instance != this)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            s_instance = this;
            // DontDestroyOnLoad(gameObject);
        }

        HideSelectCursor();
    }

    private void Update()
    {
        if(EventSystem.current.currentSelectedGameObject == null)
        {
            if(m_current_selectable != null)
            {
                m_current_selectable.Select();
            }
        }
    }

    public void SetNextTextStatus(bool next_text_force_fadein)
    {
        SetCanShowNextDialogue(true);
        m_next_text_force_fadein = next_text_force_fadein;
    }

    public static void CloseDialogueBox(Action close_callback)
    {
        m_is_showing = false;
        s_instance.m_dialogue_box.Close(close_callback);
    }

    public static void OpenDialogueBox(Action open_box_callback, Dialogue first_dialogue)
    {
        m_is_showing = true;
        s_instance.m_dialogue_box.SetFirstDialogue(first_dialogue);   // for showing first speaker's name and avatar
        s_instance.m_dialogue_box.SetNextTextStatusAction(s_instance.SetNextTextStatus);
        s_instance.m_dialogue_box.Open(open_box_callback);

        m_next_text_force_fadein = false; // the first text of each dialogue will use a custom display method
    }

    public static IEnumerator ShowDialogue(Dialogue dialogue)
    {
        yield return s_instance.m_dialogue_box.GetComponent<DialogueBox>().ShowText(dialogue, m_auto, m_next_text_force_fadein);
    }

    #region select response cursor

    public static void SetSelectedCursorPosition(Vector3 position)
    {
        if(!s_instance.m_select_cursor_transform.gameObject.activeSelf)
        {
            s_instance.m_select_cursor_transform.gameObject.SetActive(true);
        }

        s_instance.m_select_cursor_transform.position = position;
    }

    public void PlaySelectCursorAnimation()
    {
        s_instance.m_select_cursor_transform.GetComponent<Animator>().SetTrigger(GameProperties.m_click_button_animation_hash);
    }

    public static void HideSelectCursor()
    {
        s_instance.m_select_cursor_transform.gameObject.SetActive(false);
    }

    #endregion select cursor

    #region responses
    public static void CreateResponseButtons(List<Response> responses, Action<int> confirm_callback, int default_select_index)
    {
        int index = 0;
        foreach(Response response in responses)
        {
            if (response != null && !string.IsNullOrEmpty(response.m_text))
            {
                s_instance.m_response_panel.AddResponseButton(response, index++, confirm_callback);
            }
        }

        s_instance.m_response_panel.Open(default_select_index);

        m_next_text_force_fadein = false; // the first text after the response will use a custom display method
    }

    #endregion responses
}
