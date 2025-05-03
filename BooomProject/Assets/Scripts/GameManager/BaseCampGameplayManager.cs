using UnityEngine;

public class BaseCampGameplayManager : Singleton<BaseCampGameplayManager>
{
    [SerializeField] private BaseCampInputHandler _baseCampInputHandler;
    [SerializeField] private GameSceneManager _sceneManager;
    public GameSceneManager SceneManager => _sceneManager;

    [SerializeField] private EventSequenceExecutor _onEndDayEventSequenceExecutor;
    [SerializeField] private EventSequenceExecutor _onGameOverEventSequenceExecutor;

    protected override void Awake()
    {
        base.Awake();
        EventHandlerManager.playerStateChanged -= OnPlayerStateChanged;
        EventHandlerManager.playerStateChanged += OnPlayerStateChanged;
        EventHandlerManager.getCommonInputHandler -= GetCommonInputHandler;
        EventHandlerManager.getCommonInputHandler += GetCommonInputHandler;
        EventHandlerManager.OnEndWorking -= OnEndWorking;
        EventHandlerManager.OnEndWorking += OnEndWorking;
        EventHandlerManager.OnEndDay -= OnEndDay;
        EventHandlerManager.OnEndDay += OnEndDay;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        EventHandlerManager.playerStateChanged -= OnPlayerStateChanged;
        EventHandlerManager.getCommonInputHandler -= GetCommonInputHandler;
        EventHandlerManager.OnEndWorking -= OnEndWorking;
        EventHandlerManager.OnEndDay -= OnEndDay;
    }

    private void OnPlayerStateChanged()
    {
        switch (CommonGameplayManager.GetInstance().PlayerState)
        {
            case EPlayerState.PlayerIdle:
                _baseCampInputHandler.UIInputActions.Enable();
                _baseCampInputHandler.BaseCampGameplayInputActions.Enable();

                CommonGameplayManager.GetInstance().TimeManager.SetTimeScale(1.0f);
                break;

            case EPlayerState.PlayerMoving:
                _baseCampInputHandler.UIInputActions.Enable();
                _baseCampInputHandler.BaseCampGameplayInputActions.Enable();

                CommonGameplayManager.GetInstance().TimeManager.SetTimeScale(1.0f);
                break;

            case EPlayerState.InCutscene:
                _baseCampInputHandler.UIInputActions.Enable();
                _baseCampInputHandler.BaseCampGameplayInputActions.Disable();

                CommonGameplayManager.GetInstance().TimeManager.SetTimeScale(0.3f);
                _baseCampInputHandler.ShowPhoneUI(false);
                break;
        }
    }

    private CommonInput GetCommonInputHandler()
    {
        return _baseCampInputHandler;
    }

    public void OnEndWorking()
    {
    }

    public void OnEndDay()
    {
        // todo: day end
        CommonGameplayManager.GetInstance().TimeManager.GoToNextDay();

        // check game over
        if (CommonGameplayManager.GetInstance().TimeManager.IsGameOver())
        {
            CommonGameplayManager.GetInstance().TimeManager.SetPaused(true);
            _onGameOverEventSequenceExecutor.Initialize(null);
            _onGameOverEventSequenceExecutor.Execute();

            return;
        }

        _onEndDayEventSequenceExecutor.Initialize(null);
        _onEndDayEventSequenceExecutor.Execute();
    }
}
