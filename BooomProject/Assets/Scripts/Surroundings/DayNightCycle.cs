using System.Collections;
using UnityEngine;

public class DayNightCycle : MonoBehaviour {
    [Header("时间节点配置")]
    [SerializeField] private int morningHour = 6;    // 早晨切换时间
    [SerializeField] private int noonHour = 10;      // 中午切换时间
    [SerializeField] private int eveningHour = 18;   // 傍晚切换时间
    [SerializeField] private int nightHour = 21;     // 夜晚切换时间

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

    void Start() {
        if (rendererA != null)
            rendererA.color = new Color(1, 1, 1, 1);
        if (rendererB != null)
            rendererB.color = new Color(1, 1, 1, 0);
        // 监听小时变化
        timeManager.OnHourPassed.AddListener(UpdateBackground);
        // 初始化
        UpdateBackground(timeManager.currentTime);
    }

    private void UpdateBackground(GameTime time) {
        Sprite targetSprite = GetBackgroundByTime(time.hour);
        SpriteRenderer backgroundRenderer = usingRendererA ? rendererA : rendererB;
        if (backgroundRenderer.sprite != targetSprite) {
            if (fadeCoroutine != null) {
                StopCoroutine(fadeCoroutine);
            }
            CrossFadeTo(targetSprite);
        }
    }

    public void CrossFadeTo(Sprite newSprite) {
        if (usingRendererA) {
            // 将新贴图赋给隐藏的 rendererB
            rendererB.sprite = newSprite;
            StartCoroutine(CrossFade(rendererA, rendererB));
        } else {
            // 将新贴图赋给隐藏的 rendererA
            rendererA.sprite = newSprite;
            StartCoroutine(CrossFade(rendererB, rendererA));
        }
        // 交换标记
        usingRendererA = !usingRendererA;
    }

    private Sprite GetBackgroundByTime(int hour) {
        if (hour >= nightHour || hour < morningHour) {
            return nightSprite;
        } else if (hour >= eveningHour) {
            return eveningSprite;
        } else if (hour >= noonHour) {
            return noonSprite;
        } else if (hour >= morningHour) {
            return morningSprite;
        }

        return rendererA.sprite;
    }

    private IEnumerator CrossFade(SpriteRenderer fadeOutRenderer, SpriteRenderer fadeInRenderer) {
        float timer = 0f;
        Color outColor = fadeOutRenderer.color;
        Color inColor = fadeInRenderer.color;

        while (timer < fadeDuration) {
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