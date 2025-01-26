using UnityEngine;

public class PlayerBubble : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {

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
            collision.gameObject.GetComponent<Enemy>().UnitTakeDamage(_damage);

            //Trap it in a bubble 


            Destroy(gameObject);
        }


    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
