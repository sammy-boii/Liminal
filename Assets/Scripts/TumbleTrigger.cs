using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TumbleTrigger : MonoBehaviour
{
    private bool hasTriggered = false;
    public AudioSource tumble;

    private void OnTriggerEnter(Collider other)
    {
        if (!hasTriggered)
        {
            tumble.Play();
            hasTriggered = true;
        }
    }

}
