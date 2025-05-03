using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatDisplayUI : MonoBehaviour {
    private PlayerDataManager _playerDataManager;

    [Header("数值显示")]
    [SerializeField] private TextMeshProUGUI _speed;
    [SerializeField] private TextMeshProUGUI _reputation;
    [SerializeField] private TextMeshProUGUI _rating;
    [SerializeField] private Transform _medal;
    [SerializeField] private Transform _medalContainer;
    [SerializeField] private Sprite[] _medalSprites;
    private int _medals;

    private void Start() {
        _playerDataManager = CommonGameplayManager.GetInstance().PlayerDataManager;
        _medals = _playerDataManager.Medals.Value;
        // 初始化显示
        UpdateSpeedDisplay(_playerDataManager.Speed.Value);
        UpdateReputationDisplay(_playerDataManager.Reputation.Value);
        UpdateRatingDisplay(_playerDataManager.Rating.Value);
        UpdateMedalDisplay(_medals);
        // 绑定数值变更事件
        _playerDataManager.Speed.OnValueChanged.AddListener(UpdateSpeedDisplay);
        _playerDataManager.Reputation.OnValueChanged.AddListener(UpdateReputationDisplay);
        _playerDataManager.Rating.OnValueChanged.AddListener(UpdateRatingDisplay);
        _playerDataManager.Medals.OnValueChanged.AddListener(UpdateMedalDisplay);
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

    private void UpdateMedalDisplay(int value) {
        for (int i = 0; i < value; i++) {
            Transform medal = Instantiate(_medal, _medalContainer);
            medal.Find("MedalSprite").GetComponent<Image>().sprite = _medalSprites[Random.Range(0, _medalSprites.Length)];
        }
    }

    private void OnDestroy() {
        if (_playerDataManager != null) {
            _playerDataManager.Speed.OnValueChanged.RemoveListener(UpdateSpeedDisplay);
            _playerDataManager.Reputation.OnValueChanged.RemoveListener(UpdateReputationDisplay);
            _playerDataManager.Rating.OnValueChanged.RemoveListener(UpdateRatingDisplay);
        }
    }
}
