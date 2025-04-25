using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameSceneManager : MonoBehaviour
{
    [SerializeField] private GameObject _loadingPanel;
    [SerializeField] private Slider _progressBar;
    [SerializeField] private TMP_Text _progressText;
    [SerializeField] private List<GameObject> _disableWhileLoading;

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

    private IEnumerator LoadSceneAsync(string target_scene)
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
