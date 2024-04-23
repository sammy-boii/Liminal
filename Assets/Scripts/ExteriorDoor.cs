using UnityEngine;

public class ExteriorDoor : MonoBehaviour
{
    [SerializeField] Animator doorLeftAnim;
    [SerializeField] Animator doorRightAnim;

    public void Interact()
    {
        Open();
        Invoke("Close", 2f);
    }

    void Open()
    {
        doorLeftAnim.SetTrigger("open");
        doorRightAnim.SetTrigger("open");

    }

    void Close()
    {
        doorLeftAnim.SetTrigger("close");
        doorRightAnim.SetTrigger("close");
    }
}
