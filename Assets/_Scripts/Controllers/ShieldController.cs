using UnityEngine;

namespace _Scripts.Shield
{
    /// <summary>
    /// Makes the shield face the crosshair correctly with 90-degree offset.
    /// </summary>
    public class ShieldController : MonoBehaviour
    {
        [SerializeField] private Transform _player;  // Reference to player
        [SerializeField] private RectTransform _crosshair;  // UI crosshair reference
        [SerializeField] private float _shieldDistance = 2f;  // Fixed distance from player

        private void Update()
        {
            if (_player == null || _crosshair == null)
            {
                Debug.LogError("ShieldController: Missing player or crosshair reference!");
                return;
            }

            PositionShield();
            RotateShield();
        }

        private void PositionShield()
        {
            // Convert crosshair screen position to world position
            Vector3 crosshairWorldPos = Camera.main.ScreenToWorldPoint(_crosshair.position);
            crosshairWorldPos.z = 0f;  // Keep on the 2D plane

            // Get direction from player to crosshair
            Vector3 direction = (crosshairWorldPos - _player.position).normalized;

            // Set shield position at fixed distance from player
            transform.position = _player.position + direction * _shieldDistance;
        }

        private void RotateShield()
        {
            Vector3 directionToCrosshair = (transform.position - _player.position).normalized;
            float angle = Mathf.Atan2(directionToCrosshair.y, directionToCrosshair.x) * Mathf.Rad2Deg;

            transform.rotation = Quaternion.Euler(0f, 0f, angle + 90f);  // Adjust by 90 degrees
        }
    }
}