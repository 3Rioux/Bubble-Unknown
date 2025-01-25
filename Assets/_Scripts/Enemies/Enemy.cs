using System.Collections;
using UnityEngine;

public enum ENEMYSTATES { DOINGHISTHING, TRAPPED, DEAD, ERROR};

public class Enemy : MonoBehaviour
{
    [SerializeField] private int e_health;
    [SerializeField] private int e_maxHealth = 300;


    [Header("Target")]
    [SerializeField] private Transform _playerTarget;
    [SerializeField] private float _targetRange = 20f;

    [Header("Shoot")]
    [SerializeField] private GameObject _bubbleShootPrefab; // Store my Bubble Prefab To Shoot Prefab
    [SerializeField] private float _shootCooldown = 2f; // Delay between shots
    [SerializeField] private float _velocity = 10;
    private float _shootTimer = 0f; // Timer for shooting


    [Header("Trapped In Bubble")]
    [SerializeField] private bool _isTrapped = false;
    [SerializeField] private float _damageDelay = 0.5f;
    private float _timeSinceLastDamaged = 0f;
    private int _consecutiveAttackCount = 0; // keep track of the number of damage rounds the Unit took inside the bubble 

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
                break; // No behavior if dead
        }

    }

    //display found target once:
    private bool foundTarget = false;
    private void HandleNormalBehavior()
    {
        //if no target set just exit the method 
        if (_playerTarget == null) return;
        
        // Check if player is within range
        if (Vector2.Distance(transform.position, _playerTarget.position) <= _targetRange)
        {
            if (!foundTarget)
            {
                foundTarget = true;
                Debug.Log("Found Target!");
            }

            //update timer
            _shootTimer += Time.deltaTime;

            //shoot if possible 
            if (_shootTimer >= _shootCooldown)
            {
                UnitShoot();
                _shootTimer = 0f; // Reset shoot timer
            }//end inner if 

        }//end outer if 
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
        }
    }//edn HandleTrappedState


    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.CompareTag("Bubble"))
        {
            _isTrapped = true;//Trapped in the bubble 

            _currentState = ENEMYSTATES.TRAPPED;

            //Tell the Bubble to "Grow" with this enemy as the center 

            //Take Damage 



        }
        //else Ingnore

    }


    //Take Damage 
    public void UnitTakeDamage(int damage)
    {
        //remove HP 
        Health -= damage;

        if(Health  < 0)
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

        // Destroy the enemy after a delay
        Destroy(gameObject, _destroyAfterDeathDelay);
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

        Debug.Log("Enemy shot a bubble!");

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
