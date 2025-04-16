using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MaxLine : MonoBehaviour
{
    public Image dialogBox;
    
    public Text chatText;
    public int maxLen = 15;


    private void Start()
    {
        string res = "";
        chatText = GetComponent<Text>();
        dialogBox = GetComponentInParent<Image>();
      
        for(int i = 0; i < chatText.text.Length; i ++ )
        {
            if(i != 0 && i % maxLen == 0)
            {
                res += '\n';
            }
            res += chatText.text[i];
        }
        chatText.text = res;
        LayoutRebuilder.ForceRebuildLayoutImmediate(dialogBox.gameObject.GetComponent<RectTransform>());

    }
}
