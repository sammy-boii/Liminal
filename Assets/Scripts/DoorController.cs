using UnityEngine;

public class DoorController : MonoBehaviour
{
    public float interactionDistance = 3f; // Distance from the door at which the prompt appears
    public KeyCode interactKey = KeyCode.E; // Key to press to interact with the door
    public Transform player; // Reference to the player object
    public Animator doorAnimator; // Reference to the door's Animator component
    public GameObject promptUI; // Reference to the UI GameObject displaying the prompt
    public string openPrompt = "Press E to open door"; // Prompt text for opening the door
    public string closePrompt = "Press E to close door"; // Prompt text for closing the door

    private bool playerInRange = false;
    private bool doorOpen = false;

    private void Update()
    {
        // Check if the player is within interaction distance
        if (Vector3.Distance(player.position, transform.position) <= interactionDistance)
        {
            playerInRange = true;

            // Display prompt based on door state
            if (!doorOpen)
            {
                // Show prompt to open the door
                SetPromptText(openPrompt);
                if (Input.GetKeyDown(interactKey))
                {
                    OpenDoor();
                }
            }
            else
            {
                // Show prompt to close the door
                SetPromptText(closePrompt);
                if (Input.GetKeyDown(interactKey))
                {
                    CloseDoor();
                }
            }
        }
        else
        {
            playerInRange = false;
            SetPromptText("");
        }
    }

    private void SetPromptText(string text)
    {
        // Update UI text to display the prompt
        if (promptUI != null)
        {
            promptUI.GetComponent<TextMesh>().text = text;
        }
    }

    private void OpenDoor()
    {
        // Trigger "Open" animation
        doorAnimator.SetTrigger("open");
        doorOpen = true;
    }

    private void CloseDoor()
    {
        // Trigger "Close" animation
        doorAnimator.SetTrigger("close");
        doorOpen = false;
    }
}
