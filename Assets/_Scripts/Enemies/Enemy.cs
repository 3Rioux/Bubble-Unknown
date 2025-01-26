using System.Collections;
using UnityEngine;

public enum ENEMYSTATES { DOINGHISTHING, ATTACKING, TRAPPED, DEAD, ERROR};

public class Enemy : MonoBehaviour
{
    [SerializeField] private int e_health;
    [SerializeField] private int e_maxHealth = 300;
    [SerializeField] private int _unitScore = 100;



    [Header("Target")]
    [SerializeField] private Transform _playerTarget;
    [SerializeField] private float _targetRange = 10f;
    [SerializeField] private float _safeDistance = 5f; // Distance to maintain from the player

    [Header("Shoot")]
    [SerializeField] private GameObject _bubbleShootPrefab; // Store my Bubble Prefab To Shoot Prefab
    [SerializeField] private float _shootCooldown = 2f; // Delay between shots
    [SerializeField] private float _velocity = 10;
    private float _shootTimer = 0f; // Timer for shooting

    [Header("Movement")]
    [SerializeField] private float _moveSpeed = 3f;
    [SerializeField] private float _wanderRadius = 10f; // Max distance for wandering
    [SerializeField] private float _wanderCooldown = 2f; // Time between wander direction changes
    private Vector2 _wanderDirection;
    private float _wanderTimer;

    //RigidBody Component
    private Rigidbody2D _rb; // Rigidbody2D for movement

    [Header("Trapped In Bubble")]
    [SerializeField] private bool _isTrapped = false;
    [SerializeField] private float _damageDelay = 0.5f;
    private float _timeSinceLastDamaged = 0f;
    [SerializeField] private int _consecutiveAttackCount = 0; // keep track of the number of damage rounds the Unit took inside the bubble 

    [Header("Default Damage")]
    [SerializeField] private int _defaultDamageAmount = 50;

    [Header("Living State")]
    [SerializeField] private bool _isAlive = true;
    [SerializeField] private float _destroyAfterDeathDelay = 5f;

    [Header("State")]
    [SerializeField] private ENEMYSTATES _currentState = ENEMYSTATES.DOINGHISTHING;


    //========= GET & SET =========
    public int Health { get => e_health; set => e_health = value; }
    public int MaxHealth { get => e_maxHealth; set => e_maxHealth = value; }




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //set health to max health at start 
        Health = MaxHealth;

        //Test Time
        //StartCoroutine(InstantiateBubbleCoroutine());

        // Get the Rigidbody2D component
        _rb = GetComponent<Rigidbody2D>();
        if (_rb == null)
        {
            _rb = gameObject.AddComponent<Rigidbody2D>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!_isAlive) return; // Skip updates if dead

