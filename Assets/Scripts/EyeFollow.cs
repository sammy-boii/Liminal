using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeFollow : MonoBehaviour
{
    public Transform eyeDest;
    void Update()
    {
        transform.LookAt(eyeDest);
    }
}
