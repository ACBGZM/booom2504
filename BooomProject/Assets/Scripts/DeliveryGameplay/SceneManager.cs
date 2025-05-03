using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameSceneManager : MonoBehaviour {
    [SerializeField] private List<GameObject> _disableWhileLoading;
    private TimeManager _timer;
    public void LoadSync(string target_scene) {
        HandleSceneTransition(() => SceneManager.LoadScene(target_scene));
    }

    private void Awake() {
        if (SceneManager.GetActiveScene().name != "01MainMenu") {
            _timer = CommonGameplayManager.GetInstance().TimeManager;
            SceneManager.sceneLoaded += _timer.OnSceneLoaded; // 注册场景加载事件
        }
    }

    public void Start()
    {
        _fadeImage.color = Color.black;
        _fadeImage.DOFade(0f, _fadeInDuration).SetEase(Ease.Linear);
    }

    // 异步加载场景动画
    [SerializeField] Image _fadeImage;
    private float _fadeOutDuration = 1.25f;
    private float _fadeInDuration = 1.75f;
    public void LoadAsyncWithFading(string targetScene)
    {
        StartCoroutine(LoadAsyncWithFadingOut(targetScene));
    }

    private IEnumerator LoadAsyncWithFadingOut(string targetScene)
    {
        HandleSceneTransition();
        _fadeImage.gameObject.SetActive(true);
        _fadeImage.DOFade(1f, _fadeOutDuration).SetEase(Ease.Linear);
        yield return YieldHelper.WaitForSeconds(_fadeOutDuration);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(targetScene);
        asyncLoad.allowSceneActivation = false;

        while (asyncLoad.progress < 0.9f)
        {
            yield return null;
        }

        asyncLoad.allowSceneActivation = true;
    }

    private void HandleSceneTransition(System.Action loadAction = null) {
        foreach (GameObject panel in _disableWhileLoading) {
            panel.SetActive(false);
        }

        //loadAction?.Invoke();
    }

    private void OnDestroy() {
        if (SceneManager.GetActiveScene().name != "01MainMenu") {
            SceneManager.sceneLoaded -= _timer.OnSceneLoaded;
        }
    }
}
