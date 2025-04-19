using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LoadFile : MonoBehaviour
{
    /// <summary>
    /// ���ض����˿�ͷ��
    /// </summary>
    /// <param name="path">�ļ�·��</param>
    /// <returns></returns>
    public static Sprite LoadImage(string path)
    {
        // �����ļ���
        FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
        fileStream.Seek(0, SeekOrigin.Begin);

        // �����ļ����Ȼ�����
        byte[] bytes = new byte[fileStream.Length];
        fileStream.Read(bytes, 0, (int)fileStream.Length); // ��ȡ�ļ�

        // �ر��ļ���
        fileStream.Close();
        fileStream.Dispose();
        fileStream = null;

        // ����TexuTure ����ͼƬ
        Texture2D texture2D = new Texture2D(154, 147);
        texture2D.LoadImage(bytes);

        Sprite sprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f));
        return sprite;
    }
}
