using UnityEngine;
using UnityEngine.SceneManagement;

public enum EPlayerState {
    PlayerIdle,
    PlayerMoving,
    InCutscene,
}

public class CommonGameplayManager : MonoBehaviour {
    private static CommonGameplayManager _instance;
    private static bool _isQuitting = false;

    private CommonGameplayManager() { }

    public static CommonGameplayManager GetInstance() {
        // 如果应用正在退出，不生成新的实例
        if (_isQuitting) {
            Debug.LogWarning("应用程序正在退出，不创建新实例。");
            return null;
        }

        if (_instance == null) {
            lock (typeof(CommonGameplayManager)) {
                if (_instance == null) {
                    GameObject go = new GameObject("CommonGameplayManager");
                    _instance = go.AddComponent<CommonGameplayManager>();
                    DontDestroyOnLoad(go);
                }
            }
        }
        return _instance;
    }

    private void Awake() {
        if (_instance != null && _instance != this) {
            Destroy(gameObject);
        } else {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        // 初始化子管理器
        _timeManager = GetComponentInChildren<TimeManager>();
        _orderDataManager = GetComponentInChildren<OrderDataManager>();
        _nodeGraphManager = GetComponentInChildren<NodeGraphManager>();
        _playerDataManager = GetComponentInChildren<PlayerDataManager>();
    }

    private void Start() {
        Application.targetFrameRate = 60;
    }

    private TimeManager _timeManager;
    public TimeManager TimeManager => _timeManager;

    private OrderDataManager _orderDataManager;
    public OrderDataManager OrderDataManager => _orderDataManager;

    private NodeGraphManager _nodeGraphManager;
    public NodeGraphManager NodeGraphManager => _nodeGraphManager;

    private PlayerDataManager _playerDataManager;
    public PlayerDataManager PlayerDataManager => _playerDataManager;

    [SerializeField
#if UNITY_EDITOR
        , ReadOnly
#endif
    ]
    private EPlayerState _playerState = EPlayerState.PlayerIdle;
    public EPlayerState PlayerState {
        get => _playerState;
        set {
            if (_playerState != value) {
                _playerState = value;
                EventHandlerManager.CallPlayerStateChanged();
            }
        }
    }

    // 当应用退出时设置标志，避免生成新实例
    private void OnApplicationQuit() {
        _isQuitting = true;
    }
}
