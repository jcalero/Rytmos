using UnityEngine;
using System.Collections;
using System.IO;

public class FileBrowserEvents : MonoBehaviour {

	#region Fields
	public UILabel PathLabel;
	public MainMenu MainMenu;
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

	#endregion
}
