using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;

public class FileBrowserEvents : MonoBehaviour {

	#region Fields
	public UILabel PathLabel;
	public MainMenu MainMenu;
	
	private AndroidJavaClass UnityClass;
	private AndroidJavaObject UnityJavaContext;
	private AndroidJavaObject AndroidMediaAccess;
	
	private List<string> currentArtists;
	private List<string> currentAlbums;
	private List<string> currentSongNames;
	private List<string> currentSongPaths;
	
	private string currentArtist;
	private string currentAlbum;
	//public GameObject FileBrowser;
	#endregion

	#region Functions
	
	public void Awake() {
		if(Application.platform == RuntimePlatform.Android) {
			UnityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			UnityJavaContext = UnityClass.GetStatic<AndroidJavaObject>("currentActivity");
			AndroidMediaAccess = new AndroidJavaClass("com.bitera.rytmos.MediaAccessActivity");
			
			AndroidMediaAccess.CallStatic("initContext",UnityJavaContext);
		}	
	}

	private void OpenFile(string pathToFile) {
		FileInfo fInf = new FileInfo(pathToFile);
		if(fInf.Exists){
			Game.Song = pathToFile;
			MainMenu.ChangeMenu(MenuLevel.ConfirmChoice);
		} else {
			MainMenu.ChangeMenu(MenuLevel.SongNotFound);
		}
	}

	private void FileWindowClosed() {
		MainMenu.ChangeMenu(MenuLevel.Mode);
	}

	private void UpdateDirLabel(string path) {
		PathLabel.text = path;
	}

	private void OnUpClicked() {
		if (!FileBrowserMenu.RecentlyPlayedActive) gameObject.SendMessage("FolderUp");
	}
	
	private void FetchArtists() {
		GetArtistList();
		gameObject.SendMessage("DisplayArtists",currentArtists);
	}
	
	private void FetchAlbumsForArtist(string artist) {
		GetAlbumsForArtist(artist);
		gameObject.SendMessage("DisplayAlbums",currentAlbums);
	}
	
	private void FetchSongs(string[] args) {
		// args[0] = artist, args[1] = album
		GetSongs(args[0],args[1]);
		gameObject.SendMessage("DisplaySongs",new List<string>[] {currentSongNames,currentSongPaths});
	}
	
	
	private void GetArtistList() {
		
		if(currentArtists == null) {
			AndroidMediaAccess.CallStatic("initArtist");
			
			List<string> tempArtists = new List<string>();
			string artist = "";
			
			while((artist = AndroidMediaAccess.CallStatic<string>("fetchMoveArtist")) != "") {
				tempArtists.Add(artist);
			}
			
			currentArtists = tempArtists;
			
			AndroidMediaAccess.CallStatic("closeArtist");
		}
	}
	
	private void GetAlbumsForArtist(string artist) {
		
		if(artist != currentArtist) {
			currentArtist = artist;
			
			AndroidMediaAccess.CallStatic("initAlbum",artist);
			
			string album = "";
			List<string> albums = new List<string>();
			
			while((album = AndroidMediaAccess.CallStatic<string>("fetchMoveAlbum")) != "") {
				albums.Add(album);
			}
			
			AndroidMediaAccess.CallStatic("closeAlbum");
			currentAlbums = albums;
		}
	}
	
	private void GetSongs(string artist, string album) {
		
		if(artist != currentArtist || album != currentAlbum) {
			currentArtist = artist;
			currentAlbum = album;
			
			AndroidMediaAccess.CallStatic("initSong",artist,album);
						
			string[] song = new string[0];
			List<string> songTitles = new List<string>();
			List<string> songFilePaths = new List<string>();
			
			
			while((song = AndroidMediaAccess.CallStatic<string[]>("fetchMoveSong"))[0] != "") {
				songTitles.Add(song[0]);
				songFilePaths.Add(song[1]);
			}
			
			AndroidMediaAccess.CallStatic("closeSongs");
			
			currentSongNames = songTitles;
			currentSongPaths = songFilePaths;
		}
	}

	#endregion
}
