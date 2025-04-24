using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.UI;



public class ChangHomeTime : MonoBehaviour
{
    [SerializeField] private GameObject backGround;
    [SerializeField] private Sprite mBackGround;
    [SerializeField] private Sprite lBackGround;
    [SerializeField] private Sprite aBackGround;
    [SerializeField] private Sprite nBackGround;

    public void ChangePicture(int time)
    {
        if (time < 8 && time > 5)
        {
            SpriteRenderer backGroundSpriteRenderer = backGround.GetComponent<SpriteRenderer>();
            backGroundSpriteRenderer.sprite = mBackGround;
        }
        else if (time > 8 && time < 12)
        {
            SpriteRenderer backGroundSpriteRenderer = backGround.GetComponent<SpriteRenderer>();
            backGroundSpriteRenderer.sprite = lBackGround;
        }
        else if (time > 12 && time < 18)
        {
            SpriteRenderer backGroundSpriteRenderer = backGround.GetComponent<SpriteRenderer>();
            backGroundSpriteRenderer.sprite = aBackGround;
        }
        else if (time > 22)
        {
            SpriteRenderer backGroundSpriteRenderer = backGround.GetComponent<SpriteRenderer>();
            backGroundSpriteRenderer.sprite = nBackGround;
        }
    }
}
