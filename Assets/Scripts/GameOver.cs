using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    private void Awake()
    {
        GameObject jumpscareRoom = GameObject.Find("Animated JumpScare Container");
        if (!jumpscareRoom) return;
         jumpscareRoom.SetActive(false);
    }

    public void Restart()
    {
        SceneManager.LoadScene("Main Scene");
    }

        public void Quit()
        {
            // for prod
            UnityEditor.EditorApplication.isPlaying = false;

            // for standalone built ver
            // Application.Quit();
        }
}
