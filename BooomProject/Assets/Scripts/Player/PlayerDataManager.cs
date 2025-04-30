using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct SerializableKeyValuePair {
    public string key;
    public bool value;

    public SerializableKeyValuePair(string key, bool value) {
        this.key = key;
        this.value = value;
    }
}

public class PlayerDataManager : MonoBehaviour
{
    public static PlayerDataManager Instance {  get; private set; }
    [Header("基础属性")]
    public BaseStat<float> Speed = new BaseStat<float>("Speed", 5f, 0f, 15f);
    public BaseStat<int> Reputation = new BaseStat<int>("Reputation", 0, 0, 1000);
    public BaseStat<float> Rating = new BaseStat<float>("Rating", 0.8f, 0f, 1f);
    public BaseStat<int> Medals = new BaseStat<int>("Medals", 0, 0, int.MaxValue);
    [Tooltip("特殊订单UID与是否完成")]
    public Dictionary<string, bool> orderSaves= new Dictionary<string,bool>();
    //[Header("自动保存设置")]
    //[SerializeField] private bool _autoSave = true;
    //[SerializeField, Range(30, 300)] private float _saveInterval = 60f;
    private float _saveTimer;
    private bool _isDirty;

    // 加密
    private const string SAVE_KEY = "EncryptedSaves";
    private const byte XOR_KEY = 0xCD;

    private void OnEnable()
    {
        EventHandlerManager.updateAttribution += OnUpdateAttribution;
        CommonGameplayManager.GetInstance().OrderDataManager.OnOrderComplete += OrderDataManager_OnSpecialOrderComplete;
    }

    private void OnDisable()
    {
        EventHandlerManager.updateAttribution -= OnUpdateAttribution;
        CommonGameplayManager.GetInstance().OrderDataManager.OnOrderComplete -= OrderDataManager_OnSpecialOrderComplete;
    }

    private void Start()
    {
        Instance = this;
        LoadData();
        SetupEventListeners();
    }

    // void Update() {
    //    if (_autoSave && _isDirty)
    //    {
    //        _saveTimer += Time.deltaTime;
    //        if (_saveTimer >= _saveInterval)
    //        {
    //            SaveData();
    //            _saveTimer = 0f;
    //            _isDirty = false;
    //        }
    //    }
    // }

    private void SetupEventListeners() {
        Speed.OnValueChanged.AddListener(v => {
            CheckSpeedAchievements(v);
            MarkDataDirty();
        });

        Reputation.OnValueChanged.AddListener(v => {
            CheckReputationMilestones(v);
            MarkDataDirty();
        });

        Medals.OnValueChanged.AddListener(_ => MarkDataDirty());
        Rating.OnValueChanged.AddListener(_ => MarkDataDirty());
    }

    private void OrderDataManager_OnSpecialOrderComplete(string orderUID) {
        if (!orderSaves.ContainsKey(orderUID)) {
            orderSaves.Add(orderUID, true);
            MarkDataDirty();
        }
    }

    private void MarkDataDirty() {
        _isDirty = true;
        _saveTimer = 0f;
    }

    private void CheckSpeedAchievements(float speed)
    {
        if (speed >= 10f) UnlockMedal("");
    }

    private void CheckReputationMilestones(int rep)
    {
        if (rep >= 10) Rating.Add(0.1f);
    }

    public void UnlockMedal(string medalName)
    {
        Medals.Add(1);
        Debug.Log($"解锁奖章: {medalName}");
    }

    private void SaveData()
    {
        try {
            PlayerData data = new PlayerData {
                speed = Speed.Value,
                reputation = Reputation.Value,
                rating = Rating.Value,
                medals = Medals.Value
            };
            // 转换字典为可序列化列表
            data.orderSaves.Clear();
            foreach (var pair in orderSaves) {
                data.orderSaves.Add(new SerializableKeyValuePair(pair.Key, pair.Value));
            }
            string json = JsonUtility.ToJson(data);
            byte[] encryptedData = XOREncrypt(json);
            PlayerPrefs.SetString(SAVE_KEY, Convert.ToBase64String(encryptedData));
            PlayerPrefs.Save(); 
            Debug.Log("游戏数据已保存");
        } catch (Exception e) {
            Debug.LogError($"保存数据失败: {e.Message}");
        }
    }

    private void LoadData()
    {
        try {
            if (!PlayerPrefs.HasKey(SAVE_KEY)) return;
            string encryptedString = PlayerPrefs.GetString(SAVE_KEY);
            byte[] encryptedData = Convert.FromBase64String(encryptedString);
            string json = XORDecrypt(encryptedData);
            PlayerData data = JsonUtility.FromJson<PlayerData>(json);
            // 加载基础属性
            Speed.Value = data.speed;
            Reputation.Value = data.reputation;
            Rating.Value = data.rating;
            Medals.Value = data.medals;
            // 加载订单数据
            orderSaves.Clear();
            foreach (var pair in data.orderSaves) {
                orderSaves[pair.key] = pair.value;
            }
            Debug.Log("游戏数据已加载");
        } catch (Exception e) {
            Debug.LogError($"加载数据失败: {e.Message}");
            ResetToDefaultData();
        }
    }

    private void ResetToDefaultData() {
        Speed.Reset();
        Reputation.Reset();
        Rating.Reset();
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
    }

    private void OnUpdateAttribution(PlayerAttribution playerAttribution, float value)
    {
        switch(playerAttribution)
        {
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
