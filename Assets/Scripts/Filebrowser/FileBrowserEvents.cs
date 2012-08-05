using UnityEngine;
using System.Collections;
using System.IO;

public class FileBrowserEvents : MonoBehaviour {

	#region Fields
	public UILabel PathLabel;
	public MainMenu MainMenu;
	//public GameObject FileBrowser;
	#endregion

	#region Functions

	private void OpenFile(string pathToFile) {
		Game.Song = pathToFile;
		Application.LoadLevel("LoadScreen");
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

	#endregion
}
