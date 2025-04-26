using UnityEngine;

public class BaseCampGameplayManager : Singleton<BaseCampGameplayManager>
{
    [SerializeField] private BaseCampInputHandler _baseCampInputHandler;

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
                _baseCampInputHandler.UIInputActions.Enable();
                _baseCampInputHandler.BaseCampGameplayInputActions.Enable();
                break;

            case EPlayerState.PlayerMoving:
                _baseCampInputHandler.UIInputActions.Enable();
                _baseCampInputHandler.BaseCampGameplayInputActions.Enable();
                break;

            case EPlayerState.InCutscene:
                _baseCampInputHandler.UIInputActions.Enable();
                _baseCampInputHandler.BaseCampGameplayInputActions.Disable();
                _baseCampInputHandler.ShowPhoneUI(false);
                break;
        }
    }

    private CommonInput GetCommonInputHandler()
    {
        return _baseCampInputHandler;
    }
}
