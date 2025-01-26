using Unity.VisualScripting;
using UnityEngine;

public class PlayerBubble : MonoBehaviour
{

    [Header("Bubble Sprites")]
    [SerializeField] private Sprite[] _bubbleSprites; // array to hold the sprits 

    [Header("Bubble Size")]
    [SerializeField] private float growthRate = 0.5f; // Rate of growth per second
    [SerializeField] private float maxSizeMultiplier = 2f; // Maximum scale multiplier

    private bool isShot = false; // Tracks if the bubble is shot
    private Vector3 originalScale; // Stores the bubble's original scale
    private CircleCollider2D _circleCol; // Reference to the CircleCollider2D


    private Rigidbody2D rb2D;
    public bool IsShot
    {
        get => isShot;
        set
        {
            isShot = value;

            // If the bubble is shot, start growing
            if (isShot)
            {
                originalScale = transform.localScale; // Ensure we store the scale on shoot
            }
        }
    }

    private void Awake()
    {
        // Cache the CircleCollider2D component
        _circleCol = GetComponent<CircleCollider2D>();
        rb2D = GetComponent<Rigidbody2D>();

        if (_circleCol == null)
        {
            Debug.LogError("CircleCollider2D not found! Please add one to the bubble.");
        }

        // Store the original scale for growth reference
        originalScale = transform.localScale;

        int ran = (int)Random.Range(0f, 2f);
        GetComponent<SpriteRenderer>().sprite = _bubbleSprites[ran]; //pick random bubble 

    }

    private void Update()
    {
        // If the bubble is shot, grow over time
        if (isShot)
        {
            GrowBubble();
        }
    }

    private void GrowBubble()
    {
        // Gradually increase the scale up to the max size
        Vector3 targetScale = originalScale * maxSizeMultiplier;
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, growthRate * Time.deltaTime);

        // Adjust the collider radius to match the new scale
        if (_circleCol != null)
        {
            _circleCol.radius = transform.localScale.x / 2f; // Assuming the collider's default radius corresponds to half the original scale
        }

        // Stop growing when the max size is reached
        if (transform.localScale.x >= targetScale.x * 0.99f)
        {
            transform.localScale = targetScale; // Snap to max size
            isShot = false; // Stop further growth
        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.Log("Bubble Collied with Object tagged: " + collision.collider.tag + _circleCol.isTrigger);

        if (collision.collider.CompareTag("Wall"))
        {
            //stop moving if hits a wall
            rb2D.linearVelocity = Vector3.zero;
            rb2D.angularVelocity = 0;
        }


        if (collision.collider.CompareTag("Player"))
        {
            Debug.Log("Player Collected Bubble");
            //Add 1 bubble to the player health 
            collision.collider.gameObject.GetComponentInChildren<PlayerBubbleHealthManager>().BubbleHealth++;

            Destroy(gameObject);
        }


        //if the player Bounses the bubble back:
        if (collision.collider.CompareTag("Enemy"))
        {
            Debug.Log("Enemy Hit Enemy Unit");



            //Deal Damage to the Enemy 
            //collision.gameObject.GetComponent<Enemy>().UnitTakeDamage(_damage);

            //Trap it in a bubble 


            Destroy(gameObject);
        }


    }








}
