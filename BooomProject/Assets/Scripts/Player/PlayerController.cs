using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
    public static PlayerController controllerInstance { get; private set; }
    public event UnityAction<IInteractable,bool> OnInteractableObjectChange;
    public IInteractable currentInteractable => _interactables.Count > 0 ? _interactables[0] : null;

    [SerializeField] private BaseCampInputHandler _baseCampInputHandler;
    private List<IInteractable> _interactables = new List<IInteractable>();
    private bool _isWalking = false;
    private Rigidbody2D _playerRigidBody;
    private Vector2 _moveDir;
    private float _velocityXSmooth;

    private void Awake()
    {
        if (controllerInstance != null)
        {
            Debug.LogError("There are multiple PlayerController instances in the scene.");
        }
        controllerInstance = this;
        _playerRigidBody = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        _baseCampInputHandler.OnInteractAction += BaseCampInputHandlerOnInteractAction;
    }

    private void BaseCampInputHandlerOnInteractAction(object sender, System.EventArgs e)
    {
        currentInteractable?.Interact(PlayerController.controllerInstance);
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        _moveDir = _baseCampInputHandler.GetMovement();
        _isWalking = _moveDir != Vector2.zero;
        //_playerRigidBody.MovePosition(_playerRigidBody.position +
        //    (new Vector2(_moveDir.x, _moveDir.y)) * GameplaySettings.m_walk_speed * Time.deltaTime);
        _velocityXSmooth = 0f;
        float newVelocityX = Mathf.SmoothDamp(_playerRigidBody.velocity.x, GameplaySettings.m_walk_speed * _moveDir.x,
            ref _velocityXSmooth, GameplaySettings.m_acceleration_Time);
        _playerRigidBody.velocity = new Vector2(newVelocityX, _playerRigidBody.velocity.y);
        if (_moveDir.x != 0)
        {
            float facingDir = 1.0f;
            if (_moveDir.x < 0) facingDir = -1.0f;
            transform.localScale = new Vector3(facingDir * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    public void AddInteractable(IInteractable interactable)
    {
        if (!_interactables.Contains(interactable))
        {
            _interactables.Add(interactable);
        }
    }

    public void RemoveInteractable(IInteractable interactable)
    {
        if (_interactables.Contains(interactable))
        {
            _interactables.Remove(interactable);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Interactable") && collision.TryGetComponent(out IInteractable interactable))
        {
            AddInteractable(interactable);
            OnInteractableObjectChange?.Invoke(interactable, true);
        }

        if(collision.gameObject.TryGetComponent(out PlayerEnterTrigger playerEnterTrigger))
        {
            playerEnterTrigger.TriggerEvent();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Interactable") && collision.TryGetComponent(out IInteractable interactable))
        {
            RemoveInteractable(interactable);
            OnInteractableObjectChange?.Invoke(interactable, false);
        }
    }

    public bool IsWalking() => _isWalking;

    private void OnDestroy()
    {
        if (_baseCampInputHandler != null)
        {
            _baseCampInputHandler.OnInteractAction -= BaseCampInputHandlerOnInteractAction;
        }
        if (controllerInstance == this)
        {
            controllerInstance = null;
        }
    }
}
