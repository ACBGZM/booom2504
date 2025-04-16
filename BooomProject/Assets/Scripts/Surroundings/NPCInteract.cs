using UnityEngine;
using UnityEngine.Events;

public class NPCInteract : MonoBehaviour, IInteractable {
    public UnityEvent OnInteractEvent;
    public void Interact(PlayerController player) {
        Debug.Log(this.gameObject.name + " NPC Interact");
        OnInteractEvent?.Invoke();
    }
}