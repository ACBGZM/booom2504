using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    public static PlayerController controllerInstance { get; private set; }

    [SerializeField] private GameInput _gameInput;

    private bool _isWalking = false;
    private Rigidbody2D _playerRigidBody;
    private Vector2 _moveDir;

    private void Awake() {
        if (controllerInstance != null) {
            Debug.LogError("There are multiple PlayerController instances in the scene.");
        }
        controllerInstance = this;
        _playerRigidBody = GetComponent<Rigidbody2D>();
    }

    private void Start() {
        _gameInput.OnInteractAction += GameInput_OnInteractAction;
    }

    private void FixedUpdate() {
        HandleMovement();
    }

    private void GameInput_OnInteractAction(object sender, System.EventArgs e) {
        Debug.Log("Interact button clicked");
    }

    private void HandleMovement() {
        _moveDir = _gameInput.GetMovement();
        _isWalking = _moveDir != Vector2.zero;
        _playerRigidBody.MovePosition(_playerRigidBody.position + (new Vector2(_moveDir.x, _moveDir.y)) * GameplaySettings.m_walk_speed * Time.deltaTime);
        if (_moveDir.x != 0) {
            float facingDir = 1.0f;
            if (_moveDir.x < 0) facingDir = -1.0f;
            transform.localScale = new Vector3(facingDir, transform.localScale.y, transform.localScale.z);
        }
    }
}
