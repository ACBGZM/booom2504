using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MapMovement : MonoBehaviour
{
    static readonly float _padding = 20;
    readonly float left = _padding;
    readonly float right = Screen.width - _padding;
    readonly float top = Screen.height - _padding;
    readonly float bottom = _padding;
    public List<Transform> mapWorldCorners;
  
    public Vector2[] mapCurrentScreenCorners;
    private float lastMapPosX;
    private float lastMapPosY;
    // public float lastScale;
    // public bool draging;
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
    
    void Awake()
    {
        mapCurrentScreenCorners = new Vector2[mapWorldCorners.Count];
        // UpdateOrthoSize();
    }

    void Start()
    {
        _zoomInputAction = GameManager.Instance.DeliverySceneInputHandler.DeliveryGameplayInputActions.ZoomMap;
        _dragInputAction = GameManager.Instance.DeliverySceneInputHandler.DeliveryGameplayInputActions.DragMap;
        _inputPositionAction = GameManager.Instance.DeliverySceneInputHandler.DeliveryGameplayInputActions.InputPosition;
        _restoreCameraInputAction = GameManager.Instance.DeliverySceneInputHandler.DeliveryGameplayInputActions.RestoreCamera;

        _dragInputAction.started += StartDrag;
        _dragInputAction.canceled += StopDrag;
    }

    void Update()
    {
        //Debug.Log(Mouse.current);
        //Debug.Log(Keyboard.current);
        //print(Mouse.current.position.ReadValue());
     //   print(mapCurrentScreenCorners[0].x);
        // ���µ�ͼ�߽�ӳ�䵽��Ļ
        Zoom();
        Movement();
        Drag();
        UpdateWorldCornersToScreen();
  
        Adjust();

        RestoreCamera();
    }
    
    void Movement()
    {
        // ��¼�ƶ�ǰ��λ��
        lastMapPosX = _camera.transform.position.x;
        lastMapPosY = _camera.transform.position.y;
        
        return;
        
        var pos = Input.mousePosition;
        // ������ƶ�
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

    void Zoom()
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
            
            ��������30%~70%����ȫ��
            �߽绺�壨30%�������Լ��ٵ�ȫ�ٵ�25%
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
        for(int i = 0; i < mapWorldCorners.Count; i++)
        {
            mapCurrentScreenCorners[i] = _camera.WorldToScreenPoint(mapWorldCorners[i].position);
        }
    }

    private void Adjust()
    {
        //  0   3
        //  1   2
        
        // ������Ҫ����
        if (mapCurrentScreenCorners[0].x > _padding  || mapCurrentScreenCorners[2].x < Screen.width - _padding)
        {
            // ��ԭxλ��
            _camera.transform.position = new Vector3(lastMapPosX, _camera.transform.position.y, _camera.transform.position.z);
        }
        // ������Ҫ����
        if (mapCurrentScreenCorners[1].y > _padding || mapCurrentScreenCorners[3].y < Screen.height - _padding)
        {
            // ��ԭyλ��
            _camera.transform.position = new Vector3(_camera.transform.position.x, lastMapPosY, _camera.transform.position.z);
        }
    }
    
    private void StartDrag(InputAction.CallbackContext context)
    {
        dragStartPos = _camera.ScreenToWorldPoint(_inputPositionAction.ReadValue<Vector2>());
    }
    
    private void StopDrag(InputAction.CallbackContext context)
    {
    }
    
    private void Drag()
    {
        if (_dragInputAction.IsPressed())
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
