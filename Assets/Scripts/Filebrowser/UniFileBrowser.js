// UniFileBrowser 1.5
// © 2012 Starscene Software. All rights reserved. Redistribution without permission not allowed.

#pragma strict
#if !UNITY_WEBPLAYER
import System.IO;
import UnityEngine.GUILayout;
import System.Collections.Generic;

var filterFiles = true;			// Filter file names by the items in the filterFileExtensions array
var filterFileExtensions : String[];// When filterFiles is true, show only the files with these extensions
var autoAddExtension = false;		// When saving, automatically add this extension to file names
var addedExtension : String;		// Extension to use if automatically adding when saving
var useFilterButton = false;		// Have a button which allows filtering to be turned on and off
var useDeleteButton = false;		// Have a button which allows the deletion of files from within the filebrowser
var limitToInitialFolder = false;	// Navigating away from the default path is not allowed and no other folders are displayed
var showVolumes = false;			// If true, show all volumes on OS X or drive letters on Windows at the top of the folder list
var allowAppBundleBrowsing = false;	// Allow browsing .app bundles on OS X as folders (which is what they actually are)
var showHiddenOSXFiles = true;		// Shows files and folders starting with "." on OS X
var fileWindowID = 50;				// The window ID for the file requester window
var messageWindowID = 51;			// The window ID for the dialog message window

var defaultFileWindowRect = Rect(400, 150, 500, 600);
var minWindowWidth : int = 435;
var minWindowHeight : int = 300;
var messageWindowSize = Vector2(400, 160);	// Width and height of message window
var popupRect = Rect(25, 53, 300, 35);
var buttonPosition = Vector2(190, 58);	// Position of OK and Cancel buttons
var buttonSize = Vector2(120, 50);	// Size of OK and Cancel buttons
var guiDepth = -1;
var guiSkin : GUISkin;
var windowTab : Texture;
var driveIcon : Texture;
var folderIcon : Texture;
var fileIcon : Texture;

enum FileType {Open, Save, Folder}
enum MenuLevels {Letters,Artist,Album,Song}

private var scrollPos : Vector2;
private var selectedFileNumber = -1;
private var oldSelectedFileNumber = -1;
private var dirList : List.<String>;
private var fileList : List.<String>;
private var fileDisplayList : GUIContent[];

// Number of pixels used that are NOT in the actual file list window (i.e. space for buttons & stuff).  Smaller number = taller file list window
private var windowControlsSpace : int = 205;

private var scrollViewStyle : GUIStyle;
private var scrollViewStyleActive : GUIStyle;
private var popupListStyle : GUIStyle;
private var popupButtonStyle : GUIStyle;
private var popupBoxStyle : GUIStyle;
private var messageWindowStyle : GUIStyle;

private var filePath : String;
private var fileName = "";
private var frameDone = true;
private var pathList : GUIContent[];
private var showPopup = false;
private var selectedPath = -1;
private var pathChar = "/"[0];
private var windowsSystem = false;
private var numberOfVolumes : int;

private var isRecentFiles = false;
private var fileWindowOpen = false;
private var fileWindowRect : Rect;
private var windowTitle : String;
private var fileWindowTitles = ["Open file", "Save file", "Select folder"];
private var selectButtonText = ["Open", "Save", "Open"];
private var fileType = FileType.Open;
private var handleClicked = false;
private var clickedPosition : Vector3;
private var originalWindowRect : Rect;
private var cmdKey1 : int;
private var cmdKey2 : int;
private var mousePos : Vector3;
private var linePixelHeight : int;
private var selectFileInProgress = false;
private var arrowKeysDown = false;
private var showFiles = true;
private var objectToSendTo : GameObject;
private var resetPath = false;

private var recentFilesDisplayNames : List.<String>;
private var recentFileInfos : List.<FileInfo>;
private var recentFileSelectedFile : int;

private var lettersDisplayNames : List.<String>;
private var artistsDisplayNames : List.<String>;
private var albumsDisplayNames : List.<String>;
private var songsDisplayNames : List.<String>;
private var songsPaths : List.<String>;

private var currentMenuLevel : MenuLevels;
private var currentLetter : String;
private var currentArtist : String;
private var currentAlbum : String;

function Awake () {
    if (!guiSkin) {
        Debug.LogError("GUI skin missing");
        enabled = false;
        return;
    }
    
    if(Application.platform == RuntimePlatform.Android) {
    	currentMenuLevel = MenuLevels.Letters;
    	currentLetter = "";
    	currentArtist = "";
    	currentAlbum = "";
    }

    AdjustForScreenAspect();

    SetDefaultPath();
        
    // Set up file window position
    fileWindowRect = defaultFileWindowRect;
    fileWindowRect.width = Mathf.Clamp(fileWindowRect.width, minWindowWidth, 1600);	// The file browser window really doesn't need to be huge
    fileWindowRect.height = Mathf.Clamp(fileWindowRect.height, minWindowHeight, 1200);
    //fileWindowRect.x = Mathf.Min(fileWindowRect.x, Screen.width - fileWindowRect.width);	// In case small resolutions make it go partially off screen
    // Set up message window position to be in the middle of the screen
    messageWindowRect = Rect(Screen.width/2-messageWindowSize.x/2, Screen.height/2-messageWindowSize.y/2, messageWindowSize.x, messageWindowSize.y);
    
    // Styles are packaged in the GUI skin
    scrollViewStyle = guiSkin.GetStyle("listScrollview");
    scrollViewStyleActive = guiSkin.GetStyle("listScrollviewActive");
    popupListStyle = guiSkin.GetStyle("popupList");
    popupButtonStyle = guiSkin.GetStyle("popupButton");
    popupBoxStyle = guiSkin.GetStyle("popupBox");
    messageWindowStyle = guiSkin.GetStyle("messageWindow");

    // Add "." to file extensions if not already there
    for (extension in filterFileExtensions) {
        if (!extension.StartsWith(".")) {
            extension = "." + extension;
        }
    }
    if (autoAddExtension && !addedExtension.StartsWith(".")) {
        addedExtension = "." + addedExtension;
    }
    
    linePixelHeight = scrollViewStyle.CalcHeight(GUIContent(" ", folderIcon), 1.0);
    enabled = false;
}

