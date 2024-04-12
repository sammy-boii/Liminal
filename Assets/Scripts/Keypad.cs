using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class Keypad : MonoBehaviour
{
    public GameObject player;

    public Image crosshair;
    public GameObject intText;

    public GameObject keypadUI;

    public GameObject animateDoor;
    public Animator doorAnim;

    public TMP_Text text;
    public Image input;

    public string code = "4713";

    public void Number(int number)
    {
        text.text += number.ToString();

    }

    public void Enter()
    {
        text.color = Color.white;
        if (text.text == code)
        {
            input.color = Color.green;
            text.fontSize = 12;
            text.text = "Access Granted";
            Invoke("Exit", 1.3f);
        }
        else
        {
            input.color = Color.red;
            text.fontSize = 12;
            text.text = "Access Denied";
        }

        Invoke("Clear", 1f);
    }

    public void Clear()
    {
        text.text = "";
        input.color = Color.black;
        text.color = Color.red;
        text.fontSize = 20;
    }

    public void Exit()
    {
        keypadUI.SetActive(false);
        player.GetComponent<PlayerController>().enabled = true;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    void Update()
    {
        if (keypadUI.activeSelf)
        {
            intText.SetActive(false);
            crosshair.color = Color.clear;
            player.GetComponent<PlayerController>().enabled = false;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

        }

        if (text.text == "Access Granted")
        {
            doorAnim.SetBool("isOpen", true);
        }
    }
}
