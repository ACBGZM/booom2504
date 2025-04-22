using UnityEngine;

public class GameplaySettings {
    #region player
    public const float m_walk_speed = 6.0f;
    public const float m_acceleration_Time = 0.05f;     // 达到最快速度的时间
    // player triggered fade
    //public const float m_fade_duration = 0.35f;
    //public const float m_target_alpha = 0.45f;
    #endregion

    #region map
    public const float map_move_speed = 10f;
    public const float map_scroll_speed = 5f;
    public const float map_drag_speed = 50f;
    #endregion

    #region chat
    public const string left_item_prefab_path = "Prefabs/UI/ChatItemLeft";
    public const string right_item_prefab_path = "Prefabs/UI/ChatItemRight";
    #endregion

    #region dialogue system
    public const uint m_type_speed = 70;
    public const float m_character_fade_in_duration = 0.1f;
    public const float m_character_fade_out_duration = 0.1f;
    public const float m_dialogue_box_fadein_duration = 0.8f;
    public const float m_dialogue_box_fadeout_duration = 0.5f;
    public const float m_next_cursor_fade_duration = 0.5f;
    public const float m_response_fade_in_duration = 0.5f;
    public const float m_response_fade_out_duration = 0.5f;
    public const string m_response_button_prefab_path = "Prefabs/ResponseButton";
    public const string m_ruby_prefab_path = "Prefabs/RubyText";
    #endregion

    #region Dice
    // 骰子面朝上的精确匹配值
    public const float exact_match_value = 0.995f;
    #endregion
}
