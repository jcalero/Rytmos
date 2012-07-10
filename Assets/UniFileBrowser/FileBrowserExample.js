// Example of open/save usage with FileBrowser.js

private var message = "";
private var alpha = 1.0;
private var pathChar = "/"[0];

function Awake () {
	if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer) {
		pathChar = "\\"[0];
	}
}

function OnGUI () {
	if (GUI.Button(Rect(100, 50, 95, 35), "Open")) {
		SendMessage("OpenFileWindow");
	}
	if (GUI.Button(Rect(100, 125, 95, 35), "Save")) {
		SendMessage("SaveFileWindow");
	}
	if (GUI.Button(Rect(100, 200, 95, 35), "Open Folder")) {
		SendMessage("OpenFolderWindow", true);
	}
	GUI.color.a = alpha;
	GUI.Label(Rect(100, 275, 500, 30), message);
	GUI.color.a = 1.0;
}

function OpenFile (pathToFile : String) {
	var fileIndex = pathToFile.LastIndexOf(pathChar);
	message = "You selected file: " + pathToFile.Substring(fileIndex+1, pathToFile.Length-fileIndex-1);
	Fade();
}

function SaveFile (pathToFile : String) {
	var fileIndex = pathToFile.LastIndexOf(pathChar);
	message = "You're saving file: " + pathToFile.Substring(fileIndex+1, pathToFile.Length-fileIndex-1);
	Fade();
}

function OpenFolder (pathToFolder : String) {
	message = "You selected folder: " + pathToFolder;
	Fade();
}

function Fade () {
	StopCoroutine("FadeAlpha");	// Interrupt FadeAlpha if it's already running, so only one instance at a time can run
	StartCoroutine("FadeAlpha");
}

function FadeAlpha () {
	alpha = 1.0;
	yield WaitForSeconds(5.0);
	for (alpha = 1.0; alpha > 0.0; alpha -= Time.deltaTime/4) {
		 yield;
	}
	message = "";
}