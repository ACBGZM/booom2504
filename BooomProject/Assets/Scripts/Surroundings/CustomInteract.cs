using UnityEngine;
using UnityEngine.Events;

public class CustomInteract : MonoBehaviour, IInteractable
{
    public UnityEvent OnInteractEvent;

    public void Interact(PlayerController player)
    {
        OnInteractEvent?.Invoke();
    }
}
