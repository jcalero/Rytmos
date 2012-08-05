using UnityEngine;
using System.Collections;
using System.IO;

public class FileBrowserMenu : MonoBehaviour {

	#region Fields
	public GameObject FileBrowserActiveTabBG;
	public GameObject RecentlyPlayedActiveTabBG;
	public GameObject FileBrowser;
	public UILabel PathLabel;
	public UIButton UpButton;
	//public UISlicedSprite FBInactiveBackground;
	//public UISlicedSprite RPInactiveBackground;

	private static bool recentlyPlayedActive = true;
	private static bool fileBrowserActive = false;
	#endregion

	#region Functions

	private void Awake() {
		if (recentlyPlayedActive) {
			PathLabel.text = "";
		} else {
			if (!string.IsNullOrEmpty(Game.Path)) FileBrowser.SendMessage("OpenFileWindow", PlayerPrefs.GetString("filePath"));
			else FileBrowser.SendMessage("OpenFileWindow", "");
		}
	}

	private void Update() {
		if (recentlyPlayedActive) {
			if (!RecentlyPlayedActiveTabBG.active) {
				RecentlyPlayedActiveTabBG.SetActiveRecursively(true);
				UpButton.enabled = false;
			}
			if (FileBrowserActiveTabBG.active) {
				FileBrowserActiveTabBG.SetActiveRecursively(false);
				UpButton.enabled = false;
			}
		} else {
			if (!FileBrowserActiveTabBG.active) {
				FileBrowserActiveTabBG.SetActiveRecursively(true);
				UpButton.enabled = true;
				if (!string.IsNullOrEmpty(Game.Path)) FileBrowser.SendMessage("OpenFileWindow", PlayerPrefs.GetString("filePath"));
				else FileBrowser.SendMessage("OpenFileWindow", "");
			}
			if (RecentlyPlayedActiveTabBG.active) {
				RecentlyPlayedActiveTabBG.SetActiveRecursively(false);
				UpButton.enabled = true;
			}
		}
	}

	private void RecentlyPlayedTabClicked() {
		//Debug.Log("Recently played tab clicked");
		if (!recentlyPlayedActive) {
			//Debug.Log("Recently played tab activated");
			recentlyPlayedActive = true;
			fileBrowserActive = false;
			PathLabel.text = "";
			FileBrowser.SendMessage("CloseFileWindowTab");
		}
	}

	private void FileBrowserTabClicked() {
		//Debug.Log("Filebrowser tab clicked");
		if (!fileBrowserActive) {
			//Debug.Log("Filebrowser tab activated");
			recentlyPlayedActive = false;
			fileBrowserActive = true;
			if (!string.IsNullOrEmpty(Game.Path)) FileBrowser.SendMessage("OpenFileWindow", PlayerPrefs.GetString("filePath"));
			else FileBrowser.SendMessage("OpenFileWindow", "");
		}
	}

	public static bool RecentlyPlayedActive {
		get { return recentlyPlayedActive; }
	}

	#endregion
}
