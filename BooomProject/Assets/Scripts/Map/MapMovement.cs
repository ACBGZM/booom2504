using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MapMovement : MonoBehaviour
{
    private bool _isDragging;
    public Vector2 dragStartPos;

    private InputAction _zoomInputAction;
    private InputAction _dragInputAction;
    private InputAction _inputPositionAction;
    private InputAction _restoreCameraInputAction;

    [SerializeField] private Camera _camera;

    [SerializeField] private float _zoomSpeed;
    [SerializeField] private float _minOrthoSize;
    [SerializeField] private float _maxOrthoSize;

    [SerializeField] private float _dragSpeed;

    /*
    [SerializeField] private float _baseHeight;
    [SerializeField] private float _baseSize;

    private void UpdateOrthoSize()
    {
        float ratio = Screen.height / _baseHeight;
        _camera.orthographicSize = _baseSize / ratio;
    }

#if UNITY_EDITOR
    private void OnValidate() {
        if (_camera != null)
        {
            UpdateOrthoSize();
        }
    }
#endif
    */

    private Sprite _mapSprite;

    //  0   3
    //  1   2
    private List<Vector3> _mapVertices;

    private Vector2 _minBound;
    private Vector2 _maxBound;

    private void Awake()
    {
        _mapSprite = GetComponent<SpriteRenderer>().sprite;
        Bounds bounds =  _mapSprite.bounds; // AABB
        _minBound = bounds.min * transform.localScale.x * 0.98f;
        _maxBound = bounds.max * transform.localScale.x * 0.98f;

        Vector3 bottomLeft = _minBound;
        Vector3 topRight = _maxBound;
        Vector3 topLeft = new Vector3(_minBound.x, _maxBound.y, bounds.center.z);
        Vector3 bottomRight = new Vector3(_maxBound.x, _minBound.y, bounds.center.z);

        _mapVertices = new List<Vector3>()
        {
            topLeft,
            bottomLeft,
            bottomRight,
            topRight,
        };
    }

    private void Start()
    {
        _zoomInputAction = DeliveryGameplayManager.Instance.DeliverySceneInputHandler.DeliveryGameplayInputActions.ZoomMap;
        _dragInputAction = DeliveryGameplayManager.Instance.DeliverySceneInputHandler.DeliveryGameplayInputActions.DragMap;
        _inputPositionAction = DeliveryGameplayManager.Instance.DeliverySceneInputHandler.DeliveryGameplayInputActions.InputPosition;
        _restoreCameraInputAction = DeliveryGameplayManager.Instance.DeliverySceneInputHandler.DeliveryGameplayInputActions.RestoreCamera;

        _dragInputAction.started += StartDrag;
        _dragInputAction.canceled += StopDrag;
    }

    private void Update()
    {
        if (DeliveryGameplayManager.Instance.DeliverySceneInputHandler.IsPointerOverUI())
        {
            _isDragging = false;
            return;
        }

        Zoom();
        Drag();

        RestoreCameraToPlayer();
        Adjust();
    }

    private void Zoom()
    {
        float scroll = _zoomInputAction.ReadValue<float>();
        if (scroll == 0)
        {
            return;
        }

        float t = Mathf.InverseLerp(_minOrthoSize, _maxOrthoSize, _camera.orthographicSize);

        float speedFactor = GetSpeedFactor(t);
        float adjustedSpeed = _zoomSpeed * speedFactor;

        _camera.orthographicSize = Mathf.Clamp(
            _camera.orthographicSize - scroll * adjustedSpeed,
            _minOrthoSize,
            _maxOrthoSize
        );
    }

    private float GetSpeedFactor(float ratio)
    {
        /*
        Speed Factor
            |
         1  |      __________
            |     /          \
            |    /            \
        0.2 |___/              \___
            minSize          maxSize

            中心区域（30%~70%）：全速
            边界缓冲（30%）：线性减速到全速的25%
        */
        return ratio switch
        {
            > 0.3f and < 0.7f => 1.0f,
            <= 0.3f => Mathf.Lerp(0.25f, 1.0f, ratio / 0.3f),
            _ => Mathf.Lerp(0.25f, 1.0f, (1.0f - ratio) / 0.3f)
        };
    }

    private void Adjust()
    {
        // 计算当前 orthographicSize 下的视口半宽和半高
        float verticalExtent = _camera.orthographicSize;
        float horizontalExtent = verticalExtent * _camera.aspect;

        // 动态调整边界，确保相机不会超出
        float minX = _minBound.x + horizontalExtent;
        float maxX = _maxBound.x - horizontalExtent;
        float minY = _minBound.y + verticalExtent;
        float maxY = _maxBound.y - verticalExtent;

        // 如果视口比边界还大，则强制居中
        if (minX > maxX)
        {
            minX = maxX = (_minBound.x + _maxBound.x) / 2f;
        }
        if (minY > maxY)
        {
            minY = maxY = (_minBound.y + _maxBound.y) / 2f;
        }

        // 限制相机位置
        Vector3 clampedPosition =  _camera.transform.position;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, minX, maxX);
        clampedPosition.y = Mathf.Clamp(clampedPosition.y, minY, maxY);
        _camera.transform.position = clampedPosition;
    }

    private void StartDrag(InputAction.CallbackContext context)
    {
        _isDragging = true;
        dragStartPos = _camera.ScreenToWorldPoint(_inputPositionAction.ReadValue<Vector2>());
    }

    private void StopDrag(InputAction.CallbackContext context)
    {
        _isDragging = false;
    }

    private void Drag()
    {
        if (_isDragging && _dragInputAction.IsPressed())
        {
            Vector2 currentPos = _camera.ScreenToWorldPoint(_inputPositionAction.ReadValue<Vector2>());
            Vector2 diff = dragStartPos - currentPos;

            float adjustedSpeed = Mathf.Clamp(
                _dragSpeed / _camera.orthographicSize,
                _dragSpeed * 0.5f,
                _dragSpeed * 1.2f
            );

            _camera.transform.position += (Vector3)diff * adjustedSpeed;
        }
    }

    private void RestoreCameraToPlayer()
    {
        if (_restoreCameraInputAction.IsPressed())
        {
            _camera.transform.position = new(
                DeliveryGameplayManager.Instance.DeliveryPlayer.transform.position.x,
                DeliveryGameplayManager.Instance.DeliveryPlayer.transform.position.y,
                _camera.transform.position.z);
        }
    }
}
