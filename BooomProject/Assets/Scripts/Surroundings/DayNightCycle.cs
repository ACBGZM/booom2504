using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class TimePeriod {
    public string periodName;
    public int startHour;
    public int startMinute;
    public Sprite backgroundSprite;
    public AudioClip _baseCampMusic;
}

public class DayNightCycle : MonoBehaviour {
    [Header("时间段配置")]
    [SerializeField] private List<TimePeriod> timePeriods = new List<TimePeriod>();

    [Header("渲染器组件")]
    [SerializeField] private SpriteRenderer primaryRenderer;
    [SerializeField] private SpriteRenderer secondaryRenderer;

    [Header("过渡效果")]
    [SerializeField][Range(0.1f, 5f)] private float fadeDuration = 2f;
    [SerializeField] private AnimationCurve fadeCurve = AnimationCurve.Linear(0, 0, 1, 1);

    [Header("音乐设置")]
    [SerializeField] private AudioSource cameraAudioSource;
    [SerializeField][Range(0.1f, 5f)] private float musicFadeDuration = 2f;

    private List<TimePeriod> _sortedPeriods = new List<TimePeriod>();
    private AudioClip _currentAudioClip;
    private Sprite _currentActiveSprite;
    private Coroutine _activeMusicFade;
    private Coroutine _activeFade;
    private bool _isPrimaryActive = true;
    private float _currentTimeOfDay;

    private void Awake() {
        ValidateConfiguration();
        InitializeTimePeriods();
        ValidateRenderers();
    }

    private void ValidateConfiguration() {
        // 确保至少有一个时间段
        if (timePeriods.Count == 0) {
            Debug.LogError("未配置任何时间段!");
            enabled = false;
            return;
        }
        // 验证时间范围
        foreach (var period in timePeriods) {
            if (period.startHour < 0 || period.startHour > 23 ||
                period.startMinute < 0 || period.startMinute > 59) {
                Debug.LogError($"时间段 {period.periodName} 配置时间无效");
                enabled = false;
            }
        }
    }

    private void InitializeTimePeriods() {
        _sortedPeriods = timePeriods
            .OrderBy(p => GetTimeInHours(p.startHour, p.startMinute))
            .ToList();
        // 跨天时间段验证
        float totalCoverage = GetNextPeriodStartTime(_sortedPeriods.Count - 1) -
                             GetTimeInHours(_sortedPeriods[0].startHour, _sortedPeriods[0].startMinute);
        if (totalCoverage < 24f) {
            Debug.LogError("时间段配置未覆盖完整的24小时周期");
            enabled = false;
        }
    }

    private void ValidateRenderers() {
        if (primaryRenderer == null || secondaryRenderer == null) {
            Debug.LogError("渲染器组件缺失");
            enabled = false;
        }
    }
    private void Start() {
        InitializeRenderers();
        TimeManager timeManager = CommonGameplayManager.GetInstance().TimeManager;
        timeManager.OnMinutePassed.AddListener(HandleTimeUpdate);
        _currentActiveSprite = GetTargetSprite(timeManager.currentTime);
        UpdateBackgroundImmediate(_currentActiveSprite);
        // 初始化音乐
        if (cameraAudioSource == null) {
            // 自动获取音频源
            cameraAudioSource = Camera.main.GetComponent<AudioSource>();
            if (cameraAudioSource == null) {
                cameraAudioSource = Camera.main.gameObject.AddComponent<AudioSource>();
            }
        }

        TimePeriod initialPeriod = GetCurrentTimePeriod(timeManager.currentTime);
        if (initialPeriod._baseCampMusic != null) {
            _currentAudioClip = initialPeriod._baseCampMusic;
            cameraAudioSource.clip = _currentAudioClip;
            cameraAudioSource.volume = 1f;
            cameraAudioSource.Play();
        }
    }

    private void InitializeRenderers() {
        primaryRenderer.color = Color.white;
        secondaryRenderer.color = new Color(1, 1, 1, 0);
    }

    private void HandleTimeUpdate(GameTime currentTime) {
        Sprite targetSprite = GetTargetSprite(currentTime);
        if (targetSprite != null && targetSprite != _currentActiveSprite) {
            if (_activeFade != null) StopCoroutine(_activeFade);
            _activeFade = StartCoroutine(PerformCrossFade(targetSprite));
            _currentActiveSprite = targetSprite; // 更新
        }
        // 音乐更新
        TimePeriod currentPeriod = GetCurrentTimePeriod(currentTime);
        AudioClip targetClip = currentPeriod._baseCampMusic;

        if (targetClip != null && targetClip != _currentAudioClip) {
            if (_activeMusicFade != null) StopCoroutine(_activeMusicFade);
            _activeMusicFade = StartCoroutine(PerformMusicFade(targetClip));
            _currentAudioClip = targetClip;
        }
    }

