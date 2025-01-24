using UnityEngine;
using UnityEngine.Serialization;

namespace _Scripts.Shield
{
    /// <summary>
    /// Script to make the shield move around the player 
    /// </summary>
    public class ShieldMovement : MonoBehaviour
    {
        public GameObject orb;
        public float radius;
        public float radiusSpeed;
        public float rotationSpeed;

        private Transform centre;
        private Vector3 desiredPos;

        void Start()
        {
            centre = orb.transform;
            transform.position = (transform.position - centre.position).normalized * radius + centre.position;
        }

        void Update()
        {
            float rotationX = Input.GetAxis("Mouse X") * -rotationSpeed;
            transform.RotateAround(centre.position, Vector3.forward, rotationX);

            desiredPos = (transform.position - centre.position).normalized * radius + centre.position;
            transform.position = Vector3.MoveTowards(transform.position, desiredPos, radiusSpeed * Time.deltaTime);
        }
        
        
        /*[SerializeField] private GameObject shield;// holds the shield GameObject or Prefab 

        [SerializeField] private float radius;
         private  Transform pivot;

        private void Start()
        {
            // pivot = shield.transform;
            // transform.parent = pivot;
            // transform.position += Vector3.up * radius;
            
        }

        void Update()
        {
            
            
            //Vector3 orbVector = Camera.main.WorldToScreenPoint(shield.position);
            // orbVector = Input.mousePosition - orbVector;
            // float angle = Mathf.Atan2(orbVector.y, orbVector.x) * Mathf.Rad2Deg;
            //
            // pivot.position = shield.position;
            // pivot.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
            
        }
        */
   
        
    }//end ShieldMovement
}//end namespace
