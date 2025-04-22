using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class RubyTextInfo
{
    public RubyTextInfo()
    {
        m_begin_index = -1;
        m_end_index = -1;
        m_content = null;
    }

    public RubyTextInfo(int begin_index, string content)
    {
        m_begin_index = begin_index;
        m_end_index = begin_index;
        m_content = content;
    }

    public int m_begin_index { get; set; }
    public int m_end_index { get; set; }
    public string m_content {  get; set; }
}

public class AdvancedTextPreprocessor : ITextPreprocessor
{
    public Dictionary<int, float> m_pause_map { get; private set; }
    public List<RubyTextInfo> m_ruby_list { get; private set; }

    public AdvancedTextPreprocessor()
    {
        m_pause_map = new Dictionary<int, float>();
        m_ruby_list = new List<RubyTextInfo>();
    }

    string ITextPreprocessor.PreprocessText(string text)
    {
        m_pause_map.Clear();
        m_ruby_list.Clear();

        if(string.IsNullOrEmpty(text))
        {
            return "";
        }

        // find all rich text labels from the text string
        // pause labels: record index and remove them from text
        // ruby labels: create ruby data and remove them from text
        // other labels: accumulate pause label index offset
        string processing_text = text;

        int custom_label_index_offset = 0;
        int other_label_index_offset = 0;

        RubyTextInfo ruby = new RubyTextInfo();

        string rich_text_pattern = "<.*?>";

        MatchCollection rich_text_matches = Regex.Matches(processing_text, rich_text_pattern);

        foreach (Match match in rich_text_matches)
        {
            if (!match.Success)
            {
                continue;
            }

            string value = match.Value[1..^1];
            if (float.TryParse(value, out float pause_time))
            {
                int index = match.Index - custom_label_index_offset - other_label_index_offset - 1;
                if (index > 0)
                {
                    m_pause_map[index] = pause_time;
                }

                processing_text = processing_text.Remove(match.Index - custom_label_index_offset, match.Length);
                custom_label_index_offset += match.Length;
            }
            else if (Regex.IsMatch(value, "^r=.+"))
            {
                ruby.m_begin_index = match.Index - custom_label_index_offset - other_label_index_offset;
                ruby.m_content = value.Substring(2);

                processing_text = processing_text.Remove(match.Index - custom_label_index_offset, match.Length);
                custom_label_index_offset += match.Length;
            }
            else if (value == "/r")
            {
                ruby.m_end_index = match.Index - custom_label_index_offset - other_label_index_offset;
                m_ruby_list.Add(ruby);
                ruby = new RubyTextInfo();

                processing_text = processing_text.Remove(match.Index - custom_label_index_offset, match.Length);
                custom_label_index_offset += match.Length;
            }
            else
            {
                other_label_index_offset += match.Length;

                // add a placeholder for sprite label
                if (Regex.IsMatch(value, "^sprite=.+"))
                {
                    other_label_index_offset -= 1;
                }
            }
        }

        return processing_text;
    }

    public bool FindRubyAtBeginIndex(int index, out RubyTextInfo ruby)
    {
        ruby = new RubyTextInfo(0, "");
        foreach (RubyTextInfo info in m_ruby_list)
        {
            if (info.m_begin_index == index)
            {
                ruby = info;
                return true;
            }
        }
        return false;
    }
}

[RequireComponent(typeof(FadeEffect))] // todo: ?
public class AdvancedTMProUGUI : TextMeshProUGUI
{
    public enum TextDisplayMethod
    {
        None = 0,
        DirectShow,
        FadingIn,
        Typing,
    }

    // ruby text
    private GameObject m_ruby_prefab;
    private List<GameObject> m_ruby_text_objects;

    public Action m_finish_action;

    private Coroutine m_typing_coroutine;
    private int m_typing_index;

    [HideInInspector]
    public FadeEffect m_text_fade_effect;

    private AdvancedTextPreprocessor m_custom_text_preprocessor;

    public AdvancedTMProUGUI()
    {
        textPreprocessor = new AdvancedTextPreprocessor();
        m_custom_text_preprocessor = textPreprocessor as AdvancedTextPreprocessor;
    }

    protected override void Awake()
    {
        base.Awake();

        m_ruby_prefab = Resources.Load<GameObject>(GameplaySettings.m_ruby_prefab_path);
        m_ruby_text_objects = new List<GameObject>();

        m_text_fade_effect = gameObject.GetComponent<FadeEffect>();
    }

    public void Initialize()
    {
        SetText("");
        ClearAllRubyText();
    }

