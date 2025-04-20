using UnityEngine;
using UnityEngine.InputSystem;

public class DeliveryInputHandler : MonoBehaviour
{
    private Camera _mainCamera;
    private InputActions _playerInput;
    private InputAction _clickAction;
    private InputAction _togglePhoneAction;

    [SerializeField] private GameObject _phoneUI;

    private void Awake()
    {
        _playerInput = new InputActions();
        _clickAction = _playerInput.DeliveryGameplay.Click;
        _togglePhoneAction = _playerInput.DeliveryGameplay.TogglePhone;
        _mainCamera = Camera.main;
        
        _playerInput.Enable();
    }

    private void OnDestroy()
    {
        _playerInput.Disable();
    }

    private void OnEnable()
    {
        _clickAction.performed += OnClickPerformed;
        _togglePhoneAction.performed += TogglePhoneFromInput;
    }

    private void OnDisable()
    {
        _clickAction.performed -= OnClickPerformed;
        _togglePhoneAction.performed -= TogglePhoneFromInput;
    }

    private void OnClickPerformed(InputAction.CallbackContext context)
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