        // State handling
        switch (_currentState)
        {
            case ENEMYSTATES.DOINGHISTHING:
                HandleNormalBehavior();
                break;
            case ENEMYSTATES.TRAPPED:
                HandleTrappedState();
                break;
            case ENEMYSTATES.DEAD:
                Debug.Log("DEAD ENEMY!!!!!!!!!!!!!!!!!!!!!!!!!!");
                //GameManager.Instance.AddScore(_unitScore);
                break; // No behavior if dead
        }

    }

   
    private void HandleNormalBehavior()
    {
        //if no target set just exit the method 
        if (_playerTarget == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, _playerTarget.position);

        // Check if player is within range
        if (distanceToPlayer <= _targetRange)
        {
            // Move towards the player if not too close
            if (distanceToPlayer > _safeDistance)
            {
                MoveTowardPlayer();
            }
            else
            {
                StopMoving();
            }

            //update timer
            _shootTimer += Time.deltaTime;

            //shoot if possible 
            if (_shootTimer >= _shootCooldown)
            {
                UnitShoot();
                _shootTimer = 0f; // Reset shoot timer
            }//end inner if 

        }
        else
        {
            // Wander if the player is out of range
            Wander();
        }//end outer else if 
    }//edn HandleNormalBehavior

    private void HandleTrappedState()
    {
        //Update Timer for damaged
        _timeSinceLastDamaged += Time.deltaTime;

        //Take Damage If possible -> Try to break free from the bubble 
        if (_timeSinceLastDamaged >= _damageDelay)
        {
            UnitTakeDamage(_defaultDamageAmount);
            _timeSinceLastDamaged = 0f;

            if (!BreakFreeFromBubble()) return; // Try to break free if possible

           // Debug.LogError("Trapped");
        }
    }//edn HandleTrappedState


    private void MoveTowardPlayer()
    {
        Vector2 direction = (_playerTarget.position - transform.position).normalized;
        _rb.linearVelocity = direction * _moveSpeed;
    }

    public void StopMoving()
    {
        _rb.linearVelocity = Vector2.zero;
    }

    private void Wander()
    {
        _wanderTimer += Time.deltaTime;

        if (_wanderTimer >= _wanderCooldown)
        {
            // Change wander direction after cooldown
            _wanderDirection = Random.insideUnitCircle.normalized * _wanderRadius;
            _wanderTimer = 0f;
        }

        // Move in the current wander direction
        Vector2 targetPosition = (Vector2)transform.position + _wanderDirection * Time.deltaTime;
        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
        _rb.linearVelocity = direction * _moveSpeed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Enemy collided!!!!!");
        if(collision.collider.CompareTag("PlayerBubble"))
        {
            _isTrapped = true;//Trapped in the bubble 

            _currentState = ENEMYSTATES.TRAPPED;

            StopMoving();

            //Tell the Bubble to "Grow" with this enemy as the center 

            //Take Damage 

            PlayerBubble playerBubble = collision.gameObject.GetComponent<PlayerBubble>();

            if (playerBubble != null)
            {
                Rigidbody2D bubbleRb = playerBubble.GetComponent<Rigidbody2D>();
                if (bubbleRb != null)
                {
                    bubbleRb.linearVelocity = Vector2.zero;
                    bubbleRb.angularVelocity = 0f;
                }



                // Disable the colliders
                Collider2D enemyCollider = GetComponent<Collider2D>();

                if (enemyCollider != null)
                {
                   enemyCollider.enabled = false;
                }

                //// Reposition the enemy inside the bubble
                //Vector3 bubblePosition = playerBubble.transform.position;
                //transform.position = bubblePosition;

                //// Adjust the bubble scale to enclose the enemy
                //Vector3 enemySize = GetComponent<Renderer>().bounds.size;
                //playerBubble.transform.localScale = new Vector3(enemySize.x * 1.9f, enemySize.y * 1.9f, 1);

                // Center the enemy inside the bubble
                transform.position = playerBubble.transform.position;

                // Calculate the size of the bubble to enclose the enemy
                SpriteRenderer enemyRenderer = GetComponent<SpriteRenderer>();
                if (enemyRenderer != null)
                {
                    Vector3 enemySize = enemyRenderer.bounds.size;
                    float newBubbleScale = Mathf.Max(enemySize.x, enemySize.y) * 1.9f; // Adjust to properly fit the enemy

                    playerBubble.transform.localScale = new Vector3(newBubbleScale, newBubbleScale, 1);
                }

                if (bubbleRb != null)
                {
                    bubbleRb.linearVelocity = Vector2.zero;
                    bubbleRb.angularVelocity = 0f;
                }

                Debug.Log("Enemy trapped in bubble, colliders disabled, and movement paused!");
            }
            else
            {
                Debug.Log("Collided object is not PlayerBubble.");
            }



        }
        //else Ingnore

    }


    //Take Damage 
    public void UnitTakeDamage(int damage)
    {

        Debug.Log("UNIT TAKES DAMAGE " + damage.ToString());
        //remove HP 
        Health -= damage;

        Debug.Log("UNIT HP " + Health.ToString());

        if (Health  <= 0 && (_isAlive))
        {
            //enemy is dead 
            _isAlive = false;
            _currentState = ENEMYSTATES.DEAD;

            //Dead enemy logic here 
            // Trigger death logic
            HandleDeath();
            //Destroy(gameObject, 5f);//destroy after 5 seconds (For now)

        }

    }//edn UnitTakeDamage


    private void HandleDeath()
    {
        // Add death animations or sounds here

        GameManager.Instance.AddScore(_unitScore);

        //Stop Movement 
        _rb.linearVelocity = Vector2.zero;

      

        // Destroy the enemy after a delay
        Destroy(gameObject, _destroyAfterDeathDelay);
    }

    private IEnumerator DelayEnablingCollider(float delay)
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(delay);
        Collider2D enemyCollider = GetComponent<Collider2D>();
        if (enemyCollider != null)
        {
            enemyCollider.enabled = true;
        }
    }

    /// <summary>
    /// Method where the enemy breaks free from the bubble after 3 damage rounds OR
    /// if another enemy unit is nearby they will try to help them break free from the bubble 
    /// </summary>
    /// <returns></returns>
    private bool BreakFreeFromBubble()
    {
        _consecutiveAttackCount++;
        if (_consecutiveAttackCount > 3)
        {
            _isTrapped = false;
            _currentState = ENEMYSTATES.DOINGHISTHING;
            _consecutiveAttackCount = 0;

            // Start coroutine to re-enable colliders after delay
            StartCoroutine(DelayEnablingCollider(1.5f));

            Debug.Log("Enemy broke free from the bubble!");
            return true;
        }

        return false;
    }//ebd BreakFreeFromBubble


    // Enemy ATTACK logic
    private void UnitShoot()
    {
        if (_bubbleShootPrefab == null || _playerTarget == null) return;

        // Calculate direction to the player
        Vector2 direction = (_playerTarget.position - transform.position).normalized;

        // Instantiate and shoot the bubble
        GameObject bubble = Instantiate(_bubbleShootPrefab, transform.position, Quaternion.identity);
        Rigidbody2D rb = bubble.GetComponent<Rigidbody2D>();

        if (rb == null)
        {
            rb = bubble.AddComponent<Rigidbody2D>();
        }

        rb.linearVelocity = direction * _velocity;

        // Optionally destroy the bubble after some time to clean up the scene
        Destroy(bubble, 10f);

        //Debug.Log("Enemy shot a bubble!");

    }


    //Test 
    private IEnumerator InstantiateBubbleCoroutine()
    {
        while (true)
        {
            GameObject lastBubble = Instantiate(_bubbleShootPrefab, this.transform.position, Quaternion.identity);

            // Ensure the bubble has a Rigidbody2D for physics
            Rigidbody2D bubbleRigidbody = lastBubble.GetComponent<Rigidbody2D>();
            if (bubbleRigidbody == null)
            {
                bubbleRigidbody = lastBubble.AddComponent<Rigidbody2D>();
            }

            // Apply velocity to the bubble
            bubbleRigidbody.linearVelocity = new Vector2(2, 0);


            // Optionally destroy the bubble after some time to clean up the scene
            Destroy(lastBubble, 5f);

            yield return new WaitForSeconds(2);
        }
    }


    private void OnDrawGizmos()
    {
        //float ran = Random.value;
        //Gizmos.color = new Color(Random.value * ran, Random.value * ran, Random.value * ran);
        Gizmos.DrawWireSphere(this.transform.position, _targetRange);
    }

}