function AdjustForScreenAspect () { 
    var cam = Camera.mainCamera;
    var aspect = Camera.mainCamera.aspect;
    // Set up the file browser for different resolutions
    // 320x240
	if (Screen.width == 320.0f && Screen.height == 240.0f) {
//		Debug.Log("Setting up file browser for resolution: 320x240");
        defaultFileWindowRect = Rect(-26, -62, 366, 373);
	}
    // 400x240
	else if (Screen.width == 400.0f && Screen.height == 240.0f) {
//		Debug.Log("Setting up file browser for resolution: 400x240");
        defaultFileWindowRect = Rect(-26, -62, 447, 373);
	}
    // 432x240
	else if (Screen.width == 432.0f && Screen.height == 240.0f) {
//		Debug.Log("Setting up file browser for resolution: 432x240");
        defaultFileWindowRect = Rect(-10, -62, 447, 373);
	}
	// 480x320
	else if (Screen.width == 480.0f && Screen.height == 320.0f) {
//		Debug.Log("Setting up file browser for resolution: 480x320");
        defaultFileWindowRect = Rect(-26, -37, 526, 427);
	}
    // 640x480
	else if (Screen.width == 640.0f && Screen.height == 480.0f) {
//		Debug.Log("Setting up file browser for resolution: 640x480");
        defaultFileWindowRect = Rect(-24, 15, 682, 535);
	}
    // 800x480
	else if (Screen.width == 800.0f && Screen.height == 480.0f) {
		// Default, do nothing.
	}
    // 854x480
	else if (Screen.width == 854.0f && Screen.height == 480.0f) {
//        Debug.Log("Setting up file browser for resolution: 854x480");
		defaultFileWindowRect = Rect(3, 15, 842, 535);
	}
	// 960x640
	else if (Screen.width == 960.0f && Screen.height == 640.0f) {
//		Debug.Log("Setting up file browser for resolution: 960x640");
        defaultFileWindowRect = Rect(-22, 66, 997, 640);
	}
    // 1024x600
	else if (Screen.width == 1024.0f && Screen.height == 600.0f) {
//		Debug.Log("Setting up file browser for resolution: 1024x600");
		defaultFileWindowRect = Rect(-8, 53, 1034, 615);
	}
    // 1024x768
    else if (Screen.width == 1024.0f && Screen.height == 768.0f) {
//		Debug.Log("Setting up file browser for resolution: 1024x768");
        defaultFileWindowRect = Rect(-20, 107, 1057, 725);
	}
    // 1280x768
    else if (Screen.width == 1280.0f && Screen.height == 768.0f) {
//		Debug.Log("Setting up file browser for resolution: 1280x768");
        defaultFileWindowRect = Rect(-20, 107, 1312, 725);
	}
	// 1280x720
    else if (Screen.width == 1280.0f && Screen.height == 720.0f) {
//		Debug.Log("Setting up file browser for resolution: 1280x720");
        defaultFileWindowRect = Rect(19, 92, 1238, 693);
	}
	// 1280x800
    else if (Screen.width == 1280.0f && Screen.height == 800.0f) {
//		Debug.Log("Setting up file browser for resolution: 1280x720");
        defaultFileWindowRect = Rect(-14, 117, 1303, 748);
	}
    else {
        Debug.LogError("Resolution not supported! (" + Screen.width + "x" + Screen.height + ") File browser will not work properly.");
    }
}

// Internal variables for managing touches and drags
private var selected :int = -1;
private var scrollVelocity :float = 0f;
private var timeTouchPhaseEnded = 0f;
private var previousDelta :float = 0f;
private var inertiaDuration :float = 0.7f;
private var minSlideTime :float = 2.0f;
private var tapTimerStart : float = 0.1f;
private var tapTimer : float = 0.0f;

private var selectionAreaRect : Rect;

function Update () {
//    if (Input.touchCount == 1) Debug.Log("One touch"); 
//		Debug.Log("Input.touchCount: " + Input.touchCount + "; Scroll velocity: " + scrollVelocity);
	if (Input.touchCount != 1) //if this is a short touch
	{
		selected = -1;
//        Debug.Log("Scroll velocity when called: " + scrollVelocity);
		if ( scrollVelocity != 0.0f )
		{
			// slow down over time
			var t : float;
			t = Time.time;
			t = t - timeTouchPhaseEnded;
			t = t / inertiaDuration;

            // Start slowing down after minSlideTime seconds
//            if (t >= minSlideTime) scrollVelocity -= Time.deltaTime * scrollVelocity;
            scrollVelocity -= Time.deltaTime * scrollVelocity;

//			var frameVelocity : float = Mathf.Lerp(scrollVelocity, 0, t);
			scrollPos.y += scrollVelocity * Time.deltaTime;

            // Make sure it stops at some point (when slow enough).
            if (Mathf.Abs(scrollVelocity) < 100) scrollVelocity = 0.0f;

			// after N seconds, we’ve stopped
			//if (t >= inertiaDuration) scrollVelocity = 0.0f;
		}
		return;
	}

    // A touch was initiated, count down the timer.
    if (selected > -1) {
//            Debug.Log("Tap timer: " + tapTimer);
            tapTimer -= Time.deltaTime; // Count down the time you're dragging
    }
    
    var touch : Touch = Input.touches[0];
    if (touch.phase == TouchPhase.Began)
	{
//		selected = TouchToRowIndex(touch.position);
        selected = selectedFileNumber;
        tapTimer = tapTimerStart;
		previousDelta = 0.0f;
		scrollVelocity = 0.0f;
	}
	else if (touch.phase == TouchPhase.Canceled)
	{
		selected = -1;
		previousDelta = 0f;
	}
	else if (touch.phase == TouchPhase.Moved)
	{
		// dragging
		if (tapTimer < 0) selected = -1;    // If dragging for longer than tapTimer, count as a "real" drag.
		previousDelta = touch.deltaPosition.y;
		scrollPos.y += touch.deltaPosition.y * 4;
	}
	else if (touch.phase == TouchPhase.Ended)
	{
//        Debug.Log("touch phase ended");
        tapTimer = tapTimerStart; // Reset tap timer

		// Was it a tap, or a drag-release?
		if ( selected > -1 && selectionAreaRect.Contains(mousePos) && Mathf.Abs(previousDelta / touch.deltaTime) < 200.0f)
		{
//			Debug.Log("Player selected row " + selected);
            SelectFile();
		}
		else
		{
			// impart momentum, using last delta as the starting velocity
			// ignore delta = 10)
//			scrollVelocity = touch.deltaPosition.y / touch.deltaTime;
            scrollVelocity = previousDelta / touch.deltaTime;
//            Debug.Log("scrollVelocity: " + scrollVelocity);
//            Debug.Log("scrollVelocity when set: " + scrollVelocity + " (" + touch.deltaPosition.y + "/" + touch.deltaTime + ")" + " || Previous delta: " + previousDelta);
			timeTouchPhaseEnded = Time.time;
		}
	}
}

