using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OldElevator {
    public class ElevatorInteraction : MonoBehaviour {
        public LayerMask layerMaskButtons;
        public float maxInteractionDistance;
        public UnityEngine.UI.Image interactIcon;

        void Start() {
            if (layerMaskButtons == 0) layerMaskButtons = LayerMask.GetMask("ElevatorInteraction");
        }


        void Update() {
            if (Physics.Raycast(
                transform.position,
                transform.forward,
                out RaycastHit hit,
                maxInteractionDistance,
                layerMaskButtons
            )) {
                InteractionIcon.inst.SetInteractable(true);
                //player can interact with an elevator interactable object
                if (Input.GetKeyDown(KeyCode.E) || Input.GetButtonDown("Fire1")) {
                    if (hit.collider.transform.TryGetComponent(out ElevatorButton btn)) {
                        //it is a button. Press it.
                        btn.Press();
                    } else if (hit.collider.transform.TryGetComponent(out DoorTrigger door)) {
                        //it is a door. Open/Close it.
                        door.Toggle();
                    }
                }
            } else {
                //can't interact
                InteractionIcon.inst.SetInteractable(false);
            }
        }
    }
}