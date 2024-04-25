using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    public void Restart()
    {
        SceneManager.LoadScene("Main Scene");
    }

        public void Quit()
        {
            // for prod
            // UnityEditor.EditorApplication.isPlaying = false;

            // for standalone built ver
            Application.Quit();
        }
}
