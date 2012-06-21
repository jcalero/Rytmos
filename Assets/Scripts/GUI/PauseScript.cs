using UnityEngine;
using System.Collections;

public class PauseScript : MonoBehaviour
{

    #region Fields
    private UIButton resumeButton;
    private UIButton menuButton;
    private UIButton exitButton;
    private float timer;
    private bool paused = false;
    #endregion

    #region Functions

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Menu) || Input.GetKeyDown(KeyCode.Space))
        {
            if (!paused)
            {
                paused = true;
                NGUITools.FindCameraForLayer(LayerMask.NameToLayer("Pause Menu")).enabled = true;
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
        //StartCoroutine(resume());
        resume();
    }

    void OnMenuClicked()
    {
        resume("MainMenu");
    }

    void OnQuitClicked()
    {
        Application.Quit();
    }

    //IEnumerator resume() 
    //{
    //    float pauseEndTime = Time.realtimeSinceStartup + 1;
    //    while (Time.realtimeSinceStartup < pauseEndTime)
    //    {
    //        yield return 0;
    //    }
    //    paused = false;
    //    NGUITools.FindCameraForLayer(LayerMask.NameToLayer("Pause Menu")).enabled = false;
    //    Camera.main.GetComponent<AudioSource>().Play();
    //    Time.timeScale = 1;
    //}

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
