using System.Collections;
using UnityEngine;

public class Flicker : MonoBehaviour
{
    public Light flickeringLight;

    public AudioSource flicker;

    public float minFlickerInterval = 6f;
    public float maxFlickerInterval = 12f;

    void Start()
    {
        StartCoroutine(FlickerLight());
    }

    IEnumerator FlickerLight()
    {
        while (true)
        {
            flickeringLight.enabled = true;

            yield return new WaitForSeconds(Random.Range(minFlickerInterval, maxFlickerInterval));

            flickeringLight.enabled = false;

            yield return new WaitForSeconds(0.1f);
        }
    }
}
