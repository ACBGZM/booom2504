using DG.Tweening;
using UnityEngine;

public class DeliveryPlayerVisual : MonoBehaviour
{
    [SerializeField] private float _idleSwayAmount;
    [SerializeField] private float _idleSwayDuration;

    [SerializeField] private float _moveTiltAngle;
    [SerializeField] private float _moveTiltDuration;

    private Tween _idleTween;
    private Tween _moveTween;
    private Vector3 _originalLocalPosition;
    private Quaternion _originalLocalRotation;

    private SpriteRenderer _spriteRenderer;

    private void Start()
    {
        _originalLocalPosition = transform.localPosition;
        _originalLocalRotation = transform.localRotation;

        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    public void PlayIdleAnimation()
    {
        _idleTween?.Kill();
        _moveTween?.Kill();

        transform.localPosition = _originalLocalPosition;
        transform.localRotation = _originalLocalRotation;

        _idleTween = transform.DOLocalMoveX(
                _originalLocalPosition.x + _idleSwayAmount, _idleSwayDuration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }

    public void PlayMoveAnimation()
    {
        _idleTween?.Kill();
        transform.localPosition = _originalLocalPosition;
        transform.localRotation = _originalLocalRotation;

        _moveTween = transform.DOLocalRotate(
                new Vector3(0, 0, -_moveTiltAngle),
                _moveTiltDuration * 0.5f,
                RotateMode.Fast
            )
            .SetEase(Ease.InOutSine)
            .OnComplete(() => {
                _moveTween = transform.DOLocalRotate(
                        new Vector3(0, 0, _moveTiltAngle * 2), // -angle => +angle: 2 * angle
                        _moveTiltDuration,
                        RotateMode.LocalAxisAdd
                    )
                    .SetEase(Ease.InOutSine)
                    .SetLoops(-1, LoopType.Yoyo);
            });
    }

    public void StopAllAnimations()
    {
        _idleTween?.Kill();
        _moveTween?.Kill();

        transform.localPosition = _originalLocalPosition;
        transform.localRotation = _originalLocalRotation;
    }

    public void SetFlip(bool flip)
    {
        _spriteRenderer.flipX = flip;
    }
}
