using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class TimeManager : MonoBehaviour {
    [Header("Events")]
    public UnityEvent<GameTime> OnMinutePassed;    // 每分钟触发
    public UnityEvent<GameTime> OnHourPassed;    // 每小时触发
    public UnityEvent<GameTime> OnDayPassed;    // 每天触发
    // 当前游戏时间
    [Header("Current Time")]
    [SerializeField] public GameTime currentTime;
    [SerializeField] private TextMeshProUGUI _currentTimeText;
    // 时间配置参数
    [Header("Time Settings")]
    [SerializeField] private float _timeRatio = 1f; // 现实1秒 = 游戏1分钟
    [SerializeField] private bool _isPaused = false;
    [SerializeField] private float _timeScale = 1f; // 时间倍速（1=正常，2=双倍速）

    private float timer = 0f;

    void Update() {
        if (_isPaused) return;
        // 更新时间
        timer += Time.deltaTime * _timeScale;
        if (timer >= _timeRatio) {
            timer = 0f;
            AddMinute(1);
            _currentTimeText.text = currentTime.ToString();
        }
    }

    // 增加分钟数，并触发时间更新
    private void AddMinute(int minutes) {
        currentTime.minute += minutes;

        // 时间进位
        while (currentTime.minute >= 60) {
            currentTime.minute -= 60;
            AddHour(1);
        }
        OnMinutePassed?.Invoke(currentTime);
    }

    private void AddHour(int hours) {
        currentTime.hour += hours;
        if (currentTime.hour >= 24) {
            currentTime.hour = 0;
            AddDay(1);
        }
        OnHourPassed?.Invoke(currentTime);
    }

    private void AddDay(int days) {
        currentTime.day += days;
        // 假设每月固定30天（可根据需求扩展）
        if (currentTime.day > 30) {
            currentTime.day = 1;
        }
        OnDayPassed?.Invoke(currentTime);
    }

    // 控制方法
    public void SetPaused(bool paused) {
        _isPaused = paused;
    }

    public void SetTimeScale(float scale) {
        _timeScale = Mathf.Max(scale, 0f); // 禁止负数
    }
}