// using UnityEngine;
// using UnityEngine.Events;
//
// public class CustomInteract : MonoBehaviour, IInteractable
// {
//     public UnityEvent OnInteractEvent;
//
//     public string GetInteractableName() {
//         return "出口";
//     }
//
//     public void Interact(PlayerController player)
//     {
//         if (CommonGameplayManager.GetInstance().TimeManager.IsOffWork())
//         {
//             return;
//         }
//         OnInteractEvent?.Invoke();
//     }
// }
