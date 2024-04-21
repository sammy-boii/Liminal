using UnityEngine;

public class ChaseTrigger : MonoBehaviour
{
    public GameObject enemy;
    private PlayerFollow playerFollow;

    private Animator anim;

    void Start()
    {
        playerFollow = enemy.GetComponent<PlayerFollow>();
        anim = enemy.GetComponent<Animator>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            anim.SetTrigger("chase");
            playerFollow.enabled = true;
        }
    }
}
