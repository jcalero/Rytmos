using UnityEngine;
using System.Collections;
using System.IO;

public class FileBrowserMenu : MonoBehaviour {

	#region Fields
	public GameObject FileBrowserActiveTabBG;
	public GameObject RecentlyPlayedActiveTabBG;
	//public UISlicedSprite FBInactiveBackground;
	//public UISlicedSprite RPInactiveBackground;

	private bool recentlyPlayedActive = true;
	private bool fileBrowserActive = false;
	#endregion

	#region Functions

	private void Awake() {
		if (recentlyPlayedActive) {
			//RecentlyPlayedActiveTabSprite.color = Color.white;
			//FileBrowserActiveTabSprite.color = Color.clear;
			//RecentlyPlayedActiveTabSprite.enabled = true;
			//FileBrowserActiveTabSprite.enabled = false;
		} else {
			//RecentlyPlayedActiveTabSprite.color = Color.clear;
			//FileBrowserActiveTabSprite.color = Color.white;
			//FileBrowserActiveTabSprite.enabled = true;
			//RecentlyPlayedActiveTabSprite.enabled = false;
		}
	}

	private void Update() {
		if (recentlyPlayedActive) {
			if (!RecentlyPlayedActiveTabBG.active)
				RecentlyPlayedActiveTabBG.SetActiveRecursively(true);
			if (FileBrowserActiveTabBG.active)
				FileBrowserActiveTabBG.SetActiveRecursively(false);
		} else {
			if (!FileBrowserActiveTabBG.active)
				FileBrowserActiveTabBG.SetActiveRecursively(true);
			if (RecentlyPlayedActiveTabBG.active)
				RecentlyPlayedActiveTabBG.SetActiveRecursively(false);
		}
	}

	private void RecentlyPlayedTabClicked() {
		Debug.Log("Recently played tab clicked");
		if (!recentlyPlayedActive) {
			Debug.Log("Recently played tab activated");
			//RecentlyPlayedActiveTabSprite.color = Color.white;
			//FileBrowserActiveTabSprite.color = Color.clear;
			//RecentlyPlayedActiveTabSprite.enabled = true;
			//FileBrowserActiveTabSprite.enabled = false;
			recentlyPlayedActive = true;
			fileBrowserActive = false;
		}
	}

	private void FileBrowserTabClicked() {
		Debug.Log("Filebrowser tab clicked");
		if (!fileBrowserActive) {
			Debug.Log("Filebrowser tab activated");
			//FileBrowserActiveTabSprite.color = Color.white;
			//RecentlyPlayedActiveTabSprite.color = Color.clear;
			//FileBrowserActiveTabSprite.enabled = true;
			//RecentlyPlayedActiveTabSprite.enabled = false;
			recentlyPlayedActive = false;
			fileBrowserActive = true;
		}
	}

	#endregion
}
