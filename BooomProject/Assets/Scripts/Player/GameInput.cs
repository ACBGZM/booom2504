using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInput : MonoBehaviour
{
    public static GameInput gameInputInstance {  get; private set; }
    private InputActions _inputActions;

    public event EventHandler OnInteractAction;
    private void Awake() {
        if (gameInputInstance != null) {
                Debug.LogError("There are multiple PlayerController instances in the scene.");
        }
        gameInputInstance = this;
        _inputActions = new InputActions();
        _inputActions.Player.Enable();

        _inputActions.Player.Interact.performed += Interact_performed;
    }

    private void Interact_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        OnInteractAction?.Invoke(this,EventArgs.Empty);
    }

    public Vector2 GetMovement() {
        return _inputActions.Player.Move.ReadValue<Vector2>().normalized;
    }

    private void OnDestroy() {
        _inputActions.Player.Interact.performed += Interact_performed;
        _inputActions.Dispose();
    }
}