function SetDefaultPath () {
    if (!resetPath) filePath = PlayerPrefs.GetString("filePath");
    else filePath = "";
    resetPath = false;
    switch (Application.platform) {
        case RuntimePlatform.OSXEditor:
            if (filePath.Length < 1) { filePath = Application.dataPath; };
            else filePath = filePath.Substring(0, filePath.LastIndexOf(pathChar)) + pathChar;
            cmdKey1 = KeyCode.LeftApple; cmdKey2 = KeyCode.RightApple;
            break;
        case RuntimePlatform.WindowsEditor:
            if (filePath.Length < 1) { filePath = Application.dataPath; };
            pathChar = "\\"[0];	// A forward slash should work, but one user had some problems and this seemed part of the solution
            filePath = filePath.Replace("/", "\\");
            filePath = filePath.Substring(0, filePath.LastIndexOf(pathChar)) + pathChar;
            cmdKey1 = KeyCode.LeftControl; cmdKey2 = KeyCode.RightControl;
            windowsSystem = true;
            break;
        case RuntimePlatform.OSXPlayer:
            filePath = filePath.Substring(0, filePath.LastIndexOf(pathChar));
            filePath = filePath.Substring(0, filePath.LastIndexOf(pathChar)) + pathChar;
            cmdKey1 = KeyCode.LeftApple; cmdKey2 = KeyCode.RightApple;
            break;
        case RuntimePlatform.WindowsPlayer:
            pathChar = "\\"[0];
            filePath = filePath.Replace("/", "\\");
            filePath = filePath.Substring(0, filePath.LastIndexOf(pathChar)) + pathChar;
            cmdKey1 = KeyCode.LeftControl; cmdKey2 = KeyCode.RightControl;
            windowsSystem = true;
            break;
        case RuntimePlatform.IPhonePlayer:
        case RuntimePlatform.Android:
            if (filePath.Length < 1) { filePath = Directory.GetCurrentDirectory(); }
            break;
        default:
            Debug.LogError("You are not using a supported platform");
            Application.Quit();
            break;
    }
}

public function FileWindowOpen () : boolean {
    return fileWindowOpen;
}

public function GetFileWindowRect () : Rect {
    return fileWindowRect;
}

function OnGUI () {
    var defaultSkin = GUI.skin;
    GUI.skin = guiSkin;
    GUI.depth = guiDepth;
    
#if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN
    // A bit of a hack to get around the fact that the text field eats OnGUI event keyboard input when saving, making arrow key navigation impossible
    // The boolean is used to set FocusControl when drawing the file browser window to something that doesn't eat the keyboard input
    if (Input.GetKey (KeyCode.DownArrow) || Input.GetKey (KeyCode.UpArrow)) {arrowKeysDown = true;}
    else {arrowKeysDown = false;}
    
    if (Event.current.type == EventType.KeyDown) {
        FileWindowKeys();
    }
#endif

	fileWindowRect = GUI.Window (fileWindowID, fileWindowRect, DrawFileWindow, "");
    if (!showMessageWindow) {
        GUI.FocusWindow (fileWindowID);
    }
    else {
        // Double-clicking can cause an error, probably related to window focus, if the message window is drawn in the same frame when the
        // mouse pointer is over the message window.  So we wait until the next OnGUI frame if this is the case: messageWindowDelay is set
        // true in DrawFileWindow if a double-click is detected.
        if (!messageWindowDelay || !messageWindowRect.Contains(Event.current.mousePosition)) {
            GUI.Window (messageWindowID, messageWindowRect, MessageWindow, messageWindowTitle, messageWindowStyle);
            GUI.BringWindowToFront (messageWindowID);
        }
    }
    messageWindowDelay = false;
    
    // Resize window by dragging corner...must be done outside window code, or else mouse drag events outside the window are unrecognized	
    if (Event.current.type == EventType.MouseDown && Rect(fileWindowRect.width-25, fileWindowRect.height-25, 25, 25).Contains(mousePos)) {
        handleClicked = true;
        clickedPosition = mousePos;
        originalWindowRect = fileWindowRect;
    }
    if (handleClicked) {
        if (Event.current.type == EventType.MouseDrag) {
            fileWindowRect.width = Mathf.Clamp(originalWindowRect.width + (mousePos.x - clickedPosition.x), minWindowWidth, 1600);
            fileWindowRect.height = Mathf.Clamp(originalWindowRect.height + (mousePos.y - clickedPosition.y), minWindowHeight, 1200);
        }
        else if (Event.current.type == EventType.MouseUp) {
            handleClicked = false;
        }
    }

    GUI.skin = defaultSkin;
}

