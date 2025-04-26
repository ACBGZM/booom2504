using UnityEngine;
using UnityEngine.InputSystem;

public class DeliverySceneInputHandler : CommonInput
{
    private Camera _mainCamera;

    private InputActions.DeliveryGameplayActions _deliveryGameplayInputActions;
    public InputActions.DeliveryGameplayActions DeliveryGameplayInputActions => _deliveryGameplayInputActions;

    private void Awake()
    {
        base.Awake();

        _mainCamera = Camera.main;

        _deliveryGameplayInputActions = _playerInputActions.DeliveryGameplay;
        _deliveryGameplayInputActions.Click.performed += OnClickScenePerformed;
    }

    private void OnDestroy()
    {
        _deliveryGameplayInputActions.Click.performed -= OnClickScenePerformed;
    }

    private void OnClickScenePerformed(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            Ray ray = _mainCamera.ScreenPointToRay(mousePosition);
            RaycastHit2D hit = Physics2D.GetRayIntersection(ray);

            if (hit.collider != null)
            {
                hit.collider.GetComponent<IClickable>()?.OnClick();
            }
        }
    }

}
