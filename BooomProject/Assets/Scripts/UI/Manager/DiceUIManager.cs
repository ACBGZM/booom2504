using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiceUIManager : Singleton<DiceUIManager>
{
    public GameObject diceCanvas;

    protected override void init()
    {
        
    }
    private void Start()
    {
        
        HideMe();
    }
    public void ShowMe()
    {
        diceCanvas.SetActive(true);
    }

    public void HideMe()
    {
        diceCanvas.SetActive(false);
    }
}
