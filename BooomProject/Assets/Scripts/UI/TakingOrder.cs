using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TakingOrder : MonoBehaviour
{
    public int range;
    public event Action<string, string, string, string, int, Sprite, string> myOrder; 
}
