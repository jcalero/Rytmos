using UnityEngine;
using System.Collections;
using System.IO;
/// <summary>
/// MainMenu.cs
/// 
/// Handles the display of the main menu. 
/// </summary>
public class MainMenu : MonoBehaviour {

    #region Fields
    public UIDraggablePanel panel; // Inspector instance. Location: MainMenuManager.
    public UIPanel fileBrowser;
    public UIPanel fileListPanel;
    public UIPanel scrollBar;
    public UIFont fileNameFont;
    public UIButton reloadButton;
    public UIButton nextButton;
    public UIButton prevButton;
    public UIButton ArcadeButton;
    public UIButton SurvivalButton;
    public UIButton StoryButton;
    public UIButton FileBrowserSelectButton;
    public UILabel highscoresTypeLabel;
    public UILabel selectModeLabel;
    public UITable fileTable;
    public UITable folderTable;
    public bool skipMenu;           // Allows you to skip directly from the Main Menu to the Arcade game mode

    // Values for menu slider locations.
    private Vector3 quitMenu;
    private Vector3 mainMenu;
    private Vector3 modeMenu;
    private Vector3 highScoresMenu;
    private Vector3 optionsMenu;

    private bool highscoresLoaded;
    private int maxNumFiles;
    private int maxNumFolders;
    private Vector3 fileBrowserAnchor = new Vector3(-125f, 110f, 0f);
    GameObject lastFolder = null;
    private string selectedLevel;
    private bool fileBrowserEnabled;

    //private ArrayList fileList = new ArrayList();
    //private ArrayList folderList = new ArrayList();

    private bool loadedSurvival;
    private bool loadedTimeAttack = true;
    private string survival = "rytmos_hs_dm";
    private string timeAttack = "rytmos_hs_30sec";

    private static MainMenu instance;
    #endregion

    #region Functions

    void Awake() {
        if (skipMenu) Application.LoadLevel("Game");

        instance = this;
        quitMenu = new Vector3(0f, 0f, 0f);
        mainMenu = new Vector3(-650f, 0f, 0f);
        modeMenu = new Vector3(-650f * 2, 0f, 0f);
        highScoresMenu = new Vector3(-650f * 3, 0f, 0f);
        optionsMenu = new Vector3(-650f * 4, 0f, 0f);

        Game.GameState = Game.State.Menu;
    }

    void Start() {
        // Fetch high-scores from server
        if (!highscoresLoaded) {
            if (loadedSurvival) {
                StartCoroutine(HSController.GetScores(survival, true));
                StartCoroutine(HSController.GetScores(timeAttack, false));
            }
            if (loadedTimeAttack) {
                StartCoroutine(HSController.GetScores(survival, false));
                StartCoroutine(HSController.GetScores(timeAttack, true));
            }
            highscoresLoaded = true;
            FileBrowserSelectButton.isEnabled = false;
            DisableReloadButton();
        }
    }

    void Update() {
        // When the player presses "Escape" or "Back" on Android, returns to main menu screen.
        if (Input.GetKey(KeyCode.Escape))
            OnBackClicked();
    }

    /// <summary>
    /// Button handler for "Play" button
    /// </summary>
    void OnPlayClicked() {
        // Stop any current momentum to allow for Spring to begin.
        panel.currentMomentum = Vector3.zero;
        // Begin spring motion
        SpringPanel.Begin(panel.gameObject, modeMenu, 13f);
    }

    /// <summary>
    /// Button handler for "Highscores" button
    /// </summary>
    void OnHighScoresClicked() {
        // Stop any current momentum to allow for Spring to begin.
        panel.currentMomentum = Vector3.zero;
        // Begin spring motion
        SpringPanel.Begin(panel.gameObject, highScoresMenu, 13f);
    }

    /// <summary>
    /// Button handler for "Options" button
    /// </summary>
    void OnOptionsClicked() {
        // Stop any current momentum to allow for Spring to begin.
        panel.currentMomentum = Vector3.zero;
        // Begin spring motion
        SpringPanel.Begin(panel.gameObject, optionsMenu, 13f);
    }

