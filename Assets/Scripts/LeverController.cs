using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverController : MonoBehaviour
{
    public GameObject EuclideanSide;
    private int activeIndex;

    [SerializeField] EuclideanPuzzle euclideanPuzzle;

    void Start()
    {

        for (int i = 0; i < EuclideanSide.transform.childCount; i++)
        {
            Transform child = EuclideanSide.transform.GetChild(i);
            child.gameObject.SetActive(false);

            if (child.CompareTag("CorrectSide"))
            {
                int subsequentChildIndex = (i + 3) % EuclideanSide.transform.childCount;
                activeIndex = subsequentChildIndex;
                Transform subsequentChild = EuclideanSide.transform.GetChild(subsequentChildIndex);
                subsequentChild.gameObject.SetActive(true);
                break;
            }
        }
    }
    public void swapActiveChild()
    {
        Transform activeChild = EuclideanSide.transform.GetChild(activeIndex);
        activeChild.gameObject.SetActive(false);
        int subsequentChildIndex = (activeIndex + 1) % EuclideanSide.transform.childCount;
        activeIndex = subsequentChildIndex;
        Transform subsequentChild = EuclideanSide.transform.GetChild(subsequentChildIndex);
        subsequentChild.gameObject.SetActive(true);

        if(euclideanPuzzle.checkSides())
        {
            gameObject.tag = "Untagged";
            gameObject.layer = LayerMask.NameToLayer("Default");
        }
    }
}
