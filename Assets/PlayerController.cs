using UnityEngine;
using UnityEngine.InputSystem;

namespace _Scripts.Player
{
    /// <summary>
    /// Handles player movement with smooth acceleration and deceleration.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float _moveSpeed = 5f;  // Max movement speed
        [SerializeField] private float _acceleration = 10f;  // Smooth start speed
        [SerializeField] private float _deceleration = 10f;  // Smooth stop

        private Rigidbody2D _rb;
        private InputSystem_Actions _inputActions;
        private Vector2 _movementInput;
        private Vector2 _currentVelocity;

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
            // Smooth movement with acceleration & deceleration
            if (_movementInput.magnitude > 0)
            {
                _currentVelocity = Vector2.Lerp(_currentVelocity, _movementInput * _moveSpeed, _acceleration * Time.fixedDeltaTime);
            }
            else
            {
                _currentVelocity = Vector2.Lerp(_currentVelocity, Vector2.zero, _deceleration * Time.fixedDeltaTime);
            }

            _rb.linearVelocity = _currentVelocity;
        }

        private void OnDisable()
        {
            _inputActions.Player.Disable();
        }
    }
}