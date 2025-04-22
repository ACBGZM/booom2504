using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] public PlayerController _playerController;

    private const string ANIMATOR_TRIGGER_IS_WALKING = "IsWalking";

    private Animator _animator;

    private void Awake() => _animator = GetComponent<Animator>();

    private void Update() {
        _animator.SetBool(ANIMATOR_TRIGGER_IS_WALKING, _playerController.IsWalking());
    }

}
