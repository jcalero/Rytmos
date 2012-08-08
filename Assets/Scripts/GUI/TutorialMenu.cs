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
	public UIPanel FirstEnemyPopup;
	public UIPanel SecondEnemyPopup;
	public UIPanel ThirdEnemyPopup;
	public UIPanel ComboMessagePopup;
	public UIPanel SlideFingerPopup;
	public UIPanel IncreasedSpawnPopup;
	public UIPanel ShieldPopup;
	public UIPanel ShieldShowcasePopup;
	public UIPanel SuperPulsePopup;
	public UIPanel ChainPulsePopup;

	private static TutorialMenu instance;
	#endregion

	#region Functions
	void Awake() {
		instance = this;
		UITools.SetActiveState(TutorialPopup, false);
		UITools.SetActiveState(FirstEnemyPopup, false);
		UITools.SetActiveState(SecondEnemyPopup, false);
		UITools.SetActiveState(ThirdEnemyPopup, false);
		UITools.SetActiveState(ComboMessagePopup, false);
		UITools.SetActiveState(SlideFingerPopup, false);
		UITools.SetActiveState(IncreasedSpawnPopup, false);
		UITools.SetActiveState(ShieldPopup, false);
		UITools.SetActiveState(ShieldShowcasePopup, false);
		UITools.SetActiveState(SuperPulsePopup, false);
		UITools.SetActiveState(ChainPulsePopup, false);
	}

	public static void Show() {
		switch(Tutorial.sceneNumber) {
		case 1:
			UITools.SetActiveState (instance.FirstEnemyPopup, true);
			break;
		case 2:
			UITools.SetActiveState(instance.SecondEnemyPopup, true);
			break;
		case 3:
			UITools.SetActiveState(instance.ThirdEnemyPopup, true);
			break;
		case 4:
			UITools.SetActiveState(instance.ComboMessagePopup, true);
			break;
		case 5:
			UITools.SetActiveState(instance.SlideFingerPopup, true);
			break;
		case 6:
			UITools.SetActiveState(instance.IncreasedSpawnPopup, true);
			break;
		case 7:
			UITools.SetActiveState(instance.ShieldPopup, true);
			break;
		case 8:
			UITools.SetActiveState(instance.ShieldShowcasePopup, true);
			break;
		case 9:
			UITools.SetActiveState(instance.SuperPulsePopup, true);
			break;
		case 10:
			UITools.SetActiveState(instance.ChainPulsePopup, true);
			break;
		default:
			UITools.SetActiveState(instance.TutorialPopup, true);
			break;
				
		}
//		UITools.SetActiveState(instance.TutorialPopup, true);
		Debug.Log ("Scene Number: "+Tutorial.sceneNumber);
	}

	public static void Hide() {
		Tutorial.showingMessage = false;
		switch(Tutorial.sceneNumber) {
		case 1: 
			UITools.SetActiveState(instance.FirstEnemyPopup, false);
			break;
		case 2:
			UITools.SetActiveState(instance.SecondEnemyPopup, false );
			break;
		case 3:
			UITools.SetActiveState(instance.ThirdEnemyPopup, false );
			break;
		case 4:
			UITools.SetActiveState(instance.ComboMessagePopup, false );
			break;
		case 5:
			UITools.SetActiveState(instance.SlideFingerPopup, false);
			break;
		case 6:
			UITools.SetActiveState(instance.IncreasedSpawnPopup, false);
			break;
		case 7:
			UITools.SetActiveState(instance.ShieldPopup, false);
			break;
		case 8:
			UITools.SetActiveState(instance.ShieldShowcasePopup, false);
			break;
		case 9:
			UITools.SetActiveState(instance.SuperPulsePopup, false);
			break;
		case 10:
			UITools.SetActiveState(instance.ChainPulsePopup, false);
			break;
		default:
			UITools.SetActiveState(instance.TutorialPopup, false);
			break;
		}
		
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