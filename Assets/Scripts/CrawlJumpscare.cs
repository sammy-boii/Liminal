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
        if (!character.activeSelf) return;
        {
            
        }
        if (isAnimating)
        {
            character.transform.Translate(Vector3.forward * crawlSpeed * Time.deltaTime);
        }

        if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 1f && !anim.IsInTransition(0))
        {
            character.SetActive(false);
            character.transform.position = initialPosition;
            isAnimating = false; 
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            character.SetActive(true);
            initialPosition = character.transform.position; 
            anim.SetTrigger("jumpscare");
            isAnimating = true; 
        }
    }
}
