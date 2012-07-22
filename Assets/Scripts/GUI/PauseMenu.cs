using UnityEngine;
using System.Collections;
/// <summary>
/// Pause.cs
/// 
/// Handles the Pause menu. Shows the menu, handles the buttons and hides the menu on resume state.
/// </summary>
public class PauseMenu : MonoBehaviour {
	#region Fields
	public UIPanel PausePanel;

	private static PauseMenu instance;
	#endregion

	#region Functions
	void Awake() {
		instance = this;
		//menuCamera = NGUITools.FindCameraForLayer(LayerMask.NameToLayer("Pause Menu"));
		UITools.SetActiveState(PausePanel, false);
	}

	void Update() {
		// Checks if the game state has been paused. Shows or hides the Pause menu.
		//if (Game.Paused) {
		//    Debug.Log("Pause");
		//    UITools.SetActiveState(PausePanel, true);
		//} else {
		//    if (PausePanel.enabled) UITools.SetActiveState(PausePanel, false);
		//}
	}

	public static void Show() {
		UITools.SetActiveState(instance.PausePanel, true);
	}

	public static void Hide() {
		UITools.SetActiveState(instance.PausePanel, false);
	}

	/// <summary>
	/// Button handler for "Resume" button
	/// </summary>
	void OnResumeClicked() {
		Game.Resume();
	}

	/// <summary>
	/// Button handler for "Main Menu" button
	/// </summary>
	void OnMenuClicked() {
		Application.LoadLevel("MainMenu");
	}

	/// <summary>
	/// Button handler for "Quit Game" button
	/// </summary>
	void OnQuitClicked() {
		Application.Quit();
	}
	#endregion
}