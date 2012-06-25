using UnityEngine;
using System.Collections;

public class PauseScript : MonoBehaviour
{

    #region Fields
    private bool paused = false;
    private Camera menuCamera;
    #endregion

    #region Functions
    void Awake() {
        menuCamera = NGUITools.FindCameraForLayer(LayerMask.NameToLayer("Pause Menu"));
    }

    void Update() {
        if (Game.Paused) {
            pause();
        } else if (paused) {
            resume();
        }
    }

    void OnResumeClicked() {
        if (menuCamera.enabled)
            resume();
    }

    void OnMenuClicked() {
        if (menuCamera.enabled)
            resume("MainMenu");
    }

    void OnQuitClicked() {
        if (menuCamera.enabled)
            Application.Quit();
    }

    void pause() {
        paused = true;
        menuCamera.enabled = true;
    }

    void resume() {
        paused = false;
        menuCamera.enabled = false;
    }
    
    void resume(string level) {
        Game.Resume();
        paused = false;
        Time.timeScale = 1;
        Application.LoadLevel(level);
    }
    #endregion
}