    private Sprite GetTargetSprite(GameTime time) {
        float currentTime = GetTimeInHours(time.hour, time.minute);

        for (int i = 0; i < _sortedPeriods.Count; i++) {
            float periodStart = GetTimeInHours(_sortedPeriods[i].startHour, _sortedPeriods[i].startMinute);
            float periodEnd = GetNextPeriodStartTime(i);

            // 精确时间段匹配逻辑
            bool isWithinPeriod = currentTime >= periodStart && currentTime < periodEnd;
            bool isWrappingPeriod = periodEnd < periodStart && (currentTime >= periodStart || currentTime < periodEnd);

            if (isWithinPeriod || isWrappingPeriod) {
                return _sortedPeriods[i].backgroundSprite;
            }
        }
        return _sortedPeriods[0].backgroundSprite;
    }

    private float GetNextPeriodStartTime(int currentIndex) {
        if (currentIndex < _sortedPeriods.Count - 1) {
            return GetTimeInHours(
                _sortedPeriods[currentIndex + 1].startHour,
                _sortedPeriods[currentIndex + 1].startMinute
            );
        }
        // 最后一个时间段连接到第一个时间段的次日时间
        return GetTimeInHours(_sortedPeriods[0].startHour, _sortedPeriods[0].startMinute) + 24f;
    }

    private TimePeriod GetCurrentTimePeriod(GameTime time) {
        float currentTime = GetTimeInHours(time.hour, time.minute);
        for (int i = 0; i < _sortedPeriods.Count; i++) {
            float periodStart = GetTimeInHours(_sortedPeriods[i].startHour, _sortedPeriods[i].startMinute);
            float periodEnd = GetNextPeriodStartTime(i);
            bool isWithinPeriod = currentTime >= periodStart && currentTime < periodEnd;
            bool isWrappingPeriod = periodEnd < periodStart && (currentTime >= periodStart || currentTime < periodEnd);

            if (isWithinPeriod || isWrappingPeriod) {
                return _sortedPeriods[i];
            }
        }
        return _sortedPeriods[0];
    }

    private IEnumerator PerformCrossFade(Sprite newSprite) {
        SpriteRenderer fadeOut = _isPrimaryActive ? primaryRenderer : secondaryRenderer;
        SpriteRenderer fadeIn = _isPrimaryActive ? secondaryRenderer : primaryRenderer;
        if (fadeIn.sprite != newSprite) {
            fadeIn.sprite = newSprite;
            fadeIn.sortingOrder = fadeOut.sortingOrder + 1;
        }

        float progress = 0f;
        while (progress < 1f) {
            progress += Time.deltaTime / fadeDuration;
            float curveValue = fadeCurve.Evaluate(progress);

            fadeOut.color = new Color(1, 1, 1, 1 - curveValue);
            fadeIn.color = new Color(1, 1, 1, curveValue);
            yield return null;
        }
        // 切换完成后重置状态
        fadeOut.color = new Color(1, 1, 1, 0);
        fadeIn.sortingOrder = fadeOut.sortingOrder - 1;
        _isPrimaryActive = !_isPrimaryActive;
    }

    private IEnumerator PerformMusicFade(AudioClip newClip) {
        if (cameraAudioSource == null) { Debug.LogError("未配置音频源"); yield break; }
        // 淡出
        float fadeTimer = 0f;
        float startVolume = cameraAudioSource.volume;
        while (fadeTimer < musicFadeDuration) {
            fadeTimer += Time.deltaTime;
            cameraAudioSource.volume = Mathf.Lerp(startVolume, 0f,
                fadeCurve.Evaluate(fadeTimer / musicFadeDuration));
            yield return null;
        }
        // 切换
        cameraAudioSource.Stop();
        cameraAudioSource.clip = newClip;
        cameraAudioSource.Play();
        // 淡入
        fadeTimer = 0f;
        while (fadeTimer < musicFadeDuration) {
            fadeTimer += Time.deltaTime;
            cameraAudioSource.volume = Mathf.Lerp(0f, 1f,
                fadeCurve.Evaluate(fadeTimer / musicFadeDuration));
            yield return null;
        }
    }

    private void UpdateBackgroundImmediate(Sprite sprite) {
        if (_isPrimaryActive) {
            primaryRenderer.sprite = sprite;
            primaryRenderer.color = Color.white;
        } else {
            secondaryRenderer.sprite = sprite;
            secondaryRenderer.color = Color.white;
        }
    }

    private float GetTimeInHours(int hours, int minutes) => hours + minutes / 60f;
}
