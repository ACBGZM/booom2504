using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class Dialogue
{
    public string m_speaker_name;
    public Sprite m_speaker_avatar;
    [TextArea] public string m_text;
    public AdvancedTMProUGUI.TextDisplayMethod m_display_method = AdvancedTMProUGUI.TextDisplayMethod.Typing;
    public bool m_can_skip = true;

    public bool m_use_layout_1 = true;
    public Sprite m_cg_sprite;
}
