using UnityEngine;
using System.Collections;
using System.IO;
using System;

public class FileBrowser : MonoBehaviour {

    #region Fields

    private static FileBrowser instance;

    private static string currentDirectory = CurrentDirectory;
    private static string newDirectory;
    private static string[] directories;
    private static string[] files;
    private static string fileExtensions = "*.mp3";
    private static char separator = Path.DirectorySeparatorChar;

    public static int SelectedFileId = -1;
    public static bool Enabled;

    // Inspector instances. Game object: FileBrowser
    public UILabel PathLabel;
    public UIPanel FileBrowserPanel;
    public UIPanel FileListPanel;
    public UIPanel ScrollBar;
    public UIFont FileNameFont;
    public UIButton FileBrowserSelectButton;
    public UILabel SelectModeLabel;
    public UITable FileTable;
    public UITable FolderTable;
    public UIDraggablePanel FileListDraggablePanel;
    public UILabel[] FileLabels;

    private static float labelSpacing = 26;
    private static int locationID = 1;
    private static float maxYPosition;
    //private static float lastYPos;
    private static int firstLabelID;
    private static int lastLabelID;
    private static int numActiveFiles;
    private static int lastNumActiveFiles;
    private static string[] fullFileList;
    private static int maxNumFiles;
    private static int maxNumFolders;
    //private static Vector3 fileBrowserAnchor = new Vector3(-125f, 110f, 0f);
    //private static GameObject lastFolder = null;
    private static string selectedLevel;

    #endregion

    #region Properties

    public static string CurrentDirectory {
        get {
            if (currentDirectory == null) {
                return Directory.GetCurrentDirectory();
            } else {
                return currentDirectory;
            }
        }
        set {
            SetNewDirectory(value);
            SwitchDirectoryNow();
            UpdateDirectoryLabel();
        }
    }

    public static string[] Directories {
        get { return directories; }
    }

    public static string[] Files {
        get { return files; }
    }

    public static string SelectedFile {
        get {
            if (SelectedFileId > -1)
                return Files[SelectedFileId];
            else
                return null;
        }
    }

    #endregion

    #region Functions

    private void Awake() {
        instance = this;
        FileBrowserSelectButton.isEnabled = false;
    }

    private void Update() {
        if (Enabled) {
            RepositionLabels();
        }
        //Debug.Log(FileListPanel.transform.localPosition.y);
    }

    private static void SetNewDirectory(string directory) {
        try {
            Directory.GetFiles(directory, fileExtensions);      // Check if it's possible to access the folder
            newDirectory = directory;
        } catch (UnauthorizedAccessException) {
            newDirectory = currentDirectory;    // If no access to folder, don't change directory
        }
    }

    private static void SwitchDirectoryNow() {
        currentDirectory = newDirectory;
    }

    #region UIstuff
    public static void LoadFileBrowser(string level) {
        // Saves the game mode that is going to be loaded once the song has been selected
        selectedLevel = level;

        // Hides the "Select Game Mode" menu items.
        MainMenu.ToggleModeMenu(false);

        // Shows the File Browser elements
        SetActiveState(instance.FileBrowserPanel, true);
        SetActiveState(instance.ScrollBar, true);
        SetActiveState(instance.FileListPanel, true);

        Enabled = true;                 // Set FileBrowser.Enabled to true.
        UpdateDirectoryLabel();         // Update the current directory label
        ReloadFileList();               // Reload the file list 
    }

    public static void UnloadFileBrowser() {
        // Hides the File Browser elements
        SetActiveState(instance.FileBrowserPanel, false);
        SetActiveState(instance.ScrollBar, false);
        SetActiveState(instance.FileListPanel, false);

        FileBrowser.SelectedFileId = -1;        // Resets the selected file id
        Enabled = false;                        // Set FileBrowser.Enabled to false
        MainMenu.ToggleModeMenu(true);          // Shows the "Select Game Mode" menu items
    }

    /// <summary>
    /// Activate or deactivate the children of the specified transform recursively.
    /// Used for showing/hiding the filebrowser
    /// </summary>
    private static void SetActiveState(Transform t, bool state) {
        for (int i = 0; i < t.childCount; ++i) {
            Transform child = t.GetChild(i);
            //if (child.GetComponent<UIPanel>() != null) continue;

            if (state) {
                child.gameObject.active = true;
                SetActiveState(child, true);
            } else {
                SetActiveState(child, false);
                child.gameObject.active = false;
            }
        }
    }

    /// <summary>
    /// Activate or deactivate the specified panel and all of its children.
    /// Used for showing/hiding the filebrowser
    /// </summary>
    private static void SetActiveState(UIPanel panel, bool state) {
        if (state) {
            panel.gameObject.active = true;
            SetActiveState(panel.transform, true);
        } else {
            SetActiveState(panel.transform, false);
            panel.gameObject.active = false;
        }
    }

