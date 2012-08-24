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
	}
	
	public static bool InOptions {
		get {return inOptions;}		
	}
	
	public static void LeaveOptions() {
		instance.OnBackClicked();	
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

	void OnRestartSongClicked() {
		Application.LoadLevel("LoadScreen");
	}
	
	private void OnEffectsSliderChange() {
		Game.EffectsVolume = EffectsVolumeSlider.sliderValue;
	}

	private void OnMusicSliderChange() {
		Game.MusicVolume = MusicVolumeSlider.sliderValue;
	}
	
	void OnOptionsClicked() {
		inOptions = true;
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
	#endregion
}