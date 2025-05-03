using System;
using UnityEngine;

public class PlayerEnterTrigger : MonoBehaviour
{
    [SerializeField] private EventSequenceExecutor _eventSequenceExecutor;

    // private void OnTriggerEnter2D(Collider2D other)
    // {
    //     if (other.gameObject.CompareTag("Player"))
    //     {
    //         _eventSequenceExecutor.Initialize(null);
    //         _eventSequenceExecutor.Execute();
    //     }
    // }
    //
    // private void OnCollisionEnter2D(Collision2D other)
    // {
    //     if (other.gameObject.CompareTag("Player"))
    //     {
    //         _eventSequenceExecutor.Initialize(null);
    //         _eventSequenceExecutor.Execute();
    //     }
    // }

    public void TriggerEvent()
    {
        _eventSequenceExecutor.Initialize(null);
        _eventSequenceExecutor.Execute();
    }
}
