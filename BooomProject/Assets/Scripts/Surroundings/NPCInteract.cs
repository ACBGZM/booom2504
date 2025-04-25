using UnityEngine;

public class NPCInteract : MonoBehaviour, IInteractable
{
    [SerializeField] private EventSequenceExecutor executor;

    public void Interact(PlayerController player)
    {
        if (executor == null)
        {
            return;
        }

        executor.Initialize(OnExecutorFinished);
        executor.Execute();
    }

    private void OnExecutorFinished(bool success)
    {
        // Debug.Log("success: " + success);
    }
}
