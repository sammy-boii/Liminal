using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EuclideanPuzzle : MonoBehaviour
{
    [Header("Lighting ____________________________________________________________")]
    [Space(10)]

    public Material emissiveMaterial;
    public Material offMaterial;
    public Material coompletedMaterial;
    public Color completedColor;
    public List<GameObject> indicatorLights;
    public List<GameObject> lamps;

    [Space(40)]
    [Header("References ____________________________________________________________")]
    [Space(10)]

    public List<GameObject> correctSides;
    public GameObject eyes;
    public AudioSource bells;

    private bool isSolved;

    private void Start()
    {
       
        eyes.SetActive(false);

        foreach (GameObject light in indicatorLights)
        {
            Renderer renderer = light.GetComponent<Renderer>();
            renderer.material = offMaterial;
        }

        eyes.SetActive(false);
}

    public bool checkSides()
    {
        isSolved = true;
        foreach (GameObject obj in correctSides)
        {
            if (!obj.activeSelf)
            {
                isSolved = false;
                break;
            }
        }

        if (isSolved)
        {
            foreach (GameObject light in indicatorLights)
            {
                Renderer renderer = light.GetComponent<Renderer>();
                renderer.material = emissiveMaterial;
            }

            foreach (GameObject lamp in lamps)
            {
                Light light = lamp.GetComponent<Light>();
                Renderer renderer = lamp.GetComponent<Renderer>();
                renderer.material = coompletedMaterial;
                light.color = completedColor;
            }

            eyes.SetActive(true);
            bells.Play();
            Invoke("StopBells", 25f);
        }
        return isSolved;
    }

    void StopBells()
    {
        bells.Stop();
    }
}
