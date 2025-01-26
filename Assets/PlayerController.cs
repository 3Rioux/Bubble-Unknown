using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _Scripts.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float _maxSpeed = 5f;      // Max movement speed
        [SerializeField] private float _acceleration = 15f; // Acceleration factor
        [SerializeField] private float _deceleration = 20f; // Deceleration factor
       
        private Vector2 _movementInput;
        private Vector2 _targetVelocity;

        [Header("Dash Settings")]
        [SerializeField] private float _dashSpeed = 15f;    // Speed of the dash
        [SerializeField] private float _dashDuration = 0.2f; // Duration of the dash
        [SerializeField] private float _dashCooldown = 1f;  // Time before the dash can be used again
        [SerializeField] private TrailRenderer _dashTrail; 

        private bool _isDashing = false;
        private bool _canDash = true;
        private float _dashCooldownTimer;

        [SerializeField] private Animator _animator;
        private Rigidbody2D _rb;
        private InputSystem_Actions _inputActions;

        private void Awake()
        {
            if(_animator ==null) _animator = GetComponentInChildren<Animator>();
            if(_dashTrail == null) _dashTrail = GetComponentInChildren<TrailRenderer>();
            _rb = GetComponent<Rigidbody2D>();

            _inputActions = new InputSystem_Actions();
            _inputActions.Player.Enable();

            _inputActions.Player.Move.performed += ctx => _movementInput = ctx.ReadValue<Vector2>();
            _inputActions.Player.Move.canceled += ctx => _movementInput = Vector2.zero;

            _inputActions.Player.Jump.performed += _ => AttemptDash(); // Trigger dash when Dash input is pressed
        }

        private void FixedUpdate()
        {
            if (_isDashing) return; // Skip normal movement during dash

            MovePlayer();
        }

        private void MovePlayer()
        {
            if (_movementInput.sqrMagnitude > 0.01f)  // Check for small inputs
            {
                // Calculate desired velocity based on input
                _targetVelocity = _movementInput.normalized * _maxSpeed;

                // Smooth acceleration towards target velocity
                _rb.linearVelocity = Vector2.Lerp(_rb.linearVelocity, _targetVelocity, _acceleration * Time.fixedDeltaTime);
            }
            else
            {
                // Smooth deceleration to stop
                _rb.linearVelocity = Vector2.Lerp(_rb.linearVelocity, Vector2.zero, _deceleration * Time.fixedDeltaTime);
            }
            // Calculate the magnitude of the velocity
            float movementSpeed = _rb.linearVelocity.magnitude;
            _animator.SetFloat("speed", movementSpeed);

        }

        private void AttemptDash()
        {
            if (_canDash && _movementInput.sqrMagnitude > 0.01f) // Check if dashing is allowed and there's input
            {
                StartCoroutine(Dash());
            }
        }

        private IEnumerator Dash()
        {

            _dashTrail.enabled = true;
            _isDashing = true;
            _canDash = false;

            // Play dash animation (optional)
            _animator.SetTrigger("dash");


            // Apply dash force
            Vector2 dashDirection = _movementInput.normalized;
            _rb.linearVelocity = dashDirection * _dashSpeed;

            // Wait for the dash duration
            yield return new WaitForSeconds(_dashDuration);

            // Stop dash and return to normal movement
            _isDashing = false;
            _dashTrail.enabled = false;

            // Start cooldown timer
            StartCoroutine(DashCooldown());
        }

        private IEnumerator DashCooldown()
        {
            _dashCooldownTimer = _dashCooldown;

            while (_dashCooldownTimer > 0)
            {
                _dashCooldownTimer -= Time.deltaTime;
                yield return null;
            }

            _canDash = true;
        }



        private void OnDisable()
        {
            _inputActions.Player.Disable();
        }
    }
}