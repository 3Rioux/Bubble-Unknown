using UnityEngine;
using UnityEngine.InputSystem;

namespace _Scripts.UI
{
    /// <summary>
    /// Makes the crosshair follow the mouse cursor position.
    /// </summary>
    public class CrosshairController : MonoBehaviour
    {
        [SerializeField] private RectTransform _crosshairUI;  // Assign this in the Inspector

        private void Awake()
        {
            Cursor.visible = false;  // Hide the system cursor
            Cursor.lockState = CursorLockMode.None;  // Unlock cursor movement
        }

        private void Update()
        {
            FollowMouseCursor();
        }

        private void FollowMouseCursor()
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();

            // Set crosshair position directly to mouse position
            _crosshairUI.position = mousePosition;
        }
    }
}