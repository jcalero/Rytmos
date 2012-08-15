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
	public GameObject powerup;
	public GameObject player;
	public GameObject[] arrows;

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
		disableButtons = true;
		Vector3 boxLocation;
		switch(Tutorial.sceneNumber) {
		case 1:
			boxLocation = findBoxLocation(findPos("EnemyCyanPrefab(Clone)"));
			break;
		case 2:
			boxLocation = findBoxLocation(findPos("EnemyYellowPrefab(Clone)"));
			break;
		case 3:
			boxLocation = findBoxLocation(findPos("EnemyGreenPrefab(Clone)"));
			break;
		case 4:
			boxLocation = GameObject.Find ("MultiplierLabel").transform.localPosition;
			moveSprite("Sprite (arrow)", new Vector3(boxLocation.x-60, boxLocation.y-60, 0), Tutorial.sceneNumber-1);
			break;
		case 5:
			return;
		case 6:
			boxLocation = findBoxLocation(findPos("ShieldPW"));
			hasPushedFirst = false;
			moveSprite ("SelectPowerup", boxLocation, Tutorial.sceneNumber-1);
			instance.arrows[0].GetComponent<TweenPosition>().from = new Vector3(boxLocation.x, boxLocation.y-70, boxLocation.z);
			break;
		case 7:
			boxLocation = findBoxLocation(findPos ("SuperpulsePW"));
			instance.arrows[1].transform.localPosition = new Vector3(boxLocation.x, boxLocation.y-70, boxLocation.z);
			moveSprite ("SelectPowerup", boxLocation, Tutorial.sceneNumber-1);
			hasPushedFirst = false;
			break;
		case 8:
			hasPushedFirst = false;
			boxLocation = findBoxLocation(findPos ("ChainPW"));
			moveSprite ("SelectPowerup", boxLocation, Tutorial.sceneNumber-1);
			break;
		case 9:
			return;
		case 10:
			return;
		default:
			return;
		}
		moveSprite("SlicedSprite (TinyBox)", boxLocation, Tutorial.sceneNumber-1);
		
	}
	
	private static Vector3 findPos(string name) {
		return GameObject.Find (name).transform.localPosition;
	}
	
	private static void moveSprite(string sprite, Vector3 location, int sceneNumber) {
		instance.PopupPanels[sceneNumber].transform.FindChild(sprite).transform.localPosition = location;	
	}
	
	private static Vector3 findBoxLocation(Vector3 enemypos) {
		return new Vector3(((Screen.width/2)*enemypos.x/Game.screenRight), ((Screen.height/2) * enemypos.y/Game.screenTop), 0);
	}
	
	public static void Hide() {
		Tutorial.showingMessage = false;
		UITools.SetActiveState (instance.PopupPanels[Tutorial.sceneNumber-1], false);
	}

	/// <summary>
	/// Button handler for "Resume" button
	/// </summary>
	void OnResumeClicked() {
		TutorialResume();
	}
	
	void OnProceedClicked() {
		player.GetComponent<Player>().sendPulse();
		TutorialResume();
	}
	
	void OnSelectPowerup() {
		if(!hasPushedFirst) {
			player.GetComponent<Player>().changePowerup();
			//powerup.GetComponent<PowerupScript>().spawnPowerupOnScreen(1, new Vector3(20, 20, 0));
			hasPushedFirst = true;
		}
	}
	
	void OnSelectMiddle() {
		if(hasPushedFirst) {
			player.GetComponent<Player>().activatePowerup();
			TutorialResume();		
		}
	}
	
	void TutorialResume() {
		Game.disablePause = false;
		Game.CommonResumeOperation();
		Hide ();
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
		TutorialResume();
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
	