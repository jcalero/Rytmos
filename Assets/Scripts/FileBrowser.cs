using UnityEngine;
using System.Collections;
using System.IO;
using System;

public static class FileBrowser {

    #region Fields

    private static UILabel PathLabel = GameObject.Find("PathLabel").GetComponent<UILabel>();

    private static string currentDirectory = CurrentDirectory;
    private static string newDirectory;
    private static string[] directories;
    private static string[] files;
    private static char separator = Path.DirectorySeparatorChar;

    public static int selectedFileId = -1;

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
        get {
            return Directory.GetDirectories(currentDirectory);
        }
    }

    public static string[] Files {
        get {
            return Directory.GetFiles(currentDirectory, "*.mp3");
        }
    }

    public static string SelectedFile {
        get {
            if (selectedFileId > -1)
                return Files[selectedFileId];
            else
                return null;
        }
    }

    #endregion

    #region Functions

    public static void Initialise() {
        UpdateDirectoryLabel();
    }

    private static void SetNewDirectory(string directory) {
        try {
            Directory.GetFiles(directory);
            newDirectory = directory;
        } catch (UnauthorizedAccessException) {
            newDirectory = currentDirectory;
        }
    }

    private static void SwitchDirectoryNow() {
        currentDirectory = newDirectory;
    }

    private static void UpdateDirectoryLabel() {
        string[] directoryStructure = currentDirectory.Split(separator);
        string directoryPath = separator + directoryStructure[directoryStructure.Length - 1];
        int maxPathLength = 28;
        int remPathLength = maxPathLength - directoryStructure[directoryStructure.Length - 1].Length;
        int numFoldersInPath = 1;

        for (int cnt = directoryStructure.Length-2; cnt > 0; cnt--) {
            //Debug.Log("Current count: " + cnt + " : " + directoryStructure[cnt].Length + " : " + remPathLength);
            if (directoryStructure[cnt].Length < remPathLength) {
                //Debug.Log(cnt + " : " + directoryStructure[cnt]);
                directoryPath = separator + directoryStructure[cnt] + directoryPath;
                remPathLength -= directoryStructure[cnt].Length + 1;
                numFoldersInPath++;
            } else {
                cnt = 0;
            }
        }
        if (numFoldersInPath < directoryStructure.Length-1)
            directoryPath = "..." + directoryPath;
        //Debug.Log(directoryPath.Length);
        if (directoryPath.Length > maxPathLength + 6)
            directoryPath = directoryPath.Substring(0,maxPathLength) + "...";

        PathLabel.text = directoryPath;
    }

    #endregion
}
