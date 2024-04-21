using OldElevator;
using UnityEngine;
using UnityEngine.UI;

public class InteractionController : MonoBehaviour
{
    public float interactionDistance;

    // ui stuff

    public GameObject intText;
    public GameObject lockedText;
    public GameObject keypadUI;

    public LayerMask layerMask;

    [SerializeField] private Color nonInteractableColor = new Color(1, 1, 1, .1f);
    [SerializeField] private Color interactableColor = Color.green;

    public Image crosshair;

    private RaycastHit hit;

    public LayerMask layerMaskButtons;

    // external scripts

    [SerializeField] ScavengerDialogue dialogue;
    [SerializeField] Keypad keypad;
    [SerializeField] PlayerFollow playerFollow;

    void animateDoor()
    {
            GameObject doorHinge = hit.collider.gameObject;
            Animator doorAnim = doorHinge.GetComponent<Animator>();
            bool isOpen = doorAnim.GetBool("isOpen");
            doorAnim.SetBool("isOpen", !isOpen);
    }

    void handleLockedDoor()
    {
        GameObject lockedDoorHinge = hit.collider.gameObject;
        Animator lockedDoorAnim = lockedDoorHinge.GetComponent<Animator>();
        if (!lockedDoorAnim.GetBool("isOpen"))
        {
            lockedText.SetActive(true);
            intText.SetActive(false);
        }
    }

    void toggleLever()
    {
        GameObject lever = hit.collider.gameObject;
        Animator leverAnim = lever.GetComponent<Animator>();
        bool isOn = leverAnim.GetBool("isOn");
        leverAnim.SetBool("isOn", !isOn);

        LeverController leverController = lever.GetComponent<LeverController>();
        if (leverController != null)
        {
            leverController.swapActiveChild();
        }
    }

    void Start()
    {
        layerMask = LayerMask.GetMask("Interactable");
    }

    void Update()
    {

        Ray ray = new Ray(transform.position, transform.forward);

            if (Physics.Raycast(ray, out hit, interactionDistance, layerMask))
        {

            crosshair.color = interactableColor;
            intText.SetActive(true);

            if (hit.collider.CompareTag("LockedDoor"))
            {
                handleLockedDoor();
            }

            if (Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0))
            {

                if (hit.collider.CompareTag("Scavenger"))
                {
                    dialogue.StartDialogue();
                }

                if (hit.collider.CompareTag("Door"))
                {
                    animateDoor();
                    intText.SetActive(true);
                }

                if (hit.collider.CompareTag("TriggeredDoor"))
                {
                    animateDoor();
                    intText.SetActive(true);
                    playerFollow.enabled = true;
                }

                if (hit.collider.CompareTag("Lever"))
                {
                    toggleLever();
                }

                if (hit.collider.CompareTag("Keypad"))
                {
                    keypadUI.SetActive(true);
                }

                if (hit.collider.transform.TryGetComponent(out ElevatorButton btn))
                {
                    btn.Press();
                }
                if (hit.collider.transform.TryGetComponent(out DoorTrigger door))
                {
                    door.Toggle();
                }

            }
        }
        else
        {
            lockedText.SetActive(false);
            keypadUI.SetActive(false);
            intText.SetActive(false);
            crosshair.color = nonInteractableColor;
        }


    }
}