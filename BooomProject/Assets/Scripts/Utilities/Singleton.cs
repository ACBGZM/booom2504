using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    private static T instance;
    public static T Instance => instance;

    protected virtual void Awake()
    {
        if (Instance == null) instance = (T)this;
        else
        {
            Destroy(gameObject);
        }
        init();
    }

    protected virtual void OnDestroy()
    {
        if (Instance != null)
        {
            instance = null;
        }
    }

    protected virtual void init()
    {
    }
}
