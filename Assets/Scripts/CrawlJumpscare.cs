using UnityEngine;
using UnityEngine.TextCore.Text;

public class CrawlJumpscare : MonoBehaviour
{
    public Animator anim;
    public GameObject character;
    public float crawlSpeed = 1.0f;

    private bool isAnimating = false;
    private Vector3 initialPosition;

    private void Start()
    {
        character.SetActive(false);
    }

    private void Update()
    {
        if (isAnimating)
        {
            // Move the character forward
            character.transform.Translate(Vector3.forward * crawlSpeed * Time.deltaTime);
        }

        if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 1f && !anim.IsInTransition(0))
        {
            character.SetActive(false);
            character.transform.position = initialPosition; // Reset position
            isAnimating = false; // Stop moving when animation ends
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            character.SetActive(true);
            initialPosition = character.transform.position; // Store initial position
            anim.SetTrigger("jumpscare");
            isAnimating = true; // Start moving when triggered
        }
    }
}
