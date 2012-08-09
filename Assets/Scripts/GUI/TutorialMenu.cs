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
	private static bool disableButtons;
	private float timer;
	private bool buttonsDisabled;
	#endregion

	#region Functions
	void Awake() {
		timer = 0f;
		buttonsDisabled = false;
		instance = this;
		foreach(UIPanel p in instance.PopupPanels) UITools.SetActiveState (p, false);	
	}
	
	void Update() {
		
		// This bit of code disables the "continue/retry" buttons for the tutorial popup for 100ms to eliminate accidental buttonpresses.
		// (This "hack" has to be used because starting a coroutine in a static method like Show() is really really annoying)
		if(disableButtons) {
			if(!buttonsDisabled) {
				foreach(UIButton b in instance.PopupPanels[Tutorial.sceneNumber-1].GetComponentsInChildren<UIButton>()) b.isEnabled = false;
				buttonsDisabled = true;
				timer = Time.realtimeSinceStartup;
				Debug.Log("here");
			}
			else if(Time.realtimeSinceStartup-timer > 0.1f) {
				Debug.Log("here2");
				timer = 0f;
				disableButtons = false;
				buttonsDisabled = false;
				foreach(UIButton b in instance.PopupPanels[Tutorial.sceneNumber-1].GetComponentsInChildren<UIButton>()) b.isEnabled = true;
			}
		}
	}

	public static void Show() {
		UITools.SetActiveState (instance.PopupPanels[Tutorial.sceneNumber-1], true);
		disableButtons = true;
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
	#endregion
}