    /// <summary>
    /// Button handler for "Quit Game" button
    /// </summary>
    void OnQuitClicked() {
        // Stop any current momentum to allow for Spring to begin.
        panel.currentMomentum = new Vector3(0f, 0f, 0f);
        // Begin spring motion
        SpringPanel.Begin(panel.gameObject, quitMenu, 13f);
    }

    /// <summary>
    /// Button handler or "Back" button
    /// </summary>
    void OnBackClicked() {
        if (fileBrowserEnabled) {
            unLoadFileBrowser();
            return;
        }
        // Stop any current momentum to allow for Spring to begin.
        panel.currentMomentum = Vector3.zero;
        // Begin spring motion
        SpringPanel.Begin(panel.gameObject, mainMenu, 13f);
    }

    /// <summary>
    /// Button handler for "Yes, Quit Game" button
    /// </summary>
    void OnQuitConfirmedClicked() {
        Application.Quit();
    }

    /// <summary>
    /// Button handler for "Arcade" button
    /// </summary>
    void OnArcadeModeClicked() {
        LoadFileBrowser("Game");
        //Application.LoadLevel("Game");
    }


    void OnChallengeModeClicked() {
        LoadFileBrowser("DeathMatch");
        //Application.LoadLevel("DeathMatch");
    }

    void OnNextHighscoreClicked() {
        if (loadedSurvival) {
            highscoresTypeLabel.text = "Time Attack";
            loadedTimeAttack = true;
            loadedSurvival = false;
            HSController.ShowScores(timeAttack);
        } else {
            highscoresTypeLabel.text = "Survival";
            loadedSurvival = true;
            loadedTimeAttack = false;
            HSController.ShowScores(survival);
        }
    }

    void OnPrevHighscoreClicked() {
        if (loadedSurvival) {
            highscoresTypeLabel.text = "Time Attack";
            loadedTimeAttack = true;
            loadedSurvival = false;
            HSController.ShowScores(timeAttack);
        } else {
            highscoresTypeLabel.text = "Survival";
            loadedSurvival = true;
            loadedTimeAttack = false;
            HSController.ShowScores(survival);
        }
    }

    void OnReloadClicked() {
        DisableReloadButton();
        if (loadedSurvival)
            StartCoroutine(HSController.GetScores(survival, true));
        if (loadedTimeAttack)
            StartCoroutine(HSController.GetScores(timeAttack, true));
    }

    void LoadFileBrowser(string level) {
        selectedLevel = level;
        ArcadeButton.isEnabled = false;
        ArcadeButton.transform.GetChild(0).gameObject.active = false;   // Hide Background
        ArcadeButton.transform.GetChild(1).gameObject.active = false;   // Hide Label
        SurvivalButton.isEnabled = false;
        SurvivalButton.transform.GetChild(0).gameObject.active = false;   // Hide Background
        SurvivalButton.transform.GetChild(1).gameObject.active = false;   // Hide Label
        StoryButton.isEnabled = false;
        StoryButton.transform.GetChild(0).gameObject.active = false;   // Hide Background
        StoryButton.transform.GetChild(1).gameObject.active = false;   // Hide Label
        selectModeLabel.text = "Select song";
        selectModeLabel.transform.localPosition = new Vector3(selectModeLabel.transform.localPosition.x,
                                                              selectModeLabel.transform.localPosition.y + 25,
                                                              selectModeLabel.transform.localPosition.z);
        selectModeLabel.transform.localScale = new Vector3(30, 30, 30);

        SetActiveState(fileBrowser, true);
        SetActiveState(scrollBar, true);
        SetActiveState(fileListPanel, true);

        fileBrowserEnabled = true;

        FileBrowser.Initialise();
        
        ReloadFileList();
    }

    void unLoadFileBrowser() {
        SetActiveState(fileBrowser, false);
        SetActiveState(scrollBar, false);
        SetActiveState(fileListPanel, false);
        FileBrowser.selectedFileId = -1;
        fileBrowserEnabled = false;
        RestoreModeMenu();
    }

    /// <summary>
    /// Activate or deactivate the children of the specified transform recursively.
    /// </summary>

