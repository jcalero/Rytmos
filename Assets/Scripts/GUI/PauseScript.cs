using UnityEngine;
using System.Collections;

public class PauseScript : MonoBehaviour
{

    #region Fields
    private bool paused = false;
    private Camera menuCamera;
    #endregion

    #region Functions

    void Awake()
    {
        menuCamera = NGUITools.FindCameraForLayer(LayerMask.NameToLayer("Pause Menu"));
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Menu) || Input.GetKeyDown(KeyCode.Space))
        {
            if (!paused)
            {
                paused = true;
                menuCamera.enabled = true;
                Camera.main.GetComponent<AudioSource>().Pause();
                Time.timeScale = 0f;
            }
            else
            {
                resume();
            }
        }
    }

    void OnResumeClicked()
    {
        if (menuCamera.enabled)
            resume();
    }

    void OnMenuClicked()
    {
        if (menuCamera.enabled)
            resume("MainMenu");
    }

    void OnQuitClicked()
    {
        if (menuCamera.enabled)
            Application.Quit();
    }

    void resume()
    {
        paused = false;
        NGUITools.FindCameraForLayer(LayerMask.NameToLayer("Pause Menu")).enabled = false;
        Camera.main.GetComponent<AudioSource>().Play();
        Time.timeScale = 1;
    }
    
    void resume(string level)
    {
        paused = false;
        Time.timeScale = 1;
        Application.LoadLevel(level);
    }


    #endregion
}