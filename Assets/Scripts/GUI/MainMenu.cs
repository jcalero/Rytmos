using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour
{

    #region Fields
    private UIDraggablePanel panel;

    private Vector3 quitMenu;
    private Vector3 mainMenu;
    private Vector3 modeMenu;
    private Vector3 highScoresMenu;
    private Vector3 optionsMenu;

    public bool skipMenu;
    #endregion

    #region Functions

    private void Awake()
    {
        Debug.Log(Time.realtimeSinceStartup);
        if (skipMenu) Application.LoadLevel("Game");
        panel = GameObject.Find("Panel (Draggable)").GetComponent<UIDraggablePanel>();
        quitMenu = new Vector3(0f, 0f, 0f);
        mainMenu = new Vector3(-650f, 0f, 0f);
        modeMenu = new Vector3(-650f * 2, 0f, 0f);
        highScoresMenu = new Vector3(-650f * 3, 0f, 0f);
        optionsMenu = new Vector3(-650f * 4, 0f, 0f);
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
            OnBackClicked();
    }

    void OnPlayClicked()
    {
        // Stop any current momentum to allow for Spring to begin.
        panel.currentMomentum = Vector3.zero;
        // Begin spring motion
        SpringPanel.Begin(panel.gameObject, modeMenu, 13f);
    }

    void OnHighScoresClicked()
    {
        // Stop any current momentum to allow for Spring to begin.
        panel.currentMomentum = Vector3.zero;
        // Begin spring motion
        SpringPanel.Begin(panel.gameObject, highScoresMenu, 13f);
    }

    void OnOptionsClicked()
    {
        // Stop any current momentum to allow for Spring to begin.
        panel.currentMomentum = Vector3.zero;
        // Begin spring motion
        SpringPanel.Begin(panel.gameObject, optionsMenu, 13f);
    }

    void OnQuitClicked()
    {
        // Stop any current momentum to allow for Spring to begin.
        panel.currentMomentum = new Vector3(0f, 0f, 0f);
        // Begin spring motion
        SpringPanel.Begin(panel.gameObject, quitMenu, 13f);
    }

    void OnBackClicked()
    {
        // Stop any current momentum to allow for Spring to begin.
        panel.currentMomentum = Vector3.zero;
        // Begin spring motion
        SpringPanel.Begin(panel.gameObject, mainMenu, 13f);
    }

    void OnQuitConfirmedClicked()
    {
        Application.Quit();
    }

    void OnArcadeModeClicked()
    {
        Application.LoadLevel("Game");
    }

    #endregion
}
