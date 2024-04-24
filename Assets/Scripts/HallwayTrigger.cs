using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HallwayTrigger : MonoBehaviour
{
    public GameObject debris1;
    public GameObject debris2;

    private Rigidbody rb1;
    private Rigidbody rb2;

    public AudioSource rock;

    void Start()
    {
        rb1 = debris1.GetComponent<Rigidbody>();
        rb2 = debris2.GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            rb1.useGravity = true;
            rb2.useGravity = true;

            rock.Play();
        }
    }
}
