using UnityEngine;

public class DialogueEventsTest : MonoBehaviour, IInteractable
{
    [SerializeField]
    private EventSequenceExecutor executor;

    public void Interact(PlayerController player)
    {
        executor.Initialize(OnExecutorFinished);
        executor.Execute();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && collision.TryGetComponent(out PlayerController player))
        {
            //player._interactable = this;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && collision.TryGetComponent(out PlayerController player))
        {
            //if (player._interactable is DialogueEventsTest dialogue_interactor && dialogue_interactor == this)
            //{
            //    player._interactable = null;   
            //}
        }
    }

    private void OnExecutorFinished(bool success)
    {
        Debug.Log("success: " + success);
    }
}
