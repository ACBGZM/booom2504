using UnityEngine;

public class NPCInteract : MonoBehaviour, IInteractable {
    [SerializeField] private EventSequenceExecutor[] executor;
    public int currentExecutor;

    public string GetInteractableName() {
        return this.gameObject.name;
    }

    public void Interact(PlayerController player) {
        CheckOrderCount();
        if (executor[currentExecutor] == null) return;
        executor[currentExecutor].Initialize(OnExecutorFinished);
        executor[currentExecutor].Execute();
    }

    public void CheckOrderCount() {
        PlayerDataManager playerData = CommonGameplayManager.GetInstance().PlayerDataManager;

        if (playerData.finishedOrderCount == 0) {
            currentExecutor = 0;
        } else if (playerData.finishedOrderCount > 5) {
            currentExecutor = 1;
        } else if (playerData.finishedOrderCount > 10) {
            currentExecutor = 2;
        }
    }

    private void OnExecutorFinished(bool success) {
        // Debug.Log("success: " + success);
    }
}
