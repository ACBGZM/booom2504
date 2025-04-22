using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

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

    [SerializeField] private DeliverySceneInputHandler _deliverySceneInputHandler;
    public DeliverySceneInputHandler DeliverySceneInputHandler => _deliverySceneInputHandler;

    [SerializeField] private GameSceneManager _sceneManager;
    public GameSceneManager SceneManager => _sceneManager;

    // todo: currently dialogue ui manager is a singleton
    // [SerializeField] private DialogueUIManager _dialogueUIManager;
    // public DialogueUIManager DialogueUIManager => _dialogueUIManager;

    public enum DeliveryGameplayState
    {
        PlayerIdle,
        PlayerMoving,
        InCutscene,
    }

    private DeliveryGameplayState _gameplayState = DeliveryGameplayState.PlayerIdle;

    public DeliveryGameplayState GameplayState
    {
        get => _gameplayState;
        set
        {
            _gameplayState = value;
            switch (_gameplayState)
            {
                case DeliveryGameplayState.PlayerIdle:
                    _deliverySceneInputHandler.UIInputActions.Enable();
                    _deliverySceneInputHandler.DeliveryGameplayInputActions.Enable();
                    break;
                case DeliveryGameplayState.PlayerMoving:
                    _deliverySceneInputHandler.UIInputActions.Disable();
                    _deliverySceneInputHandler.DeliveryGameplayInputActions.Disable();
                    _deliverySceneInputHandler.DeliveryGameplayInputActions.TogglePhone.Enable();
                    break;
                case DeliveryGameplayState.InCutscene:
                    _deliverySceneInputHandler.UIInputActions.Enable();
                    _deliverySceneInputHandler.DeliveryGameplayInputActions.Disable();
                    _deliverySceneInputHandler.ShowPhoneUI(false);
                    break;
            }
        }
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