using UnityEngine;
using System.Collections;
using System.IO;

public class FileBrowserEvents : MonoBehaviour {

    #region Fields
    public UILabel PathLabel;
    #endregion

    #region Functions
    
    private void OpenFile(string pathToFile) {
        Game.Song = pathToFile;
        Application.LoadLevel("Game");
    }

    private void FileWindowClosed() {
        MainMenu.ToggleFileBrowserPanel(false);
        MainMenu.ToggleModeMenu(true);
        MainMenu.ToggleBackModeButton(true);
    }

    private void UpdateDirLabel(string path) {
        PathLabel.text = path;
    }

    #endregion
}