    public IEnumerator ShowText(string text, TextDisplayMethod method)
    {
        if (!this.gameObject.activeInHierarchy) yield break;

        Initialize();

        if(m_typing_coroutine != null)
        {
            StopCoroutine(m_typing_coroutine);
            m_typing_coroutine = null;
        }

        ClearAllRubyText();

        SetText(text);
        yield return null;  // wait one frame for text preprocessing

        switch (method)
        {
            case TextDisplayMethod.DirectShow:
            {
                m_text_fade_effect.m_render_opacity = 1.0f;
                CreateAllRubyText();
                m_finish_action?.Invoke();
                break;
            }
            case TextDisplayMethod.FadingIn:
            {
                m_text_fade_effect.m_render_opacity = 0.1f;
                m_text_fade_effect.Fade(1.0f, GameplaySettings.m_character_fade_in_duration, m_finish_action);

                CreateAllRubyText();
                foreach (GameObject ruby in m_ruby_text_objects)
                {
                    ruby.GetComponent<FadeEffect>()?.Fade(1.0f, GameplaySettings.m_character_fade_in_duration, null);
                }
                break;
            }
            case TextDisplayMethod.Typing:
            {
                m_text_fade_effect.Fade(1.0f, GameplaySettings.m_character_fade_in_duration, null);
                m_typing_coroutine = StartCoroutine(Typing());
                break;
            }
            default:
            {
                break;
            }
        }
    }

    IEnumerator Typing()
    {
        ForceMeshUpdate();

        for(int i = 0; i < m_characterCount; ++i)
        {
            ModifyCharacterAlphaAtIndex(i, 0);
        }

        for (m_typing_index = 0; m_typing_index < m_characterCount; ++m_typing_index)
        {
            // display single character
            yield return CharacterFadeIn(m_typing_index);

            // take a pause for next character
            if (m_custom_text_preprocessor.m_pause_map.TryGetValue(m_typing_index, out float pause_time))
            {
                yield return YieldHelper.WaitForSeconds(pause_time, true);
            }
            else
            {
                yield return YieldHelper.WaitForSeconds(1.0f / GameplaySettings.m_type_speed, true);
            }
        }

        m_finish_action?.Invoke();
    }

    IEnumerator CharacterFadeIn(int index)
    {
        if (m_custom_text_preprocessor.FindRubyAtBeginIndex(index, out RubyTextInfo ruby))
        {
            CreateSingleRubyText(ruby);
        }

        float duration = GameplaySettings.m_character_fade_in_duration;
        if (duration <= 0)
        {
            ModifyCharacterAlphaAtIndex(index, 255);
        }
        else
        {
            float time = 0.0f;
            while(time < GameplaySettings.m_character_fade_in_duration)
            {
                time = Mathf.Clamp(time + Time.unscaledDeltaTime, 0.0f, duration);
                ModifyCharacterAlphaAtIndex(index, (byte)(time / duration * 255));
                yield return null;
            }
        }
    }

    private void ModifyCharacterAlphaAtIndex(int index, byte alpha)
    {
        if (!textInfo.characterInfo[index].isVisible)
        {
            return;
        }

        TMP_CharacterInfo character_info = textInfo.characterInfo[index];
        int material_index = character_info.materialReferenceIndex;
        int vertex_index = character_info.vertexIndex;

        for(int i = 0; i < 4; ++i)
        {
            textInfo.meshInfo[material_index].colors32[vertex_index + i].a = alpha;
        }

        UpdateVertexData();
    }

    public void ClearCurrentText()
    {
        m_text_fade_effect?.Fade(0.0f, GameplaySettings.m_character_fade_out_duration, null);
        Initialize();
    }

    public void QuickShowRemainingText()
    {
        if(m_typing_coroutine != null)
        {
            StopCoroutine(m_typing_coroutine);

            ModifyCharacterAlphaAtIndex(m_typing_index++, (byte)(255));

            for (; m_typing_index < m_characterCount; ++m_typing_index)
            {
                StartCoroutine(CharacterFadeIn(m_typing_index));
            }
        }

        m_finish_action?.Invoke();
    }

    private void CreateSingleRubyText(RubyTextInfo ruby_text_info)
    {
        GameObject ruby = Instantiate(m_ruby_prefab, transform);
        ruby.GetComponent<TextMeshProUGUI>().SetText(ruby_text_info.m_content);
        ruby.GetComponent<TextMeshProUGUI>().color = textInfo.characterInfo[ruby_text_info.m_begin_index].color;
        ruby.GetComponent<FadeEffect>().Fade(1.0f, (ruby_text_info.m_end_index - ruby_text_info.m_begin_index) * GameplaySettings.m_character_fade_in_duration, null);
        ruby.transform.localPosition = (textInfo.characterInfo[ruby_text_info.m_begin_index].topLeft + textInfo.characterInfo[ruby_text_info.m_end_index - 1].topRight) / 2.0f;
        ruby.transform.localPosition = new Vector3(ruby.transform.localPosition.x, ruby.transform.localPosition.y - 10, ruby.transform.localPosition.z);
        m_ruby_text_objects.Add(ruby);
    }

    private void CreateAllRubyText()
    {
        foreach (RubyTextInfo info in m_custom_text_preprocessor.m_ruby_list)
        {
            CreateSingleRubyText(info);
        }
    }

    public void ClearAllRubyText()
    {
        foreach (GameObject ruby in m_ruby_text_objects)
        {
            Destroy(ruby);
        }
        m_ruby_text_objects.Clear();
    }
}
