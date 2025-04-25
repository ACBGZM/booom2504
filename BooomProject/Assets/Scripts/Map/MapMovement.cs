using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MapMovement : MonoBehaviour
{
    private static readonly float _padding = 20;
    private readonly float left = _padding;
    private readonly float right = Screen.width - _padding;
    private readonly float top = Screen.height - _padding;
    private readonly float bottom = _padding;
    public List<Transform> mapWorldCorners;

    public Vector2[] mapCurrentScreenCorners;
    private float lastMapPosX;
    private float lastMapPosY;
    // public float lastScale;
    private bool _isDragging;

    public Vector2 dragStartPos;

    private InputAction _zoomInputAction;
    private InputAction _dragInputAction;
    private InputAction _inputPositionAction;
    private InputAction _restoreCameraInputAction;

    [SerializeField] private Camera _camera;
    [SerializeField] private float _baseHeight;
    [SerializeField] private float _baseSize;

    [SerializeField] private float _zoomSpeed;
    [SerializeField] private float _minOrthoSize;
    [SerializeField] private float _maxOrthoSize;

    [SerializeField] private float _dragSpeed;
    [SerializeField] private Vector2[] _dragBoundary;

    /*
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

    private void Awake()
    {
        mapCurrentScreenCorners = new Vector2[mapWorldCorners.Count];
        // UpdateOrthoSize();
    }

    private void Start()
    {
        _zoomInputAction = GameManager.Instance.DeliverySceneInputHandler.DeliveryGameplayInputActions.ZoomMap;
        _dragInputAction = GameManager.Instance.DeliverySceneInputHandler.DeliveryGameplayInputActions.DragMap;
        _inputPositionAction = GameManager.Instance.DeliverySceneInputHandler.DeliveryGameplayInputActions.InputPosition;
        _restoreCameraInputAction = GameManager.Instance.DeliverySceneInputHandler.DeliveryGameplayInputActions.RestoreCamera;

        _dragInputAction.started += StartDrag;
        _dragInputAction.canceled += StopDrag;
    }

    private void Update()
    {
        if (GameManager.Instance.DeliverySceneInputHandler.IsPointerOverUI())
        {
            _isDragging = false;
            return;
        }

        Zoom();
        Movement();
        Drag();
        UpdateWorldCornersToScreen();

        Adjust();

        RestoreCamera();
    }

    private void Movement()
    {
        // 记录移动前的位置
        lastMapPosX = _camera.transform.position.x;
        lastMapPosY = _camera.transform.position.y;

        return;

        var pos = Input.mousePosition;
        // 摄像机移动
        if (pos.x <= left)
        {
            _camera.transform.Translate(Vector2.left * GameplaySettings.map_move_speed * Time.deltaTime);
        }
        if (pos.x >= right)
        {
            _camera.transform.Translate(Vector2.right * GameplaySettings.map_move_speed * Time.deltaTime);
        }

        if (pos.y >= top)
        {
            _camera.transform.Translate(Vector2.up * GameplaySettings.map_move_speed * Time.deltaTime);
        }
        if (pos.y <= bottom)
        {
            _camera.transform.Translate(Vector2.down * GameplaySettings.map_move_speed * Time.deltaTime);
        }
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

    private void UpdateWorldCornersToScreen()
    {
        for (int i = 0; i < mapWorldCorners.Count; i++)
        {
            mapCurrentScreenCorners[i] = _camera.WorldToScreenPoint(mapWorldCorners[i].position);
        }
    }

    private void Adjust()
    {
        //  0   3
        //  1   2

        // 左右需要调整
        if (mapCurrentScreenCorners[0].x > _padding || mapCurrentScreenCorners[2].x < Screen.width - _padding)
        {
            // 还原x位置
            _camera.transform.position = new Vector3(lastMapPosX, _camera.transform.position.y, _camera.transform.position.z);
        }
        // 上下需要调整
        if (mapCurrentScreenCorners[1].y > _padding || mapCurrentScreenCorners[3].y < Screen.height - _padding)
        {
            // 还原y位置
            _camera.transform.position = new Vector3(_camera.transform.position.x, lastMapPosY, _camera.transform.position.z);
        }
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

    private void RestoreCamera()
    {
        if (_restoreCameraInputAction.IsPressed())
        {
            _camera.transform.position = new(
                GameManager.Instance.DeliveryPlayer.transform.position.x,
                GameManager.Instance.DeliveryPlayer.transform.position.y,
                _camera.transform.position.z);
        }
    }
}
