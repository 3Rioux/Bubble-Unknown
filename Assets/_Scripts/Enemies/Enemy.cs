using UnityEngine;

public enum ENEMYSTATES { DOINGHISTHING, TRAPPED, ERROR};

public class Enemy : MonoBehaviour
{
    [SerializeField] private int e_health;
    [SerializeField] private int e_maxHealth = 300;


    [Header("Target")]
    [SerializeField] private Transform _playerTarget;
    [SerializeField] private float _targetRange = 20;

    [Header("Shoot")]
    [SerializeField]
    private GameObject _bubbleShootPrefab; // Store my Bubble Prefab To Shoot Prefab
    [SerializeField]
    private float _velocity = 10;


    [Header("Trapped In Bubble")]
    [SerializeField] private bool _isTrapped = false;
    [SerializeField] private float _damageDelay = 0.5f;
    [SerializeField] private float _timeSinceLastDamaged = 0f;
    private int _consecutiveAttackCount = 0; // keep track of the number of damage rounds the Unit took inside the bubble 

    [SerializeField] private int _defaultDamageAmount = 50;

    [Header("Living State")]
    [SerializeField] private bool _isAlive = true;
    [SerializeField] private float _destroyAfterDeathDelay = 5f;

    //========= GET & SET =========
    public int Health { get => e_health; set => e_health = value; }
    public int MaxHealth { get => e_maxHealth; set => e_maxHealth = value; }




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //set health to max health at start 
        Health = MaxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.CompareTag("Bubble"))
        {
            _isTrapped = true;//Trapped in the bubble 

            //Tell the Bubble to "Grow" with this enemy as the center 

            //Take Damage 



        }
        //else Ingnore

    }


    //Take Damage 
    private void UnitTakeDamage(int damage)
    {
        //remove HP 
        Health -= damage;

        if(Health  < 0)
        {
            //enemy is dead 
            _isAlive = false; 

            //Dead enemy logic here 
            Destroy(gameObject, 5f);//destroy after 5 seconds (For now)

        }

    }//edn UnitTakeDamage


    /// <summary>
    /// Method where the enemy breaks free from the bubble after 3 damage rounds OR
    /// if another enemy unit is nearby they will try to help them break free from the bubble 
    /// </summary>
    /// <returns></returns>
    private bool BreakFreeFromBubble()
    {
        //increase damage round count 
        _consecutiveAttackCount++;
        //ALWAYS^^^

        //check consecutive attack amount:
        if (_isAlive && _consecutiveAttackCount > 3)
        {
            //breaks free
            _isTrapped = false;


            //return to DOINGHISTHING state 



        }

    }


    // Enemy ATTACK logic
    private void UnitShoot()
    {
        //Shoot @ Player

        //Delay between shoots 

        //

    }


}
