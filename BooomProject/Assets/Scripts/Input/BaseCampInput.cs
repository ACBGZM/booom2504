using System;
using UnityEngine;

public class BaseCampInputHandler : CommonInput
{
    private InputActions.BaseCampGameplayActions _baseCampGameplayInputActions;
    public InputActions.BaseCampGameplayActions BaseCampGameplayInputActions => _baseCampGameplayInputActions;

    public event EventHandler OnInteractAction;

    private new void Awake()
    {
        base.Awake();

        _baseCampGameplayInputActions = _playerInputActions.BaseCampGameplay;
        _baseCampGameplayInputActions.Enable();
        _baseCampGameplayInputActions.Interact.performed += Interact_performed;
    }

    private new void OnDestroy()
    {
        base.OnDestroy();

        _baseCampGameplayInputActions.Interact.performed += Interact_performed;
    }

    private void Interact_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnInteractAction?.Invoke(this, EventArgs.Empty);
    }

    public Vector2 GetMovement()
    {
        return _playerInputActions.BaseCampGameplay.Move.ReadValue<Vector2>().normalized;
    }
}
