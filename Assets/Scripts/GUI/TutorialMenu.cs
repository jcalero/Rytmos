using UnityEngine;
using System.Collections;
/// <summary>
/// Pause.cs
/// 
/// Handles the Pause menu. Shows the menu, handles the buttons and hides the menu on resume state.
/// </summary>
public class TutorialMenu : MonoBehaviour {
	#region Fields
	public UIPanel TutorialPopup;

	private static TutorialMenu instance;
	#endregion

	#region Functions
	void Awake() {
		instance = this;
		UITools.SetActiveState(TutorialPopup, false);
	}

	public static void Show() {
		UITools.SetActiveState(instance.TutorialPopup, true);
	}

	public static void Hide() {
		UITools.SetActiveState(instance.TutorialPopup, false);
	}

	/// <summary>
	/// Button handler for "Resume" button
	/// </summary>
	void OnResumeClicked() {
		Debug.Log ("Does this work?");
		Game.Resume();
	}

	/// <summary>
	/// Button handler for "Main Menu" button
	/// </summary>
	void OnMenuClicked() {
		Application.LoadLevel("MainMenu");
	}

	void OnRestartSongClicked() {
		Application.LoadLevel("LoadScreen");
	}
	
	void OnRetryClicked() {
		Debug.Log ("Does this work?");
		// Call retry functionality
	}

	/// <summary>
	/// Button handler for "Quit Game" button
	/// </summary>
	void OnQuitClicked() {
		Application.Quit();
	}
	#endregion
}