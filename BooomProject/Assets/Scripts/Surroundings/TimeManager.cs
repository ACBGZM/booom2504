using System;
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
    private TextMeshProUGUI _currentDayText;
    private const string SAVE_KEY = "GameTimeData";
    private float _offWorkTime;
    private float _endDayTime;

    enum DayStatus
    {
        Work,
        OffWork
    };
    private DayStatus _dayStatus = DayStatus.Work;

    void Awake() {
        // LoadTime(); // 加载保存的时间
        // FindAndUpdateTimeDisplay(); // 初始化查找
        OnMinutePassed.AddListener(CheckAndNotifyTimeEvents);
        _offWorkTime = GameplaySettings._offWorkHour + GameplaySettings._offWorkMinute / 60.0f;
        _endDayTime = GameplaySettings._endDayHour + GameplaySettings._endDayMinute / 60.0f;
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        FindAndUpdateTimeDisplay();
        _isPaused = false;
    }

    private void FindAndUpdateTimeDisplay() {
        FindTimeTextInScene();
        UpdateUI();
    }

    private void FindTimeTextInScene() {
        GameObject timeDisplayObj = GameObject.FindWithTag("TimeDisplay");
        if (timeDisplayObj != null) {
            _currentTimeText = timeDisplayObj.GetComponent<TextMeshProUGUI>();
        }

        GameObject dayDisplay = GameObject.FindWithTag("DayDisplay");
        if (dayDisplay != null) {
            _currentDayText = dayDisplay.GetComponent<TextMeshProUGUI>();
        }
    }

    private void UpdateUI() {
        if (_currentTimeText != null) {
            _currentTimeText.text = currentTime.GetHourAndMinute();
        }

        if (_currentDayText != null)
        {
            _currentDayText.text = currentTime.GetDay();
        }
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

    public void GoToNextDay()
    {
        ++currentTime.day;
        currentTime.hour = GameplaySettings._wakeUpHour;
        currentTime.minute = GameplaySettings._wakeUpMinute;

        _isPaused = true;

        OnDayPassed?.Invoke(currentTime);

        _dayStatus = DayStatus.Work;
    }

    public bool IsGameOver()
    {
        return currentTime.day > GameplaySettings._maxGameDayCount;
    }

    public bool IsOffWork()
    {
        return currentTime.hour == GameplaySettings._offWorkHour &&
               currentTime.minute >= GameplaySettings._offWorkMinute
               || currentTime.hour > GameplaySettings._offWorkHour;
    }

    private void CheckAndNotifyTimeEvents(GameTime time)
    {
        float t = currentTime.hour + currentTime.minute / 60.0f;
        if (t >= _offWorkTime && _dayStatus == DayStatus.Work)
        {
            EventHandlerManager.EndWorking();
            _dayStatus = DayStatus.OffWork;
        }

        if (t >= _endDayTime && _dayStatus == DayStatus.OffWork)
        {
            EventHandlerManager.EndDay();
        }
    }
}
