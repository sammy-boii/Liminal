using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HierarchyShifter : MonoBehaviour
{
    public Transform elevator;
    public Transform mainBuilding;

    public Transform player;
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("elevator");
            player.parent = elevator;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("buidling");
            player.parent = mainBuilding;
        }
    }
}
