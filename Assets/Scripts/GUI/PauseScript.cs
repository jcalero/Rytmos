using UnityEngine;
using System.Collections;

public class PauseScript : MonoBehaviour
{

    #region Fields
    private int textWidth = (int)(Screen.width / 1.15);
    private int buttonWidth = 200;
    private string pauseText = "Paused";
    private string resumeButtonText = "Resume Game";
    private string menuButtonText = "Main Menu";
    private string exitButtonText = "Quit Game";

    bool paused = false;
    #endregion

    #region Properties

    #endregion

    #region Functions

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Menu) || Input.GetKeyDown(KeyCode.Space))
        {
            if (!paused)
            {
                paused = true;
                Time.timeScale = 1;
            }
            else
            {
                resume();
            }
        }
    }

    void OnGUI()
    {
        if (paused)
        {
            GUI.Box(new Rect(Screen.width / 2 - textWidth / 2, 90, textWidth, 300),"");
            GUI.Box(new Rect(Screen.width / 2 - textWidth / 2, 100, textWidth, 50), pauseText);
            if (GUI.Button(new Rect(Screen.width / 2 - buttonWidth / 2, 180, buttonWidth, 40), resumeButtonText))
                resume();
            if (GUI.Button(new Rect(Screen.width / 2 - buttonWidth / 2, 220, buttonWidth, 40), menuButtonText))
                resumeLevel("MainMenu");
            if (GUI.Button(new Rect(Screen.width / 2 - buttonWidth / 2, 260, buttonWidth, 40), exitButtonText))
            {
                Debug.Log("Exiting Game");
                Application.Quit();
            }
        }
    }

    void resume() 
    {
        paused = false;
        Time.timeScale = 1;
    }

    void resumeLevel(string level)
    {
        paused = false;
        Time.timeScale = 1;
        Application.LoadLevel(level);
    }


    #endregion
}
