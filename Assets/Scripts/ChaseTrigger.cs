using UnityEngine;

public class ChaseTrigger : MonoBehaviour
{
    public GameObject enemy;
    private PlayerFollow playerFollow;

    void Start()
    {
        playerFollow = enemy.GetComponent<PlayerFollow>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerFollow.enabled = true;
        }
    }
}
