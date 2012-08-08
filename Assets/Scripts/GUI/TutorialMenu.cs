using UnityEngine;
using System.Collections;
/// <summary>
/// Pause.cs
/// 
/// Handles the Pause menu. Shows the menu, handles the buttons and hides the menu on resume state.
/// </summary>
public class TutorialMenu : MonoBehaviour {
	#region Fields
	public UIPanel[] PopupPanels;

	private static TutorialMenu instance;
	#endregion

	#region Functions
	void Awake() {
		instance = this;
		foreach(UIPanel p in PopupPanels) UITools.SetActiveState (p, false);	
	}

	public static void Show() {
		UITools.SetActiveState (instance.PopupPanels[Tutorial.sceneNumber-1], true);
	}

	public static void Hide() {
		Tutorial.showingMessage = false;
		UITools.SetActiveState (instance.PopupPanels[Tutorial.sceneNumber-1], false);
	}

	/// <summary>
	/// Button handler for "Resume" button
	/// </summary>
	void OnResumeClicked() {
		Game.Resume(true);
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
		// Call revert to start functionality
		Game.Resume(true);
		Application.LoadLevel("LoadScreen");
		
	}

	/// <summary>
	/// Button handler for "Quit Game" button
	/// </summary>
	void OnQuitClicked() {
		Application.Quit();
	}
	
	private IEnumerator DelayButton(UIButton button, float time) {
		button.isEnabled = false;
		yield return new WaitForSeconds(time);
		button.isEnabled = true;
	}
	#endregion
}