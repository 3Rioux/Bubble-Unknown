using System.Collections;
using UnityEngine;

public class ShieldHealth : MonoBehaviour
{

    //[SerializeField] float[] _shieldSizeArray = new float[3] { 0.5f, 1.0f, 1.5f };

    [Header("Shield Size Settings")]
    [SerializeField] private float _shieldMaxSize = 1.5f; // Maximum scale x
    [SerializeField] private float _shieldMinSize = 0.5f; // Minimum scale x
    [SerializeField] private float _damageAmount = 0.5f; // Amount to reduce the shield's scale
    [SerializeField] private float _regenDelay = 2f; // Delay before the shield starts regenerating
    [SerializeField] private float _regenSpeed = 0.25f; // Speed at which the shield regenerates

    private Coroutine _regenCoroutine; // Reference to the regeneration coroutine

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("EnemyBubble"))
        {
            // Damage the shield by reducing its x scale
            AdjustShieldSize(-_damageAmount);
        }
    }

    private void AdjustShieldSize(float amount)
    {
        // Adjust the shield's scale x
        Vector3 currentScale = transform.localScale;
        currentScale.x = Mathf.Clamp(currentScale.x + amount, _shieldMinSize, _shieldMaxSize);
        transform.localScale = currentScale;

        // Restart regeneration if the shield was damaged
        if (amount < 0)
        {
            if (_regenCoroutine != null)
            {
                StopCoroutine(_regenCoroutine);
            }
            _regenCoroutine = StartCoroutine(RegenShield());
        }
    }

    private IEnumerator RegenShield()
    {
        // Wait for the regeneration delay
        yield return new WaitForSeconds(_regenDelay);

        // Gradually regenerate the shield size back to max
        while (transform.localScale.x < _shieldMaxSize)
        {
            Vector3 currentScale = transform.localScale;
            currentScale.x = Mathf.Clamp(currentScale.x + (_regenSpeed * Time.deltaTime), _shieldMinSize, _shieldMaxSize);
            transform.localScale = currentScale;

            yield return null;
        }

        // Clear the coroutine reference when regeneration is complete
        _regenCoroutine = null;
    }


}
