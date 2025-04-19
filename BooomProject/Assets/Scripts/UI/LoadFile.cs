using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LoadFile : MonoBehaviour
{
    /// <summary>
    /// 加载订单顾客头像
    /// </summary>
    /// <param name="path">文件路径</param>
    /// <returns></returns>
    public static Sprite LoadImage(string path)
    {
        // 创建文件流
        FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
        fileStream.Seek(0, SeekOrigin.Begin);

        // 创建文件长度缓冲区
        byte[] bytes = new byte[fileStream.Length];
        fileStream.Read(bytes, 0, (int)fileStream.Length); // 读取文件

        // 关闭文件流
        fileStream.Close();
        fileStream.Dispose();
        fileStream = null;

        // 创建TexuTure 更换图片
        Texture2D texture2D = new Texture2D(154, 147);
        texture2D.LoadImage(bytes);

        Sprite sprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f));
        return sprite;
    }
}
