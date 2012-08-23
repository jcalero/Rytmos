using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System;

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
		SendRecentSongList();
		if (recentlyPlayedActive) {
			PathLabel.text = "";
		} else {
			
			//if (!string.IsNullOrEmpty(Game.Path)) FileBrowser.SendMessage("OpenFileWindow", PlayerPrefs.GetString("filePath"));
			//else FileBrowser.SendMessage("OpenFileWindow", "");
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
				if(Application.platform == RuntimePlatform.Android) FileBrowser.SendMessage("FetchArtists");
				FileBrowserActiveTabBG.SetActiveRecursively(true);
				UpButton.enabled = true;
				//if (!string.IsNullOrEmpty(Game.Path)) FileBrowser.SendMessage("OpenFileWindow", PlayerPrefs.GetString("filePath"));
				//else FileBrowser.SendMessage("OpenFileWindow", "");
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
			FileBrowser.SendMessage("OpenRecentFilesWindow");
		}
	}

	private void FileBrowserTabClicked() {
//		Debug.Log("Filebrowser tab clicked");
		if (!fileBrowserActive) {
		
			if(Application.platform == RuntimePlatform.WindowsEditor) {

				//Debug.Log("Filebrowser tab activated");
				recentlyPlayedActive = false;
				fileBrowserActive = true;
				FileBrowser.SendMessage("CloseRecentFilesWindowTab");
				if (!string.IsNullOrEmpty(Game.Path)) FileBrowser.SendMessage("OpenFileWindow", PlayerPrefs.GetString("filePath"));
				else FileBrowser.SendMessage("OpenFileWindow", "");
				
			}
			else if(Application.platform == RuntimePlatform.Android) {
				
//				FileBrowser.SendMessage("CloseRecentFilesWindowTab");
				recentlyPlayedActive = false;
				fileBrowserActive = true;
				FileBrowser.SendMessage("FetchArtists");
			
			}
		}
	}

	private void SendRecentSongList() {
		string file = "";

		// Set the file path to save the song info to
		if (Application.platform == RuntimePlatform.Android
			|| Application.platform == RuntimePlatform.OSXEditor
			|| Application.platform == RuntimePlatform.IPhonePlayer)
			file = (Application.persistentDataPath + "/recentlist.r");
		else if (Application.platform == RuntimePlatform.WindowsPlayer
			|| Application.platform == RuntimePlatform.WindowsEditor)
			file = (Application.persistentDataPath + "\\recentlist.r");
		else {
			Debug.Log("PLATFORM NOT SUPPORTED YET");
			file = "";
		}

		List<string> displayName = new List<string>();
		List<FileInfo> songPath = new List<FileInfo>();

		// Gather rows from the file
		List<string> fileRows = new List<string>();
		string line;
		try {
			using (StreamReader sr = new StreamReader(file)) {
				while ((line = sr.ReadLine()) != null) {
					fileRows.Add(line);
				}
				sr.Close();
			}
			// Create the new lists that are to be sent to the file browser

			for (int i = 0; i < fileRows.Count; i++) {
				string song = fileRows[i].Split('|')[0] + " - " + fileRows[i].Split('|')[1];
				FileInfo fInf = new FileInfo(fileRows[i].Split('|')[2]);
				
				if(fInf.Exists) {
					if (song == "Unknown - Unknown") displayName.Add(fileRows[i].Split('|')[0] + " - " + fileRows[i].Split('|')[1] + " (" + new FileInfo(fileRows[i].Split('|')[2]).Name + ")");
					else displayName.Add(fileRows[i].Split('|')[0] + " - " + fileRows[i].Split('|')[1]);
					songPath.Add(fInf);
				}
			}
		} catch (Exception e) {
			Debug.LogWarning(e.Message + " But don't worry, file will be created once at least one song has been started.");
		}

		FileBrowser.SendMessage("FetchRecentFilesNames", displayName);
		FileBrowser.SendMessage("FetchRecentFilesInfos", songPath);

	}
	

	public static bool RecentlyPlayedActive {
		get { return recentlyPlayedActive; }
	}



	#endregion
}