private function DrawFileWindow () {
    //GUI.DragWindow (Rect(0,0, 10000, 50));
    mousePos = Event.current.mousePosition;
    
    if (showMessageWindow) {GUI.enabled = false;}
    
    // Folder use button if this is a FileType.Folder window
    if (fileType == FileType.Folder &&
            GUI.Button (Rect(fileWindowRect.width - 100, popupRect.y, 78, popupRect.height), "Select")) {
        if (objectToSendTo == null) {
            objectToSendTo = gameObject;
        }
        CloseFileWindow();
        objectToSendTo.SendMessage ("OpenFolder", filePath);
        return;
    }

    // Editable file name if saving
    if (fileType != FileType.Save) {GUI.enabled = false;}
    if (fileType != FileType.Folder) {GUI.TextField (Rect(115, 100, fileWindowRect.width-140, 30), "", 60);}
    if (!showMessageWindow) {GUI.enabled = true;}
    
    //if (fileType == FileType.Open) {GUI.Label (Rect(25, 101, 90, 30), "Open file:");}
    //else if (fileType == FileType.Save) {GUI.Label (Rect(25, 101, 90, 30), "Save as:");}

    // List of folders/files
    selectionAreaRect = Rect(25, 135, fileWindowRect.width-50, fileWindowRect.height-windowControlsSpace);
    if (arrowKeysDown) {
        GUI.SetNextControlName ("Area");
    }
    BeginArea (selectionAreaRect, "", "box");
        scrollPos = BeginScrollView (scrollPos);
        if (selected == -1) selectedFileNumber = SelectionGrid (selectedFileNumber, fileDisplayList, 1, scrollViewStyle, MaxWidth(1600));
        else selectedFileNumber = SelectionGrid (selectedFileNumber, fileDisplayList, 1, scrollViewStyleActive, MaxWidth(1600));
        recentFileSelectedFile = selectedFileNumber;
        // See if a different file name was chosen, so we don't overwrite any user input in the text box except when needed
        if (selectedFileNumber != oldSelectedFileNumber && frameDone) {
            oldSelectedFileNumber = selectedFileNumber;
            if (selectedFileNumber >= dirList.Count && selectedFileNumber - dirList.Count < fileList.Count) {
                fileName = fileList[selectedFileNumber - dirList.Count];
            }
            else {	// No file name if directory is selected
                fileName = "";
            }
            if (fileType == FileType.Save && autoAddExtension && !fileName.EndsWith(addedExtension)) {
                fileName += addedExtension;
            }
        }
        EndScrollView();
    EndArea();	
    if (arrowKeysDown) {
        GUI.FocusControl ("Area");
    }
    
    // Double-click - only in file selection area
#if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN
    if (Event.current.clickCount == 2 && selectionAreaRect.Contains(mousePos) && frameDone) {
        SelectFile();
        WaitForFrame();
        messageWindowDelay = true;
    }
#else
//    for (var evt : Touch in Input.touches) {
//        if (evt.tapCount == 2 && selectionAreaRect.Contains(mousePos) && frameDone) {
//            SelectFile();
//            WaitForFrame();
//            messageWindowDelay = true;
//            break;
//        }
//    }
#endif
    
    // Filter button
    if (useFilterButton) {
        GUI.Label(Rect(40, fileWindowRect.height-76, 120, 50), windowTab);
        if (GUI.Button(Rect(50, fileWindowRect.height-66, 80, 33), filterFiles? "Show all" : "Filter") ) {
            filterFiles = !filterFiles;
            GetCurrentFileInfo();
        }
    }
    
    // Delete button
    if (useDeleteButton && fileType != FileType.Folder) {
        if (fileType == FileType.Open) {
            if (fileName == "") {
                GUI.enabled = false;
            }
        }
        else {
            if (fileName == "" || (autoAddExtension && fileName == addedExtension)) {
                GUI.enabled = false;
            }
        }
        if (GUI.Button(Rect(150, fileWindowRect.height-buttonPosition.y, buttonSize.x, buttonSize.y), "Delete") ) {
            DeleteFile();
        }
    }
    if (!showMessageWindow) {
        GUI.enabled = true;
    }
    
    // Cancel button
//    if (GUI.Button(Rect(fileWindowRect.width-buttonPosition.x, fileWindowRect.height-buttonPosition.y, buttonSize.x, buttonSize.y), "Cancel") ) {
//        CloseFileWindow();
//    }
    
    // Open/Save button
    if (fileType == FileType.Open) {
        if (selectedFileNumber == -1) {
            GUI.enabled = false;
        }
    }
    else if (fileType == FileType.Folder) {
        if (selectedFileNumber == -1 || selectedFileNumber >= dirList.Count) {
            GUI.enabled = false;
        }
    }
    else {
        if (fileName == "" || (autoAddExtension && fileName == addedExtension)) {
            GUI.enabled = false;
        }
    }
//    if (GUI.Button(Rect(fileWindowRect.width-buttonPosition.x/2, fileWindowRect.height-buttonPosition.y, buttonSize.x, buttonSize.y),
//            selectButtonText[fileType])) {
//        SelectFile();
//    }
    
    if (!showMessageWindow) {
        GUI.enabled = true;
    }
    
    // Path list popup -- done last so it's drawn on top of other stuff
//    if (pathList.Length > 0 && Popup.List (popupRect, showPopup, selectedPath, pathList[0], pathList,
//                                           popupButtonStyle, popupBoxStyle, popupListStyle)) {
//        if (selectedPath > 0 && !limitToInitialFolder) {
//            BuildPathList(selectedPath);
//        }
//    }
}

enum MessageWindowType {Error, Confirm}
private var messageWindowType : MessageWindowType;
private var button1Text : String;
private var button2Text : String;
private var message : String;
private var showMessageWindow = false;
private var messageWindowTitle : String;
private var messageWindowRect : Rect;
private var confirm = true;
private var messageWindowDelay = false;

private function ShowError (msg : String) {
    message = msg;
    messageWindowTitle = "Error";
    showMessageWindow = true;
    messageWindowType = MessageWindowType.Error;
    fileName = "";
}

