using UnityEngine;
using System.Collections;
using System.IO;
using System;
using System.Collections.Generic;
/// <summary>
/// MainMenu.cs
/// 
/// Handles the display of the main menu. 
/// </summary>
public class MainMenu2 : MonoBehaviour {

	#region Fields
	// Menu panels (Inspector instances, location: MainMenuManager)
	public UIPanel MainMenuBasePanel;
	public UIPanel MainMenuPlayPanel;
	public UIPanel MainMenuQuitPanel;
	public UIPanel MainMenuModePanel;
	public UIPanel MainMenuOptionsPanel;
	public UIPanel MainMenuScoresPanel;

	// Current state of the menu.
	public MenuLevel CurrentMenuLevel;

	// Mode menu level buttons
	public UIButton ArcadeButton;
	public UIButton CasualButton;
	public UIButton ChallengeButton;
	public UIButton StoryButton;

	// Mode menu level particles;
	public ParticleSystem ArcadeParticles;
	public ParticleSystem CasualParticles;
	public ParticleSystem ChallengeParticles;
	public ParticleSystem StoryParticles;


	// Mode menu button states
	private bool arcadeButtonActive;
	private bool casualButtonActive;
	private bool challengeButtonActive;
	private bool storyButtonActive;
	#endregion

	#region Functions

	void Awake() {
		Game.GameState = Game.State.Menu;
		CurrentMenuLevel = MenuLevel.Base;
	}

	void Start() {
	}

	void Update() {
		// When the player presses "Escape" or "Back" on Android, returns to main menu screen
		// or goes to the quit menu if on the main menu already.
		if (Input.GetKeyDown(KeyCode.Escape)) {
			if (CurrentMenuLevel == MenuLevel.Base) ChangeMenu(MenuLevel.Quit);
			else ChangeMenu(MenuLevel.Base);
		}
	}

	/// <summary>
	/// Change the menu now to watever the CurrentMenuLevel is set to.
	/// </summary>
	private void ChangeMenu() {
		ChangeMenu(CurrentMenuLevel);
	}

	/// <summary>
	/// Change the menu to the given menu level. Also set CurrentMenuLevel appropriately.
	/// </summary>
	/// <param name="menu">The MenuLevel enum to change the menu level to.</param>
	private void ChangeMenu(MenuLevel menu) {
		CurrentMenuLevel = menu;
		switch (CurrentMenuLevel) {
			case MenuLevel.Base:
				if (MainMenuQuitPanel.enabled) UITools.SetActiveState(MainMenuQuitPanel, false);
				if (MainMenuModePanel.enabled) UITools.SetActiveState(MainMenuModePanel, false);
				UITools.SetActiveState(MainMenuPlayPanel, true);
				break;
			case MenuLevel.Mode:
				UITools.SetActiveState(MainMenuPlayPanel, false);
				UITools.SetActiveState(MainMenuModePanel, true);
				break;
			case MenuLevel.Options:
				break;
			case MenuLevel.Quit:
				UITools.SetActiveState(MainMenuPlayPanel, false);
				UITools.SetActiveState(MainMenuQuitPanel, true);
				break;
			case MenuLevel.Scores:
				break;
		}
	}

	#region Main Menu buttons
	/// <summary>
	/// Button handler for "Play" button
	/// </summary>
	void OnPlayClicked() {
		ChangeMenu(MenuLevel.Mode);
	}

	/// <summary>
	/// Button handler for "Highscores" button
	/// </summary>
	void OnHighScoresClicked() {
	}

	/// <summary>
	/// Button handler for "Options" button
	/// </summary>
	void OnOptionsClicked() {
	}
	#endregion

	#region Other Buttons (Back)
	/// <summary>
	/// Button handler or "Back" button
	/// </summary>
	void OnBackClicked() {
		ChangeMenu(MenuLevel.Base);
	}
	#endregion

	#region Quit menu buttons
	/// <summary>
	/// Button handler for "Yes, Quit Game" button
	/// </summary>
	void OnQuitConfirmedClicked() {
		Application.Quit();
	}

	/// <summary>
	/// Button handler for "No, Don't Quit Game" button
	/// </summary>
	private void OnQuitCancelClicked() {
		ChangeMenu(MenuLevel.Base);
	}
	#endregion

	#region Select Mode buttons (Shows file browser)
	/// <summary>
	/// Button handler for "Arcade" button
	/// </summary>
	private void OnArcadeButtonClicked() {
		if (arcadeButtonActive) {
			FadeOutMenu();
			Debug.Log("Theoretically Loading Game");
			//StartCoroutine(LoadLevelDelayed("Game", 1f));
		} else {
			Vector3 buttonScale = ArcadeButton.GetComponentInChildren<UISlicedSprite>().transform.localScale;
			buttonScale =
				new Vector3(buttonScale.x*1.1f, buttonScale.y*1.1f, buttonScale.z);
			arcadeButtonActive = true;
		}
	}

	/// <summary>
	/// Button handler for "Casual" button
	/// </summary>
	private void OnCasualButtonClicked() {

	}

	/// <summary>
	/// Button handler for "Challenge" button
	/// </summary>
	private void OnChallengeButtonClicked() {

	}
	/// <summary>
	/// Button handler for "Story" button
	/// </summary>
	private void OnStoryButtonClicked() {

	}
	#endregion

	#region Highscore buttons
	void OnNextHighscoreClicked() {
	}

	void OnPrevHighscoreClicked() {
	}

	void OnReloadClicked() {
	}
	#endregion

	#region Utilities
	/// <summary>
	/// Loads the specified level after a certain amount of time
	/// </summary>
	/// <param name="level">The level string to load</param>
	/// <param name="time">The amount of seconds to wait before loading</param>
	/// <returns></returns>
	private IEnumerator LoadLevelDelayed(string level, float time) {
		yield return new WaitForSeconds(time);
		Application.LoadLevel(level);
	}

	private void FadeOutMenu() {

	}
	#endregion

	#endregion
}

public enum MenuLevel {
	Base,
	Mode,
	Quit,
	Scores,
	Options
}