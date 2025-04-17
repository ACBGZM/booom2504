using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{

    private static T instance;
    public static T Instance => instance;
    // Start is called before the first frame update

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
