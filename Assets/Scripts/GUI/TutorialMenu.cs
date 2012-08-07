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
		Debug.Log ("Scene Number: "+Tutorial.sceneNumber);
	}

	public static void Hide() {
		UITools.SetActiveState(instance.TutorialPopup, false);
	}

	/// <summary>
	/// Button handler for "Resume" button
	/// </summary>
	void OnResumeClicked() {
		Game.Resume();
		Tutorial.timeStamp = Tutorial.audioTimer;
		Tutorial.colorStore = EnemySpawnScript.currentlySelectedEnemy;
		for(int i = 0; i<EnemySpawnScript.spawnPositions.Length; i++) {
			Tutorial.spawnPosStore[i] = EnemySpawnScript.spawnPositions[i];
		}
		if(EnemySpawnScript.spawnerCounter == 2) Tutorial.numSpawners = false;
		else Tutorial.numSpawners = true;
		Tutorial.storeRotation = EnemySpawnScript.getRotation;
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
		// Call retry functionality
		Tutorial.SkipTo(Tutorial.timeStamp-1);
		Debug.Log("Scene: "+Tutorial.sceneNumber);
		switch(Tutorial.sceneNumber) {
		case 1:	
			//Make sure this button is disabled.
			break;
		case 2:
			Tutorial.firstSpawn = false;
			Tutorial.firstEnemyMessage = false;
			Tutorial.secondSpawn = false;
			Tutorial.secondEnemyMessage = false;
			EnemySpawnScript.spawnCount = 0;
			Player.multiplier = 1;
			Tutorial.sceneNumber = 1;
			Player.KillStreakCounter = 0;
			break;
		case 3:
			Tutorial.secondSpawn = false;
			Tutorial.secondEnemyMessage = false;
			Tutorial.thirdEnemyMessage = false;
			Tutorial.thirdSpawn = false;
			Tutorial.done = false;
			EnemySpawnScript.spawnCount = 2;
			Player.multiplier = 1;
			Tutorial.sceneNumber = 2;
			Player.KillStreakCounter = 2;
			break;
		case 4:
			Tutorial.thirdEnemyMessage = false;
			Tutorial.thirdSpawn = false;
			Tutorial.done = false;
			EnemySpawnScript.spawnCount = 4;
			Player.multiplier = 1;
			Tutorial.sceneNumber = 3;
			Player.KillStreakCounter = 4;
			break;
		case 5:
			Tutorial.comboMessage = false;
			Player.multiplier = 2;
			Player.KillStreakCounter = 6;
			Tutorial.sceneNumber = 4;
			break;
		case 6:
			break;
		case 7:
			break;
		case 8:
			break;
		case 9:
			break;
		case 10:
			break;
		case 11:
			
			break;
			
		}

		EnemySpawnScript.currentlySelectedEnemy = Tutorial.colorStore;
		if(Tutorial.numSpawners) {
			for(int i=0; i<Tutorial.spawnPosStore.Length; i++) {
				EnemySpawnScript.spawnPositions[i] = Tutorial.spawnPosStore[i];
			}
		} else {
			for(int i=0; i<Tutorial.spawnPosStore.Length-1; i++) {
				EnemySpawnScript.spawnPositions[i] = Tutorial.spawnPosStore[i];
			}
		}
		
		Level.SetUpParticlesFeedback(EnemySpawnScript.spawnPositions.Length, EnemySpawnScript.currentlySelectedEnemy);
		if(Tutorial.numSpawners) EnemySpawnScript.spawnerCounter = 3;
		else EnemySpawnScript.spawnerCounter = 2;
		EnemySpawnScript.getRotation = Tutorial.storeRotation;
		Game.Resume (true);
		
	}

	/// <summary>
	/// Button handler for "Quit Game" button
	/// </summary>
	void OnQuitClicked() {
		Application.Quit();
	}
	#endregion
}