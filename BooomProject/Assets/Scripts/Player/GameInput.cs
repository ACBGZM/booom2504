using System;
using UnityEngine;

public class GameInput : MonoBehaviour {
    public static GameInput gameInputInstance { get; private set; }

    public event EventHandler OnInteractAction;
    public event EventHandler OnRaisePhoneAction;

    private InputActions _inputActions;

    private void Awake() {
        if (gameInputInstance != null) {
            Debug.LogError("There are multiple BaseCampGameplayController instances in the scene.");
        }
        gameInputInstance = this;
        _inputActions = new InputActions();
        _inputActions.BaseCampGameplay.Enable();

        _inputActions.BaseCampGameplay.Interact.performed += Interact_performed;
        _inputActions.BaseCampGameplay.RaisePhone.performed += RaisePhone_performed;
    }

    private void RaisePhone_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        OnRaisePhoneAction?.Invoke(this,EventArgs.Empty);
    }

    private void Interact_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        OnInteractAction?.Invoke(this, EventArgs.Empty);
    }

    public Vector2 GetMovement() {
        return _inputActions.BaseCampGameplay.Move.ReadValue<Vector2>().normalized;
    }

    private void OnDestroy() {
        _inputActions.BaseCampGameplay.Interact.performed += Interact_performed;
        _inputActions.Dispose();
    }
}