using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class CommonInput : MonoBehaviour
{
    protected InputActions _playerInputActions;

    private InputActions.UIActions _uiInputActions;
    public InputActions.UIActions UIInputActions => _uiInputActions;

    [SerializeField] private GameObject _phoneUI;
    [SerializeField] private GameObject _phoneMask; // 手机弹窗遮罩

    protected void Awake()
    {
        _playerInputActions = new InputActions();
        _uiInputActions = _playerInputActions.UI;

        _playerInputActions.CommonGameplay.TogglePhone.performed += TogglePhoneFromInput;

    }

    protected void OnDestroy()
    {
        _playerInputActions.CommonGameplay.TogglePhone.performed -= TogglePhoneFromInput;
    }

    protected void OnEnable()
    {
        _playerInputActions.Enable();
    }

    protected void OnDisable()
    {
        _playerInputActions.Disable();
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
            _phoneMask.SetActive(show);
        }
    }

    public void TogglePhone()
    {
        if (_phoneUI != null)
        {
            _phoneUI.SetActive(!_phoneUI.activeInHierarchy);
            _phoneMask.SetActive(!_phoneMask.activeInHierarchy);
        }
    }

    public bool IsPointerOverUI()
    {
        return EventSystem.current.IsPointerOverGameObject(PointerInputModule.kMouseLeftId);
    }
}
