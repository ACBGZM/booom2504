using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class NPCInteract : MonoBehaviour, IInteractable {
    [SerializeField] private EventSequenceExecutor[] executor;
    [SerializeField] private EventSequenceExecutor[] _randomExecutors;
    private Dictionary<int, bool> _executorsFinished = new Dictionary<int, bool>();
    private Animator _animator;

    public int currentExecutor;

    private void Start()
    {
        if(_animator == null) _animator=GetComponent<Animator>();
        for (int i = 0; i < executor.Length; ++i)
        {
            _executorsFinished.Add(i, false);
        }
        CheckOrderCount();
    }

    public string GetInteractableName() {
        return this.gameObject.name;
    }

    public void Interact(PlayerController player) {
        CheckOrderCount();
        if (executor[currentExecutor] != null && !_executorsFinished[currentExecutor])
        {
            executor[currentExecutor].Initialize(OnExecutorFinished);
            executor[currentExecutor].Execute();
            _executorsFinished[currentExecutor] = true;
        }
        else
        {
            int random = Random.Range(0, _randomExecutors.Length);
            _randomExecutors[random].Initialize(OnExecutorFinished);
            _randomExecutors[random].Execute();
        }
    }

    public void CheckOrderCount() {
        PlayerDataManager playerData = CommonGameplayManager.GetInstance().PlayerDataManager;

        if (playerData.finishedOrderCount == 0) {
            currentExecutor = 0;
        } else if (playerData.finishedOrderCount > 5) {
            currentExecutor = 1;
        } else if (playerData.finishedOrderCount > 10) {
            currentExecutor = 2;
            _animator.SetBool("ShowTail", true);
        }
    }

    private void OnExecutorFinished(bool success) {
        // Debug.Log("success: " + success);
    }
}
