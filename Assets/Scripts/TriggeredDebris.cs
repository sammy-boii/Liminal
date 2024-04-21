using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggeredDebris : MonoBehaviour
{
    [SerializeField] CameraShake camerashake;

    public ParticleSystem dust;

    private void Start()
    {
        dust.Pause();
        dust.gameObject.SetActive(false);
    }

    void OnCollisionEnter(Collision collision)
    {
        StartCoroutine(camerashake.Shake(.1f, .05f));
        dust.gameObject.SetActive(true);
        if (dust.isPaused)
        {
            dust.Play();
        }
    }
}
