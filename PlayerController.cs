using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player.Controller
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private Transform playerCamera;
        [SerializeField] private Transform playerCameraRoot;
        // [SerializeField] private float _mouseSensitivity = 21.9f;
        private CustomInput input = null;
        private Vector2 moveInput = Vector2.zero;
        private Rigidbody2D _playerRigidbody;
        private CircleCollider2D _playerCollider;
        private SpriteRenderer _playerRenderer;
        private float lerpSpeed = 5.0f;
        private bool _isDodging = false;
        private bool _isSprinting = false;
        private float _targetSpeed;
        private float _currentSpeed;
        private float _forceVelocity;
        private float _walkSpeed = 5.0f;
        private float _sprintSpeed = 10.0f;
        private float _dodgeSpeed = 12.5f;
        private float _dodgeDurationTimer = 0.0f;
        private float _dodgeCooldownTimer = 0.0f;
        private float _dodgeCooldown = 1.5f;
        private float _dodgeDuration = 0.3f;
        private Vector2 _dodgeDirection;
        private Color _dodgeColor;
        private Color _playerColor;
        private float _pulseTime;

        // Start is called before the first frame update
        void Start()
        {
        }

        private void Awake()
        {
            input = new CustomInput();
            
            _playerRenderer = GetComponent<SpriteRenderer>();
            _playerRigidbody = GetComponent<Rigidbody2D>();
            _playerCollider = GetComponent<CircleCollider2D>();
            _playerColor = _playerRenderer.color;
            _dodgeColor = new Color(_playerRenderer.color.r, _playerRenderer.color.g, _playerRenderer.color.b, 0.0f);
        }

        private void OnEnable()
        {
            input.Enable();
            input.Player.Movement.performed += OnMovementPerformed;
            input.Player.Movement.canceled += OnMovementCanceled;
            input.Player.Dodge.performed += OnDodgePerformed;
            input.Player.Sprint.performed += OnSprintPerformed;
            input.Player.Sprint.canceled += OnSprintCanceled;
        }

        private void OnDisable()
        {
            input.Disable();
            input.Player.Movement.performed -= OnMovementPerformed;
            input.Player.Movement.canceled -= OnMovementCanceled;
            input.Player.Dodge.performed -= OnDodgePerformed;
            input.Player.Sprint.performed -= OnSprintPerformed;
            input.Player.Sprint.canceled -= OnSprintCanceled;
        }

        // Update is called once per frame
        void Update()
        {
            Move();
            Dodge();
            CameraMovement();
        }

        private void HideCursor()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void ShowCursor()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        private void OnMovementPerformed(InputAction.CallbackContext context)
        {
            moveInput = context.ReadValue<Vector2>();
        }

        private void OnMovementCanceled(InputAction.CallbackContext context)
        {
            moveInput = Vector2.zero;
        }

        private void OnSprintPerformed(InputAction.CallbackContext context)
        {
            _isSprinting = context.ReadValueAsButton();
        }

        private void OnSprintCanceled(InputAction.CallbackContext context)
        {
            _isSprinting = false;
        }

        private void OnDodgePerformed(InputAction.CallbackContext context)
        {
            if (_dodgeCooldownTimer <= 0.0f)
            {
                _dodgeCooldownTimer = _dodgeCooldown;
                _isDodging = true;
                _dodgeDirection = moveInput;
                _pulseTime = 0.0f;
            }
        }

        private void Move()
        {
            _targetSpeed = _isSprinting ? _sprintSpeed : _walkSpeed;

            _currentSpeed = new Vector2(_playerRigidbody.velocity.x, _playerRigidbody.velocity.y).magnitude;

            if (_currentSpeed < _targetSpeed)
            {
                _forceVelocity = Mathf.Lerp(_currentSpeed, _targetSpeed, Time.deltaTime * lerpSpeed);
            }
            else
            {
                _forceVelocity = _targetSpeed;
            }

            Vector2 targetVelocity = new Vector2(moveInput.x, moveInput.y).normalized * _forceVelocity;
            Vector2 velocityDiff = targetVelocity - _playerRigidbody.velocity;

            _playerRigidbody.AddForce(velocityDiff, ForceMode2D.Impulse);
        }

        private void CameraMovement()
        {
            playerCamera.position = Vector3.Lerp(playerCamera.position, playerCameraRoot.position, Time.deltaTime * lerpSpeed);
        }
        private void Dodge()
        {
            if (_isDodging)
            {
                _dodgeDurationTimer += Time.deltaTime;

                _pulseTime += Time.deltaTime*5;
                float pulseFactor = Mathf.PingPong(_pulseTime, 1);

                _playerRenderer.color = Color.Lerp(_playerColor, _dodgeColor, pulseFactor);
                _playerCollider.enabled = false;
                Vector2 targetVelocity = new Vector2(_dodgeDirection.x, _dodgeDirection.y).normalized * _dodgeSpeed;
                Vector2 velocityDiff = targetVelocity - _playerRigidbody.velocity;
                _playerRigidbody.AddForce(velocityDiff, ForceMode2D.Impulse);
            }

            if (_dodgeDurationTimer >= _dodgeDuration)
            {
                _isDodging = false;
                _dodgeDurationTimer = 0.0f;
                _playerRenderer.color = _playerColor;
                _playerCollider.enabled = true;
            }

            if (_dodgeCooldownTimer > 0.0f)
            {
                _dodgeCooldownTimer -= Time.deltaTime;
            }
        }
    }
}