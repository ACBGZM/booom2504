using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(AudioSource))]
public class SoundsManager : Singleton<SoundsManager>
{
    public AudioClip good;
    public AudioClip bad;
    public AudioSource audioSource;
    protected override void init()
    {
        audioSource = GetComponent<AudioSource>();
    }


    
}
