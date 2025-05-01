using System.Collections;
using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [Header("时间节点配置")]

    [SerializeField] private int dawnHour;    // 早晨切换时间
    [SerializeField] private int dawnMinute;    // 早晨切换时间
    [SerializeField] private int morningHour;    // 早晨切换时间
    [SerializeField] private int morningMinute;  // 早晨切换分钟
    [SerializeField] private int noonHour;      // 中午切换时间
    [SerializeField] private int noonMinute;     // 中午切换分钟
    [SerializeField] private int duskHour;   // 傍晚切换时间
    [SerializeField] private int duskMinute;  // 傍晚切换分钟
    [SerializeField] private int nightHour;     // 夜晚切换时间
    [SerializeField] private int nightMinute;    // 夜晚切换分钟

    [Header("背景贴图")]
    [SerializeField] private Sprite morningSprite;
    [SerializeField] private Sprite noonSprite;
    [SerializeField] private Sprite eveningSprite;
    [SerializeField] private Sprite nightSprite;

    [Header("组件引用")]
    [SerializeField] private SpriteRenderer rendererA;
    [SerializeField] private SpriteRenderer rendererB;
    [SerializeField] private TimeManager timeManager;

    [Header("过渡效果")]
    [SerializeField] private float fadeDuration = 2.0f;

    private Coroutine fadeCoroutine;
    private bool usingRendererA = true;


    private float _dawnTime;
    private float _morningTime;
    private float _noonTime;
    private float _duskTime;
    private float _nightTime;

    private void Awake()
    {
        _dawnTime = dawnHour + dawnMinute / 60.0f;
        _morningTime = morningHour + morningMinute / 60.0f;
        _noonTime = noonHour + noonMinute / 60.0f;
        _duskTime = duskHour + duskMinute / 60.0f;
        _nightTime = nightHour + nightMinute / 60.0f;
    }

    private void Start()
    {
        if (rendererA != null)
            rendererA.color = new Color(1, 1, 1, 1);
        if (rendererB != null)
            rendererB.color = new Color(1, 1, 1, 0);
        // 监听小时变化
        timeManager.OnMinutePassed.AddListener(UpdateBackground);
        // 初始化
        UpdateBackground(timeManager.currentTime);
    }

    private void UpdateBackground(GameTime time)
    {
        Sprite targetSprite = GetBackgroundByTime(time.hour, time.minute);
        SpriteRenderer backgroundRenderer = usingRendererA ? rendererA : rendererB;
        if (backgroundRenderer.sprite != targetSprite)
        {
            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
            }
            CrossFadeTo(targetSprite);
        }
    }

    public void CrossFadeTo(Sprite newSprite)
    {
        if (usingRendererA)
        {
            // 将新贴图赋给隐藏的 rendererB
            rendererB.sprite = newSprite;
            StartCoroutine(CrossFade(rendererA, rendererB));
        } else
        {
            // 将新贴图赋给隐藏的 rendererA
            rendererA.sprite = newSprite;
            StartCoroutine(CrossFade(rendererB, rendererA));
        }
        // 交换标记
        usingRendererA = !usingRendererA;
    }

    private Sprite GetBackgroundByTime(int hour, int minute)
    {
        float time = hour + minute / 60.0f;

        if (time >= _nightTime || time < _dawnTime)
        {
            return nightSprite;
        } else if (time >= _duskTime && time < _nightTime)
        {
            return eveningSprite;
        } else if (time >= _noonTime && time < _duskTime)
        {
            return noonSprite;
        } else if (time >= _morningTime && time < _noonTime
                   || time >= _dawnTime && time < _morningTime) // todo: dawn
        {
            return morningSprite;
        }

        return rendererA.sprite;
    }

    private IEnumerator CrossFade(SpriteRenderer fadeOutRenderer, SpriteRenderer fadeInRenderer)
    {
        float timer = 0f;
        Color outColor = fadeOutRenderer.color;
        Color inColor = fadeInRenderer.color;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float alpha = timer / fadeDuration;
            fadeOutRenderer.color = new Color(outColor.r, outColor.g, outColor.b, 1f - alpha);
            fadeInRenderer.color = new Color(inColor.r, inColor.g, inColor.b, alpha);
            yield return null;
        }

        fadeOutRenderer.color = new Color(outColor.r, outColor.g, outColor.b, 0f);
        fadeInRenderer.color = new Color(inColor.r, inColor.g, inColor.b, 1f);
    }
}