private function ShowConfirmMessage (title : String, msg : String, b1Text : String, b2Text : String) {
    message = msg;
    button1Text = b1Text;
    button2Text = b2Text;
    messageWindowTitle = title;
    showMessageWindow = true;
    messageWindowType = MessageWindowType.Confirm;
}

private function MessageWindow () {
    Space(32);
    Label(message);
    
    if (messageWindowType == MessageWindowType.Error) {
        if (GUI.Button (Rect(messageWindowSize.x/2-25, messageWindowSize.y-(buttonSize.y+15), 50, buttonSize.y), "OK") && frameDone) {
            CloseMessageWindow(false);
        }
    }
    else if (messageWindowType == MessageWindowType.Confirm) {
        if (GUI.Button (Rect(messageWindowSize.x/2-110, messageWindowSize.y-(buttonSize.y+15), 100, buttonSize.y), button1Text) && frameDone) {
            CloseMessageWindow(false);
        }
        if (GUI.Button (Rect(messageWindowSize.x/2+10, messageWindowSize.y-(buttonSize.y+15), 100, buttonSize.y), button2Text) && frameDone) {
            CloseMessageWindow(true);
        }
    }
}

private function CloseMessageWindow (isConfirmed : boolean) {
    showMessageWindow = false;
    confirm = isConfirmed;
}

// Work-around for behavior where double-clicking selects files it shouldn't
protected function WaitForFrame () {
    frameDone = false;
    selectedFileNumber = -1;
    yield;
    frameDone = true;
    selectedFileNumber = -1;
}

#if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN
private function FileWindowKeys () {
    var arrowKey = 0;
    switch (Event.current.keyCode) {
        case KeyCode.DownArrow: arrowKey = 1; break;
        case KeyCode.UpArrow: arrowKey = -1; break;
        case KeyCode.Return: ReturnHit(); break;
        case KeyCode.Escape: EscapeHit(); break;
    }
    if (arrowKey == 0) return;
    
    // Go to top or bottom of list if alt key is down
    if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)) {
        if (arrowKey == -1) {selectedFileNumber = 0;}
        else {selectedFileNumber = fileDisplayList.Length-1;}
    }
    // Go up or down a folder hierarchy level if command key is down (command/apple on OS X, control on Windows)
    else if (!limitToInitialFolder && (Input.GetKey(cmdKey1) || Input.GetKey(cmdKey2)) ) {
        if (arrowKey == -1 && pathList.Length > 1) {
            BuildPathList(1);
            return;
        }
        else if (selectedFileNumber >= 0 && selectedFileNumber < dirList.Count) {
            SelectFile();
            return;
        }
    }
    // Move file selection up or down
    else {
        selectedFileNumber += arrowKey;
        if (selectedFileNumber < -1) {
            selectedFileNumber = fileDisplayList.Length-1;
        }
        selectedFileNumber = Mathf.Clamp(selectedFileNumber, 0, fileDisplayList.Length-1);
    }
    
    // Handle keyboard scrolling of the list view properly
    var wantedPos = linePixelHeight * selectedFileNumber;
    if (wantedPos < scrollPos.y) {
        scrollPos.y = wantedPos;
    }
    else if (wantedPos > scrollPos.y + fileWindowRect.height - (windowControlsSpace + linePixelHeight + linePixelHeight/2)) {
        scrollPos.y = wantedPos - (fileWindowRect.height - (windowControlsSpace + linePixelHeight + linePixelHeight/2));
    }
}

private function ReturnHit () {
    if (showMessageWindow) {
        CloseMessageWindow(true);
    }
    else {
        SelectFile();
    }
}

private function EscapeHit () {
    if (showMessageWindow) {
        CloseMessageWindow(false);
    }
    else {
        fileWindowOpen = false;
    }
}
#endif

private function BuildPathList (pathEntry : int) {
    filePath = "";
    for (var i = pathList.Length-1; i >= pathEntry; i--) {
        filePath += pathList[i].text;
        if (i < pathList.Length-1 || windowsSystem) {filePath += pathChar;}
    }
    selectedPath = -1;
    GetCurrentFileInfo();
}

protected function DisplayLetters(letters : List.<String>) {
	lettersDisplayNames = new List.<String>();
	lettersDisplayNames = letters;
	currentMenuLevel = MenuLevels.Letters;
	isRecentFiles = false;
	GetCurrentFileInfo();
	ShowFileWindow();
}

protected function DisplayArtists(artists : List.<String>) {
	artistsDisplayNames = new List.<String>();
	artistsDisplayNames = artists;
}

protected function DisplayAlbums(albums : List.<String>) {
	albumsDisplayNames = new List.<String>();
	albumsDisplayNames = albums;
}

protected function DisplaySongs(songs : List.<String>[]) {
	songsDisplayNames = new List.<String>();
	songsDisplayNames = songs[0];
	
	songsPaths = new List.<String>();
	songsPaths = songs[1];
}

protected function FetchRecentFilesNames (displayNames : List.<String>) {
    recentFilesDisplayNames = new List.<String>();
    recentFilesDisplayNames = displayNames;
}

protected function FetchRecentFilesInfos (fileInfos : List.<FileInfo> ) {
    recentFileInfos = new List.<FileInfo>();
    recentFileInfos = fileInfos;
}

