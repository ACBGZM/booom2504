using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class APIChat : MonoBehaviour
{
    [SerializeField] private string apiUrl = "http://localhost:5000/chat";

    // 请求数据结构
    [System.Serializable]
    private class RequestData
    {
        public string message;
    }

    // 响应数据结构
    [System.Serializable]
    private class ResponseData
    {
        public string response;
        public string error;
    }
    private void Start()
    {
        SendChatRequest("给出一个安全的邮件地址");
    }
    public void SendChatRequest(string message)
    {
        StartCoroutine(PostRequest(message));
    }

    private IEnumerator PostRequest(string message)
    {
        // 创建请求数据
        RequestData requestData = new RequestData { message = message };
        string jsonData = JsonUtility.ToJson(requestData);

        using UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);

        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Accept", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"Error: {request.error}");
            yield break;
        }

        // 解析响应
        ResponseData response = JsonUtility.FromJson<ResponseData>(request.downloadHandler.text);

        if (!string.IsNullOrEmpty(response.error))
        {
            Debug.LogError($"API Error: {response.error}");
        }
        else
        {
            Debug.Log($"Received: {response.response}");
            // 在这里处理响应内容
        }
    }
}