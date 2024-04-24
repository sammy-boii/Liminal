using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryTrigger : MonoBehaviour
{
    public GameObject victoryUI;

    public GameObject player;
    public Camera playerCam;

    public GameObject victoryRoom;

    public AudioSource chaseMusic;

    private void Awake()
    {
        victoryRoom.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        chaseMusic.Stop();
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