private function GetCurrentFileInfo () {
    dirList = new List.<String>();
    fileList = new List.<String>();
    if (isRecentFiles) {
        // Put file names into a sorted array
        if (showFiles && recentFileInfos.Count > 0) {
            for (var i = 0; i < recentFileInfos.Count; i++) {
                fileList.Add(recentFileInfos[i].Name);
            }
        }

        // Create the file list that's actually displayed
        fileDisplayList = new GUIContent[recentFilesDisplayNames.Count];
        for (i = 0; i < fileList.Count; i++) {
            fileDisplayList[i] = new GUIContent(recentFilesDisplayNames[i], fileIcon);
        }
    
        // Reset stuff so no filenames are selected and the scrollbar is always at the top
        selectedFileNumber = oldSelectedFileNumber = -1;
        scrollPos = Vector2.zero;
    } else if (Application.platform == RuntimePlatform.WindowsEditor){
        var info = new DirectoryInfo(filePath);
        if (!info.Exists) {
            resetPath = true;
            HandleError("The directory \"" + filePath + "\" does not exist");
            return;
        }

        try {
            var fileInfo = info.GetFiles();
            var dirInfo = info.GetDirectories();
        }
        catch (err) {
            if (err instanceof System.UnauthorizedAccessException) {
                HandleAccessError(err.Message);
                return;
            } else {
                HandleError(err.Message);
                return;
            }
        }
        // Put folder names into a sorted array
        if (!limitToInitialFolder && dirInfo.Length > 0) {
            for (i = 0; i < dirInfo.Length; i++) {
                // Don't include ".app" folders or hidden folders, if set
                if (dirInfo[i].Name.EndsWith(".app") && !allowAppBundleBrowsing) continue;
                if (dirInfo[i].Name.StartsWith(".") && !showHiddenOSXFiles) continue;
                dirList.Add(dirInfo[i].Name);
            }
            dirList.Sort();
        }
    
        // Get volumes/drives, if set
        if (showVolumes && !limitToInitialFolder) {
            if (!windowsSystem) {
                try {
                    info = new DirectoryInfo("/Volumes");
                    dirInfo = info.GetDirectories();
                    numberOfVolumes = dirInfo.Length;
                    for (i = 0; i < numberOfVolumes; i++) {
                        dirList.Insert(i, dirInfo[i].Name);
                    }
                }
                catch (err) {
                    HandleError(err.Message);
                }
            }
            else {
                var drives = Directory.GetLogicalDrives();
                for (i = 0; i < drives.Length; i++) {
                    dirList.Insert(i, drives[i].Substring(0, drives[i].Length-1));
                }
                numberOfVolumes = drives.Length;
            }
        }
    
        // Put file names into a sorted array
        if (showFiles && fileInfo.Length > 0) {
            for (i = 0; i < fileInfo.Length; i++) {
                // Don't include hidden files, if set
                if (fileInfo[i].Name.StartsWith(".") && !showHiddenOSXFiles) continue;
                if (filterFiles && filterFileExtensions.Length > 0) {
                    // Go through all extensions for this file type
                    var dontAddFile = true;
                    for (var j = 0; j < filterFileExtensions.Length; j++) {
                        if (fileInfo[i].Name.ToLower().EndsWith(filterFileExtensions[j])) {
                            dontAddFile = false;
                            break;
                        }
                    }
                    if (dontAddFile) continue;
                }
                fileList.Add(fileInfo[i].Name);
            }
            fileList.Sort();
        }

        // Create the combined folder + file list that's actually displayed
        fileDisplayList = new GUIContent[dirList.Count + fileList.Count];
        for (i = 0; i < dirList.Count; i++) {
            if (showVolumes && i < numberOfVolumes) {
                fileDisplayList[i] = new GUIContent(dirList[i], driveIcon);
            }
            else {
                fileDisplayList[i] = new GUIContent(dirList[i], folderIcon);	
            }
        }
        for (i = 0; i < fileList.Count; i++) {
            fileDisplayList[i + dirList.Count] = new GUIContent(fileList[i], fileIcon);
        }
    
        // Get path list
        var currentPathList = filePath.Split(pathChar);
        var pathListArray = new List.<String>();
        for (i = 0; i < currentPathList.length-1; i++) {
            if (currentPathList[i] == "") {pathListArray.Add(pathChar.ToString());}
            else {pathListArray.Add(currentPathList[i]);}
        }
        pathListArray.Reverse();
        pathList = new GUIContent[pathListArray.Count];
        for (i = 0; i < pathList.Length; i++) {
            pathList[i] = new GUIContent(pathListArray[i], folderIcon);
        }
    
        // Reset stuff so no filenames are selected and the scrollbar is always at the top
        selectedFileNumber = oldSelectedFileNumber = -1;
        scrollPos = Vector2.zero;
        if (autoAddExtension && fileType == FileType.Save) {
            fileName = addedExtension;
        }
        else {
            fileName = "";
        }
        UpdateDirectoryLabel();
    } 
    else if (Application.platform == RuntimePlatform.Android) {
    	
    	
    	if(currentMenuLevel == MenuLevels.Letters && lettersDisplayNames.Count > 0) {
    		fileDisplayList = new GUIContent[lettersDisplayNames.Count];
    		for (var t = 0; t < lettersDisplayNames.Count; t++) {
            	fileDisplayList[t] = new GUIContent(lettersDisplayNames[t], fileIcon);
        	}
    	}
    	else if(currentMenuLevel == MenuLevels.Artist && artistsDisplayNames.Count > 0) {
	        fileDisplayList = new GUIContent[artistsDisplayNames.Count];
    		for (var l = 0; l < artistsDisplayNames.Count; l++) {
            	fileDisplayList[l] = new GUIContent(artistsDisplayNames[l], fileIcon);
        	}
    	}
    	else if(currentMenuLevel == MenuLevels.Album && albumsDisplayNames.Count > 0) {
    		fileDisplayList = new GUIContent[albumsDisplayNames.Count];
    		for (var k = 0; k < albumsDisplayNames.Count; k++) {
            	fileDisplayList[k] = new GUIContent(albumsDisplayNames[k], fileIcon);
        	}
    	}
    	else if(currentMenuLevel == MenuLevels.Song && songsDisplayNames.Count > 0) {
            
            for (var r = 0; r < songsPaths.Count; r++) {
                fileList.Add(songsPaths[r]);
            }
    	
    		fileDisplayList = new GUIContent[songsDisplayNames.Count];
    		for (var h = 0; h < songsDisplayNames.Count; h++) {
            	fileDisplayList[h] = new GUIContent(songsDisplayNames[h], fileIcon);
        	}
    	}
    	// else... wut? empty list somewhere?
		UpdateDirectoryLabel();
        selectedFileNumber = oldSelectedFileNumber = -1;
        scrollPos = Vector2.zero;
    }
}

