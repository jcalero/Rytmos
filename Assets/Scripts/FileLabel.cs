using UnityEngine;
using System.Collections;

public class FileLabel : MonoBehaviour {

    #region Fields

    private UILabel label;
    private bool isActive;
    private UIButton selectButton;
    Color oldColor;

    public int id;
    #endregion

    #region Functions

    void Awake() {
        label = gameObject.GetComponent<UILabel>();
        selectButton = GameObject.Find("SelectButton").GetComponent<UIButton>();
        oldColor = label.color;
    }

    void Update() {
        if (FileBrowser.selectedFileId == id) {
            label.color = oldColor;
            label.color = Color.red;
            isActive = true;
        } else {
            label.color = oldColor;
            isActive = false;
        }
    }

    public void OnClicked() {
        if (!isActive) {
            FileBrowser.selectedFileId = id;
            selectButton.isEnabled = true;
            //Debug.Log("File id clicked on: " + id);
            //if (FileBrowser.Files.Length > 0) {
            //    Debug.Log("File with id " + id + ": " + FileBrowser.Files[id]);
            //}
        } else {
            FileBrowser.selectedFileId = -1;
            selectButton.isEnabled = false;
        }
    }
    #endregion
}
