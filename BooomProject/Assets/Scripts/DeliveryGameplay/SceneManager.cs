using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameSceneManager : MonoBehaviour {
    [SerializeField] private GameObject _loadingPanel;
    [SerializeField] private Slider _progressBar;
    [SerializeField] private TMP_Text _progressText;
    [SerializeField] private List<GameObject> _disableWhileLoading;

    public void LoadSync(string target_scene) {
        HandleSceneTransition(() => SceneManager.LoadScene(target_scene));
    }

    public void LoadAsync(string target_scene) {
        StartCoroutine(LoadSceneAsync(target_scene));
    }

    private IEnumerator LoadSceneAsync(string target_scene) {
        HandleSceneTransition();
        _loadingPanel.SetActive(true);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(target_scene);
        asyncLoad.allowSceneActivation = false;

        float fakeProgress = 0;
        while (!asyncLoad.isDone) {
            fakeProgress = UpdateProgressUI(asyncLoad, fakeProgress);

            if (fakeProgress >= 1f && asyncLoad.progress >= 0.9f) {
                asyncLoad.allowSceneActivation = true;
            }

            yield return null;
        }

        // 场景加载完成后，清理加载界面
        _loadingPanel.SetActive(false);
        foreach (GameObject panel in _disableWhileLoading) {
            panel.SetActive(true);
        }
    }

    private void HandleSceneTransition(System.Action loadAction = null) {
        foreach (GameObject panel in _disableWhileLoading) {
            panel.SetActive(false);
        }
        loadAction?.Invoke();
    }

    private float UpdateProgressUI(AsyncOperation operation, float fakeProgress) {
        fakeProgress += Time.deltaTime * 0.3f;
        float progress = Mathf.Clamp01(operation.progress / 0.9f);

        _progressText.text = $"{fakeProgress:P0}";
        _progressBar.value = fakeProgress;

        return Mathf.Min(fakeProgress, 1f);
    }
}
