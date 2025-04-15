
using UnityEngine;

public class GameplaySettings
{
    #region player

    public const float m_walk_speed = 8.0f;

    // player triggered fade
    public const float m_fade_duration = 0.35f;
    public const float m_target_alpha = 0.45f;

    #endregion


    #region dialogue system

    public const uint  m_type_speed = 70;
    public const float m_character_fade_in_duration = 0.1f;
    public const float m_character_fade_out_duration = 0.1f;
    public const float m_dialogue_box_fadein_duration = 0.8f;
    public const float m_dialogue_box_fadeout_duration = 0.5f;
    public const float m_next_cursor_fade_duration = 0.5f;
    public const float m_response_fade_in_duration = 0.5f;
    public const float m_response_fade_out_duration = 0.5f;

    #endregion
}
