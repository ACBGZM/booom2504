using UnityEngine;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    private GameManager() { }
    public static GameManager GetInstance() // => s_instance;
    {
        if (_instance == null)
        {
            lock (typeof(GameManager))
            {
                if (_instance == null)
                {
                    GameObject go = new GameObject("GameManager");
                    _instance = go.AddComponent<GameManager>();
                    //DontDestroyOnLoad(go);
                }
            }
        }

        return _instance;
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
            //DontDestroyOnLoad(gameObject);
        }
    }

    [SerializeField] private DeliveryPlayer _deliveryPlayer;
    public DeliveryPlayer DeliveryPlayer => _deliveryPlayer;
    
    [SerializeField] private NodeGraphManager _nodeGraphManager;
    public NodeGraphManager NodeGraphManager => _nodeGraphManager;
    
    [SerializeField] private DeliveryInputHandler _deliveryInputHandler;
    public DeliveryInputHandler DeliveryInputHandler => _deliveryInputHandler;
}
