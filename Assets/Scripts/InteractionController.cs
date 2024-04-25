using UnityEngine;
using UnityEngine.UI;

public class InteractionController : MonoBehaviour
{

    [Space(10)]
    [Header("UI Elements ____________________________________________________________")]
    [Space(5)]

    public GameObject intText;
    public GameObject lockedText;
    public GameObject keypadUI;

    [Space(10)]
    [Header("Crosshair ____________________________________________________________")]
    [Space(5)]

    public Image crosshair;
    [SerializeField] private Color nonInteractableColor = new Color(1, 1, 1, .1f);
    [SerializeField] private Color interactableColor = Color.green;

    [Space(10)]
    [Header("Raycast Settings ____________________________________________________________\"")]
    [Space(10)]

    public LayerMask layerMask;
    public float interactionDistance;
    private RaycastHit hit;

    [Space(10)]
    [Header("Sounds ____________________________________________________________\"")]
    [Space(10)]

    public AudioSource doorOpen;
    public AudioSource doorClose;
    public AudioSource lockedDoorClip;
    public AudioSource leverClip;
    public AudioSource stab;
    public AudioSource chaseMusic;

    [Space(10)]
    [Header("External Scripts ____________________________________________________________\"")]
    [Space(10)]

    [SerializeField] ScavengerDialogue dialogue;
    [SerializeField] Keypad keypad;
    [SerializeField] PlayerFollow playerFollow;
    [SerializeField] EuclideanPuzzle puzzle;

    void animateDoor()
    {
            GameObject doorHinge = hit.collider.gameObject;
            Animator doorAnim = doorHinge.GetComponent<Animator>();
            bool isOpen = doorAnim.GetBool("isOpen");
            doorAnim.SetBool("isOpen", !isOpen);
        if (!doorAnim.GetBool("isOpen"))
        {
            doorClose.Play();
        }
        else
        {
            doorOpen.Play();
        }
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

        leverClip.Play();

        LeverController leverController = lever.GetComponent<LeverController>();
        if (leverController != null)
        {
            leverController.swapActiveChild();
        }
    }
    void animateExteriorDoor()
    {
        ExteriorDoor exteriorDoor = hit.collider.gameObject.GetComponent<ExteriorDoor>();
        exteriorDoor.Interact();
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

                if (hit.collider.CompareTag("LockedDoor"))
                {
                    lockedDoorClip.Play();
                }

                if (hit.collider.CompareTag("Door"))
                {
                    animateDoor();
                }

                if (hit.collider.CompareTag("TriggeredDoor"))
                {
                    animateDoor();
                    stab.Stop();
                    chaseMusic.Play();
                    playerFollow.enabled = true;
                }

                // puzzle section

                if (hit.collider.CompareTag("Lever"))
                {
                    toggleLever();
                }

                if (hit.collider.CompareTag("Keypad"))
                {
                    keypadUI.SetActive(true);
                }
                // elevator section

                if (hit.collider.TryGetComponent(out ExteriorDoor _))
                {
                    animateExteriorDoor();
                }

                if (hit.collider.TryGetComponent(out ElevatorButton elevatorBtn))
                {
                    elevatorBtn.Press();
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