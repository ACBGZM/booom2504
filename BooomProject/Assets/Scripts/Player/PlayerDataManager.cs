using UnityEngine;
using System.Collections.Generic;

public class PlayerDataManager : MonoBehaviour {
    [Header("基础属性")]
    public BaseStat<float> Speed = new BaseStat<float>("Speed", 5f, 0f, 15f);
    public BaseStat<int> Reputation = new BaseStat<int>("Reputation", 0, 0, 1000);
    public BaseStat<float> Rating = new BaseStat<float>("Rating", 0.8f, 0f, 1f);
    public BaseStat<int> Medals = new BaseStat<int>("Medals", 0, 0, int.MaxValue);
    
    // [Header("自动保存设置")]
    // [SerializeField] private bool _autoSave = true;
    // [SerializeField] private float _saveInterval = 60f;

    private float _saveTimer;

    void Start() {
        LoadData();
        SetupEventListeners();
    }

    // void Update() {
    //     if (_autoSave) {
    //         _saveTimer += Time.deltaTime;
    //         if (_saveTimer >= _saveInterval) {
    //             SaveData();
    //             _saveTimer = 0f;
    //         }
    //     }
    // }

    private void SetupEventListeners() {
        Speed.OnValueChanged.AddListener(v => CheckSpeedAchievements(v));
        Reputation.OnValueChanged.AddListener(CheckReputationMilestones);
    }

    private void CheckSpeedAchievements(float speed) {
        if (speed >= 10f) UnlockMedal("飞人");
    }

    private void CheckReputationMilestones(int rep) {
        if (rep >= 10) Rating.Add(0.1f);
    }

    public void UnlockMedal(string medalName) {
        Medals.Add(1);
        Debug.Log($"解锁奖章: {medalName}");
    }

    private void SaveData() {
        PlayerData data = new PlayerData {
            speed = Speed.Value,
            reputation = Reputation.Value,
            rating = Rating.Value,
            medals = Medals.Value
        };

        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString("PlayerData", json);
        PlayerPrefs.Save();
    }

    private void LoadData() {
        if (PlayerPrefs.HasKey("PlayerData")) {
            string json = PlayerPrefs.GetString("PlayerData");
            PlayerData data = JsonUtility.FromJson<PlayerData>(json);

            Speed.Value = data.speed;
            Reputation.Value = data.reputation;
            Rating.Value = data.rating;
            Medals.Value = data.medals;
        }
    }
}
