using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class TimeManager : MonoBehaviour {
    [Header("Events")]
    public UnityEvent<GameTime> OnMinutePassed;    // ÿ���Ӵ���
    public UnityEvent<GameTime> OnHourPassed;    // ÿСʱ����
    public UnityEvent<GameTime> OnDayPassed;    // ÿ�촥��
    // ��ǰ��Ϸʱ��
    [Header("Current Time")]
    [SerializeField] public GameTime currentTime;
    [SerializeField] private TextMeshProUGUI _currentTimeText;
    // ʱ�����ò���
    [Header("Time Settings")]
    [SerializeField] private float _timeRatio = 1f; // ��ʵ1�� = ��Ϸ1����
    [SerializeField] private bool _isPaused = false;
    [SerializeField] private float _timeScale = 1f; // ʱ�䱶�٣�1=������2=˫���٣�

    private float timer = 0f;

    void Update() {
        if (_isPaused) return;
        // ����ʱ��
        timer += Time.deltaTime * _timeScale;
        if (timer >= _timeRatio) {
            timer = 0f;
            AddMinute(1);
            _currentTimeText.text = currentTime.ToString();
        }
    }

    // ���ӷ�������������ʱ�����
    private void AddMinute(int minutes) {
        currentTime.minute += minutes;

        // ʱ���λ
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
        // ����ÿ�¹̶�30�죨�ɸ���������չ��
        if (currentTime.day > 30) {
            currentTime.day = 1;
        }
        OnDayPassed?.Invoke(currentTime);
    }

    // ���Ʒ���
    public void SetPaused(bool paused) {
        _isPaused = paused;
    }

    public void SetTimeScale(float scale) {
        _timeScale = Mathf.Max(scale, 0f); // ��ֹ����
    }
}