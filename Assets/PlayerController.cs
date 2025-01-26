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

        private Rigidbody2D _rb;
        private InputSystem_Actions _inputActions;
        private Vector2 _movementInput;
        private Vector2 _targetVelocity;
        
        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();

            _inputActions = new InputSystem_Actions();
            _inputActions.Player.Enable();

            _inputActions.Player.Move.performed += ctx => _movementInput = ctx.ReadValue<Vector2>();
            _inputActions.Player.Move.canceled += ctx => _movementInput = Vector2.zero;
        }

        private void FixedUpdate()
        {
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
        }

        private void OnDisable()
        {
            _inputActions.Player.Disable();
        }
    }
}