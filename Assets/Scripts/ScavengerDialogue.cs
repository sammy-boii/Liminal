using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScavengerDialogue : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public string[] lines;
    public float textSpeed;

    private int index;
    private bool isDialogueActive = false;

    void Start()
    {
        textComponent.text = string.Empty;
    }

    void Update()
    {
        if (isDialogueActive && Input.GetMouseButtonDown(0) && textComponent.text == lines[index])
        {
            NextLine();
        }
    }

    public void StartDialogue()
    {
        if (!isDialogueActive)
        {
            isDialogueActive = true;
            index = 0;
            StartCoroutine(TypeLine());
        }
    }

    public IEnumerator TypeLine()
    {
        if (index == lines.Length - 1)
        {
            textComponent.text = lines[index];
            yield break;
        }

        for (int i = 0; i < lines[index].Length; i++)
        {
            textComponent.text += lines[index][i];
            yield return new WaitForSeconds(textSpeed);
        }
    }


    void NextLine()
    {
        if (index < lines.Length - 1)
        {
            index++;
            textComponent.text = string.Empty;
            StartCoroutine(TypeLine());
        }
        else
        {
            isDialogueActive = false;
            gameObject.SetActive(false);
        }
    }
}
