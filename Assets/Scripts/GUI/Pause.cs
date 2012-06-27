using UnityEngine;
using System.Collections;
/// <summary>
/// Pause.cs
/// 
/// Handles the Pause menu. Shows the menu, handles the buttons and hides the menu on resume state.
/// </summary>
public class Pause : MonoBehaviour
{
    #region Fields
    private Camera menuCamera;
    #endregion

    #region Functions
    void Awake() {
        menuCamera = NGUITools.FindCameraForLayer(LayerMask.NameToLayer("Pause Menu"));
    }

    void Update() {
        // Checks if the game state has been paused. Shows or hides the Pause menu.
        if (Game.Paused) {
            ShowPause();
        } else {
            if (menuCamera.enabled) HidePause();
        }
    }

    /// <summary>
    /// Button handler for "Resume" button
    /// </summary>
    void OnResumeClicked() {
        if (menuCamera.enabled) {
            Game.Resume();
            HidePause();
        }
    }

    /// <summary>
    /// Button handler for "Main Menu" button
    /// </summary>
    void OnMenuClicked() {
        if (menuCamera.enabled) {
            Game.Resume();
            Application.LoadLevel("MainMenu");
        }
    }

    /// <summary>
    /// Button handler for "Quit Game" button
    /// </summary>
    void OnQuitClicked() {
        if (menuCamera.enabled)
            Application.Quit();
    }

    /// <summary>
    /// Shows the pause menu
    /// </summary>
    void ShowPause() {
        menuCamera.enabled = true;
    }

    /// <summary>
    /// Hides the pause menu
    /// </summary>
    void HidePause() {
        menuCamera.enabled = false;
    }
    #endregion
}