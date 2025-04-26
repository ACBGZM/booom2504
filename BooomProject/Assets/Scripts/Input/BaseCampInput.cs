using System;
using UnityEngine;

public class GameInput : CommonInput
{
    public event EventHandler OnInteractAction;

    private void Awake()
    {
        base.Awake();

        _playerInputActions.BaseCampGameplay.Enable();
        _playerInputActions.BaseCampGameplay.Interact.performed += Interact_performed;
    }

    private void OnDestroy()
    {
        base.OnDestroy();

        _playerInputActions.BaseCampGameplay.Interact.performed += Interact_performed;
        _playerInputActions.Dispose();
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