    void SetActiveState(Transform t, bool state) {
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
    /// </summary>

    void SetActiveState(UIPanel panel, bool state) {
        if (state) {
            panel.gameObject.active = true;
            SetActiveState(panel.transform, true);
        } else {
            SetActiveState(panel.transform, false);
            panel.gameObject.active = false;
        }
    }



    public void ReloadFileList() {
        string[] directories = FileBrowser.Directories;
        string[] files = FileBrowser.Files;
        char separator = Path.DirectorySeparatorChar;
        int numFolders = directories.Length;
        int numFiles = files.Length;

        folderTable.transform.localPosition = fileBrowserAnchor;

        //Debug.Log("Current Directory: " + FileBrowser.CurrentDirectory);
        //Debug.Log("Folders: " + numFolders + " (" + maxNumFolders + ")");
        //Debug.Log("Files: " + numFiles + " (" + maxNumFiles + ")");

        // If there are more folders than we have labels we need to add more
        if (maxNumFolders < numFolders) {
            // Generate new folder labels
            for (int cnt = maxNumFolders; cnt < numFolders; cnt++) {
                // Set up the label
                UILabel newLabel = NGUITools.AddWidget<UILabel>(folderTable.gameObject);
                newLabel.font = fileNameFont;
                newLabel.text = new DirectoryInfo(directories[cnt]).Name + separator;
                newLabel.color = Color.yellow;
                newLabel.pivot = UIWidget.Pivot.Left;
                newLabel.MakePixelPerfect();
                BoxCollider col = newLabel.gameObject.AddComponent<BoxCollider>();
                col.size = new Vector3(22f, 1.6f, 0f);
                col.center = new Vector3(col.size.x / 2, 0f, 0f);
                newLabel.gameObject.AddComponent<UIDragPanelContents>();

                // Set up the click handler
                FolderLabel folderLabel = newLabel.gameObject.AddComponent<FolderLabel>();
                folderLabel.id = cnt;

                // Set up the click handler
                UIButtonMessage buttonMessage = newLabel.gameObject.AddComponent<UIButtonMessage>();
                buttonMessage.functionName = "OnClicked";

                // Save last folder for location reference to position the files below it
                lastFolder = newLabel.gameObject;
            }
            // Update maxNumFolders
            maxNumFolders = numFolders;
        }

        // If there are more files than we have labels we need to add more
        if (maxNumFiles < numFiles) {
            // Generate files
            for (int cnt = maxNumFiles; cnt < numFiles; cnt++) {
                // Set up the label
                UILabel newLabel = NGUITools.AddWidget<UILabel>(fileTable.gameObject);
                newLabel.font = fileNameFont;
                newLabel.text = Path.GetFileName(files[cnt]);
                newLabel.pivot = UIWidget.Pivot.Left;
                newLabel.MakePixelPerfect();
                BoxCollider col = newLabel.gameObject.AddComponent<BoxCollider>();
                col.size = new Vector3(22f, 1.6f, 0f);
                col.center = new Vector3(col.size.x / 2, 0f, 0f);
                newLabel.gameObject.AddComponent<UIDragPanelContents>();

                // Pass messages to label script and filebrowser
                FileLabel fileLabel = newLabel.gameObject.AddComponent<FileLabel>();
                fileLabel.id = cnt;

                // Set up the click handler
                UIButtonMessage buttonMessage = newLabel.gameObject.AddComponent<UIButtonMessage>();
                buttonMessage.functionName = "OnClicked";
            }
            // Update maxNumFiles
            maxNumFiles = numFiles;
        }

        if (maxNumFolders > numFolders) {
            for (int cnt = numFolders; cnt < maxNumFolders; cnt++) {
                GameObject tempLabel = folderTable.transform.GetChild(cnt).gameObject;
                tempLabel.gameObject.active = false;
            }
        }

        if (maxNumFiles > numFiles) {
            for (int cnt = numFiles; cnt < maxNumFiles; cnt++) {
                GameObject tempLabel = fileTable.transform.GetChild(cnt).gameObject;
                tempLabel.gameObject.active = false;
            }
        }

        // Reload folders on old labels
        for (int cnt = 0; cnt < numFolders; cnt++) {
            // Re-enable old labels if needed
            GameObject newLabelObject = folderTable.transform.GetChild(cnt).gameObject;
            if (!newLabelObject.gameObject.active)
                newLabelObject.gameObject.active = true;

            // Set up the label
            UILabel newLabel = folderTable.GetComponentsInChildren<UILabel>()[cnt];
            newLabel.text = new DirectoryInfo(directories[cnt]).Name + separator;

            // Assign label id
            FolderLabel folderLabel = newLabel.gameObject.GetComponent<FolderLabel>();
            folderLabel.id = cnt;

            // Save last folder for location reference to position the files below it
            if (numFolders > 0)
                lastFolder = folderTable.transform.GetChild(numFolders - 1).gameObject;
            else
                lastFolder = folderTable.transform.GetChild(numFolders).gameObject;
        }

        // Reload filenames on old labels
        for (int cnt = 0; cnt < numFiles; cnt++) {
            // Re-enable old labels if needed
            GameObject newLabelObject = fileTable.transform.GetChild(cnt).gameObject;
            if (!newLabelObject.gameObject.active)
                newLabelObject.gameObject.active = true;

            // Set up the label
            UILabel newLabel = fileTable.GetComponentsInChildren<UILabel>()[cnt];
            if (!newLabel.gameObject.active)
                newLabel.gameObject.active = true;
            newLabel.text = Path.GetFileName(files[cnt]);

            // Assign label id
            FileLabel fileLabel = newLabel.gameObject.GetComponent<FileLabel>();
            fileLabel.id = cnt;
        }

        folderTable.Reposition();
        if (numFolders > 0) {
            fileTable.transform.position = new Vector3(lastFolder.transform.position.x,
                                                       lastFolder.transform.position.y - 0.04f,
                                                       fileTable.transform.position.z);
        } else {
            fileTable.transform.localPosition = fileBrowserAnchor;
        }

        fileTable.Reposition();
    }

    private void OnFileBrowserUpClicked() {
        if (Directory.GetParent(FileBrowser.CurrentDirectory) != null)
            FileBrowser.CurrentDirectory = Directory.GetParent(FileBrowser.CurrentDirectory).FullName;
        else
            return;
        fileListPanel.GetComponent<UIDraggablePanel>().ResetPosition();
        FileBrowser.selectedFileId = -1;
        FileBrowserSelectButton.isEnabled = false;
        ReloadFileList();
    }

    private void OnFileBrowserSelectClicked() {
        if (FileBrowser.SelectedFile == null)
            return;
        Game.Song = FileBrowser.SelectedFile;
        Debug.Log("File selected: " + Game.Song);
        Application.LoadLevel(selectedLevel);
    }

    private void RestoreModeMenu() {
        ArcadeButton.isEnabled = true;
        ArcadeButton.transform.GetChild(0).gameObject.active = true;   // Hide Background
        ArcadeButton.transform.GetChild(1).gameObject.active = true;   // Hide Label
        SurvivalButton.isEnabled = true;
        SurvivalButton.transform.GetChild(0).gameObject.active = true;   // Hide Background
        SurvivalButton.transform.GetChild(1).gameObject.active = true;   // Hide Label
        StoryButton.isEnabled = true;
        StoryButton.transform.GetChild(0).gameObject.active = true;   // Hide Background
        StoryButton.transform.GetChild(1).gameObject.active = true;   // Hide Label
    }

    public static void DisableReloadButton() {
        instance.reloadButton.isEnabled = false;
        instance.reloadButton.transform.GetChild(0).gameObject.active = false;
        instance.reloadButton.transform.GetChild(1).gameObject.active = false;
        instance.nextButton.isEnabled = false;
        instance.prevButton.isEnabled = false;
    }

    public static void EnableReloadButton() {
        instance.reloadButton.isEnabled = true;
        instance.reloadButton.transform.GetChild(0).gameObject.active = true;
        instance.reloadButton.transform.GetChild(1).gameObject.active = true;
        instance.nextButton.isEnabled = true;
        instance.prevButton.isEnabled = true;
    }
    #endregion
}
