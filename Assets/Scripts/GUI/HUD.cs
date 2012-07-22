using UnityEngine;
using System.Collections;
/// <summary>
/// GameHud.cs
/// 2012-06-27
/// 
/// Handles the heads up display (text and scores) during gameplay.
/// </summary>
public class HUD : MonoBehaviour {
	#region Fields
	// These public objects are set in the Editor/Inspector. Object: "HUDManager".
	public UILabel ScoreValueLabel;
	public UILabel MultiplierLabel;
	public UILabel EnergyLabel;
	public UILabel EnergyValueLabel;
	public UIPanel DevModePanel;
	public Camera HUDCamera;

	// The Game HUD camera coordinates
	private float screenLeft;
	private float screenTop;
	private float screenCentre;
	private float screenRight;
	private float screenBottom;

	// Label offset values
	private float topMargin = -30f;
	private float rightMargin = -30f;
	#endregion

	#region Functions

	void Awake() {
		// Assign HUD camera coordinates
		screenLeft = -HUDCamera.pixelWidth / 2;
		screenTop = HUDCamera.pixelHeight / 2;
		screenCentre = 0;
		screenRight = -screenLeft;
		screenBottom = -screenTop;
	}

	void Start() {
		// Position the HUD relative to the screen
		ScoreValueLabel.transform.localPosition = new Vector2(screenCentre, screenTop + topMargin);
		ScoreValueLabel.MakePixelPerfect();
		MultiplierLabel.transform.localPosition = new Vector2(screenRight + rightMargin, screenTop + topMargin);
		MultiplierLabel.MakePixelPerfect();

		//Debug.Log(">>>>>>>>>>>>>> HUD TESTING VALUES <<<<<<<<<<<<<<<<<<");
		//Debug.Log("HUD Camera aspect: " + HUDCamera.aspect);
		//Debug.Log("HUD Camera dimensions: " + HUDCamera.pixelWidth + " x " + HUDCamera.pixelHeight);
		//Debug.Log("HUD Camera Viewport sides: " + "Left: " + HUDCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).x + " Top: " + HUDCamera.ViewportToWorldPoint(new Vector3(0, 1, 0)).y);
		//Debug.Log("Score label world position: (" + ScoreValueLabel.transform.position.x + ", " + ScoreValueLabel.transform.position.y + ")");
		//Debug.Log("Score label local position: (" + ScoreValueLabel.transform.localPosition.x + ", " + ScoreValueLabel.transform.localPosition.y + ")");
		//Debug.Log(">>>>>>>>>>>>>>>>>> END OF TEST <<<<<<<<<<<<<<<<<<<<<");
	}

	void Update() {
		// Update game values
		ScoreValueLabel.text = "[55BBEE]" + UITools.FormatNumber(Player.score.ToString());
		MultiplierLabel.text = MultiplierText;
		//EnergyValueLabel.text = "[736AFF]" + Player.energy;

		// Enable the dev mode panel if dev mode is enabled
		//if (Game.DevMode) {
		//    if (!DevModePanel.enabled) UITools.SetActiveState(DevModePanel, true);
		//} else {
		//    if (DevModePanel.enabled) UITools.SetActiveState(DevModePanel, false);
		//}
	}

	private string MultiplierText {
		get {
			int multiplier = Player.multiplier;
			string tempText = "";
			if (multiplier >= 20)
				tempText = "[DD9922]x" + Player.multiplier;
			else if (multiplier >= 15)
				tempText = "[CCAA22]x" + Player.multiplier;
			else if (multiplier >= 10)
				tempText = "[AADD55]x" + Player.multiplier;
			else if (multiplier >= 5)
				tempText = "[77DDFF]x" + Player.multiplier;
			else
				tempText = "x" + Player.multiplier;
			return tempText;
		}
	}

	#endregion
}
