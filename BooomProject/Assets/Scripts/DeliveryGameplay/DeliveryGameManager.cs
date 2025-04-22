using UnityEditor;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    private void Start()
    {
        Application.targetFrameRate = 60;
    }

    [SerializeField] private DeliveryPlayer _deliveryPlayer;
    public DeliveryPlayer DeliveryPlayer => _deliveryPlayer;

    [SerializeField] private NodeGraphManager _nodeGraphManager;
    public NodeGraphManager NodeGraphManager => _nodeGraphManager;

    [SerializeField] private DeliveryInputHandler _deliveryInputHandler;
    public DeliveryInputHandler DeliveryInputHandler => _deliveryInputHandler;

    [SerializeField] private GameSceneManager _sceneManager;
    public GameSceneManager SceneManager => _sceneManager;
    
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