    public static void ReloadFileList() {
        int numLabels = instance.FileLabels.Length; // Number of labels for files/folders in the file browser

        instance.FileListDraggablePanel.ResetPosition();

        // Load directories and files from current directory
        directories = Directory.GetDirectories(currentDirectory);
        files = Directory.GetFiles(currentDirectory, fileExtensions);

        // How many folders to display
        int foldersToDisplay = (directories.Length < numLabels) ? directories.Length : numLabels;
        int filesToDisplay = (files.Length < numLabels - foldersToDisplay) ? files.Length : numLabels - foldersToDisplay;
        numActiveFiles = foldersToDisplay + filesToDisplay;

        //Debug.Log(lastNumActiveFiles);
        // There are more files to display than labels, enable the required amount of labels
        if (numActiveFiles >= lastNumActiveFiles) {
            //Debug.Log("ENABLE LABELS");
            for (int cnt = 0; cnt < numActiveFiles; cnt++) {
                if (!instance.FileLabels[cnt].gameObject.active)
                    instance.FileLabels[cnt].gameObject.active = true;
            }
        }

        // Set the label info
        for (int cnt = 0; cnt < foldersToDisplay; cnt++) {      // Folders
            instance.FileLabels[cnt].text = new DirectoryInfo(directories[cnt]).Name;
            instance.FileLabels[cnt].gameObject.GetComponent<FileLabel>().isFolder = true;
            instance.FileLabels[cnt].gameObject.GetComponent<FileLabel>().id = cnt;
        }
        for (int fileCnt = 0; fileCnt < filesToDisplay; fileCnt++) {        // Files
            int labelCnt = fileCnt + directories.Length;
            instance.FileLabels[labelCnt].text = new DirectoryInfo(files[fileCnt]).Name;
            instance.FileLabels[labelCnt].gameObject.GetComponent<FileLabel>().isFolder = false;
            instance.FileLabels[labelCnt].gameObject.GetComponent<FileLabel>().id = fileCnt;
        }

        // There are more labels than files to display, disable extra labels
        if (numActiveFiles < numLabels) {
            for (int cnt = numActiveFiles; cnt < numLabels; cnt++) {
                instance.FileLabels[cnt].gameObject.active = false;
            }
        }

        // Create a list of all files and folders together
        fullFileList = new string[directories.Length + files.Length];
        directories.CopyTo(fullFileList, 0);
        files.CopyTo(fullFileList, directories.Length);

        // Save the array id of the last label for positioning of subsequent labels
        lastLabelID = numActiveFiles - 1;

        lastNumActiveFiles = numActiveFiles;

        // Reset the table contents (to reset any previous rearrangements in other folders)
        instance.FileTable.Reposition();

        Debug.Log("Directories: " + directories.Length);
        Debug.Log("Files: " + files.Length);
        //foreach (string line in fullFileList) {
        //    string line2 = new DirectoryInfo(line).Name;
        //    Debug.Log(line2);
        //}
    }

