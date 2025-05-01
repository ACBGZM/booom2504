using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameSceneManager : MonoBehaviour {
    [SerializeField] private List<GameObject> _disableWhileLoading;

    public void LoadSync(string target_scene) {
        HandleSceneTransition(() => SceneManager.LoadScene(target_scene));
    }

    public void Start()
    {
        _fadeImage.color = Color.black;
        _fadeImage.DOFade(0f, _fadeInDuration).SetEase(Ease.Linear);
    }

    // 异步加载场景动画
    [SerializeField] Image _fadeImage;
    private float _fadeOutDuration = 0.75f;
    private float _fadeInDuration = 1.25f;
    public void LoadAsyncWithFading(string targetScene)
    {
        StartCoroutine(LoadAsyncWithFadingOut(targetScene));
    }

    private IEnumerator LoadAsyncWithFadingOut(string targetScene)
    {
        _fadeImage.DOFade(1f, _fadeOutDuration).SetEase(Ease.Linear);
        HandleSceneTransition();
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

    public void QuitGame()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
    }
}
