using UnityEngine;
using UnityEngine.InputSystem;

public class DeliverySceneInputHandler : MonoBehaviour
{
    private Camera _mainCamera;
    private InputActions _playerInput;

    private InputActions.DeliveryGameplayActions _deliveryGameplayInputActions;
    public InputActions.DeliveryGameplayActions DeliveryGameplayInputActions => _deliveryGameplayInputActions;
    
    private InputActions.UIActions _uiInputActions;
    public InputActions.UIActions UIInputActions => _uiInputActions;

    [SerializeField] private GameObject _phoneUI;

    private void Awake()
    {
        _playerInput = new InputActions();
        _mainCamera = Camera.main;
        
        _deliveryGameplayInputActions = _playerInput.DeliveryGameplay;
        _uiInputActions = _playerInput.UI;
        
        _playerInput.Enable();
    }

    private void OnDestroy()
    {
        _playerInput.Disable();
    }

    private void OnEnable()
    {
        _deliveryGameplayInputActions.Click.performed += OnClickScenePerformed;
        _deliveryGameplayInputActions.TogglePhone.performed += TogglePhoneFromInput;
    }

    private void OnDisable()
    {
        _deliveryGameplayInputActions.Click.performed -= OnClickScenePerformed;
        _deliveryGameplayInputActions.TogglePhone.performed -= TogglePhoneFromInput;
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
    
    private void TogglePhoneFromInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            TogglePhone();
        }
    }
    
    public void ShowPhoneUI(bool show)
    {
        if (_phoneUI != null)
        {
            _phoneUI.SetActive(show);
        }
    }

    public void TogglePhone()
    {
        if (_phoneUI != null)
        {
            _phoneUI.SetActive(!_phoneUI.activeInHierarchy);
        }
    }
}
