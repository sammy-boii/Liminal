using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerFollow : MonoBehaviour
{
    public Transform player;
    public NavMeshAgent agent;
    public Animator anim;

    public void Update()
    {
        anim.SetTrigger("chase");
        agent.destination = player.position;
        }
}
