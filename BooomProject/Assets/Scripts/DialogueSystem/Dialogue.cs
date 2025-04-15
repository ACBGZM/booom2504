using UnityEngine;

[System.Serializable]
public class Dialogue
{
    public string m_speaker_name;
    public Sprite m_speaker_avatar;
    [TextArea] public string m_text;
    public AdvancedTMProUGUI.TextDisplayMethod m_display_method = AdvancedTMProUGUI.TextDisplayMethod.Typing;
    public bool m_can_skip = true;
}
