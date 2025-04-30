using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 更换鼠标图片
/// </summary>
public class MouseTxu2D : MonoBehaviour
{
    [SerializeField] Texture2D mouseTex;//放图片

    private  void Start()
    {
        Cursor.SetCursor(mouseTex, new Vector2(55, 63), CursorMode.Auto);
        DontDestroyOnLoad(this);
    }
}
