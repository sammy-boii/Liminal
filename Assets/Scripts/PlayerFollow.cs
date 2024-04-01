using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerFollow : MonoBehaviour
{

    public Transform player;
    private NavMeshAgent agent;
    public Animator animator;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        agent.destination = player.position;
        if (agent.remainingDistance <= agent.stoppingDistance )
        {
            animator.ResetTrigger("chase");
            animator.SetTrigger("idle");
        }
        else
        {
            animator.ResetTrigger("idle");
            animator.SetTrigger("chase");
        }
    }
}
