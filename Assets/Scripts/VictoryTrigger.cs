using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryTrigger : MonoBehaviour
{
    public GameObject victoryUI;

    public GameObject player;
    public Camera playerCam;

    private GameObject victoryRoom;
    private void Awake()
    {
        victoryRoom = GameObject.Find("Animated Victory Container");
        victoryRoom.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        victoryRoom.SetActive(true);
        DisableControls();
        playerCam.gameObject.SetActive(false);
        Invoke("EnableUI", 5f);
    }

    void DisableControls()
    {
        player.GetComponent<PlayerController>().enabled = false;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    void EnableUI()
    {
        victoryUI.SetActive(true);
    }
}
