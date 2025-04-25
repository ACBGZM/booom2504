using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] public PlayerController _playerController;

    private Animator _animator;

    private void Awake() => _animator = GetComponent<Animator>();

    private void Update()
    {
        _animator.SetBool(GameplaySettings.ANIMATOR_BOOL_IS_WALKING, _playerController.IsWalking());
    }
}
