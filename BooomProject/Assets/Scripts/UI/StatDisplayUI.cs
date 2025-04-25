using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatDisplayUI : MonoBehaviour {
    [SerializeField] private PlayerDataManager _playerDataManager;

    [Header("数值显示")]
    [SerializeField] private TextMeshProUGUI _speed;
    [SerializeField] private TextMeshProUGUI _reputation;
    [SerializeField] private TextMeshProUGUI _rating;
    private int _medals;

    private void Start() {
        // 初始化显示
        UpdateSpeedDisplay(_playerDataManager.Speed.Value);
        UpdateReputationDisplay(_playerDataManager.Reputation.Value);
        UpdateRatingDisplay(_playerDataManager.Rating.Value);

        // 绑定数值变更事件
        _playerDataManager.Speed.OnValueChanged.AddListener(UpdateSpeedDisplay);
        _playerDataManager.Reputation.OnValueChanged.AddListener(UpdateReputationDisplay);
        _playerDataManager.Rating.OnValueChanged.AddListener(UpdateRatingDisplay);
    }

    private void UpdateSpeedDisplay(float value) {
        _speed.text = $"{value:F1}"; // F1格式保留1位小数
    }

    private void UpdateReputationDisplay(int value) {
        _reputation.text = $"{value}";
    }

    private void UpdateRatingDisplay(float value) {
        _rating.text = $"{value:P0}"; // P0格式显示百分比
    }

    private void OnDestroy() {
        if (_playerDataManager != null) {
            _playerDataManager.Speed.OnValueChanged.RemoveListener(UpdateSpeedDisplay);
            _playerDataManager.Reputation.OnValueChanged.RemoveListener(UpdateReputationDisplay);
            _playerDataManager.Rating.OnValueChanged.RemoveListener(UpdateRatingDisplay);
        }
    }
}
