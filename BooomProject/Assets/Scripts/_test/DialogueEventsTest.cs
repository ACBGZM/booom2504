using UnityEngine;

public class DialogueEventsTest : MonoBehaviour, IInteractable
{
    [SerializeField]
    private EventSequenceExecutor executor;

    public void Interact(_PlayerController player)
    {
        executor.Initialize(OnExecutorFinished);
        executor.Execute();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && collision.TryGetComponent(out _PlayerController player))
        {
            player.m_interactable = this;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && collision.TryGetComponent(out _PlayerController player))
        {
            if (player.m_interactable is DialogueEventsTest dialogue_interactor && dialogue_interactor == this)
            {
                player.m_interactable = null;   
            }
        }
    }

    private void OnExecutorFinished(bool success)
    {
        Debug.Log("success: " + success);
    }
}
