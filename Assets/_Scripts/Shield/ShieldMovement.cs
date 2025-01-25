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


       

        [SerializeField] private GameObject player;
        [SerializeField] private float radius;
        [SerializeField] private float radiusSpeed;
        [SerializeField] private float rotationSpeed;

         private Transform centre;
         private Vector3 desiredPos;

        private void Awake()
        {
            cam = Camera.main;
        }

        void Start()
         {
            centre = player.transform;
            transform.position = (transform.position - centre.position).normalized * radius + centre.position;

            if (player == null)
            {
                Debug.LogError("Player GameObject is not assigned!");
            }
        }



        //void Update()
        //{
        //    if (player == null) return;

        //    // Get the mouse position in world space
        //    Vector2 mouseWorldPosition = cam.ScreenToWorldPoint(Input.mousePosition);

        //    // Calculate the direction from the player to the mouse
        //    Vector2 directionToMouse = (mouseWorldPosition - (Vector2)player.transform.position).normalized;

        //    // Calculate the desired position on the circle (orbit) around the player
        //    Vector2 desiredPosition = (Vector2)player.transform.position + directionToMouse * radius;

        //    // Smoothly move the shield to the desired position
        //    transform.position = Vector2.Lerp(transform.position, desiredPosition, rotationSpeed * Time.deltaTime);

        //    // Rotate the shield to always face the mouse
        //    float angle = Mathf.Atan2(directionToMouse.y, directionToMouse.x) * Mathf.Rad2Deg;
        //    transform.rotation = Quaternion.Euler(0, 0, angle - 30);
        //}
        //void Update()
        //{//working

        //    float rotationX = Input.GetAxis("Mouse X") * -rotationSpeed;
        //    transform.RotateAround(centre.position, Vector3.forward, rotationX);

        //    desiredPos = (transform.position - centre.position).normalized * radius + centre.position;
        //    transform.position = Vector3.MoveTowards(transform.position, desiredPos, radiusSpeed * Time.deltaTime);
        //}

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


//void Update()
//{
//    Vector2 mouseWorldPosition = cam.ScreenToWorldPoint(Input.mousePosition);
//    Vector2 directionToMouse = (mouseWorldPosition - (Vector2)player.transform.position).normalized;
//    float angle = Mathf.Atan2(directionToMouse.y, directionToMouse.x) * Mathf.Rad2Deg;


//    transform.RotateAround(centre.position, Vector3.forward, angle);

//    desiredPos = (transform.position - centre.position).normalized * radius + centre.position;
//    transform.position = Vector3.MoveTowards(transform.position, desiredPos, radiusSpeed * Time.deltaTime);
//}