using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSwitch : MonoBehaviour
{
    public Light elevatorLight;

    public void Toggle()
    {
        if (elevatorLight.enabled)
        {
            elevatorLight.enabled = false;
        }
        else
        {
            elevatorLight.enabled = true;
        }
    }
}