private function HandleAccessError (errorMessage : String) {
    ShowError(errorMessage);
	BuildPathList(0);
}

private function HandleError (errorMessage : String) {
    ShowError(errorMessage);
    SetDefaultPath();
    fileDisplayList = new GUIContent[0];
    pathList = new GUIContent[0];
    OpenFileWindow(filePath);
}

public function SetPath (thisPath : String) {
    filePath = thisPath;
    if (!filePath.EndsWith(pathChar.ToString())) {
        filePath += pathChar;
    }
    if (windowsSystem) {
        filePath = filePath.Replace("/", "\\");
    }
}

public function SetGameObject (go : GameObject) {
    objectToSendTo = go;
}

public function OpenRecentFilesWindow () {
    if (fileWindowOpen) return;

    isRecentFiles = true;

    showFiles = true;
    fileType = FileType.Open;
    ShowFileWindow();
}

public function OpenFileWindow ( path : String ) {
    if (fileWindowOpen) return;
    if (path != "") {
//        switch (Application.platform) {
//            case RuntimePlatform.WindowsEditor:
//                filePath = path + "\\";
//                break;
//            case RuntimePlatform.Android:
//                filePath = path + "/";
//                break;
//        }
        filePath = path;
    }

    isRecentFiles = false;

    showFiles = true;
    fileType = FileType.Open;
    ShowFileWindow();
}

public function OpenFolderWindow (showFiles : boolean) {
    if (fileWindowOpen) return;

    this.showFiles = showFiles;
    fileType = FileType.Folder;
    ShowFileWindow();
}

public function SaveFileWindow () {
    if (fileWindowOpen) return;

    showFiles = true;
    fileType = FileType.Save;
    ShowFileWindow();
}

private function ShowFileWindow () {
	if (!isRecentFiles) UpdateDirectoryLabel();
    GetCurrentFileInfo();
    windowTitle = fileWindowTitles[fileType];
    fileWindowOpen = true;
    enabled = true;
}

public function CloseFileWindow () {
    if (showMessageWindow) return;	// Don't let window close if error/confirm window is open
    PlayerPrefs.SetString("filePath", filePath);
    fileWindowOpen = false;
    selectedFileNumber = oldSelectedFileNumber = -1;
    fileName = "";
    SendMessage ("FileWindowClosed", SendMessageOptions.DontRequireReceiver);
    // For maximum efficiency, the OnGUI function in this script doesn't run at all when the file browser window isn't open,
    // but is enabled in ShowFileWindow when necessary
    enabled = false;
}

public function CloseFileWindowTab () {
    if (showMessageWindow) return;	// Don't let window close if error/confirm window is open
    PlayerPrefs.SetString("filePath", filePath);
    fileWindowOpen = false;
    selectedFileNumber = oldSelectedFileNumber = -1;
    fileName = "";
    // For maximum efficiency, the OnGUI function in this script doesn't run at all when the file browser window isn't open,
    // but is enabled in ShowFileWindow when necessary
    enabled = false;
}

public function CloseRecentFilesWindowTab () {
    if (showMessageWindow) return;	// Don't let window close if error/confirm window is open
    fileWindowOpen = false;
    selectedFileNumber = oldSelectedFileNumber = -1;
    fileName = "";
    // For maximum efficiency, the OnGUI function in this script doesn't run at all when the file browser window isn't open,
    // but is enabled in ShowFileWindow when necessary
    enabled = false;
}

public function SetWindowTitle (title : String) {
    windowTitle = title;
}

private function DeleteFile () : IEnumerator {
    if (showMessageWindow || selectFileInProgress || (selectedFileNumber >= 0 && selectedFileNumber < dirList.Count)) return;

    selectFileInProgress = true;

    if (File.Exists(filePath + fileName)) {
        ShowConfirmMessage("Warning", 'Are you sure you want to delete "' + fileName + '"?', "Cancel", "Delete");
        while (showMessageWindow) {
            yield;
        }
        if (!confirm) {
            selectFileInProgress = false;
            return;
        }
    }
    else {
        selectFileInProgress = false;
        return;
    }

    try {
        File.Delete(filePath + fileName);
    }
    catch (err) {
        ShowError(err.Message);
    }
    
    GetCurrentFileInfo();
    selectFileInProgress = false;
}

