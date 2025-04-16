using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitInteract : MonoBehaviour,IInteractable
{
    public void Interact(PlayerController player) {
        Debug.Log("Exit Interact");
    }
}
