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
	
	private AndroidJavaClass UnityClass;
	private AndroidJavaObject UnityJavaContext;
	private AndroidJavaObject AndroidMediaAccess;
	
	private List<string> artists;
	#endregion

	#region Functions

	private void Awake() {
		UnityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		UnityJavaContext = UnityClass.GetStatic<AndroidJavaObject>("currentActivity");
		AndroidMediaAccess = new AndroidJavaClass("com.bitera.rytmos.MediaAccessActivity");
		
		AndroidMediaAccess.CallStatic("initContext",UnityJavaContext);
		
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
		//Debug.Log("Filebrowser tab clicked");
		if (!fileBrowserActive) {
			//Debug.Log("Filebrowser tab activated");
			recentlyPlayedActive = false;
			fileBrowserActive = true;
			FileBrowser.SendMessage("CloseRecentFilesWindowTab");
			if (!string.IsNullOrEmpty(Game.Path)) FileBrowser.SendMessage("OpenFileWindow", PlayerPrefs.GetString("filePath"));
			else FileBrowser.SendMessage("OpenFileWindow", "");
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
	
	private List<string> GetArtistList() {
		
		if(this.artists == null) {
			AndroidMediaAccess.CallStatic("initArtist");
			
			System.Collections.Generic.List<string> tempArtists = new System.Collections.Generic.List<string>();
			string artist = "";
			
			while((artist = AndroidMediaAccess.CallStatic<string>("fetchMoveArtist")) != "") {
				tempArtists.Add(artist);
			}
			
			this.artists = tempArtists;
			
			AndroidMediaAccess.CallStatic("closeArtist");
		}
		
		return this.artists;
	}
	
	private List<string> GetAlbumsForArtist(string artist) {
		
		AndroidMediaAccess.CallStatic("initAlbum",artist);
		
		string album = "";
		System.Collections.Generic.List<string> albums = new System.Collections.Generic.List<string>();
		
		while((album = AndroidMediaAccess.CallStatic<string>("fetchMoveAlbum")) != "") {
			albums.Add(album);
		}
		
		AndroidMediaAccess.CallStatic("closeAlbum");
		
		return albums;
	}
	
	private List<string[]> GetSongsForAlbum(string artist, string album) {
		
		AndroidMediaAccess.CallStatic("initSong",artist,album);
					
		string[] song = new string[0];
		System.Collections.Generic.List<string[]> songs = new System.Collections.Generic.List<string[]>();
		
		while((song = AndroidMediaAccess.CallStatic<string[]>("fetchMoveSong"))[0] != "") {
			songs.Add((string[])song.Clone());
		}
		
		AndroidMediaAccess.CallStatic("closeSong");
		
		return songs;
	}

	public static bool RecentlyPlayedActive {
		get { return recentlyPlayedActive; }
	}



	#endregion
}
