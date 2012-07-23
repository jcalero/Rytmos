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
	public UILabel FPSLabel;
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
	private float rightMargin = -60f;
	private float bottomMargin = 20f;

	// Static self instance
	private static HUD instance;
	#endregion

	#region Mono Functions

	void Awake() {
		instance = this;

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
		FPSLabel.transform.localPosition = new Vector2(screenRight + (rightMargin / 5), screenBottom + bottomMargin);
		FPSLabel.MakePixelPerfect();
		FPSLabel.transform.localScale = new Vector2(20, 20);

		// Initialise display values
		UpdateScore();
		UpdateMultiplier();

		//Debug.Log(">>>>>>>>>>>>>> HUD TESTING VALUES <<<<<<<<<<<<<<<<<<");
		//Debug.Log("HUD Camera aspect: " + HUDCamera.aspect);
		//Debug.Log("HUD Camera dimensions: " + HUDCamera.pixelWidth + " x " + HUDCamera.pixelHeight);
		//Debug.Log("HUD Camera Viewport sides: " + "Left: " + HUDCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).x + " Top: " + HUDCamera.ViewportToWorldPoint(new Vector3(0, 1, 0)).y);
		//Debug.Log("Score label world position: (" + ScoreValueLabel.transform.position.x + ", " + ScoreValueLabel.transform.position.y + ")");
		//Debug.Log("Score label local position: (" + ScoreValueLabel.transform.localPosition.x + ", " + ScoreValueLabel.transform.localPosition.y + ")");
		//Debug.Log(">>>>>>>>>>>>>>>>>> END OF TEST <<<<<<<<<<<<<<<<<<<<<");
	}

	void Update() {
		// Enable the dev mode panel if dev mode is enabled
		//if (Game.DevMode) {
		//    if (!DevModePanel.enabled) UITools.SetActiveState(DevModePanel, true);
		//} else {
		//    if (DevModePanel.enabled) UITools.SetActiveState(DevModePanel, false);
		//}
	}
	#endregion
	
	#region Properties
	private string MultiplierText {
		get {
			int multiplier = Player.multiplier;
			string tempText = "";
			if (multiplier >= 20)
				tempText = "[FF4400]x" + Player.multiplier;
			else if (multiplier >= 15)
				tempText = "[CCAA22]x" + Player.multiplier;
			else if (multiplier >= 10)
				tempText = "[AADD55]x" + Player.multiplier;
			else if (multiplier >= 5)
				tempText = "[77DDFF]x" + Player.multiplier;
			else if (multiplier == 1)
				tempText = "";
			else
				tempText = "x" + Player.multiplier;
			return tempText;
		}
	}
	public static Camera Camera {
		get {
			return instance.HUDCamera;
		}
	}
	#endregion

	#region Functions
	public static void UpdateScore() {
		instance.ScoreValueLabel.text = "[55BBEE]" + UITools.FormatNumber(Player.score.ToString());
		instance.ScoreValueLabel.animation.Play("Thump");
	}
	public static void UpdateMultiplier() {
		instance.MultiplierLabel.text = instance.MultiplierText;
		if (Player.multiplier == 1) {
			instance.MultiplierLabel.transform.localScale = new Vector2(32, 32);
			instance.MultiplierLabel.animation.wrapMode = WrapMode.Default;
			return;
		} else if (Player.multiplier > 1 && Player.multiplier <= 4) {
			instance.MultiplierLabel.animation.Play("Thump");
		} else if (Player.multiplier > 4 && Player.multiplier <= 9) {
			instance.MultiplierLabel.animation.Play("Thump2");
		} else if (Player.multiplier > 9 && Player.multiplier <= 14) {
			instance.MultiplierLabel.animation.Play("Thump3");
		} else if (Player.multiplier > 14 && Player.multiplier <= 19) {
			instance.MultiplierLabel.animation.Play("Thump4");
			instance.MultiplierLabel.animation.Play("Thump4-loop");
		} else if (Player.multiplier == 20) {
			instance.MultiplierLabel.animation.Play("Thump5");
			instance.MultiplierLabel.animation.wrapMode = WrapMode.Loop;
		}
	}
	#endregion
}
