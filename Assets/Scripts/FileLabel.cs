using UnityEngine;
using System.Collections;

public class FileLabel : MonoBehaviour {

    #region Fields

    // Inspector instances. Location: File Label (UI Root (2D) -> File Browser (Clipped Panel))
    public UIButton selectButton;
    public UIDraggablePanel fileListPanel;

    public bool isFolder;

    private UILabel label;
    private Color oldColor;
    private bool isActive;
    
    public int id;
    
    #endregion

    #region Functions

    private void Awake() {
        label = gameObject.GetComponent<UILabel>();
        oldColor = label.color;
    }

    private void Update() {
        if (isFolder) {
            label.color = Color.yellow;
        }
        if (!isFolder) {
            if (FileBrowser.SelectedFileId == id) {
                label.color = oldColor;
                label.color = Color.red;
                isActive = true;
            } else {
                label.color = oldColor;
                isActive = false;
            }
        }
    }

    public void OnClicked() {
        if (isFolder) {
            fileListPanel.ResetPosition();
            FileBrowser.CurrentDirectory = FileBrowser.Directories[id];
            selectButton.isEnabled = false;
            FileBrowser.SelectedFileId = -1;
            FileBrowser.ReloadFileList();
            Debug.Log("Folder id clicked on: " + id);
           // Debug.Log("Folder clicked on: " + FileBrowser.Directories[id]);
            //Debug.Log("Current directory: " + FileBrowser.CurrentDirectory);
        } else {
            if (!isActive) {
                FileBrowser.SelectedFileId = id;
                selectButton.isEnabled = true;
                Debug.Log("File id clicked on: " + id);
                if (FileBrowser.Files.Length > 0) {
                    Debug.Log("File with id " + id + ": " + FileBrowser.Files[id]);
                }
            } else {
                FileBrowser.SelectedFileId = -1;
                selectButton.isEnabled = false;
            }
        }
    }
    #endregion
}