    private static void RepositionLabels() {
        // Downwards repositioning
        if (fullFileList.Length > numActiveFiles + locationID) {
            //float curYPos = instance.FileListDraggablePanel.transform.localPosition.y;
            //if (curYPos != lastYPos) Debug.Log("CurYPos :" + curYPos + " : LastYPos : " + lastYPos);
            //Debug.Log("In the loop");
            if (instance.FileListDraggablePanel.transform.localPosition.y > 27 * locationID) {
                //Debug.Log("Y position: " + instance.FileListDraggablePanel.transform.localPosition.y + " : " + locationID);
                //Debug.Log("lastLabelID: " + lastLabelID + " :: " + (numActiveFiles - 1));
                //Debug.Log("Scrolling > DOWN < :: " + instance.FileListDraggablePanel.transform.localPosition.y + " :: " + locationID + " :: " + 70 * locationID + " :: " + (locationID - 1) % numActiveFiles);
                instance.FileLabels[(locationID - 1) % numActiveFiles].transform.localPosition = new Vector3(0f,
                                                                             instance.FileLabels[lastLabelID].transform.localPosition.y - labelSpacing,
                                                                             0f);
                instance.FileLabels[(locationID - 1) % numActiveFiles].text = new DirectoryInfo(fullFileList[numActiveFiles + locationID]).Name;

                if (numActiveFiles + locationID <= directories.Length)
                    instance.FileLabels[(locationID - 1) % numActiveFiles].GetComponent<FileLabel>().isFolder = true;
                else
                    instance.FileLabels[(locationID - 1) % numActiveFiles].GetComponent<FileLabel>().isFolder = false;

                if (!instance.FileLabels[(locationID - 1) % numActiveFiles].GetComponent<FileLabel>().isFolder)
                    instance.FileLabels[(locationID - 1) % numActiveFiles].GetComponent<FileLabel>().id = (numActiveFiles + locationID) - directories.Length;
                else
                    instance.FileLabels[(locationID - 1) % numActiveFiles].GetComponent<FileLabel>().id = locationID - 1;

                
                firstLabelID = locationID % numActiveFiles;
                locationID++;
                lastLabelID = (locationID - 2) % numActiveFiles;

                //Debug.Log("FirstLabelID: " + firstLabelID + " : LastLabelID: " + lastLabelID);
                //maxYPosition += instance.FileListDraggablePanel.transform.localPosition.y;
            }
        }
        if (fullFileList.Length > numActiveFiles + locationID - 1 && locationID > 1) {
            if (instance.FileListDraggablePanel.transform.localPosition.y < (27 * (locationID - 1)) && locationID > 1) {
                //Debug.Log(instance.FileListDraggablePanel.transform.localPosition.y);
                //Debug.Log("Scrolling > UP < :: " + instance.FileListDraggablePanel.transform.localPosition.y + " :: " + (locationID - 1) + " :: " + (70 * (locationID - 1)) + " :: " + (locationID - 1) % numActiveFiles + " : " + firstLabelID);
                instance.FileLabels[(locationID - 2) % numActiveFiles].transform.localPosition = new Vector3(0f,
                                                                             instance.FileLabels[firstLabelID].transform.localPosition.y + labelSpacing,
                                                                             0f);
                instance.FileLabels[(locationID - 2) % numActiveFiles].text = new DirectoryInfo(fullFileList[locationID - 2]).Name;

                if (locationID - 2 < directories.Length)
                    instance.FileLabels[(locationID - 2) % numActiveFiles].GetComponent<FileLabel>().isFolder = true;
                else
                    instance.FileLabels[(locationID - 2) % numActiveFiles].GetComponent<FileLabel>().isFolder = false;

                if (!instance.FileLabels[(locationID - 2) % numActiveFiles].GetComponent<FileLabel>().isFolder)
                    instance.FileLabels[(locationID - 2) % numActiveFiles].GetComponent<FileLabel>().id = (locationID - 2) - directories.Length;
                else
                    instance.FileLabels[(locationID - 2) % numActiveFiles].GetComponent<FileLabel>().id = locationID - 2;

                firstLabelID = (firstLabelID - 1) >= 0 ? (firstLabelID - 1) % numActiveFiles : numActiveFiles - 1;
                locationID--;
                lastLabelID = lastLabelID - 1 >= 0 ? lastLabelID - 1 : numActiveFiles - lastLabelID - 1;

                //Debug.Log("FirstLabelID: " + firstLabelID + " : LastLabelID: " + lastLabelID);
                //Debug.Log("LocationID: " + locationID);
            }
        }

        //lastYPos = instance.FileListDraggablePanel.transform.localPosition.y;
    }

    private static void UpdateDirectoryLabel() {
        string[] directoryStructure = currentDirectory.Split(separator);
        string directoryPath = separator + directoryStructure[directoryStructure.Length - 1];
        int maxPathLength = 28;
        int remPathLength = maxPathLength - directoryStructure[directoryStructure.Length - 1].Length;
        int numFoldersInPath = 1;

        for (int cnt = directoryStructure.Length - 2; cnt > 0; cnt--) {
            if (directoryStructure[cnt].Length < remPathLength) {
                directoryPath = separator + directoryStructure[cnt] + directoryPath;
                remPathLength -= directoryStructure[cnt].Length + 1;
                numFoldersInPath++;
            } else {
                cnt = 0; // Terminate for-loop -> stop adding folders to path
            }
        }
        if (numFoldersInPath < directoryStructure.Length - 1)
            directoryPath = "..." + directoryPath;
        if (directoryPath.Length > maxPathLength + 6)
            directoryPath = directoryPath.Substring(0, maxPathLength) + "...";

        instance.PathLabel.text = directoryPath;
    }

    #region Button click managers
    private void OnUpClicked() {
        instance.FileListDraggablePanel.ResetPosition();
        if (Directory.GetParent(FileBrowser.CurrentDirectory) != null)
            FileBrowser.CurrentDirectory = Directory.GetParent(FileBrowser.CurrentDirectory).FullName;
        else
            return;
        SelectedFileId = -1;
        FileBrowserSelectButton.isEnabled = false;
        instance.FileTable.Reposition();
        ReloadFileList();
    }

    private void OnSelectClicked() {
        if (FileBrowser.SelectedFile == null)
            return;
        Game.Song = FileBrowser.SelectedFile;
        Debug.Log("File selected: " + Game.Song);
        Application.LoadLevel(selectedLevel);
    }
    #endregion

    #endregion

    #endregion
}
