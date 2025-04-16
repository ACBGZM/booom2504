using UnityEngine;

public class Node : MonoBehaviour, IClickable 
{
    public void OnClick() 
    {
        Debug.Log("node clicked");
    }
}
