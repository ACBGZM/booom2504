using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class FloatingButton : MonoBehaviour
{
    public Transform TargetObject { get; set; }
    public Vector3 Offset { get; set; }= new Vector3(0, 1.5f, 0);

    [SerializeField] private Camera _mainCamera;

    private RectTransform _buttonRect;
    private Button _button;

    void Start()
    {
        _buttonRect = GetComponent<RectTransform>();
        _button = GetComponent<Button>();
    }

    void Update()
    {
        if (TargetObject == null)
        {
            return;
        }

        Vector3 worldPos = TargetObject.position + Offset;
        Vector2 screenPos = _mainCamera.WorldToScreenPoint(worldPos);

        _buttonRect.position = screenPos;

        if (worldPos.z < _mainCamera.transform.position.z)
        {
            SetActive(false);
        }
        else
        {
            SetActive(true);
        }
    }

    public void SetActive(bool active)
    {
        _buttonRect.gameObject.SetActive(active);
    }

    public void SetButtonAction(UnityAction action)
    {
        if (action == null)
        {
            _button.onClick.RemoveAllListeners();
        }
        else
        {
            _button.onClick.AddListener(action);
        }
    }
}
