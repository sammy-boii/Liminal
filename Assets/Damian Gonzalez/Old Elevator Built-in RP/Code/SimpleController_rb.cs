using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace OldElevator {

    [RequireComponent(typeof (Rigidbody))]
    public class SimpleController_rb : MonoBehaviour {
        public static Transform thePlayer; //easy access
        Rigidbody rb;
        Transform cam;
        float rotX = 0;
        public float walkSpeed = 3f;
        public float runSpeed = 5f;
        public float mouseSensitivity = 1f;

        public bool grounded;

        public float jumpForce = 4f;
        public float distanceFloorToPlayer = .9f;


        void Start() {
            thePlayer = transform;
            rb = GetComponent<Rigidbody>();
            cam = transform.GetChild(0);
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            rotX = cam.eulerAngles.x;
        }


        void Update() {


            //grounded = Physics.Raycast(transform.position, distanceFloorToPlayer

            //if not grounded, make the player fall
            if (grounded && Input.GetButtonDown("Jump"))
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);



            float speed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
            Vector3 forwardNotTilted = new Vector3(transform.forward.x, 0, transform.forward.z);



            rb.velocity =
                forwardNotTilted * speed * Input.GetAxis("Vertical") * Time.deltaTime    //move forward
                +
                transform.right * speed * Input.GetAxis("Horizontal") * Time.deltaTime  //slide to sides
                +
                Vector3.up * rb.velocity.y                                              //jump and fall
            ;

            //rotate player left/right, and try to make player stand straight if tilted
            transform.rotation = (Quaternion.Lerp(
                transform.rotation * Quaternion.Euler(0, Input.GetAxis("Mouse X") * mouseSensitivity, 0),
                Quaternion.Euler(0, transform.eulerAngles.y, 0),
                .1f
            ));

            //look up and down
            rotX += Input.GetAxis("Mouse Y") * mouseSensitivity * -1;
            rotX = Mathf.Clamp(rotX, -90f, 90f); //clamp look 
            cam.localRotation = Quaternion.Euler(rotX, 0, 0);

        }
    }

}
