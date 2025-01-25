using UnityEngine;

public class EnemyBubble : MonoBehaviour
{
    [SerializeField] private int _damage = 150; // make it 50% of enemies HP 


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            Debug.Log("Enemy Hit Player");
            //Pop 1 of the players lives 

        }

        if (collision.collider.CompareTag("PlayerBubble"))
        {
            Debug.Log("Enemy Hit Player Bubble");
            //Pop 1 of the players lives 

        }

        //if the player Bounses the bubble back:
        if (collision.collider.CompareTag("Enemy"))
        {
            //Deal Damage to the Enemy 
            collision.gameObject.GetComponent<Enemy>().UnitTakeDamage(_damage);

        }


    }
}
