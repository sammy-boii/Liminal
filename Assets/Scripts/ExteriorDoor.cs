using UnityEngine;

public class ExteriorDoor : MonoBehaviour
{
    [SerializeField] Animator doorLeftAnim;
    [SerializeField] Animator doorRightAnim;

    public InteriorDoor interiorDoor;

    public AudioSource doorOpen;
    public AudioSource doorClose;

    private bool isAnimating = false;

    public void Interact()
    {
        if (isAnimating)
        {
            return;
        }

        Open();
        Invoke("OpenInteriorDoor", 1f); 
        Invoke("Close", 2f);
    }

    void OpenInteriorDoor()
    {
        interiorDoor.Open();
    }

    void Open()
    {
        isAnimating = true;

        doorLeftAnim.SetTrigger("open");
        doorRightAnim.SetTrigger("open");
        doorOpen.Play();
    }

    void Close()
    {
        isAnimating = true;

        doorLeftAnim.SetTrigger("close");
        doorRightAnim.SetTrigger("close");

        Invoke("PlayCloseAudio", 1f);

        Invoke("SetIsAnimatingFalse", doorLeftAnim.GetCurrentAnimatorClipInfo(0)[0].clip.length);
    }

    void PlayCloseAudio()
    {
        doorClose.Play();
    }

    private void SetIsAnimatingFalse()
    {
        isAnimating = false;
    }
}
