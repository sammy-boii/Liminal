using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlesTrigger : MonoBehaviour
{
    public ParticleSystem dust;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            dust.Play();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            dust.Stop();
        }
    }
}
