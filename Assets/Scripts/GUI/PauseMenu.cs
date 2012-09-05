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
	public UIPanel OptionsPanel;
	public UIPanel BackgroundPanel;
	public UIPanel TrialMessagePanel;
	
	public UISlider MusicVolumeSlider;
	public UISlider EffectsVolumeSlider;
	
	private static bool inOptions;
	private static PauseMenu instance;
	#endregion

	#region Functions
	void Awake() {
		instance = this;
		inOptions = false;
		UITools.SetActiveState(instance.PausePanel, false);
		UITools.SetActiveState(instance.OptionsPanel,false);
		UITools.SetActiveState(instance.BackgroundPanel,false);
		if (Application.loadedLevelName == "Game") UITools.SetActiveState(TrialMessagePanel, false);
	}
	
	public static bool InOptions {
		get {return inOptions;}		
	}
	
	public static void LeaveOptions() {
		instance.OnBackClicked();	
	}

	public static void Show() {
		UITools.SetActiveState(instance.PausePanel, true);
		UITools.SetActiveState(instance.BackgroundPanel,true);
	}

	public static void Hide() {
		UITools.SetActiveState(instance.BackgroundPanel,false);
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

	void OnRestartSongClicked() {
		Application.LoadLevel("LoadScreen");
	}
	
	private void OnEffectsSliderChange() {
		Game.EffectsVolume = instance.EffectsVolumeSlider.sliderValue;
	}

	private void OnMusicSliderChange() {
		Game.MusicVolume = instance.MusicVolumeSlider.sliderValue;
	}
	
	void OnOptionsClicked() {
		inOptions = true;
		instance.MusicVolumeSlider.sliderValue = Game.MusicVolume;
		instance.EffectsVolumeSlider.sliderValue = Game.EffectsVolume;
		UITools.SetActiveState(instance.OptionsPanel,true);
		UITools.SetActiveState(instance.PausePanel,false);
	}

	/// <summary>
	/// Button handler for "Quit Game" button
	/// </summary>
	void OnQuitClicked() {
		Application.Quit();
	}
	
	void OnBackClicked() {
		UITools.SetActiveState(instance.PausePanel,true);
		UITools.SetActiveState(instance.OptionsPanel,false);
		
		// Now that the volume levels have been set, update them everywhere!
		AudioPlayer.changeVolume();
		Player.ChangeVolume();
		inOptions = false;
	}
	
	// Trial message stuff
	void OnContinueClicked() {
		Game.CommonResumeOperation();
		Application.LoadLevel("Win");
	}

	public static void ShowTrialMessage() {
		UITools.SetActiveState(instance.TrialMessagePanel, true);
	}
	#endregion
}