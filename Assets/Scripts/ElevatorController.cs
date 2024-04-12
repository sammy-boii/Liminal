using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorController : MonoBehaviour
{
    public Collider upButtonCollider;
    public Collider downButtonCollder;
    void Start()
    {
        GameObject upButton = GameObject.FindGameObjectWithTag("Up Button");
        GameObject downButton = GameObject.FindGameObjectWithTag("Down Button");
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Up up and away");
        }
    }
}
