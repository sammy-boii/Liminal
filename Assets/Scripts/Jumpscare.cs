using UnityEngine;

public class Jumpscare : MonoBehaviour
{
    public Animator anim;
    public GameObject character;

    private void Start()
    {
        character.SetActive(false);
    }

    private void Update()
    {
        if (!character.activeSelf) return;
        if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 1f && !anim.IsInTransition(0))
        {
            character.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            character.SetActive(true);
            anim.SetTrigger("jumpscare");
        }
    }
}
