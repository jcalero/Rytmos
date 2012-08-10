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
//	public UIPanel[] PausePanels;
	public GameObject powerup;
	public GameObject player;

	private static TutorialMenu instance;
	private static bool disableButtons;
	private float timer;
	private bool buttonsDisabled;
	
	private static bool hasPushedFirst;
	#endregion

	#region Functions
	void Awake() {
		timer = 0f;
		buttonsDisabled = false;
		hasPushedFirst = false;
		instance = this;
		foreach(UIPanel p in instance.PopupPanels) UITools.SetActiveState (p, false);
//		foreach(UIPanel p in instance.PausePanels) UITools.SetActiveState(p, false);
	}
	
	void Update() {
		
		// This bit of code disables the "continue/retry" buttons for the tutorial popup for 100ms to eliminate accidental buttonpresses.
		// (This "hack" has to be used because starting a coroutine in a static method like Show() is really really annoying)
		if(disableButtons) {
			if(!buttonsDisabled) {
				foreach(UIButton b in instance.PopupPanels[Tutorial.sceneNumber-1].GetComponentsInChildren<UIButton>()) b.isEnabled = false;
				buttonsDisabled = true;
				timer = Time.realtimeSinceStartup;
			}
			else if(Time.realtimeSinceStartup-timer > 0.1f) {
				timer = 0f;
				disableButtons = false;
				buttonsDisabled = false;
				foreach(UIButton b in instance.PopupPanels[Tutorial.sceneNumber-1].GetComponentsInChildren<UIButton>()) b.isEnabled = true;
			}
		}
	}

	public static void Show() {
		UITools.SetActiveState (instance.PopupPanels[Tutorial.sceneNumber-1], true);
		if(Tutorial.sceneNumber == 6) 
			hasPushedFirst = false;
		
		disableButtons = true;
	}

	public static void Hide() {
		Tutorial.showingMessage = false;
		UITools.SetActiveState (instance.PopupPanels[Tutorial.sceneNumber-1], false);
//		UITools.SetActiveState (instance.PausePanels[Tutorial.sceneNumber-1], false);
	}
	
	public static void HideContinue() {
		UITools.SetActiveState(instance.PopupPanels[Tutorial.sceneNumber-1], false);
//		UITools.SetActiveState(instance.PausePanels[Tutorial.sceneNumber-1], true);
	}

	/// <summary>
	/// Button handler for "Resume" button
	/// </summary>
	void OnResumeClicked() {
		if(Tutorial.sceneNumber == 1 || Tutorial.sceneNumber == 2 
		|| Tutorial.sceneNumber == 3 || Tutorial.sceneNumber == 7) {
			HideContinue();
		} else {
			Game.Resume(true);	
		}
	}
	
	void OnProceedClicked() {
		
		
		if(Tutorial.sceneNumber == 1 || Tutorial.sceneNumber == 2 || Tutorial.sceneNumber == 3) {
			Tutorial.sentTutorialPulse = true;
			player.GetComponent<Player>().sendPulse(true);
		}
		Game.Resume(true);
	}
	
	void OnSelectPowerup() {
		player.GetComponent<Player>().changePowerup();
		hasPushedFirst = true;
		powerup.GetComponent<PowerupScript>().spawnPowerupOnScreen(1, new Vector3(20, 20, 0));
		Tutorial.SetUpParticlesFeedback(4, new Vector3(20,20,0));
	}
	
	void OnSelectMiddle() {
		if(hasPushedFirst) {
			player.GetComponent<Player>().activatePowerup();
			Game.Resume(true);
		}
	}
	
	void OnSlideProceedClicked() {
		player.GetComponent<Player>().sendPulse(true);
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
	