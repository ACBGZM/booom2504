using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public struct SerializableKeyValuePair {
    public string key;
    public int value;

    public SerializableKeyValuePair(string key, int value) {
        this.key = key;
        this.value = value;
    }
}

public class PlayerDataManager : MonoBehaviour {
    [Header("调试设置")]
    public bool debugMode = false;

    [Header("基础属性")]
    public BaseStat<float> Speed = new BaseStat<float>("Speed", 5f, 0f, 15f);
    public BaseStat<int> Reputation = new BaseStat<int>("Reputation", 0, 0, 1000);
    public BaseStat<int> Medals = new BaseStat<int>("Medals", 0, 0, int.MaxValue);

    public BaseStat<float> Rating = new BaseStat<float>("Rating", 0f, 0f, 1f);
    [Tooltip("订单UID与完成次数")]
    public Dictionary<string, int> orderSaves = new Dictionary<string, int>();
    [Tooltip("订单完成总数")]
    public int finishedOrderCount;
    [Tooltip("订单好评数")]
    public int goodOrderCount;

    private const string SAVE_FILE_NAME = "player_save.json";
    private const byte XOR_KEY = 0xCD;
    private float _saveTimer;
    [Header("自动保存设置")]
    [SerializeField] private bool _autoSave = true;
    [SerializeField, Range(30, 300)] private float _saveInterval = 60f;
    private bool _isDirty;
    private OrderDataManager _orderDataManager;

    private string GetSaveFilePath() {
        return Path.Combine(Directory.GetCurrentDirectory(), SAVE_FILE_NAME);
    }

    private void Start() {
        _orderDataManager = CommonGameplayManager.GetInstance().OrderDataManager;
        _orderDataManager.OnOrderComplete += OrderDataManager_OnOrderComplete;
        LoadData();
        SetupEventListeners();
    }

    void Update() {
        if (_autoSave && _isDirty) {
            _saveTimer += Time.deltaTime;
            if (_saveTimer >= _saveInterval) {
                SaveData();
                _saveTimer = 0f;
                _isDirty = false;
            }
        }
    }

    private void SaveData() {

        return;
        try {
            PlayerData data = new PlayerData {
                speed = Speed.Value,
                reputation = Reputation.Value,
                medals = Medals.Value,
                goodOrderCount = this.goodOrderCount
            };

            data.orderSaves.Clear();
            foreach (var pair in orderSaves) {
                data.orderSaves.Add(new SerializableKeyValuePair(pair.Key, pair.Value));
            }

            string json = debugMode
                ? JsonUtility.ToJson(data, true)
                : JsonUtility.ToJson(data);
            string filePath = GetSaveFilePath();

            if (debugMode) {
                File.WriteAllText(filePath, json);
                Debug.Log($"调试模式保存未加密:\n{json}");
            } else {
                byte[] encryptedData = XOREncrypt(json);
                File.WriteAllBytes(filePath, encryptedData);
            }

            Debug.Log($"游戏数据已保存到: {filePath}");
            _isDirty = false;
        } catch (Exception e) {
            Debug.LogError($"保存数据失败: {e.Message}");
        }
    }

    private void LoadData()
    {
        return;
        try {
            string filePath = GetSaveFilePath();
            if (!File.Exists(filePath)) {
                Debug.Log("存档文件不存在，使用默认数据");
                ResetToDefaultData();
                return;
            }

            string json;
            if (debugMode) {
                json = File.ReadAllText(filePath);
                Debug.Log($"调试模式加载未加密:\n{json}");
            } else {
                byte[] encryptedData = File.ReadAllBytes(filePath);
                json = XORDecrypt(encryptedData);
            }

            PlayerData data = JsonUtility.FromJson<PlayerData>(json);
            Speed.Value = data.speed;
            Reputation.Value = data.reputation;
            Medals.Value = data.medals;
            goodOrderCount = data.goodOrderCount;

            orderSaves.Clear();
            foreach (var pair in data.orderSaves) {
                orderSaves[pair.key] = pair.value;
                finishedOrderCount += pair.value;
            }
            if (finishedOrderCount == 0) {
                Rating.Value = 0;
            } else {
                Rating.Value = (float)goodOrderCount / finishedOrderCount;
            }
        } catch (Exception e) {
            Debug.LogError($"加载数据失败: {e.Message}");
            ResetToDefaultData();
        }
    }

    private void SetupEventListeners() {
        Speed.OnValueChanged.AddListener(_ => MarkDataDirty());
        Reputation.OnValueChanged.AddListener(_ => MarkDataDirty());
        Medals.OnValueChanged.AddListener(_ => MarkDataDirty());
    }

    private void OrderDataManager_OnOrderComplete(string obj) {
        UnlockMedal(obj);
    }

    public void AddGoodOrderCount() {
        goodOrderCount++;
        MarkDataDirty();
    }

    public void AddFinishedOrderCount() {
        finishedOrderCount++;
        MarkDataDirty();
    }

    private void MarkDataDirty() {
        _isDirty = true;
        _saveTimer = 0f;
    }

    public void UnlockMedal(string medalName) {
        Medals.Add(1);
        Debug.Log($"解锁奖章: {medalName}");
    }

    private void ResetToDefaultData() {
        Speed.Reset();
        Reputation.Reset();
        Medals.Reset();
        orderSaves.Clear();
    }

    private static byte[] XOREncrypt(string data) {
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(data);
        for (int i = 0; i < bytes.Length; i++) {
            bytes[i] = (byte)(bytes[i] ^ XOR_KEY);
        }
        return bytes;
    }

    private static string XORDecrypt(byte[] data) {
        byte[] decrypted = new byte[data.Length];
        for (int i = 0; i < data.Length; i++) {
            decrypted[i] = (byte)(data[i] ^ XOR_KEY);
        }
        return System.Text.Encoding.UTF8.GetString(decrypted);
    }

    private void OnApplicationQuit() {
        if (_isDirty) SaveData();
        EventHandlerManager.updateAttribution -= OnUpdateAttribution;
    }

    private void OnUpdateAttribution(PlayerAttribution playerAttribution, float value) {
        switch (playerAttribution) {
            case PlayerAttribution.Speed:
                Speed.Value += value;
                break;
            case PlayerAttribution.Reputation:
                Reputation.Value += (int)value;
                break;
            default: break;
        }
    }
}
