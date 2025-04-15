using System;
using System.Collections;
using UnityEngine;

[RequireComponent (typeof(CanvasGroup))]
public class FadeEffect : MonoBehaviour
{
    private CanvasGroup m_canvas_group;

    private AnimationCurve m_fading_curve;

    private Coroutine m_fade_coroutine;

    public float m_render_opacity
    {
        get => m_canvas_group.alpha;
        set => m_canvas_group.alpha = value;
    }

    private void Awake()
    {
        m_canvas_group = GetComponent<CanvasGroup>();
        m_render_opacity = 0.0f;
        m_fading_curve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    }

    public void Fade(float a, float duration, Action callback)
    {
        if(m_render_opacity == a)
        {
            callback?.Invoke();
            return;
        }

        if (duration <= 0.0f)
        {
            m_render_opacity = a;
            callback?.Invoke();
        }
        else
        {
            if(m_fade_coroutine != null)
            {
                StopCoroutine(m_fade_coroutine);
            }

            StartCoroutine(Fading(a, duration, callback));
        }
    }

    private IEnumerator Fading(float a, float duration, Action callback)
    {
        float time = 0;
        float former_a = m_render_opacity;
        while(time < duration)
        {
            time = Mathf.Clamp(time + Time.unscaledDeltaTime, 0, duration);
            m_render_opacity = Mathf.Lerp(former_a, a, m_fading_curve.Evaluate(time / duration));
            yield return null;
        }

        callback?.Invoke();
    }
}
