using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class TimeManager : MonoBehaviour
{
    [Header("Events")]
    public UnityEvent<GameTime> OnMinutePassed;    // 每分钟触发
    public UnityEvent<GameTime> OnHourPassed;    // 每小时触发
    public UnityEvent<GameTime> OnDayPassed;    // 每天触发
    // 当前游戏时间
    [Header("Current Time")]
    [SerializeField] public GameTime currentTime;
    // 时间配置参数
    [Header("Time Settings")]
    [SerializeField] private float _timeRatio = 1f; // 现实1秒 = 游戏1分钟
    [SerializeField] private bool _isPaused = false;
    [SerializeField] private float _timeScale = 1f; // 时间倍速（1=正常，2=双倍速）

    private float timer = 0f;
    private TextMeshProUGUI _currentTimeText;
    private const string SAVE_KEY = "GameTimeData";

    void Awake() {
        SceneManager.sceneLoaded += OnSceneLoaded; // 注册场景加载事件
        // LoadTime(); // 加载保存的时间
        FindAndUpdateTimeDisplay(); // 初始化查找
    }

    private void OnDestroy() {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        FindAndUpdateTimeDisplay();
    }

    private void FindAndUpdateTimeDisplay() {
        _currentTimeText = FindTimeTextInScene();
        UpdateUI();
    }

    private TextMeshProUGUI FindTimeTextInScene() {
        GameObject timeDisplayObj = GameObject.FindWithTag("TimeDisplay");
        if (timeDisplayObj != null) {
            return timeDisplayObj.GetComponent<TextMeshProUGUI>();
        }
        Debug.LogWarning("TimeDisplay object not found in scene.");
        return null;
    }

    private void UpdateUI() {
        if (_currentTimeText != null)
            _currentTimeText.text = currentTime.ToString();
    }

    // 更新时间
    private void Update() {
        if (_isPaused) return;

        timer += Time.deltaTime * _timeScale;
        if (timer >= _timeRatio) {
            timer = 0f;
            AddMinute(1);
            UpdateUI();
        }
    }

    // 增加分钟数，并触发时间更新
    private void AddMinute(int minutes) {
        currentTime.minute += minutes;

        // 处理分钟进位
        if (currentTime.minute >= 60) {
            int carryHours = currentTime.minute / 60;
            currentTime.minute %= 60;
            AddHour(carryHours);
        }

        OnMinutePassed?.Invoke(currentTime);
    }

    private void AddHour(int hours) {
        currentTime.hour += hours;

        // 处理小时进位
        if (currentTime.hour >= 24) {
            int carryDays = currentTime.hour / 24;
            currentTime.hour %= 24;
            AddDay(carryDays);
        }

        OnHourPassed?.Invoke(currentTime);
    }

    private void AddDay(int days) {
        currentTime.day += days;
        OnDayPassed?.Invoke(currentTime);
    }

    // 控制方法
    public void SetPaused(bool paused)
    {
        _isPaused = paused;
    }

    public void SetTimeScale(float scale)
    {
        _timeScale = Mathf.Max(scale, 0f); // 禁止负数
    }
/*
    // 保存时间数据
    public void SaveTime() {
        string json = JsonUtility.ToJson(currentTime);
        PlayerPrefs.SetString(SAVE_KEY, json);
        PlayerPrefs.Save();
    }

    // 加载时间数据
    public void LoadTime() {
        if (PlayerPrefs.HasKey(SAVE_KEY)) {
            string json = PlayerPrefs.GetString(SAVE_KEY);
            currentTime = JsonUtility.FromJson<GameTime>(json);
        }
    }

    // 退出游戏时自动保存
    private void OnApplicationQuit() {
        SaveTime();
    }
*/
    public void UpdateTimeDisplay(TextMeshProUGUI newDisplay) {
        _currentTimeText = newDisplay;
        if (_currentTimeText != null) {
            _currentTimeText.text = currentTime.ToString();
        }
    }
}
