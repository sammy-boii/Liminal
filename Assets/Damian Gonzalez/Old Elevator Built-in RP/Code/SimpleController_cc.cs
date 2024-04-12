using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace OldElevator {

    [RequireComponent(typeof (CharacterController))]
    public class SimpleController_cc : MonoBehaviour {
        public static Transform thePlayer; //easy access
         CharacterController cc;
        Transform cam;
        float rotX = 0;
        public float walkSpeed = 3f;
        public float runSpeed = 5f;
        public float mouseSensitivity = 1f;

        public bool grounded;

        public float gravity = 9.81f;
        float verticalVelocity = 0;

        public float jumpForce = 4f;


        void Start() {
            thePlayer = transform;
            cc = GetComponent<CharacterController>();
            cam = transform.GetChild(0);
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            rotX = cam.eulerAngles.x;
        }


        void Update() {


            grounded = cc.isGrounded;

            //if not grounded, make the player fall
            if (!grounded) {
                verticalVelocity -= gravity * Time.deltaTime;
            } else {
                verticalVelocity = -.1f;
                //since he's grounded, he can jump
                if (Input.GetButtonDown("Jump")) verticalVelocity = jumpForce;
            }


            float speed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
            Vector3 forwardNotTilted = new Vector3(transform.forward.x, 0, transform.forward.z);



            cc.Move(
                forwardNotTilted * speed * Input.GetAxis("Vertical") * Time.deltaTime    //move forward
                +
                transform.right * speed * Input.GetAxis("Horizontal") * Time.deltaTime  //slide to sides
                +
                Vector3.up * verticalVelocity * Time.deltaTime                          //jump and fall
            );

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
