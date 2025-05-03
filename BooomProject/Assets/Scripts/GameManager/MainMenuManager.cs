using UnityEditor;
using UnityEngine;

public class MainMenuManager : Singleton<MainMenuManager>
{
    public void QuitGame()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
