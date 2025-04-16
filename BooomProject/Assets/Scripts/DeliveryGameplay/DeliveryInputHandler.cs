using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class DeliveryInputHandler : MonoBehaviour
{
    private Camera m_main_camera;
    private InputActions m_player_input;
    private InputAction m_click_action;
    private InputAction m_test_action;

    private void Awake()
    {
        m_player_input = new InputActions();
        m_click_action = m_player_input.DeliveryGameplay.Click;
        m_test_action = m_player_input.DeliveryGameplay.Test;
        m_main_camera = Camera.main;
        
        m_player_input.Enable();
    }

    private void OnDestroy()
    {
        m_player_input.Disable();
    }

    private void OnEnable()
    {
        m_click_action.performed += OnClickPerformed;
        m_test_action.performed += Test;
    }

    private void OnDisable()
    {
        m_click_action.performed -= OnClickPerformed;
        m_test_action.performed -= Test;
    }

    private void OnClickPerformed(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            Ray ray = m_main_camera.ScreenPointToRay(mousePosition);
            RaycastHit2D hit = Physics2D.GetRayIntersection(ray);

            if (hit.collider != null)
            {
                hit.collider.GetComponent<IClickable>()?.OnClick();
            }
        }
    }
    
    private void Test(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            Debug.Log("Test action performed");
        }
    }
}
