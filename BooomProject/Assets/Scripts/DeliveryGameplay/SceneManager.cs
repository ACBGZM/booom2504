using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;

public class GameSceneManager : MonoBehaviour
{
    [SerializeField] GameObject _loadingPanel;
    [SerializeField] Slider _progressBar;
    [SerializeField] TMP_Text _progressText;
    [SerializeField] List<GameObject> _disableWhileLoading;

    public void LoadSync(string target_scene)
    {
        foreach (GameObject panel in _disableWhileLoading)
        {
            panel.SetActive(false);
        }

        SceneManager.LoadScene(target_scene);
    }

    public void LoadAsync(string target_scene)
    {
        foreach (GameObject panel in _disableWhileLoading)
        {
            panel.SetActive(false);
        }

        StartCoroutine(LoadSceneAsync(target_scene));
    }

    IEnumerator LoadSceneAsync(string target_scene)
    {
        _loadingPanel.SetActive(true);
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(target_scene);

        asyncLoad.allowSceneActivation = false;

        float fakeProgress = 0;
        while (!asyncLoad.isDone)
        {
            fakeProgress += Time.deltaTime * 0.3f;
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            _progressText.text = fakeProgress.ToString("p2");

            _progressBar.value = fakeProgress;
            if (fakeProgress >= 1f && progress >= 1f)
            {
                asyncLoad.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
