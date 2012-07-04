using UnityEngine;
using System.Collections;

public class FolderLabel : MonoBehaviour {

    #region Fields
    public int id;
    public MainMenu mainMenu;
    private UIButton selectButton;
    public UIDraggablePanel fileListPanel;
    #endregion

    #region Functions

    void Awake() {
        mainMenu = GameObject.Find("MainMenuManager").GetComponent<MainMenu>();
        fileListPanel = NGUITools.FindInParents<UIDraggablePanel>(gameObject);
        selectButton = GameObject.Find("SelectButton").GetComponent<UIButton>();
    }

    public void OnClicked() {
        FileBrowser.CurrentDirectory = FileBrowser.Directories[id];
        selectButton.isEnabled = false;
        FileBrowser.selectedFileId = -1;
        mainMenu.ReloadFileList();
        fileListPanel.ResetPosition();
        //Debug.Log("Folder id clicked on: " + id);
        //Debug.Log("Current directory: " + FileBrowser.CurrentDirectory);
    }
    #endregion
}
