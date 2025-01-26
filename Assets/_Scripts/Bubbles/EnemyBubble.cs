using UnityEngine;

public class EnemyBubble : MonoBehaviour
{
    [SerializeField] private int _damage = 150; // make it 50% of enemies HP 

    CircleCollider2D _circleCol; // this enemies sphere collider 

    [SerializeField] private int _bounceCount = 0; 
    [SerializeField] private int _maxBounces = 3; 

    private void Awake()
    {
        _circleCol = GetComponent<CircleCollider2D>();
        //turn off by default 
        _circleCol.isTrigger = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        _bounceCount++;
        if (_bounceCount == _maxBounces) Destroy(gameObject);

        if (collision.collider.CompareTag("Player"))
        {
            Debug.Log("Enemy Hit Player");
            //Pop 1 of the players lives 
            collision.gameObject.GetComponentInChildren<PlayerBubbleHealthManager>().PlayerTakeDamage();

            Destroy(gameObject);
        }

        //if (collision.collider.CompareTag("PlayerBubble"))
        //{
        //    Debug.Log("Enemy Hit Player Bubble");
        //    //Pop 1 of the players lives 


        //    Destroy(gameObject);
        //}

        //if the player Bounses the bubble back:
        if (collision.collider.CompareTag("Enemy"))
        {
            Debug.Log("Enemy Hit Enemy Unit");



            //Deal Damage to the Enemy 
            collision.gameObject.GetComponent<Enemy>().UnitTakeDamage(_damage);

            //Trap it in a bubble 


            Destroy(gameObject);
        }


    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            //Debug.Log("Exits-----------------------");
            //turn the Collider On after exits the body 
            _circleCol.isTrigger = false;
        }
    }

   
}
