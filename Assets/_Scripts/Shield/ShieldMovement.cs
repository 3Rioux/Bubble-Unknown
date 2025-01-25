using UnityEngine;
using UnityEngine.Serialization;

namespace _Scripts.Shield
{
    /// <summary>
    /// Script to make the shield move around the player
    /// https://discussions.unity.com/t/solved-rotate-around-an-object-based-on-mouse-position/649185/2 
    /// </summary>
    public class ShieldMovement : MonoBehaviour
    {
        private Camera cam;

        private Vector2 MousePos
        {
            get
            {
                Vector2 pos = cam.ScreenToWorldPoint(Input.mousePosition);
                return pos;
            }
        }


        private void Awake()
        {
            cam = Camera.main;
        }

        [SerializeField] private GameObject orb;
        [SerializeField] private float radius;
        [SerializeField] private float radiusSpeed;
        [SerializeField] private float rotationSpeed;

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

        //rotate around pivot of this object 
        //private Camera cam;

        //private Vector2 MousePos
        //{
        //    get
        //    {
        //        Vector2 pos = cam.ScreenToWorldPoint(Input.mousePosition);
        //        return pos;
        //    }
        //}


        //private void Awake()
        //{
        //    cam = Camera.main;
        //}


        //private void Update()
        //{
        //    Vector2 dir = (Vector2)transform.position - MousePos;

        //    float angle = (Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);

        //    transform.eulerAngles = new Vector3(0f,0f,angle);

        //}

    }//end ShieldMovement
}//end namespace
