using UnityEngine;

public class InteriorDoor : MonoBehaviour
{
    [SerializeField] Animator doorLeftAnim;
    [SerializeField] Animator doorRightAnim;

    public bool isOpen = false;

    public AudioSource doorOpen;
    public AudioSource doorClose;

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
        if (isOpen)
        {
            return;
        }

        doorLeftAnim.SetTrigger("open");
        doorRightAnim.SetTrigger("open");

        doorOpen.Play();

        isOpen = true;
    }

    public void Close()
    {
        if (!isOpen)
        {
            return;
        }

        doorLeftAnim.SetTrigger("close");
        doorRightAnim.SetTrigger("close");

        doorClose.Play();

        isOpen = false;
    }
}
