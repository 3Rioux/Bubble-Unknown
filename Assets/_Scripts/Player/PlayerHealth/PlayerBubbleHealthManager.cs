
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// This scrip will manage the bubbles (Lives/HP) that the player has left and their behaviour 
/// </summary>
public class PlayerBubbleHealthManager : MonoBehaviour
{
    [Header("Bubbles/Lives Left")]
    [SerializeField]
    private int bubbleHealth = 3;//number of bubbles I have left <-  [SerializeField] private int numberOfTempBubble = 20; // Number of bubbles to create
    [SerializeField] 
    private GameObject _bubblePrefab; // store my Bubble Prefab that is floating arround the player 
    [SerializeField] 
    private List<GameObject> _bubblesList = new List<GameObject>(); // store my Bubbles 


    [SerializeField] private float attractionSpeed = 5f;

    [Header("Shoot")]
    [SerializeField] 
    private GameObject _bubbleShootPrefab; // Store my Bubble Prefab To Shoot Prefab
    [SerializeField]
    private float _velocity = 10;
    [SerializeField]
    private GameObject _shootDirection; // get it from the shield or better yet just do the opposite of the shield while getting it from mouse(X) + camera pos

   
    [SerializeField] private float radius = 5f; // Radius of the circle around the player

    public int BubbleHealth { get => bubbleHealth; set => bubbleHealth = value; }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Cursor.lockState = CursorLockMode.Locked;
        DisplayBubbles();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.GameStateMachine.CurrentState == GameState.PAUSED) return; // DONT Allow player input if Game is Pause  

        //Also need to lock the mouse so that when i click the player does NOT Shoot 

        DisplayBubbles();

        //Test Re-Displaying Bubbles
        //if (Keyboard.current.spaceKey.wasPressedThisFrame)
        //{
        //    //Spawn / Display the bubbles in the list to the user
        //    DisplayBubbles();
        //}

        //Test Taking Damage
        if (Keyboard.current.qKey.wasPressedThisFrame) PlayerTakeDamage();

        //Shoot Bubbles
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
           
            ShootBubble();
        }
    }

    private void FixedUpdate()
    {
        //AttractBubbleToCenter();
    }

    //Display / Spawn Bubble 
    private void DisplayBubbles()
    {

        //Destroy(_bubblesList[0]);
        //Clear the old bubbles and load the new ones
        if (_bubblesList.Count > 0)
        {
            foreach (GameObject bubble in _bubblesList)
            {
                Destroy(bubble);
            }
            _bubblesList.Clear();
        }

        //Display New Ammount
        for (int i = 0; i < BubbleHealth; i++)
        {
            // Angle between each bubble in radians
            float angle = (360f / BubbleHealth) * i * Mathf.Deg2Rad;

            // Calculate the position offset using trigonometry
            Vector2 spawnPositionOffset = new Vector2(
                Mathf.Cos(angle) * radius, // X offset
                Mathf.Sin(angle) * radius  // Y offset
            );

            // Final spawn position relative to the player's position
            Vector2 spawnPosition = (Vector2)this.transform.position + spawnPositionOffset;

            // Instantiate bubble at the calculated position with no rotation
            AddBubble(Instantiate(_bubblePrefab, spawnPosition, Quaternion.identity, this.transform));
        }
    }//end DisplayBubbles

    //Add Bubble 
    private void AddBubble(GameObject bubble)
    {
        _bubblesList.Add(bubble);
    }//end AddBubble

    //Remove Bubble
    private void RemoveBubble()
    {
        //Destroy bubble
        Destroy(_bubblesList[_bubblesList.Count - 1]);
        BubbleHealth--;//reduce bubbles left count

        //Remove first or last bubble in the list and move the rest up (Think Snake Game Code for the bubble positions)
       
    }//edn RemoveBubble

    [SerializeField] private AudioClip _damageClip;
    [SerializeField] private AudioClip _shootClip;

    //Take Damage
    public void PlayerTakeDamage()
    {
        //Remove a life
        RemoveBubble();
        _bubblesList.RemoveAt(_bubblesList.Count - 1);

        if (BubbleHealth > 0)
        {
            //Audio FX
            SoundManager.Instance.PlaySFX(_damageClip);
            //Screen Turn Red 

            //??? Screen Shake ???

            //...
        }
        else
        {
            //Player Dead Let the GM know 
            //GameManager.Instance.PlayerDeath();
            GameManager.Instance.GameOver();

        }



    }//end PlayerTakeDamage

    //Shoot Bubble 
    private void ShootBubble()
    {
        ////Stop the player from throwing away his last life:
        //if (_bubblesList.Count > 1)
        //{
        //    //Remove a life
        //    RemoveBubble();

        //}//end if 

        ////Shoot the bubble Front or Back of List
        ////(OR shoot from the shield and just remove one of the bubbles)

        ////Update Bubbles Displayed 

        // Stop the player from throwing away their last life
        if (_bubblesList.Count <= 1)
        {
            Debug.Log("Cannot shoot the last life!");
            return;
        }

        SoundManager.Instance.PlaySFX(_shootClip);

        // Get the last bubble in the list
        GameObject lastBubble = _bubblesList[_bubblesList.Count - 1];

        // Detach the bubble from the player's transform
        lastBubble.transform.parent = null;

        // Remove the bubble from the list
        // Remove the bubble from the list
        _bubblesList.RemoveAt(_bubblesList.Count - 1);
        BubbleHealth--;//reduce bubbles left count

        // Ensure the bubble has a Rigidbody2D for physics
        Rigidbody2D bubbleRigidbody = lastBubble.GetComponent<Rigidbody2D>();
        if (bubbleRigidbody == null)
        {
            bubbleRigidbody = lastBubble.AddComponent<Rigidbody2D>();
        }

        

        // Calculate the shooting direction
        // Use the bubble's current position and rotation to determine the forward direction
        //Vector2 shootingDirection = (lastBubble.transform.position - this.transform.position).normalized;

        // Calculate the direction from the player to the mouse
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mousePosition.z = 0; // Ensure we're working in 2D space
        Vector2 directionToMouse = (mousePosition - transform.position).normalized;

        // Calculate the opposite direction
        Vector2 shootingDirection = directionToMouse;

        // Apply velocity to the bubble
        bubbleRigidbody.linearVelocity = shootingDirection * _velocity;

        // Optionally destroy the bubble after some time to clean up the scene
        //Destroy(lastBubble, 5f);

        //Set Sate of bubble to is shot 
        lastBubble.GetComponent<PlayerBubble>().IsShot = true;

        // Update the displayed bubbles
        DisplayBubbles();

    }//end ShootBubble


    private void AttractBubbleToCenter( )
    {
        Vector2 centerPoint = (Vector2)transform.position;

        
        foreach (GameObject bubble in _bubblesList)
        {
                if (bubble == null) return;

            float distanceToCenter = Vector2.Distance(bubble.transform.position, centerPoint);
            if (distanceToCenter <= .25f) // Example range
            {
                // Calculate the direction from the bubble to the center
                Vector2 directionToCenter = (centerPoint - (Vector2)bubble.transform.position).normalized;

                // Move the bubble toward the center
                bubble.transform.position = Vector2.MoveTowards(
                    bubble.transform.position,
                    centerPoint,
                    attractionSpeed * Time.deltaTime
                );
            }
        }
        
    }//edn AttractBubbleToCenter


    //[Header("Gravity Settings")]
    //[SerializeField] public float GRAVITY_PULL = 0.78f; // Strength of the gravity pull
    //[SerializeField] public float m_GravityRadius = 1f; // Radius of the gravity effect

    //private void OnTriggerStay2D(Collider2D other)
    //{
    //    // Check if the object is tagged as "PlayerBubble"
    //    if (other.CompareTag("PlayerBubble"))
    //    {
    //        // Calculate the intensity of the gravity based on the distance
    //        float gravityIntensity = Vector2.Distance(transform.position, other.transform.position) / m_GravityRadius;

    //        // Ensure the object has a Rigidbody2D
    //        Rigidbody2D rb = other.attachedRigidbody;
    //        if (rb != null)
    //        {
    //            // Apply the gravity force
    //            Vector2 forceDirection = (transform.position - other.transform.position).normalized;
    //            rb.AddForce(forceDirection * gravityIntensity * rb.mass * GRAVITY_PULL * Time.smoothDeltaTime);

    //            // Visualize the force with a debug ray
    //            Debug.DrawRay(other.transform.position, forceDirection, Color.green);
    //        }
    //    }
    //}

}//end PlayerBubbleHealthManager
