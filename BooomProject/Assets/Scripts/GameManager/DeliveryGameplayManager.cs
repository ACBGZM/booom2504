using UnityEngine;

public class DeliveryGameplayManager : Singleton<DeliveryGameplayManager>
{
    [SerializeField] private DeliveryPlayer _deliveryPlayer;
    public DeliveryPlayer DeliveryPlayer => _deliveryPlayer;

    [SerializeField] private DeliverySceneInputHandler _deliverySceneInputHandler;
    public DeliverySceneInputHandler DeliverySceneInputHandler => _deliverySceneInputHandler;

    [SerializeField] private GameSceneManager _sceneManager;

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
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        EventHandlerManager.playerStateChanged -= OnPlayerStateChanged;
        EventHandlerManager.getCommonInputHandler -= GetCommonInputHandler;
    }

    private void OnPlayerStateChanged()
    {
        switch (CommonGameplayManager.GetInstance().PlayerState)
        {
            case EPlayerState.PlayerIdle:
                _deliverySceneInputHandler.UIInputActions.Enable();
                _deliverySceneInputHandler.DeliveryGameplayInputActions.Enable();
                break;

            case EPlayerState.PlayerMoving:
                _deliverySceneInputHandler.UIInputActions.Disable();
                _deliverySceneInputHandler.DeliveryGameplayInputActions.Enable();
                _deliverySceneInputHandler.DeliveryGameplayInputActions.Click.Disable();
                break;

            case EPlayerState.InCutscene:
                _deliverySceneInputHandler.UIInputActions.Enable();
                _deliverySceneInputHandler.DeliveryGameplayInputActions.Disable();
                _deliverySceneInputHandler.ShowPhoneUI(false);
                break;
        }
    }

    private CommonInput GetCommonInputHandler()
    {
        return _deliverySceneInputHandler;
    }
}
