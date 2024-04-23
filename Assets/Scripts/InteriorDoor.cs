using UnityEngine;

public class InteriorDoor : MonoBehaviour
{
    [SerializeField] Animator doorLeftAnim;
    [SerializeField] Animator doorRightAnim;

    public ElevatorController elevator;

    public bool isOpen = false;

    public void Interact()
    {
        if (isOpen)
        {
            Close();
        }
        else
        {
            Open();
        }
    }

    public void Open()
    {
        if (elevator.isMoving) return;
        doorLeftAnim.SetTrigger("open");
        doorRightAnim.SetTrigger("open");
        isOpen = true;
    }

    public void Close()
    {
        doorLeftAnim.SetTrigger("close");
        doorRightAnim.SetTrigger("close");
        isOpen = false;
    }
}
