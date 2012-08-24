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

	private static PauseMenu instance;
	#endregion

	#region Functions
	void Awake() {
		instance = this;
		UITools.SetActiveState(PausePanel, false);
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
		UITools.SetActiveState(instance.PausePanel,false);
		UITools.SetActiveState(instance.OptionsPanel,true);
	}

	/// <summary>
	/// Button handler for "Quit Game" button
	/// </summary>
	void OnQuitClicked() {
		Application.Quit();
	}
	
	void OnBackClicked() {
		UITools.SetActiveState(instance.OptionsPanel,false);
		UITools.SetActiveState(instance.PausePanel,true);
		
		// Now that the volume levels have been set, update them everywhere!
		AudioPlayer.changeVolume();
		Player.ChangeVolume();
	}
	#endregion
}