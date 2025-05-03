using UnityEngine;
using UnityEngine.Events;

public class DeliveryGameplayManager : Singleton<DeliveryGameplayManager>
{
    [SerializeField] private DeliveryPlayer _deliveryPlayer;
    public DeliveryPlayer DeliveryPlayer => _deliveryPlayer;

    [SerializeField] private DeliverySceneInputHandler _deliverySceneInputHandler;
    public DeliverySceneInputHandler DeliverySceneInputHandler => _deliverySceneInputHandler;

    [SerializeField] private FloatingButton _floatingEnterButton;

    [SerializeField] private GameSceneManager _sceneManager;
    public GameSceneManager SceneManager => _sceneManager;

    [SerializeField] private EventSequenceExecutor _onEndWorkingEventSequenceExecutor;

    // todo: currently dialogue ui manager is a singleton
    // [SerializeField] private DialogueUIManager _dialogueUIManager;
    // public DialogueUIManager DialogueUIManager => _dialogueUIManager;

    protected override void Awake()
    {
        base.Awake();
        EventHandlerManager.playerStateChanged -= OnPlayerStateChanged;
        EventHandlerManager.playerStateChanged += OnPlayerStateChanged;
        EventHandlerManager.getCommonInputHandler -= GetCommonInputHandler;
        EventHandlerManager.getCommonInputHandler += GetCommonInputHandler;
        EventHandlerManager.OnEndWorking -= OnEndWorking;
        EventHandlerManager.OnEndWorking += OnEndWorking;
    }

    private void Start()
    {
        CommonGameplayManager.GetInstance().NodeGraphManager.ResetCurrentNode();
        CommonGameplayManager.GetInstance().NodeGraphManager.RefreshMovingHints();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        EventHandlerManager.playerStateChanged -= OnPlayerStateChanged;
        EventHandlerManager.getCommonInputHandler -= GetCommonInputHandler;
        EventHandlerManager.OnEndWorking -= OnEndWorking;
    }

    private void OnPlayerStateChanged()
    {
        switch (CommonGameplayManager.GetInstance().PlayerState)
        {
            case EPlayerState.PlayerIdle:
                _deliverySceneInputHandler.UIInputActions.Enable();
                _deliverySceneInputHandler.DeliveryGameplayInputActions.Enable();

                CommonGameplayManager.GetInstance().TimeManager.SetTimeScale(0.5f);
                break;

            case EPlayerState.PlayerMoving:
                _deliverySceneInputHandler.UIInputActions.Disable();
                _deliverySceneInputHandler.DeliveryGameplayInputActions.Enable();
                _deliverySceneInputHandler.DeliveryGameplayInputActions.Click.Disable();

               // CommonGameplayManager.GetInstance().TimeManager.SetTimeScale(1.0f);
                break;

            case EPlayerState.InCutscene:
                _deliverySceneInputHandler.UIInputActions.Enable();
                _deliverySceneInputHandler.DeliveryGameplayInputActions.Disable();
                _deliverySceneInputHandler.ShowPhoneUI(false);

                CommonGameplayManager.GetInstance().TimeManager.SetTimeScale(0.3f);
                break;
        }
    }

    private CommonInput GetCommonInputHandler()
    {
        return _deliverySceneInputHandler;
    }

    public void ShowEnterButton(bool show, Transform targetObject, UnityAction action)
    {
        _floatingEnterButton.SetActive(show);
        _floatingEnterButton.TargetObject = targetObject;
        _floatingEnterButton.SetButtonAction(action);
    }

    public void OnEndWorking()
    {
        CommonGameplayManager.GetInstance().PauseGame();
        _onEndWorkingEventSequenceExecutor.Initialize((bool success)=>
        {
            CommonGameplayManager.GetInstance().ResumeGame();
        });
        _onEndWorkingEventSequenceExecutor.Execute();
    }
}
