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

    public GameObject enemy;

    private void Awake()
    {
        victoryRoom.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        chaseMusic.Stop();
        enemy.SetActive(false);
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