private function SelectFile () : IEnumerator {
    if (showMessageWindow || selectFileInProgress) return;

	if (Application.platform == RuntimePlatform.WindowsEditor) {
	    // If user opened a folder, change directories
	    if (selectedFileNumber >= 0 && selectedFileNumber < dirList.Count) {
	        filePath += dirList[selectedFileNumber] + pathChar;
	        if (showVolumes && selectedFileNumber < numberOfVolumes) {
	            if (!windowsSystem) {
	                filePath = "/Volumes/" + dirList[selectedFileNumber] + pathChar;
	            }
	            else {
	                filePath = dirList[selectedFileNumber] + pathChar + pathChar;
	            }
	        }
	        GetCurrentFileInfo();
	        return;
	    }
	    
	    // Do nothing if there's no file name, or if saving and no real filename has been supplied
	    if (fileName == "" || (fileType == FileType.Save && autoAddExtension && fileName == addedExtension)) return;
	    
	    selectFileInProgress = true;
	    var thisFileName = fileName;	// Make sure to keep the file name as it was when selected, since it can change later
	    // Check for duplicate file name when saving
	    if (fileType == FileType.Save) {
	        if (autoAddExtension && !thisFileName.EndsWith(addedExtension)) {
	            thisFileName += addedExtension;
	        }
	        for (var i = 0; i < fileList.Count; i++) {
	            var file = fileList[i];
	            if (file == thisFileName) {
	                ShowConfirmMessage ("Warning", "A file with that name already exists. Are you sure you want to replace it?", "Cancel", "Replace");
	                while (showMessageWindow) {
	                    fileName = thisFileName;
	                    yield;
	                }
	                if (!confirm) {
	                    selectFileInProgress = false;
	                    return;
	                }
	            }
	        }
	    }
	
	    if (objectToSendTo == null) {
	        objectToSendTo = gameObject;
	    }
	
	    // If user selected a name, load/save that file
	    if (fileType == FileType.Open) {
	        CloseFileWindow();
	        if (isRecentFiles) {
	//            Debug.Log(recentFileSelectedFile);
	            objectToSendTo.SendMessage ("OpenFile", recentFileInfos[recentFileSelectedFile].FullName);
	        } else {
	            objectToSendTo.SendMessage ("OpenFile", filePath + thisFileName);
	        }
	    }
	    else if (fileType == FileType.Save) {
	        CloseFileWindow();
	        objectToSendTo.SendMessage ("SaveFile", filePath + thisFileName);
	        GetCurrentFileInfo();	// Refresh with new file in case of error
	    }
	    
	    selectFileInProgress = false;
    }
    else if (Application.platform == RuntimePlatform.Android) {
    	if (isRecentFiles) {
    		var tempFileNumber = selectedFileNumber;
    		CloseFileWindow();
	    	gameObject.SendMessage ("OpenFile", recentFileInfos[tempFileNumber].FullName);
	    }
    	else if(currentMenuLevel == MenuLevels.Letters) {
    		currentMenuLevel = MenuLevels.Artist;
    		currentLetter = lettersDisplayNames[selectedFileNumber];
    		currentArtist = "";
    		gameObject.SendMessage("FetchArtistsForLetter",lettersDisplayNames[selectedFileNumber]);
    		GetCurrentFileInfo();
    	}
    	else if(currentMenuLevel == MenuLevels.Artist) {
    		currentMenuLevel = MenuLevels.Album;
    		currentAlbum = "";
    		currentArtist = artistsDisplayNames[selectedFileNumber];
    		gameObject.SendMessage("FetchAlbumsForArtist",artistsDisplayNames[selectedFileNumber]);
    		GetCurrentFileInfo();
    	} else if (currentMenuLevel == MenuLevels.Album) {
    		currentMenuLevel = MenuLevels.Song;
    		currentAlbum = albumsDisplayNames[selectedFileNumber];
    		var args : String[] = new String[2];
    		args[0] = currentArtist;
    		args[1] = albumsDisplayNames[selectedFileNumber];
    		gameObject.SendMessage("FetchSongs",args);
    		GetCurrentFileInfo();
    	} else if (currentMenuLevel == MenuLevels.Song) {
    		tempFileNumber = selectedFileNumber;
    		CloseFileWindow();
    		gameObject.SendMessage("OpenFile",songsPaths[tempFileNumber]);
    	}
    }
}

// Button handler for the file browser up button, needs to be protected or the compiler thinks the method is never used.
protected function FolderUp () {
	if(Application.platform == RuntimePlatform.WindowsEditor) {
		if (pathList.Length > 1) {
			BuildPathList(1);
			UpdateDirectoryLabel();
		}
	} else if(Application.platform == RuntimePlatform.Android) {
		if (currentMenuLevel == MenuLevels.Song) {
			currentMenuLevel = MenuLevels.Album;
			currentAlbum = "";
			gameObject.SendMessage("FetchAlbumsForArtist",currentArtist);
    		GetCurrentFileInfo();
		}
		else if (currentMenuLevel == MenuLevels.Album) {
			currentMenuLevel = MenuLevels.Artist;
			currentAlbum = "";
			currentArtist = "";
			gameObject.SendMessage("FetchArtistsForLetter",currentLetter);
    		GetCurrentFileInfo();
		}
		else if (currentMenuLevel == MenuLevels.Artist) {
			currentMenuLevel = MenuLevels.Letters;
			currentAlbum = "";
			currentArtist = "";
			currentLetter = "";
			gameObject.SendMessage("FetchLetters");
		}
	}

}

private function UpdateDirectoryLabel() {
	if(Application.platform == RuntimePlatform.WindowsEditor) {
		var directoryStructure : String[];
		var directoryPath : String;
		var maxPathLength : int = 55;
		var remPathLength : int;
		var numFoldersInPath : int = 1;
		
	    directoryStructure = filePath.Split(pathChar);
	
	    remPathLength = maxPathLength - directoryStructure[directoryStructure.Length - 1].Length;
	    
	    for (var cnt = directoryStructure.Length - 2; cnt > -1; cnt--) {
	        if (directoryStructure[cnt].Length < remPathLength) {
	            directoryPath = pathChar + directoryStructure[cnt] + directoryPath;
	            remPathLength -= directoryStructure[cnt].Length + 1;
	            numFoldersInPath++;
	        } else {
	            cnt = 0; // Terminate for-loop -> stop adding folders to path
	        }
	    }
	    if (numFoldersInPath < directoryStructure.Length - 1) {
	        directoryPath = "..." + directoryPath;
	    } else {
	    	directoryPath = directoryPath.Substring(1);
	    	if (directoryStructure.Length == 2) { directoryPath = directoryPath + pathChar;}
	    }
	    if (directoryPath.Length > maxPathLength + 6) {
	        directoryPath = directoryPath.Substring(0, maxPathLength) + "...";
	    }
		
		if (objectToSendTo == null) {
	        objectToSendTo = gameObject;
	    }
	   	objectToSendTo.SendMessage ("UpdateDirLabel", directoryPath);
   	} else if(Application.platform == RuntimePlatform.Android) {
   		if(currentMenuLevel == MenuLevels.Letters) {
   			gameObject.SendMessage("UpdateDirLabel","Select artists by alphabetical order");
   		}
   		else if(currentMenuLevel == MenuLevels.Artist) {
	   		gameObject.SendMessage("UpdateDirLabel","Select Artist");
   		}
   		else if(currentMenuLevel == MenuLevels.Album) {
	   		gameObject.SendMessage("UpdateDirLabel","Select album by artist " + currentArtist);
   		}
   		else if(currentMenuLevel == MenuLevels.Song) {
   			gameObject.SendMessage("UpdateDirLabel","Select song from album " + currentAlbum + " by artist " + currentArtist);
   		}
   	}
}
#endif