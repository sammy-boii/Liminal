using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggeredDebris : MonoBehaviour
{
    [SerializeField] CameraShake camerashake;

    void OnCollisionEnter(Collision collision)
    {
        StartCoroutine(camerashake.Shake(.1f, .05f));
    }
}
